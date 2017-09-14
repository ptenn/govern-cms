using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using GovernCMS.ViewModels;

namespace GovernCMS.Web
{
    public static class CalendarExtensions
    {
        public static IHtmlString CalendarDisplay(this HtmlHelper helper, IEnumerable<CalendarEventViewModel> calendarEvents, int indentSize)
        {
            string delimiter = "";
            string indentString = "";
            for (int i = 0; i < indentSize; i++)
            {
                indentString += " ";
            }


            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(indentString).Append("$(function() {\n");
            stringBuilder.Append(indentString).Append("    $('#calendar').fullCalendar({\n");
            stringBuilder.Append(indentString).Append("      header:\n");
            stringBuilder.Append(indentString).Append("      {\n");
            stringBuilder.Append(indentString).Append("          left: 'prev,next today',\n");
            stringBuilder.Append(indentString).Append("          center: 'title',\n");
            stringBuilder.Append(indentString).Append("          right: 'month,basicWeek,basicDay',\n");
            stringBuilder.Append(indentString).Append("      },\n");
            stringBuilder.Append(indentString).Append("      editable: true,\n");
            stringBuilder.Append(indentString).Append("      eventLimit: true,\n");
            stringBuilder.Append(indentString).Append("      events: [\n");
            foreach(CalendarEventViewModel calendarEvent in calendarEvents)
            {
                stringBuilder.Append(indentString).Append(delimiter);
                stringBuilder.Append(indentString).Append("          {\n");
                stringBuilder.Append(indentString).Append("              title: '" + calendarEvent.EventName + "',\n");
                stringBuilder.Append(indentString).Append("              start: '" + calendarEvent.StartDate + "',\n");
                if (string.IsNullOrEmpty(calendarEvent.EventUrl) && !string.IsNullOrEmpty(calendarEvent.EndDate))
                {
                    stringBuilder.Append(indentString).Append("              end: '" + calendarEvent.EndDate + "'\n");
                }
                else if (!string.IsNullOrEmpty(calendarEvent.EventUrl) && !string.IsNullOrEmpty(calendarEvent.EndDate))
                {
                    stringBuilder.Append(indentString).Append("                    end: '" + calendarEvent.EndDate + "',\n");
                    stringBuilder.Append(indentString).Append("                    url: '" + calendarEvent.EventUrl + "'\n");
                }
                stringBuilder.Append(indentString).Append("          }");
                        delimiter = ",\n";
            }
            stringBuilder.Append(indentString).Append("      ]\n");
            stringBuilder.Append(indentString).Append("   });\n");
            stringBuilder.Append(indentString).Append("});");        
            return new HtmlString(stringBuilder.ToString());
        }
    }
}