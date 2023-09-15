using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;
using System.Threading;
using Google.Protobuf.WellKnownTypes;
using System.Management;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace AppKenzo
{

    public partial class MainMenu : Window
    {
        string userL = "";
        string userP = "";
        string userKey = "";
        DateTime dateTime = new DateTime();

    public MainMenu()
        {
            try
            {
                InitializeComponent();

                Main();

                if (isCorrectVersion())
                {

                    return;
                }
                //Async(State)
                asyncStateWork();
            }
            catch (Exception ex)
            {

                MessageBox.Show("[100]Exception: " + ex.Message);
            }
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

        private void ButtonCloseApplication_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
            Environment.Exit(0);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
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

            ////HWID
            //var mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
            //ManagementObjectCollection mbsList = mbs.Get();
            //string idUser = "";
            //foreach (ManagementObject mo in mbsList)
            //{
            //    idUser = mo["ProcessorId"].ToString();
            //    break;
            //}


            //MySqlCommand commandToCheckSub = new MySqlCommand("SELECT `login`, `pass`, `hwid`, `NameOfPC` FROM `users` WHERE `login` = @uL " +
            //    "AND `pass` = @uP AND `HWID` = @hwid AND `NameOfPC` = @pcname", dataBase.getConnection());
            //commandToCheckSub.Parameters.Add("@uL", MySqlDbType.VarChar).Value = userL;
            //commandToCheckSub.Parameters.Add("@uP", MySqlDbType.VarChar).Value = userP;
            //commandToCheckSub.Parameters.Add("@hwid", MySqlDbType.VarChar).Value = idUser;
            //commandToCheckSub.Parameters.Add("@pcname", MySqlDbType.VarChar).Value = userData.pcUName;


            //dataBase.openConnection();
            //MySqlDataReader readerCheckSub = commandToCheckSub.ExecuteReader();
            //try
            //{
            //    if (readerCheckSub.HasRows == true)
            //    {

            //    }
            //    else
            //    {
            //        userData.need = false;
            //        thread.Abort();
            //        this.Dispatcher.Invoke((Action)(() =>  //Window.Dispatcher
            //        {
            //            this.Close();
            //            readerCheckSub.Close();
            //            MessageBox.Show("ERROR 703", "Error");
            //            Environment.Exit(0);
            //        }));
            //    }
            //    readerCheckSub.Close();
            //}
            //catch (Exception ex)
            //{
            //    thread.Abort();
            //    this.Close();
            //    MessageBox.Show("[101]Exception: " + ex.Message);
            //    Environment.Exit(0);
            //}
            


            if (table.Rows.Count > 0)
            {
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
                    MessageBox.Show("Версия вашего лаунчера устарела!\nСкачайте новую!");
                    Environment.Exit(0);
                }));
                Environment.Exit(0);
                return true;
            }
        }

       
        public void Main()
        {

            //HWID
            var mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
            ManagementObjectCollection mbsList = mbs.Get();
            string idUser = "";
            foreach (ManagementObject mo in mbsList)
            {
                idUser = mo["ProcessorId"].ToString();
                break;
            }


            DataBase dataBase = new DataBase();

            DataTable table = new DataTable();

            MySqlDataAdapter adapter = new MySqlDataAdapter();

            //User Login
            MySqlCommand commandTakeLogin = new MySqlCommand("SELECT `login` FROM `users` WHERE `HWID` = @hwid", dataBase.getConnection());
            commandTakeLogin.Parameters.Add("@hwid", MySqlDbType.VarChar).Value = idUser;
            dataBase.openConnection();
            MySqlDataReader readerUL = commandTakeLogin.ExecuteReader();
            while (readerUL.Read())
            {
                string var = (string)readerUL[0];
                userL = var;
            }
            readerUL.Close();

            //User Password
            MySqlCommand commandTakePass = new MySqlCommand("SELECT `pass` FROM `users` WHERE `HWID` = @hwid", dataBase.getConnection());
            commandTakePass.Parameters.Add("@hwid", MySqlDbType.VarChar).Value = idUser;
            dataBase.openConnection();
            MySqlDataReader readerUP = commandTakePass.ExecuteReader();
            while (readerUP.Read())
            {
                string var = (string)readerUP[0];
                userP = var;
            }
            readerUP.Close();


            //User Key
            MySqlCommand commandTakeKey = new MySqlCommand("SELECT `keyy` FROM `users` WHERE `HWID` = @hwid", dataBase.getConnection());
            commandTakeKey.Parameters.Add("@hwid", MySqlDbType.VarChar).Value = idUser;
            dataBase.openConnection();
            MySqlDataReader readerUKey = commandTakeKey.ExecuteReader();
            while (readerUKey.Read())
            {
                string var = (string)readerUKey[0];
                userKey = var;
            }
            readerUKey.Close();

            //User Sub
            MySqlCommand commandTakeSub = new MySqlCommand("SELECT `DateToDelete` FROM `users` WHERE `HWID` = @hwid", dataBase.getConnection());
            commandTakeSub.Parameters.Add("@hwid", MySqlDbType.VarChar).Value = idUser;
            dataBase.openConnection();
            MySqlDataReader readerUSub = commandTakeSub.ExecuteReader();
            while (readerUSub.Read())
            {
                DateTime tTime = (DateTime)readerUSub[0];
                dateTime = tTime;
            }
            readerUSub.Close();
            login.Text = "Login: " + userL.ToString();
            pass.Text = "Password: " + userP.ToString();
            key.Text = "Key: " + userKey.ToString();
            HWID.Text = "HWID: " + idUser.ToString();
            deleteAcc.Text = "Subscription is active until: " + dateTime.ToString("dd/MM/yyyy HH:mm ");
            pcName.Text = "PC Name: " + userData.pcUName.ToString();
            IP.Text = "IP Adress: " + userData.pubIp.ToString();

        }

    }
}
