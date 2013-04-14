using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace DutchMe
{
    public partial class vivek : PhoneApplicationPage
    {
        public vivek()
        {
            InitializeComponent();
        }

        

       
     

        

        private void HubTile_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        private void HubTile_Tap_2(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShareLinkTask shareLinkTask = new ShareLinkTask();

            shareLinkTask.Title = "DutchMe";
            shareLinkTask.LinkUri = new Uri("http://www.windowsphone.com/s?appid=56a4ee96-f4e4-4c44-833a-775401025ddb", UriKind.Absolute);//change the link address
            shareLinkTask.Message = "Hey try this app and manage your dues with all your friends. A great way to manage your personal finance";

            shareLinkTask.Show();
        }

        private void HubTile_Tap_3(object sender, System.Windows.Input.GestureEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "DutchMe";

            emailComposeTask.To = "maskaravivek@hotmail.com";


            emailComposeTask.Show();
        }

        private void HubTile_Tap_4(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();

            webBrowserTask.Uri = new Uri("http://www.twitter.com/maskaravivek", UriKind.Absolute);

            webBrowserTask.Show();
        }

        private void HubTile_Tap_5(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();

            webBrowserTask.Uri = new Uri("http://www.facebook.com/pages/Devilsnare/263516920442834", UriKind.Absolute);

            webBrowserTask.Show();
        }

        private void HubTile_Tap_6(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MarketplaceSearchTask marketplaceSearchTask = new MarketplaceSearchTask();

            marketplaceSearchTask.SearchTerms = "devilsnare";

            marketplaceSearchTask.Show();
        }
    }
}