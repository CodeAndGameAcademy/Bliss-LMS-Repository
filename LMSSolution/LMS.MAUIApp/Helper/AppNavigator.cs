using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.MAUIApp.Helper
{
    public static class AppNavigator
    {
        // Auth Flow (NO BACK)
        public static void SetRoot(Page page)
        {
            Application.Current!.Windows[0].Page =
                new NavigationPage(page);
        }

        // App Flow (WITH BACK)
        public static async Task Push(Page page)
        {
            var nav = Application.Current!.Windows[0].Page.Navigation;
            await nav.PushAsync(page);
        }
    }
}
