using AzurePlayground.Database;
using System;
using System.Linq;

namespace AzurePlayground {
    public partial class Test : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            var context = new PlaygroundContext();

            resultDisplay.InnerText = context.Users.Count().ToString();
        }
    }
}