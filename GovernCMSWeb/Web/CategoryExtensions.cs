using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using GovernCMS.Models;

namespace GovernCMS.Web
{
    public static class CategoryExtensions
    {
        public static IHtmlString CategoryDisplay(this HtmlHelper helper, IEnumerable<Category> categories, int indentSize)
        {
            string indentString = "";
            for (int i = 0; i < indentSize; i++)
            {
                indentString += " ";
            }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<div class=\"dd\" name=\"category-nestable\" id=\"nestable\">\n");
            stringBuilder.Append(BuildList(categories, indentString));
            stringBuilder.Append(indentString).Append("</div>");
            return new HtmlString(stringBuilder.ToString());
        }

        private static IHtmlString BuildList(IEnumerable<Category> categories, string indentString)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(indentString).Append("  <ol class=\"dd-list\">\n");

            foreach (Category category in categories)
            {
                stringBuilder.Append(indentString).Append("    <li class=\"dd-item\" data-type=\"section\" data-id=\"" + category.CategoryId + "\" id=\"" + category.CategoryId + "\">\n");
                stringBuilder.Append(indentString).Append("      <div class=\"dd-handle\">\n");
                stringBuilder.Append(indentString).Append("        <span style=\"font-weight: bold\" >" + category.CategoryName + "</span>\n");
                stringBuilder.Append(indentString).Append("      </div>\n");
                if (category.SubCategories.Any())
                {
                    indentString += "  ";
                    stringBuilder.Append(BuildList(category.SubCategories, indentString));
                }
                stringBuilder.Append(indentString).Append("    </li>\n");
            }
            stringBuilder.Append(indentString).Append("  </ol>\n");

            return new HtmlString(stringBuilder.ToString());
        }
    }
}
