using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;

namespace DutchMe
{
    public partial class settings : PhoneApplicationPage
    {
        int cur, take, give;
        String cur_val="$", take_val="#FF0000", give_val="#00FF00";
        public class Data1
        {
            public string name { get; set; }
            public string color { get; set; }
        }
        public class Data
        {
            public string name { get; set; }
            public string symbol { get; set; }
        }
        int flag = 0;
        public settings()
        {
            InitializeComponent();
            done();
        }

        public IList<Settings> GetSettings()
        {
            IList<Settings> list = null;
            using (DutchMe.MainPage.DutchMeDataContext context = new DutchMe.MainPage.DutchMeDataContext(DutchMe.MainPage.DutchMeDataContext.DBConnectionString))
            {
                IQueryable<Settings> query = from c in context.sett select c;
                list = query.ToList();
            }

            return list;
        }
        public void done()
        {
            if (flag == 0)
            {
                var resourceStream = Application.GetResourceStream(new Uri("currency.txt", UriKind.RelativeOrAbsolute));
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

                            }
                            else
                            {
                                String[] temp = line.Split(' ');
                                Data n = new Data();
                                n.name = temp[0];
                                n.symbol = temp[1];
                                listPicker.Items.Add(n);

                            }
                        } while (line != null);
                    }
                }
                flag = 1;

                var resourceStream1 = Application.GetResourceStream(new Uri("colors.txt", UriKind.RelativeOrAbsolute));
                if (resourceStream1 != null)
                {
                    using (var reader = new StreamReader(resourceStream1.Stream))
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
                                Data1 n = new Data1();
                                n.color = temp[1];
                                n.name = temp[0];
                                listPicker1.Items.Add(n);
                                listPicker2.Items.Add(n);
                            }
                        }
                        while (line != null);
                    }
                }
            }
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
                    take = due.index;
                    take_val = due.value;
                }
                if (due.serial == 3)
                {
                    give = due.index;
                    give_val = due.value;
                }
            }
            listPicker.SelectedIndex = cur;
            listPicker1.SelectedIndex = take;
            listPicker2.SelectedIndex = give;
           
        }
        private void listPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Get the data object that represents the current selected item
            Data data = (sender as ListPicker).SelectedItem as Data;
            cur = listPicker.SelectedIndex;
            cur_val = data.symbol;
            //Get the selected ListPickerItem container instance    
            ListPickerItem selectedItem = this.listPicker.ItemContainerGenerator.ContainerFromItem(data) as ListPickerItem;
        }
        private void listPicker1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Get the data object that represents the current selected item
            Data1 data = (sender as ListPicker).SelectedItem as Data1;
            take = listPicker1.SelectedIndex;
            take_val = data.color;
            //Get the selected ListPickerItem container instance    
            ListPickerItem selectedItem = this.listPicker.ItemContainerGenerator.ContainerFromItem(data) as ListPickerItem;
        }
        private void listPicker2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Get the data object that represents the current selected item
            Data1 data = (sender as ListPicker).SelectedItem as Data1;
            give = listPicker2.SelectedIndex;
            give_val = data.color;
            //Get the selected ListPickerItem container instance    
            ListPickerItem selectedItem = this.listPicker.ItemContainerGenerator.ContainerFromItem(data) as ListPickerItem;
        }
        private void UpdateDutch(String entity_name, int index,String value)
        {
            using (DutchMe.MainPage.DutchMeDataContext context = new DutchMe.MainPage.DutchMeDataContext(DutchMe.MainPage.DutchMeDataContext.DBConnectionString))
            {
                
                IQueryable<Settings> entityQuery = from c in context.sett where c.name == entity_name select c;
                Settings entityToUpdate = entityQuery.FirstOrDefault();
                
                entityToUpdate.index = index;
                entityToUpdate.value = value;

                // save changes to the database
                context.SubmitChanges();
            }
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            UpdateDutch("Currency",cur,cur_val);
            UpdateDutch("Take", take, take_val);
            UpdateDutch("Give", give, give_val);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("All your data will be deleted and default setting will be restored. Are you sure to continue","DutchMe",MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.OK)
            {
                using (DutchMe.MainPage.DutchMeDataContext context = new DutchMe.MainPage.DutchMeDataContext(DutchMe.MainPage.DutchMeDataContext.DBConnectionString))
                {

                    if (context.DatabaseExists())
                    {
                        // create database if it does not exist
                        context.DeleteDatabase();
                        
                    }
                }
            }
        }

        
    }
}