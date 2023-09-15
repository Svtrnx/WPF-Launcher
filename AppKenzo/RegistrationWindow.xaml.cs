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

namespace AppKenzo
{
    
    /// <summary>
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        DataBase dataBase = new DataBase();

        DataTable table = new DataTable();

        MySqlDataAdapter adapter = new MySqlDataAdapter();

        public RegistrationWindow()
        {

            InitializeComponent();

            if (isCorrectVersion())
            {
                return;
            }

            //Async(State)
            asyncStateWork();
            
            
        }

        //Asynchronous function for state
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AuthWindow mainWindow = new AuthWindow();
            mainWindow.Show();
            Hide();
            Close();
        }
        
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://t.me/YoungKENZO");
        }

        public void buttonRegMenu_Click(object sender, RoutedEventArgs e)
        {
            string loginUserReg = textBoxLogin.Text;
            string passUserReg = textBoxPassword.Password.ToString();
            string passUserReg2 = textBoxPassword2.Password.ToString();
            string keyUserReg = textKeyBox.Text;
            string keyUserRegForDeleting = textKeyBox.Text;
            string keyUserRegForEvent = textKeyBox.Text;
            string keyDateRemaning = textKeyBox.Text;

            userData.forKey = textKeyBox.Text;

            string forw = userData.forKey;


            int mainPanelInfo = 0;
            int updateTimeSub = 0;


        loginCheck:

            if (Regex.IsMatch(loginUserReg, @"\p{IsCyrillic}") || loginUserReg.Length < 5)
            {
                textBoxLogin.Background = Brushes.Red;
                textBoxLogin.ToolTip = "This field is not entered correctly!";
                MessageBox.Show("This field is not entered correctly!");
                return;

            }

            textBoxLogin.Background = Brushes.Transparent;


            if (Regex.IsMatch(passUserReg, @"\p{IsCyrillic}") || passUserReg.Length < 6)
            {
                //passwordCheck:
                textBoxPassword.Background = Brushes.Red;
                textBoxPassword.ToolTip = "This field is not entered correctly!";
                MessageBox.Show("This field is not entered correctly!");
                return;
            }
            
            textBoxPassword.Background = Brushes.Transparent;

            if (passUserReg != passUserReg2)
            {
                //passwordRightCheck:
                textBoxPassword2.Background = Brushes.Red;
                textBoxPassword2.ToolTip = "The passwords don't match!";
                MessageBox.Show("The passwords don't match!");
                return;
            }

            textBoxPassword2.Background = Brushes.Transparent;

            if (Regex.IsMatch(loginUserReg, @"\p{IsCyrillic}") || loginUserReg.Length < 5)
            {
                goto loginCheck;
            }
            
            if (Regex.IsMatch(keyUserReg, @"\p{IsCyrillic}") || keyUserReg.Length < 5)
            {
                textKeyBox.Background = Brushes.Red;
                textKeyBox.ToolTip = "This field is not entered correctly!";
                MessageBox.Show("This field is not entered correctly!");
                return;
            }

            textKeyBox.Background = Brushes.Transparent;

            //If it didn't find the key
            if (isKeyDoesntExists())
            {
                return;
            }

            //If a user is found
            if (isUserExists())
            {
                return;
            }

            //Getting HWID

            var mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
            ManagementObjectCollection mbsList = mbs.Get();
            string id = "";
            foreach (ManagementObject mo in mbsList)
            {
                id = mo["ProcessorId"].ToString();
                break;
            }


            //Random Event
            Random rand = new Random();
            int randToEvent = (int)rand.Next(1, 10000 + 3);


            AuthWindow authWindow = new AuthWindow();


            //Insertion
            MySqlCommand command = new MySqlCommand("INSERT INTO `users` (`login`, `pass`, `keyy`, `HWID`, `NameOfPC`, `userIP`) " +
                "VALUES (@login, @password, @keyA , @hwid, @pcname, @userIP)", dataBase.getConnection());
            
            
            //Deleting KEY registartion
            MySqlCommand commandToDelete = new MySqlCommand("DELETE FROM `keysforapp` WHERE `keysapp` = @deletingKey", dataBase.getConnection());


            //Taking var from mainPanel(state)
            MySqlCommand commandMainPanel = new MySqlCommand("SELECT `mainP` FROM `mainpanel`", dataBase.getConnection());
            


            //Data base connection to take mainPanel var
            dataBase.openConnection();
            MySqlDataReader reader2 = commandMainPanel.ExecuteReader();

            while (reader2.Read())
            {
                int mP = (int)reader2[0];

                mainPanelInfo = mP;
            }
            reader2.Close();

            //Update time
            MySqlCommand commandUpdateTime = new MySqlCommand("UPDATE `updatetimesub` SET `time`= @updateTime", dataBase.getConnection());
            commandUpdateTime.Parameters.Add("@updateTime", MySqlDbType.VarChar).Value = mainPanelInfo;

            commandUpdateTime.ExecuteNonQuery();
            
            dataBase.closeConnection();

            //Беру время 7/14/31 используя MainPanel манипулируя стейтами
            MySqlCommand commandupdateTimeSub = new MySqlCommand("UPDATE `updatetimesub` SET `updatetimesub`.`time` = " +
                "CASE WHEN `updatetimesub`.`time` = 1 THEN 7 WHEN `updatetimesub`.`time` = 2 THEN 14 " +
                "WHEN `updatetimesub`.`time` = 3 THEN 31 ELSE `updatetimesub`.`time` = 1 END", dataBase.getConnection());

            dataBase.openConnection();
            if(commandupdateTimeSub.ExecuteNonQuery() == 1)
            {

            }
            dataBase.closeConnection();

            
            MySqlCommand commandTakingTimeOfEndSubscription = new MySqlCommand("SELECT `time` FROM `updatetimesub`", dataBase.getConnection());
            
            dataBase.openConnection();

            MySqlDataReader reader3 = commandTakingTimeOfEndSubscription.ExecuteReader();

            while (reader3.Read())
            {
                int tTime = (int)reader3[0];

                updateTimeSub += tTime;
            }
            reader3.Close();
            dataBase.closeConnection();
            
            //Event time логика
            MySqlCommand commandEventTimeSubscription = new MySqlCommand("CREATE EVENT " + loginUserReg + "_" + randToEvent.ToString() + " ON SCHEDULE AT " +
                "CURRENT_TIMESTAMP + INTERVAL @intervalDaysForEvent DAY DO DELETE FROM `users` WHERE ``.`keyy` = '" + keyUserRegForEvent + "'", dataBase.getConnection());

            
            MySqlCommand commandEndOfSubscriprion = new MySqlCommand("UPDATE `users` SET `DateToDelete`= DATE_ADD(`TimeReg`, INTERVAL @intervalDays DAY) " +
                "WHERE `keyy` = @keyDate ", dataBase.getConnection());




            //Gettig Time
            //MySqlCommand commandd = new MySqlCommand("@GettingTime", dataBase.getConnection());
            command.Parameters.Add("@login", MySqlDbType.VarChar).Value = loginUserReg;
            command.Parameters.Add("@password", MySqlDbType.VarChar).Value = passUserReg;
            command.Parameters.Add("@keyA", MySqlDbType.VarChar).Value = keyUserReg;
            command.Parameters.Add("@hwid", MySqlDbType.VarChar).Value = id;
            command.Parameters.Add("@pcname", MySqlDbType.VarChar).Value = userData.pcUName;
            command.Parameters.Add("@userIP", MySqlDbType.VarChar).Value = userData.pubIp;
            commandToDelete.Parameters.Add("@deletingKey", MySqlDbType.VarChar).Value = keyUserRegForDeleting;
            commandEndOfSubscriprion.Parameters.Add("@keyDate", MySqlDbType.VarChar).Value = keyDateRemaning;
            commandEndOfSubscriprion.Parameters.Add("@intervalDays", MySqlDbType.VarChar).Value = updateTimeSub;
            commandEventTimeSubscription.Parameters.Add("@intervalDaysForEvent", MySqlDbType.VarChar).Value = updateTimeSub;

            dataBase.openConnection();

            /*
            command.ExecuteNonQuery();
            comm.ExecuteNonQuery();
            */

            if (command.ExecuteNonQuery() == 1 && commandEventTimeSubscription.ExecuteNonQuery() == 0 && commandToDelete.ExecuteNonQuery() == 1 
                && commandEndOfSubscriprion.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Регистрация успешна!");
                authWindow.Show();
                Hide();
                Close();
                Console.Beep(300, 400);
            }
            else
            {
                MessageBox.Show("Ошибка!");
                Thread.Sleep(10000);
                Close();
                System.Windows.Application.Current.Shutdown();
            }

            dataBase.closeConnection();


        }

        public Boolean isCorrectVersion()
        {

            DataBase dataBase = new DataBase();

            DataTable table = new DataTable();

            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand command = new MySqlCommand("SELECT * FROM `versions` WHERE `state` = 1", dataBase.getConnection());

            adapter.SelectCommand = command;
            adapter.Fill(table);

            //Flow on version state

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
                    MessageBox.Show("Версия вашего лаунчера устарела!\nСкачайте новую!");
                    Environment.Exit(0);
                }));
                Environment.Exit(0);
                return true;
            }
        }


        public Boolean isUserExists()
        {
            DataBase dataBase = new DataBase();

            DataTable table = new DataTable();

            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand command = new MySqlCommand("SELECT * FROM `users` WHERE `login` = @uL", dataBase.getConnection());

            command.Parameters.Add("@uL", MySqlDbType.VarChar).Value = textBoxLogin.Text;

            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {
                MessageBox.Show("Такой логин уже есть, введите другой!");
                return true;
            }
            else
            {
                return false;
            }
        }  
        
        public Boolean isKeyDoesntExists()

        {
            DataBase dataBase = new DataBase();

            DataTable table = new DataTable();

            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand command = new MySqlCommand("SELECT * FROM `keysforapp` WHERE `keysapp` = @keyA", dataBase.getConnection());

            command.Parameters.Add("@keyA", MySqlDbType.VarChar).Value = textKeyBox.Text;

            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {
                return false;
            }
            else
            {

                MessageBox.Show("Неверный ключ!");
                textKeyBox.Background = Brushes.Red;
                return true;
            }
        }
}
}
