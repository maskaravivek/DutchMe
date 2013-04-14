using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using System.IO;
using Microsoft.Phone.Tasks;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using Microsoft.Phone;

namespace DutchMe
{
    public partial class MainPage : PhoneApplicationPage
    {
        CameraCaptureTask cameraCaptureTask;
        PhotoChooserTask photoChooserTask;
        int cur, tak, giv;
        String cur_val, take_val, give_val;
        String pic_name;
        int flag1 = 0,flag2=0,flag3=0,flag4=0,flag5=0;
        public class Data
        {
            public string color_name { get; set; }
            public string color1 { get; set; }
        }
        public class Entities
        {
            public string name { get; set; }
            public string date { get; set; }
            public string amount { get; set; }
            public string color { get; set; }
            public string category_color { get; set; }
            public string type { get; set; }
            public string message { get; set; }
        }
        String list_name="friends",list_color=""+Colors.Purple;
        // Constructor
        List<string> entity = new List<string>();
        public MainPage()
        {
            
            InitializeComponent();
           
            cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += new EventHandler<PhotoResult>(cameraCaptureTask_Completed);
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
            using (DutchMeDataContext context = new DutchMeDataContext(DutchMeDataContext.DBConnectionString))
            {
                
                if (!context.DatabaseExists())
                {
                    // create database if it does not exist
                    context.CreateDatabase();
                    AddList("friends",""+Colors.Purple);
                    AddList("foes", "" + Colors.Brown);
                    AddList("others", "" + Colors.Orange);
                    AddLocation("canteen");
                    AddLocation("college");
                    AddLocation("office");
                    AddMe("I owe", "" + DateTime.Now.ToLongDateString(), 0, false);
                    AddMe("Others owe", "" + DateTime.Now.ToLongDateString(), 0, true);
                    AddMe("Expenditure",""+DateTime.Now.ToLongDateString(),0,false);
                    AddSettings("Currency",146,"$");
                    AddSettings("Give", 23, "#FF0000");
                    AddSettings("Take", 16, "#00FF00");
                }
            }
           
            IList<Settings> e = this.GetSettings();

            foreach (Settings due in e)
            {
                if (due.serial == 1)
                {
                    cur = due.index;
                    cur_val = due.value;
                }
                if (due.serial == 2)
                {
                    tak = due.index;
                    take_val = due.value;
                }
                if (due.serial == 3)
                {
                    giv = due.index;
                    give_val = due.value;
                }
            }
            IList<Category> even = this.GetList();
            list1.Items.Clear();
            foreach (Category hp in even)
            {
                list1.Items.Add(hp.name);
            }
            displayEntity();
          
        }
        private void displayEntity()
        {
            IList<Dutch> eve = this.GetEntity();
            list2.Items.Clear();
            foreach (Dutch hp in eve)
            {
                String a;
                if (hp.balance % 1.0 != 0.0)
                    a = hp.balance.ToString("0.00");
                else
                    a = ""+hp.balance;
                Entities hb = new Entities();
                hb.name = (string)hp.name;
                hb.date = (string)hp.date;
                hb.amount =cur_val+""+ a;
                if (hp.take)
                    hb.color = ""+take_val;
                else
                    hb.color = "" + give_val;
                hb.category_color = color_to_use(hp.list);
                hb.message=cur_val+"*"+hp.name+"*"+hp.serial+"*"+hp.list;
                list2.Items.Add(hb);
            }
        }
        private void displayMe()
        {
            using (DutchMeDataContext context = new DutchMeDataContext(DutchMeDataContext.DBConnectionString))
            {
                if (context.DatabaseExists())
                {
                    IList<Me> eve = this.GetMe();
                    IList<Dutch> e = this.GetEntity();
                    double give = 0, take = 0;
                    foreach (Dutch due in e)
                    {
                        if (due.take)
                            take += due.balance;
                        else
                            give += due.balance;
                    }
                    UpdateMe("I owe", "" + DateTime.Now.ToLongDateString(), give, 1);
                    UpdateMe("Others owe", "" + DateTime.Now.ToLongDateString(), take, 1);
                    list10.Items.Clear();
                    foreach (Me hp in eve)
                    {
                        String a;
                        if (hp.amount % 1.0 != 0.0)
                            a = hp.amount.ToString("0.00");
                        else
                            a = "" + hp.amount;
                        Entities hb = new Entities();
                        hb.name = (string)hp.record;
                        hb.date = (string)hp.date;
                        hb.amount = cur_val + "" + a;
                        hb.message = "."+cur_val + "*" + hp.record;
                        if (hp.status)
                            hb.color = "" + take_val;
                        else
                            hb.color = "" + give_val;
                        hb.category_color = "Green";

                        list10.Items.Add(hb);
                    }
                }
            }
            
        }
        private void me()
        { 
        
        }
        private void AddEntity(String name,double balance,String date, bool take, String cate)
        {
            using (DutchMeDataContext context = new DutchMeDataContext(DutchMeDataContext.DBConnectionString))
            {
                Dutch du = new Dutch();
                du.name = name;
                du.balance = balance;
                du.date = date;
                du.take = take;
                du.list = cate;
                context.Entities.InsertOnSubmit(du);
                context.SubmitChanges();
            }
        }
        private void AddSettings(String name,int index,String value)
        {
            using (DutchMeDataContext context = new DutchMeDataContext(DutchMeDataContext.DBConnectionString))
            {
                Settings du = new Settings();
                du.name = name;
                du.index = index;
                du.value = value;
                context.sett.InsertOnSubmit(du);
                context.SubmitChanges();
            }
        }
        private void AddList(String name,String color)
        {
            using (DutchMeDataContext context = new DutchMeDataContext(DutchMeDataContext.DBConnectionString))
            {
                Category du = new Category();
                du.name = name;
                du.cat_color = color;
                context.Cate.InsertOnSubmit(du);
                context.SubmitChanges();
            }
        }
        private void AddMe(String name, String date,double amount,bool status)
        {
            using (DutchMeDataContext context = new DutchMeDataContext(DutchMeDataContext.DBConnectionString))
            {
                Me du = new Me();
                du.record = name;
                du.date = date;
                du.amount = amount;
                du.status = status;
                context.mine.InsertOnSubmit(du);
                context.SubmitChanges();
            }
        }
        public class DutchMeDataContext : DataContext
        {
            public static string DBConnectionString = "Data Source=isostore:/DutchMe.sdf";
            public DutchMeDataContext(string connectionString)
                : base(connectionString)
            { }
            public Table<Dutch> Entities
            {
                get
                {
                    return this.GetTable<Dutch>();
                }
            }
            public Table<Category> Cate
            {
                get
                {
                    return this.GetTable<Category>();
                }
            }
            public Table<History> Hist
            {
                get
                {
                    return this.GetTable<History>();
                }
            }
            public Table<MeHistory> MeHist
            {
                get
                {
                    return this.GetTable<MeHistory>();
                }
            }
            public Table<Me> mine
            {
                get
                {
                    return this.GetTable<Me>();
                }
            }
            public Table<Location> Location
            {
                get
                {
                    return this.GetTable<Location>();
                }
            }
            public Table<Settings> sett
            {
                get
                {
                    return this.GetTable<Settings>();
                }
            }

        }
        public IList<Category> GetList()
        {
            IList<Category> list = null;
            using (DutchMeDataContext context = new DutchMeDataContext(DutchMeDataContext.DBConnectionString))
            {
                IQueryable<Category> query = from c in context.Cate select c;
                list = query.ToList();
            }

            return list;
        }
        public IList<Settings> GetSettings()
        {
            IList<Settings> list = null;
            using (DutchMeDataContext context = new DutchMeDataContext(DutchMeDataContext.DBConnectionString))
            {
                IQueryable<Settings> query = from c in context.sett select c;
                list = query.ToList();
            }

            return list;
        }
        public IList<Me> GetMe()
        {
            IList<Me> list = null;
            using (DutchMeDataContext context = new DutchMeDataContext(DutchMeDataContext.DBConnectionString))
            {
                IQueryable<Me> query = from c in context.mine select c;
                list = query.ToList();
            }

            return list;
        }
        public IList<Location> GetLocation()
        {
            IList<Location> list = null;
            using (DutchMeDataContext context = new DutchMeDataContext(DutchMeDataContext.DBConnectionString))
            {
                IQueryable<Location> query = from c in context.Location select c;
                list = query.ToList();
            }

            return list;
        }
        public IList<Dutch> GetEntity()
        {
            IList<Dutch> list = null;
            using (DutchMeDataContext context = new DutchMeDataContext(DutchMeDataContext.DBConnectionString))
            {
                IQueryable<Dutch> query = from c in context.Entities select c;
                list = query.ToList();
            }

            return list;
        }
        private void TextBox_GotFocus_1(object sender, RoutedEventArgs e)
        {
            add.Text = String.Empty;
        }
        private void UpdateDutch(String entity_name,String date,double transaction,bool transaction_type)
        {
            using (DutchMeDataContext context = new DutchMeDataContext(DutchMeDataContext.DBConnectionString))
            {
                double amt=0;
                bool sta=true;
                IQueryable<Dutch> entityQuery = from c in context.Entities where c.name == entity_name select c;
                Dutch entityToUpdate = entityQuery.FirstOrDefault();
                amt = entityToUpdate.balance;
                sta = entityToUpdate.take;
                if (!sta)
                    amt *= -1;
                if (transaction_type)
                    amt += transaction;
                else
                    amt -= transaction;
                if (amt > 0)
                    sta = true;
                else
                {
                    sta = false;
                    amt *= -1;
                }
                    entityToUpdate.balance = amt;
                entityToUpdate.date = date;
                entityToUpdate.take = sta;

                // save changes to the database
                context.SubmitChanges();
            }
        }
        private void UpdateMe(String name,String date, double transaction)
        {
            using (DutchMeDataContext context = new DutchMeDataContext(DutchMeDataContext.DBConnectionString))
            {
                double amt = 0;
                IQueryable<Me> entityQuery = from c in context.mine where c.record == name select c;
                Me entityToUpdate = entityQuery.FirstOrDefault();
                amt = entityToUpdate.amount;
                amt += transaction;
                entityToUpdate.amount = amt;
                entityToUpdate.date = date;

                // save changes to the database
                context.SubmitChanges();
            }
        }
        private void UpdateMe(String name, String date, double transaction,int i)
        {
            using (DutchMeDataContext context = new DutchMeDataContext(DutchMeDataContext.DBConnectionString))
            {
                double amt = 0;
                IQueryable<Me> entityQuery = from c in context.mine where c.record == name select c;
                Me entityToUpdate = entityQuery.FirstOrDefault();
                entityToUpdate.amount = transaction;
                entityToUpdate.date = date;

                // save changes to the database
                context.SubmitChanges();
            }
        }
        private void ClearDutch(String entity_name, String date)
        {
            using (DutchMeDataContext context = new DutchMeDataContext(DutchMeDataContext.DBConnectionString))
            {
                
                IQueryable<Dutch> entityQuery = from c in context.Entities where c.name == entity_name select c;
                Dutch entityToUpdate = entityQuery.FirstOrDefault();
                entityToUpdate.balance = 0;
                entityToUpdate.date = date;

                // save changes to the database
                context.SubmitChanges();
            }
        }
        private void DeleteDutch(String entity_name)
        {
            using (DutchMeDataContext context = new DutchMeDataContext(DutchMeDataContext.DBConnectionString))
            {
                // find a city to delete
                IQueryable<Dutch> entityQuery = from c in context.Entities where c.name == entity_name select c;
                Dutch entityToDelete = entityQuery.FirstOrDefault();

                // delete city from the context
                context.Entities.DeleteOnSubmit(entityToDelete);

                // save changes to the database
                context.SubmitChanges();
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            canvas1.Visibility = System.Windows.Visibility.Collapsed;
        }
        private void AddHistory(int code, double trans, String date, bool take,String note,String picture,String loc)
        {
            using (DutchMeDataContext context = new DutchMeDataContext(DutchMeDataContext.DBConnectionString))
            {
                History du = new History();
                du.name_code = code;
                du.transaction =trans;
                du.date = date;
                du.note = note;
                du.picture = picture;
                du.take = take;
                du.location = loc;
                context.Hist.InsertOnSubmit(du);
                context.SubmitChanges();
            }
        }
        private void AddLocation(String loc)
        {
            using (DutchMeDataContext context = new DutchMeDataContext(DutchMeDataContext.DBConnectionString))
            {
                Location du = new Location();
                du.name = loc;
                context.Location.InsertOnSubmit(du);
                context.SubmitChanges();
            }
        }
        private void addMeHistory(double trans, String date,String note,String picture,String location)
        {
            using (DutchMeDataContext context = new DutchMeDataContext(DutchMeDataContext.DBConnectionString))
            {
                MeHistory du = new MeHistory();
                du.transaction = trans;
                du.date = date;
                du.note = note;
                du.picture = picture;
                du.location = location;
                context.MeHist.InsertOnSubmit(du);
                context.SubmitChanges();
            }
        }
        private int getNameCode(String name)
        {
            int cod=1;
            IList<Dutch> ca = this.GetEntity();
            foreach (Dutch c in ca)
            {
                if (c.name.Equals(name))
                {
                    cod = c.serial;
                    break;
                }
            }
            return cod;
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                String nam = add.Text;
                double amt = Convert.ToDouble(amount.Text);
                bool stat = true;
                DateTime dt = date.Value.Value;
                TimeSpan tym = time.Value.Value.TimeOfDay;
                dt = dt.Date + tym;
                String dat = "" + dt.ToLongDateString();
                //String l = location_add.Text;
                String l = ""+list_loc_p.SelectedItem;
                if (give.IsChecked == true)
                    stat = false;
                if (!add.Text.Equals(""))
                    AddEntity(nam, amt, dat, stat, list_name);
                AddHistory(getNameCode(nam), amt, dat, stat,note_p.Text,pic_name,l);
                canvas1.Visibility = System.Windows.Visibility.Collapsed;
                displayEntity();
                displayMe();
                note_p.Text = String.Empty;
                pic_name = "";
            }
            catch
            {
                MessageBox.Show("There was some error processing your request","Error",MessageBoxButton.OK);
            }
        }
        private String color_to_use(String list_name_used)
        {
            String clr=""+Colors.LightGray;
            IList < Category > ca= this.GetList();
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
        private List<string> list_of_people(String list_name_used_here)
        {
            List<string> peo = new List<string>();
            IList<Dutch> ca = this.GetEntity();
            foreach (Dutch c in ca)
            {
                if (c.list.Equals(list_name_used_here))
                    peo.Add((string)c.name);
            }
            return peo;
        }
        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            canvas1.Visibility = System.Windows.Visibility.Visible;
            if(flag1==0)
            updateLocationList();
            flag1=1;
        }

        private void TextBox_GotFocus_2(object sender, RoutedEventArgs e)
        {
            amount.Text = String.Empty;
        }

        private void list1_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            list_name = list1.SelectedValue.ToString();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            IList<Category> even = this.GetList();
            list3.Items.Clear();
            
            foreach (Category hp in even)
            {
                Entities dat = new Entities();
                dat.name = hp.name;
                dat.type = "list";
                list3.Items.Add(dat);
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            IList<Dutch> eve = this.GetEntity();
            list3.Items.Clear();
            
            foreach (Dutch hp in eve)
            {
                Entities dat = new Entities();
                dat.name = hp.name;
                dat.type = "entity";
                list3.Items.Add(dat);
            }
            Entities data = new Entities();
            data.name = "me";
            data.type = "my";
            list3.Items.Add(data);
        }

        private void Button_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
             System.Windows.Media.Color backgroundColor = (System.Windows.Media.Color)Application.Current.Resources["PhoneAccentColor"];
            Button click = (Button)sender;
            click.Background = new System.Windows.Media.SolidColorBrush(backgroundColor);

            if (click.Tag.ToString().Equals("entity") || click.Tag.ToString().Equals("my"))
            {
                entity.Add("" + click.Content);
                list4.Items.Add("" + click.Content);
            }
            else if (click.Tag.ToString().Equals("list"))
            {
                List<string> dome = list_of_people("" + click.Content);
                for (int i = 0; i < dome.Count; i++)
                {
                    entity.Add(dome.ElementAt(i));
                    list4.Items.Add("" + dome.ElementAt(i));
                }
                
            }
            click.Visibility = System.Windows.Visibility.Collapsed;
            stack1.Visibility = System.Windows.Visibility.Visible;
            ad.Visibility = System.Windows.Visibility.Visible;
        }

        private void Button_Tap_2(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Button click = (Button)sender;
            if(entity.Count==0)
                stack1.Visibility = System.Windows.Visibility.Collapsed;
            MessageBoxResult res=MessageBox.Show("Do you want to remove the item","Remove item",MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.OK)
            {
                entity.Remove(""+click.Content);
                click.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            canvas2.Visibility = System.Windows.Visibility.Collapsed;
            stack2.Visibility = System.Windows.Visibility.Collapsed;
            
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            try
            {
                bool status = true;
                if (ta.IsChecked == true)
                    status = true;
                else if (gi.IsChecked == true)
                    status = false;
                double amt = Convert.ToDouble(update.Text);
                DateTime dt = date_add.Value.Value;
                TimeSpan tym = time_add.Value.Value.TimeOfDay;
                dt = dt.Date + tym;
                String dat = "" + dt.ToLongDateString();
                //String l = location3.Text;
                String l = ""+list_loc_p1.SelectedItem;
                AddHistory(getNameCode(nam.Text), amt, dat, status,note_p.Text,pic_name,l);
                UpdateDutch(nam.Text, dat, amt, status);
                displayMe();
                note_p.Text = String.Empty;
                pic_name = "";
            }
            catch
            {
                MessageBox.Show("There was some error processing your request","Error",MessageBoxButton.OK );
            }
            displayEntity();
            //stack2.Visibility = System.Windows.Visibility.Collapsed;
            //stack7.Visibility = System.Windows.Visibility.Collapsed;
            //stack3.Visibility = System.Windows.Visibility.Collapsed;
            canvas2.Visibility = System.Windows.Visibility.Collapsed;
            
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            //stack3.Visibility = System.Windows.Visibility.Visible;
            //stack2.Visibility = System.Windows.Visibility.Visible;
            //stack7.Visibility = System.Windows.Visibility.Visible;
        }

        private void TextBlock_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TextBlock click=(TextBlock)sender;
            canvas2.Visibility = System.Windows.Visibility.Visible;
            nam.Text = click.Text;
            if(flag2==0)
            updateLocationList1();
            flag2 = 1;
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            stack5.Visibility = System.Windows.Visibility.Visible;
        }

        private void dutch_amt_GotFocus(object sender, RoutedEventArgs e)
        {
            dutch_amt.Text = String.Empty;
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            canvas3.Visibility = System.Windows.Visibility.Collapsed;
            canvas1.Visibility = System.Windows.Visibility.Visible;
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            String nam = list.Text;
            String color = list_color;
            AddList(nam, color);
            list1.Items.Add(nam);
            canvas3.Visibility = System.Windows.Visibility.Collapsed;
            canvas1.Visibility = System.Windows.Visibility.Visible;
        }

       

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            var resourceStream = Application.GetResourceStream(new Uri("colors.txt", UriKind.RelativeOrAbsolute));
            if (resourceStream != null)
            {
                using (var reader = new StreamReader(resourceStream.Stream))
                {
                    string line;
                    do
                    {
                        line = reader.ReadLine();
                        if (string.IsNullOrEmpty(line))
                        {
                            continue;
                        }
                        else
                        {

                            String[] temp = line.Split('\t');
                            Data n = new Data();
                            n.color1 = temp[1];
                            n.color_name = temp[0];
                            list5.Items.Add(n);

                        }
                    }
                    while (line != null);
                }
            }
            canvas3.Visibility = System.Windows.Visibility.Visible;
            canvas1.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void TextBlock_Tap_2(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TextBlock click = (TextBlock)sender;
            list_color = ""+click.Tag;
        }

        private void Rectangle_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            list_color = "" + rect.Fill;
        }

        private void TextBlock_Tap_3(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TextBlock click = (TextBlock)sender;
            
            MessageBoxResult res = MessageBox.Show("Do you wish to see the transaction history?","history",MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.OK)
            {

                NavigationService.Navigate(new Uri("/history.xaml?name=" + click.Tag, UriKind.Relative));
            }
        }

        private void Button_Click_12(object sender, RoutedEventArgs e)
        {
            stack4.Visibility = System.Windows.Visibility.Visible;
            stack6.Visibility = System.Windows.Visibility.Visible;
        }

        private void Button_Click_13(object sender, RoutedEventArgs e)//dutch button
        {
            try
            {
                bool stat=true;
                if(d_give.IsChecked==true)
                    stat=false;
                double dut = Convert.ToDouble(dutch_amt.Text);
                dut /= entity.Count;
                DateTime dt = d_date.Value.Value;
                TimeSpan tym = d_time.Value.Value.TimeOfDay;
                dt = dt.Date + tym;
                String dat = "" + dt.ToLongDateString();
                //String l = location1.Text;
                String l = ""+list_loc_d.SelectedItem;
                for (int i = 0; i < entity.Count; i++)
                {
                    if (!entity.ElementAt(i).Equals("me"))
                    {
                        AddHistory(getNameCode(entity.ElementAt(i)), dut, dat, stat, note_d.Text, pic_name, l);
                        UpdateDutch(entity.ElementAt(i), dat, dut, stat);
                    }
                    else
                    {
                        
                        if (stat)
                            UpdateMe("Others owe", dat, dut);
                        else
                        {
                            UpdateMe("Expenditure", dat, dut);
                            UpdateMe("I owe", dat, dut);
                        }
                        addMeHistory(dut, dat, note.Text, pic_name, l);
                    }
                }
                displayEntity();
                displayMe();
                MessageBox.Show("The amount was dutched among people", "DutchMe", MessageBoxButton.OK);
                list3.Items.Clear();
                list4.Items.Clear();
                note_d.Text = String.Empty;
                pic_name = "";
                ad.Visibility = System.Windows.Visibility.Collapsed;
                stack1.Visibility = System.Windows.Visibility.Collapsed;
                stack5.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch
            {
                MessageBox.Show("There was some error in processing your request","DutchMe",MessageBoxButton.OK);
            }
            }

        private void MenuItem_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MenuItem click = (MenuItem)sender;
            DeleteDutch("" + click.Tag);
            displayEntity();
        }

        private void MenuItem_Tap_2(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MenuItem click = (MenuItem)sender;
            NavigationService.Navigate(new Uri("/history.xaml?name=" + click.Tag, UriKind.Relative));
        }

        private void MenuItem_Tap_3(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MenuItem click = (MenuItem)sender;
            DateTime dt =DateTime.Now;
            String dat = "" + dt.ToLongDateString();
                
                    double amt = 0;
                    bool sta = true;
                using (DutchMeDataContext context = new DutchMeDataContext(DutchMeDataContext.DBConnectionString))
                {
                    
                    IQueryable<Dutch> entityQuery = from c in context.Entities where c.name == ""+click.Tag select c;
                    Dutch entityToUpdate = entityQuery.FirstOrDefault();
                    amt = entityToUpdate.balance;
                    sta = entityToUpdate.take;
                }
            if(sta)
                sta=false;
            else
                sta=true;
            AddHistory(getNameCode("" + click.Tag), amt, dat, sta,"clear dues",pic_name,"my location");
            ClearDutch("" + click.Tag, dat);
            displayEntity();
        }

        private void ApplicationBarMenuItem_Click_1(object sender, EventArgs e)//share link task
        {
            ShareLinkTask shareLinkTask = new ShareLinkTask();

            shareLinkTask.Title = "DutchMe";
            shareLinkTask.LinkUri = new Uri("http://www.windowsphone.com/s?appid=82408a50-be43-4b79-9c73-5f88159e1392", UriKind.Absolute);//change the link address
            shareLinkTask.Message = "Hey try this useful app which lets me edit HTML files on the go. It also has a great tutorial section with loads of examples and a color picker to help you find the right ones";

            shareLinkTask.Show();
        }

        private void ApplicationBarMenuItem_Click_2(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/vivek.xaml",UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click_2(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        private void ApplicationBarMenuItem_Click_3(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/how.xaml", UriKind.Relative));
        }
        private void SaveToIsolatedStorage(Stream imageStream, string fileName)
        {
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                
                IsolatedStorageFileStream fileStream = myIsolatedStorage.CreateFile(fileName);
                BitmapImage bitmap = new BitmapImage();
                bitmap.SetSource(imageStream);

                WriteableBitmap wb = new WriteableBitmap(bitmap);
                wb.SaveJpeg(fileStream, wb.PixelWidth, wb.PixelHeight, 0, 85);
                fileStream.Close();
            }
        }
        private void ReadFromIsolatedStorage(string fileName)
        {
            WriteableBitmap bitmap = new WriteableBitmap(200, 200);
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile(fileName, FileMode.Open, FileAccess.Read))
                {
                    // Decode the JPEG stream.
                    bitmap = PictureDecoder.DecodeJpeg(fileStream);
                }
            }
            this.picture.Source = bitmap;
        }
        private void amt_spent_GotFocus_1(object sender, RoutedEventArgs e)
        {
            amt_spent.Text = String.Empty;
        }
        void cameraCaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                MessageBox.Show(e.ChosenPhoto.Length.ToString());

                //Code to display the photo on the page in an image control named myImage.
                System.Windows.Media.Imaging.BitmapImage bmp = new System.Windows.Media.Imaging.BitmapImage();
                bmp.SetSource(e.ChosenPhoto);
                if (canvas4.Visibility == System.Windows.Visibility.Visible)
                    picture.Source = bmp;
                else if (canvas5.Visibility == System.Windows.Visibility.Visible)
                    picture_d.Source = bmp;
                if (canvas6.Visibility == System.Windows.Visibility.Visible)
                    picture_p.Source = bmp;
                pic_name = "" + DateTime.Now.Month + "" + DateTime.Now.Hour + "" + DateTime.Now.Minute + "" + DateTime.Now.Second;
                SaveToIsolatedStorage(e.ChosenPhoto, "" + pic_name);
            }
        }
        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                MessageBox.Show(e.ChosenPhoto.Length.ToString());

                //Code to display the photo on the page in an image control named myImage.
                System.Windows.Media.Imaging.BitmapImage bmp = new System.Windows.Media.Imaging.BitmapImage();
                bmp.SetSource(e.ChosenPhoto);
                if(canvas4.Visibility==System.Windows.Visibility.Visible)
                picture.Source = bmp;
                else if (canvas5.Visibility == System.Windows.Visibility.Visible)
                    picture_d.Source = bmp;
                if (canvas6.Visibility == System.Windows.Visibility.Visible)
                    picture_p.Source = bmp;
                pic_name = "" + DateTime.Now.Month + "" + DateTime.Now.Hour + "" + DateTime.Now.Minute + "" + DateTime.Now.Second;
                SaveToIsolatedStorage(e.ChosenPhoto, "" + pic_name);
            }
        }
        private void Button_Click_14(object sender, RoutedEventArgs e)
        {
            canvas4.Visibility = System.Windows.Visibility.Collapsed;
        }// cancel in canvas 4

        private void Button_Click_15(object sender, RoutedEventArgs e)//add in canvas 4
        {
            
            canvas4.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Button_Click_16(object sender, RoutedEventArgs e)
        {
            cameraCaptureTask.Show();
        }

        private void Button_Click_17(object sender, RoutedEventArgs e)
        {
            photoChooserTask.Show();
        }

        private void Button_Click_18(object sender, RoutedEventArgs e)
        {
            stack8.Visibility = System.Windows.Visibility.Visible;
        }

        private void Button_Tap_3(object sender, System.Windows.Input.GestureEventArgs e)
        {
            canvas4.Visibility = System.Windows.Visibility.Visible;
           
        }

        private void ad_me_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String not = note.Text;
                double amt = Convert.ToDouble(amt_spent.Text);
                DateTime dt = me_date.Value.Value;
                TimeSpan tym = me_time.Value.Value.TimeOfDay;
                dt = dt.Date + tym;
                String date = "" + dt.ToLongDateString();
                //String loc = location.Text;
                String loc = ""+list_loc_m.SelectedItem;
                addMeHistory(amt, date, not, pic_name,loc);
                UpdateMe("Expenditure", date, amt);
                displayMe();
                note.Text = String.Empty;
                pic_name = "";
            }
            catch
            {
                MessageBox.Show("Some error occured in processing your request","Error",MessageBoxButton.OK);
            }
            }

        private void location_GotFocus_1(object sender, RoutedEventArgs e)
        {
            //location.Text = String.Empty;
        }

        private void location_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void ApplicationBarMenuItem_Click_4(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/settings.xaml",UriKind.Relative));
            
        }

        private void Image_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if(stack9.Visibility==System.Windows.Visibility.Collapsed)
            stack9.Visibility = System.Windows.Visibility.Visible;
            else if (stack9.Visibility == System.Windows.Visibility.Visible)
                stack9.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Image_Tap_2(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (stack4.Visibility == System.Windows.Visibility.Collapsed)
                stack4.Visibility = System.Windows.Visibility.Visible;
            else if (stack4.Visibility == System.Windows.Visibility.Visible)
                stack4.Visibility = System.Windows.Visibility.Collapsed;
            img.Visibility = System.Windows.Visibility.Visible;
        }

        private void Image_Tap_3(object sender, System.Windows.Input.GestureEventArgs e)
        {
            canvas6.Visibility = System.Windows.Visibility.Visible;
        }

        private void Image_Tap_4(object sender, System.Windows.Input.GestureEventArgs e)
        {
            cameraCaptureTask.Show();
            Image img = (Image)sender;
            img.Opacity = 100;
        }

        private void Image_Tap_5(object sender, System.Windows.Input.GestureEventArgs e)
        {
            canvas4.Visibility = System.Windows.Visibility.Visible;
        }

        private void Image_Tap_6(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (stack8.Visibility == System.Windows.Visibility.Collapsed)
            {
                if(flag5==0)
                updateLocationList3();
                flag5 = 1;
                stack8.Visibility = System.Windows.Visibility.Visible;
            }
            else if (stack8.Visibility == System.Windows.Visibility.Visible)
                stack8.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Image_Tap_7(object sender, System.Windows.Input.GestureEventArgs e)
        {
            canvas5.Visibility = System.Windows.Visibility.Visible;
        }

        private void Button_Click_19(object sender, RoutedEventArgs e)
        {
            canvas6.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Button_Click_20(object sender, RoutedEventArgs e)
        {
            canvas6.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Button_Click_21(object sender, RoutedEventArgs e)
        {
            canvas5.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Button_Click_22(object sender, RoutedEventArgs e)
        {
            canvas5.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Image_Tap_8(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (stack2.Visibility == System.Windows.Visibility.Visible)
                stack2.Visibility = System.Windows.Visibility.Collapsed;
            else if (stack2.Visibility == System.Windows.Visibility.Collapsed)
                stack2.Visibility = System.Windows.Visibility.Visible;
        }

        private void Image_Tap_9(object sender, System.Windows.Input.GestureEventArgs e)
        {
            canvas6.Visibility = System.Windows.Visibility.Visible;
        }

        private void Image_Tap_10(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (stack5.Visibility == System.Windows.Visibility.Visible)
                stack5.Visibility = System.Windows.Visibility.Collapsed;
            else if (stack5.Visibility == System.Windows.Visibility.Collapsed)
            {
                stack5.Visibility = System.Windows.Visibility.Visible;
                if(flag3==0)
                updateLocationList2();
                flag3 = 1;
            }
        }

        private void Button_Click_23(object sender, RoutedEventArgs e)
        {
            canvas7.Visibility = System.Windows.Visibility.Visible;
        }
        private void updateLocationList()
        {
            List<string> loc = new List<string>();
            IList<Location> eve = this.GetLocation();
            foreach (Location hp in eve)
            {
                loc.Add(hp.name);
            }
            list_loc_p.ItemsSource = loc;
        }
        private void updateLocationList2()
        {
            
            IList<Location> eve = this.GetLocation();
            foreach (Location hp in eve)
            {
                list_loc_d.Items.Add(hp.name);
            }
        }
        private void updateLocationList3()
        {
            
            IList<Location> eve = this.GetLocation();
            foreach (Location hp in eve)
            {
                list_loc_m.Items.Add(hp.name);
            }
        }
        private void updateLocationList1()
        {
            
            IList<Location> eve = this.GetLocation();
            foreach (Location hp in eve)
            {
                list_loc_p1.Items.Add(hp.name);
            }
        }
        private void Button_Click_24(object sender, RoutedEventArgs e)
        {
            canvas7.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Button_Click_25(object sender, RoutedEventArgs e)
        {
            String l = loc_vvk.Text;
            if(l!=null)
            AddLocation(l);
            if (l == null)
            {
                AddLocation("my location");
            }
            if (canvas1.Visibility == System.Windows.Visibility.Visible)
            {
                updateLocationList();
            }
            else if (canvas3.Visibility == System.Windows.Visibility.Visible)
                updateLocationList1();
            canvas7.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void PivotItem_Loaded_1(object sender, RoutedEventArgs e)
        {
            displayMe();
        }

        private void ApplicationBarIconButton_Click_3(object sender, EventArgs e)
        {
            IList<Settings> en = this.GetSettings();

            foreach (Settings due in en)
            {
                if (due.serial == 1)
                {
                    cur = due.index;
                    cur_val = due.value;
                }
                if (due.serial == 2)
                {
                    tak = due.index;
                    take_val = due.value;
                }
                if (due.serial == 3)
                {
                    giv = due.index;
                    give_val = due.value;
                }
            }
            displayEntity();
            displayMe();
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            IList<Settings> en = this.GetSettings();

            foreach (Settings due in en)
            {
                if (due.serial == 1)
                {
                    cur = due.index;
                    cur_val = due.value;
                }
                if (due.serial == 2)
                {
                    tak = due.index;
                    take_val = due.value;
                }
                if (due.serial == 3)
                {
                    giv = due.index;
                    give_val = due.value;
                }
            }
            displayEntity();
            displayMe();
        }

        

       
        
    }
    [Table]
    public class  Dutch
    {
        [Column(IsDbGenerated = true, IsPrimaryKey = true)]
        public int serial
        {
            get;
            set;
        }
        [Column]
        public string name
        {
            get;
            set;
        }
        [Column]
        public double balance
        {
            get;
            set;
        }
        [Column]
        public bool take
        {
            get;
            set;
        }
        [Column]
        public string date
        {
            get;
            set;
        }
        [Column]
        public string list
        {
            get;
            set;
        }
    }
    [Table]
    public class History
    {
        [Column(IsDbGenerated = true, IsPrimaryKey = true)]
        public int serial
        {
            get;
            set;
        }
        [Column]
        public int name_code
        {
            get;
            set;
        }
        [Column]
        public double transaction
        {
            get;
            set;
        }
        [Column]
        public bool take
        {
            get;
            set;
        }
        [Column]
        public string date
        {
            get;
            set;
        }
        [Column]
        public string picture
        {
            get;
            set;
        }
        [Column]
        public string note
        {
            get;
            set;
        }
        [Column]
        public string location
        {
            get;
            set;
        }
        
    }
    [Table]
    public class MeHistory
    {
        [Column(IsDbGenerated = true, IsPrimaryKey = true)]
        public int serial
        {
            get;
            set;
        }
        [Column]
        public double transaction
        {
            get;
            set;
        }
        [Column]
        public string date
        {
            get;
            set;
        }
        [Column]
        public string picture
        {
            get;
            set;
        }
        [Column]
        public string note
        {
            get;
            set;
        }
        [Column]
        public string location
        {
            get;
            set;
        }

    }
    [Table]
    public class Me
    {
        [Column(IsDbGenerated = true, IsPrimaryKey = true)]
        public int serial
        {
            get;
            set;
        }
        [Column]
        public string record
        {
            get;
            set;
        }
        [Column]
        public string date
        {
            get;
            set;
        }
        [Column]
        public double amount
        {
            get;
            set;
        }
        [Column]
        public bool status
        {
            get;
            set;
        }

    }
    [Table]
    public class Category
    {
        [Column(IsPrimaryKey=true,IsDbGenerated=true)]
        public int serial
        {
            get;
            set;
        }
        [Column]
        public string name
        {
            get;
            set;
        }
        [Column]
        public string cat_color
        {
            get;
            set;
        }
    }
    [Table]
    public class Location
    {
        [Column(IsDbGenerated = true, IsPrimaryKey = true)]
        public int serial
        {
            get;
            set;
        }
        [Column]
        public string name
        {
            get;
            set;
        }
    }
    [Table]
    public class Settings
    {
        [Column(IsDbGenerated = true, IsPrimaryKey = true)]
        public int serial
        {
            get;
            set;
        }
        [Column]
        public String name
        {
            get;
            set;
        }
        [Column]
        public int index
        {
            get;
            set;
        }
        [Column]
        public String value
        {
            get;
            set;
        }
    }
}
