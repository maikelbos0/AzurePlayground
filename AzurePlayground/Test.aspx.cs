using AzurePlayground.Database;
using System;
using System.Linq;

namespace AzurePlayground {
    public partial class Test : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            var context = new PlaygroundContext();

            try {
                //context.DatabaseInitialize();

                resultDisplay.InnerText = context.Users.Count().ToString();
            }
            catch (Exception ex) {
                resultDisplay.InnerHtml = $"{ex.Message}<br/>{ex.StackTrace.Replace(Environment.NewLine, "<br/>")}";
            }
        }
    }
}