using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GovernCMS.Models;
using GovernCMS.Services;
using GovernCMS.Services.Impl;
using GovernCMS.Utils;
using GovernCMS.ViewModels;
namespace GovernCMS.Controllers
{
    public class BoardController : ErrorHandlingController
    {
        private IWebsiteService websiteService;

        public BoardController()
        {
            websiteService = new WebsiteService();
        }

        // GET: Board
        public ActionResult Manage(int? websiteId)
        {
            UserCheck();
            User currentUser = (User)Session[Constants.CURRENT_USER];
            int organizationId = 1;
            IList<Website> websites = websiteService.FindWebsitesByOrganizationIdIncludeBoards(currentUser.OrganizationId);
            IList<SelectListItem> selectListItems = new List<SelectListItem>();
            IList<SelectListItem> boardSelectList = new List<SelectListItem>();

            IList<Board> allBoards = new List<Board>();

            IDictionary<int, IList<Board>> websiteBoards = new Dictionary<int, IList<Board>>();
            foreach (var website in websites)
            {
                SelectListItem item = new SelectListItem()
                {
                    Text = website.SiteName,
                    Value = website.Id.ToString()
                };
                selectListItems.Add(item);

                // Get the Boards for each Website
                IList<Board> boards = websiteService.FindBoardsByWebsiteId(website.Id);

                if (boards != null)
                {
                    foreach (var board in boards)
                    {
                        allBoards.Add(board);
                    }
                    websiteBoards.Add(website.Id, boards);
                }
            }

            int boardId = 0;
            string boardName = "";
            IList<BoardCard> selectedBoardCards = new List<BoardCard>();
            if (websites.Count > 0)
            {
                if (websiteId == null)
                {
                    websiteId = websites.First().Id;
                    if (websiteBoards.ContainsKey(websiteId.Value))
                    {
                        Boolean firstBoard = true;
                        IList<Board> selectedBoards = websiteBoards[websiteId.Value];
                        foreach (var board in selectedBoards)
                        {
                            if (firstBoard)
                            {
                                boardId = board.Id;
                                boardName = board.BoardName;
                                selectedBoardCards = board.BoardCards.ToList();
                                firstBoard = false;
                            }
                            SelectListItem item = new SelectListItem()
                            {
                                Text = board.BoardName,
                                Value = board.Id.ToString()
                            };
                            boardSelectList.Add(item);
                        }
                    }

                }
            }

            BoardViewModel boardViewModel = new BoardViewModel();
            boardViewModel.WebsiteId = websiteId.GetValueOrDefault();
            boardViewModel.BoardId = boardId;
            boardViewModel.BoardName = boardName;
            boardViewModel.WebsiteSelectList = new SelectList(selectListItems);
            boardViewModel.BoardSelectList = new SelectList(boardSelectList);
            boardViewModel.BoardCards = selectedBoardCards;

            return View("ManageBoard", boardViewModel);
        }

        public JsonResult BoardAdd(int websiteId, string name)
        {
            Board board = new Board();
            board.WebsiteId = websiteId;
            board.BoardName = name;
            board.CreateDate = DateTime.Now.Date;

            // Save Board
            return Json(board);
        }

        public JsonResult BoardAddCard(int boardId, string cardHeader, string cardBody)
        {
            BoardCard boardCard = new BoardCard();
            boardCard.BoardId = boardId;
            boardCard.CardHeader = cardHeader;
            boardCard.CardBody = cardBody;
            boardCard.CreateDate = DateTime.Now.Date;

            // Save card

            return Json(boardCard);
        }

    }
}