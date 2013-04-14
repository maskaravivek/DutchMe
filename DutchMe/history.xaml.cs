using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Phone;
using System.Windows.Media.Imaging;

namespace DutchMe
{
    public partial class history : PhoneApplicationPage
    {
        public class Entities
        {
            public string name { get; set; }
            public string detail { get; set; }
            public string amount { get; set; }
            public string color { get; set; }
            public string category_color { get; set; }
            public string type { get; set; }
            public ImageSource img_name { get; set; }
        }
        String cur_val="$";
        public history()
        {
            InitializeComponent();
        }
        public IList<History> GetHistory()
        {
            IList<History> list = null;
            using (DutchMe.MainPage.DutchMeDataContext context = new DutchMe.MainPage.DutchMeDataContext(DutchMe.MainPage.DutchMeDataContext.DBConnectionString))
            {
                IQueryable<History> query = from c in context.Hist select c;
                list = query.ToList();
            }

            return list;
        }
        public IList<MeHistory> GetMeHist()
        {
            IList<MeHistory> list = null;
            using (DutchMe.MainPage.DutchMeDataContext context = new DutchMe.MainPage.DutchMeDataContext(DutchMe.MainPage.DutchMeDataContext.DBConnectionString))
            {
                IQueryable<MeHistory> query = from c in context.MeHist select c;
                list = query.ToList();
            }

            return list;
        }
        public IList<Category> GetList()
        {
            IList<Category> list = null;
            using (DutchMe.MainPage.DutchMeDataContext context = new DutchMe.MainPage.DutchMeDataContext(DutchMe.MainPage.DutchMeDataContext.DBConnectionString))
            {
                IQueryable<Category> query = from c in context.Cate select c;
                list = query.ToList();
            }

            return list;
        }
        private String color_to_use(String list_name_used)
        {
            String clr = "" + Colors.LightGray;
            IList<Category> ca = this.GetList();
            foreach (Category c in ca)
            {
                if (c.name.Equals(list_name_used))
                {
                    clr = c.cat_color;
                    break;
                }
            }
            return clr;
        }
        private void displayHistory(int name_cod,String list)
        {
            IList<History> eve = this.GetHistory();
            list1.Items.Clear();
            foreach (History hp in eve)
            {
                if (hp.name_code != name_cod)
                    continue;
                Entities hb = new Entities();
                //if (hp.take)
                //    hb.name = "from";
                //else
                //    hb.name = "to";


                String sb=hp.note;
                if (sb.Equals(""))
                {
                    if (hp.take)
                        sb = "from";
                    else
                        sb = "to";

                }
                String a;
                if (hp.transaction % 1.0 != 0.0)
                    a = hp.transaction.ToString("0.00");
                else
                    a = "" + hp.transaction;
                hb.name = sb;
                if (!hp.location.Equals("my location"))
                    hb.detail = hp.location + " on " + (string)hp.date;
                else
                    hb.detail = (string)hp.date;
                hb.amount = cur_val+""+a;
                if (hp.take)
                    hb.color = "" + Colors.Green;
                else
                    hb.color = "" + Colors.Red;
                try
                {
                    var isoFile = IsolatedStorageFile.GetUserStoreForApplication();
                    var imageStream = isoFile.OpenFile(hp.picture, FileMode.Open, FileAccess.Read);
                    var imageSource = PictureDecoder.DecodeJpeg(imageStream);
                    hb.img_name = imageSource;
                }
                catch
                {
                    BitmapImage image = new BitmapImage(new Uri("/ApplicationIcon.png", UriKind.Relative));
                    hb.img_name = image;
                }
                    hb.category_color = color_to_use(list);
                list1.Items.Add(hb);
            }
            progress.Visibility = System.Windows.Visibility.Collapsed;
        }
        private void displayHistory(bool stat)
        {
            IList<History> eve = this.GetHistory();
            list1.Items.Clear();
            foreach (History hp in eve)
            {
                if (hp.take != stat)
                    continue;
                Entities hb = new Entities();
                //if (hp.take)
                //    hb.name = "from";
                //else
                //    hb.name = "to";


                String sb = hp.note;
                if (sb.Equals(""))
                {
                    if (hp.take)
                        sb = "from";
                    else
                        sb = "to";

                }
                String a;
                if (hp.transaction % 1.0 != 0.0)
                    a = hp.transaction.ToString("0.00");
                else
                    a = "" + hp.transaction;
                hb.name = sb;
                if (!hp.location.Equals("my location"))
                    hb.detail = hp.location + " on " + (string)hp.date;
                else
                    hb.detail = (string)hp.date;
                hb.amount = cur_val + "" + a;
                if (hp.take)
                    hb.color = "" + Colors.Green;
                else
                    hb.color = "" + Colors.Red;
                try
                {
                    var isoFile = IsolatedStorageFile.GetUserStoreForApplication();
                    var imageStream = isoFile.OpenFile(hp.picture, FileMode.Open, FileAccess.Read);
                    var imageSource = PictureDecoder.DecodeJpeg(imageStream);
                    hb.img_name = imageSource;
                }
                catch
                {
                    BitmapImage image = new BitmapImage(new Uri("/ApplicationIcon.png", UriKind.Relative));
                    hb.img_name = image;
                }
                hb.category_color = color_to_use("friends");
                list1.Items.Add(hb);
            }
            progress.Visibility = System.Windows.Visibility.Collapsed;
        }
        private void displayMeHistory()
        {
            IList<MeHistory> eve = this.GetMeHist();
            list1.Items.Clear();
            foreach (MeHistory hp in eve)
            {
                
                Entities hb = new Entities();
                //if (hp.take)
                //    hb.name = "from";
                //else
                //    hb.name = "to";


                String sb = hp.note;
                if (sb.Equals(""))
                {
                        sb = "to";

                }
                hb.name = sb;
                String a;
                if (hp.transaction % 1.0 != 0.0)
                    a = hp.transaction.ToString("0.00");
                else
                    a = "" + hp.transaction;
                if (!hp.location.Equals("my location"))
                    hb.detail = hp.location + " on " + (string)hp.date;
                else
                    hb.detail = (string)hp.date;
                hb.amount = cur_val + "" + a;
                
                    hb.color = "" + Colors.Red;
                try
                {
                    var isoFile = IsolatedStorageFile.GetUserStoreForApplication();
                    var imageStream = isoFile.OpenFile(hp.picture, FileMode.Open, FileAccess.Read);
                    var imageSource = PictureDecoder.DecodeJpeg(imageStream);
                    hb.img_name = imageSource;
                }
                catch
                {
                    BitmapImage image = new BitmapImage(new Uri("/ApplicationIcon.png", UriKind.Relative));
                    hb.img_name = image;
                }
                hb.category_color = color_to_use("friends");
                list1.Items.Add(hb);
            }
            progress.Visibility = System.Windows.Visibility.Collapsed;
        }
        private void ContentPanel_Loaded(object sender, RoutedEventArgs e)
        {
            String cod=NavigationContext.QueryString["name"];
            if (!cod.StartsWith("."))
            {

                String[] temp = cod.Split('*');
                tit.Text = temp[1] + "\'s history";
                int co = Convert.ToInt32(temp[2]);
                cur_val = temp[0];
                displayHistory(co, temp[3]);
                
            }
            else
            {
                cod = cod.Substring(1);

                String[] temp = cod.Split('*');
                bool stat = true;
                if (temp[1].Equals("Others owe"))
                {
                    tit.Text = "Others owe me";
                    stat = true;
                    cur_val = temp[0];
                    displayHistory(stat);
                }
                else if (temp[1].Equals("I owe"))
                {
                    tit.Text = "I owe";
                    stat = false;
                    cur_val = temp[0];
                    displayHistory(stat);
                }
                else if (temp[1].Equals("Expenditure"))
                {
                    tit.Text = "Expenditure";
                    cur_val = temp[0];
                    displayMeHistory();
                }


                
            }
        }
    }
}