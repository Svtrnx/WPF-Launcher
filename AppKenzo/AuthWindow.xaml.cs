using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Management;
using System.Threading;
using System.Net;
using MaterialDesignThemes.Wpf;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;

namespace AppKenzo
{
    /// <summary>
    /// </summary>
    /// 
        
    public partial class AuthWindow : Window
    {
        DataBase dataBase = new DataBase();

        DataTable table = new DataTable();

        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public AuthWindow()
        {

            InitializeComponent();

            if (isCorrectVersion())
            {
                return;
            }

            //Async(State)
            asyncStateWork();



            /*RegistrationWindow registrationWindow = new RegistrationWindow();
            string textRegKey = registrationWindow.textKeyBox.Text;
            MessageBox.Show(textRegKey);*/

        }


        //Asynchronous function for version state
        async void asyncStateWork()
        {
            await Task.Run(() =>
            {
                while (userData.need)
                {
                    Thread.Sleep(10000);
                    if (isCorrectVersion())
                    {
                        return;
                    }
                }
            });
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {

            string loginUser = textBoxLogin.Text;
            string passUser = textBoxPassword.Password.ToString();

            string checkHWID = "";
            string checkPcName = "";

            //HWID
            var mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
            ManagementObjectCollection mbsList = mbs.Get();
            string idUser = "";
            foreach (ManagementObject mo in mbsList)
            {
                idUser = mo["ProcessorId"].ToString();
                break;
            }

            //MessageBox.Show(idUser.ToString());

            if (Regex.IsMatch(loginUser, @"\p{IsCyrillic}") || loginUser.Length < 5)
            {
               textBoxLogin.Background = Brushes.Red;
               textBoxLogin.ToolTip = "This field is not entered correctly!";
               MessageBox.Show("This field is not entered correctly!");
                return;
            }

            if (Regex.IsMatch(passUser, @"\p{IsCyrillic}") || passUser.Length < 6)
            {
                textBoxPassword.Background = Brushes.Red;
                textBoxPassword.ToolTip = "This field is not entered correctly!";
                MessageBox.Show("This field is not entered correctly!");
                return;
            }
            
            
            MainMenu mainMenu = new MainMenu();

            DataBase dataBase = new DataBase();

            DataTable table = new DataTable();

            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand command = new MySqlCommand("SELECT * FROM `users` WHERE `login` = @uL AND `pass` = @uP", dataBase.getConnection());
            
            //checking HWID
            MySqlCommand commandToCheckHWID = new MySqlCommand("SELECT `HWID` FROM `users` WHERE `login` = @ulHWID AND `pass` = @upHWID", dataBase.getConnection());
            commandToCheckHWID.Parameters.Add("@ulHWID", MySqlDbType.VarChar).Value = loginUser;
            commandToCheckHWID.Parameters.Add("@upHWID", MySqlDbType.VarChar).Value = passUser;

            dataBase.openConnection();
            MySqlDataReader readerHWID = commandToCheckHWID.ExecuteReader();

            while (readerHWID.Read())
            {
                string mP = (string)readerHWID[0];

                checkHWID = mP;
            }
            readerHWID.Close();
            //PC name check
            MySqlCommand commandToCheckPcName = new MySqlCommand("SELECT `NameOfPC` FROM `users` WHERE `login` = @ulPC AND `pass` = @upPC" +
             " AND `HWID` = @hwidPC", dataBase.getConnection());
            commandToCheckPcName.Parameters.Add("@hwidPC", MySqlDbType.VarChar).Value = checkHWID;
            commandToCheckPcName.Parameters.Add("@ulPC", MySqlDbType.VarChar).Value = loginUser;
            commandToCheckPcName.Parameters.Add("@upPC", MySqlDbType.VarChar).Value = passUser;

            dataBase.openConnection();
            MySqlDataReader readerPC = commandToCheckPcName.ExecuteReader();
            while (readerPC.Read())
            {
                string PC = (string)readerPC[0];
                checkPcName = PC;
            }
            readerPC.Close();

            //Log and pass check
            if (idUser == checkHWID == true || userData.pcUName == checkPcName == true)
            {

            }
            else
            {
                Close();
                MessageBox.Show("Incorrect Login or Password\nERROR 701");
                Environment.Exit(0);
            }



            command.Parameters.Add("@uL", MySqlDbType.VarChar).Value = loginUser;
            command.Parameters.Add("@uP", MySqlDbType.VarChar).Value = passUser;
            

  

            adapter.SelectCommand = command;
            adapter.Fill(table);

            if(table.Rows.Count > 0)
            {
                mainMenu.Show();
                Hide();
                Close();
                Console.Beep(300, 400);
            }
            else
            {
                MessageBox.Show("Error in authorization!");
                return;
                /*Close();
                System.Windows.Application.Current.Shutdown();*/
            }


            //update ip
            MySqlCommand commandToUpdateIp = new MySqlCommand("UPDATE `users` SET `userIP`= @updateIp", dataBase.getConnection());
            commandToUpdateIp.Parameters.Add("@updateIp", MySqlDbType.VarChar).Value = userData.pubIp;
            dataBase.openConnection();
            try
            {
                adapter.SelectCommand = commandToUpdateIp;
                adapter.Fill(table);
                dataBase.closeConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
                Environment.Exit(0);
            }
        }

        public Boolean isCorrectVersion()
        {
            DataBase dataBase = new DataBase();

            DataTable table = new DataTable();

            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand command = new MySqlCommand("SELECT * FROM `versions` WHERE `state` = 1", dataBase.getConnection());

            adapter.SelectCommand = command;
            adapter.Fill(table);

            //Поток на version state

            Thread thread = new Thread(asyncStateWork);
            thread.Start();

            if (table.Rows.Count > 0)
            {
                //MessageBox.Show("Верная версия!");
                return false;
            }
            else
            {
                userData.need = false;
                thread.Abort();
                this.Dispatcher.Invoke((Action)(() =>  //Window.Dispatcher
                {
                    Close();
                    System.Windows.Application.Current.Shutdown();
                    MessageBox.Show("Your launcher version is outdated!\nDownload a new one!");
                    Environment.Exit(0);
                }));
                Environment.Exit(0);
                return true;
            }
        }



        private void ButtonCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
            Environment.Exit(0);

        }

        private void ButtonHelpClick_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://t.me/YoungKENZO");
        }


        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void textBoxLogin_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            RegistrationWindow registrationWindow = new RegistrationWindow();
            registrationWindow.Show();
            Close();
        }
    }
}
