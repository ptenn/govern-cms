using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GovernCMS.Models;
using GovernCMS.Utils;
using log4net;

namespace GovernCMS.Controllers
{
    public class ArtifactController : ErrorHandlingController
    {
        private static ILog logger = LogManager.GetLogger(typeof(UserController));

        private GovernCmsContext db = new GovernCmsContext();

        public ActionResult Index()
        {
            UserCheck();
            User currentUser = (User)Session[Constants.CURRENT_USER];

//            IList<Artifact> artifacts = db.Artifacts.Where(a => a.)
            return View();
        }

        public ActionResult Add()
        {
            throw new System.NotImplementedException();
        }

    }
}