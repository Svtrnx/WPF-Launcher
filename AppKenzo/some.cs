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
using System.Net.Http;

namespace AppKenzo
{
    public class userData
    {
        public readonly static string pubIp = new System.Net.WebClient().DownloadString("https://api.ipify.org");

        public static string forKey;

        static public bool need = true;
        


        //Name Of PC
        public readonly static String pcUName = System.Net.Dns.GetHostName();

    }
       
}


