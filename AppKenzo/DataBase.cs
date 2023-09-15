using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DotNetEnv;

Env.Load();


namespace AppKenzo
{
    class Encode
    {
        public static string SERVER = Environment.GetEnvironmentVariable("SERVER");
        public static string PORT = Environment.GetEnvironmentVariable("PORT");
        public static string USERNAME = Environment.GetEnvironmentVariable("USERNAME");
        public static string PASSWORD = Environment.GetEnvironmentVariable("PASSWORD");
        public static string DATABASE = Environment.GetEnvironmentVariable("DATABASE");

        public static readonly string encodedServer = Convert.ToBase64String(Encoding.UTF8.GetBytes(SERVER));
        public static readonly string encodedPort = Convert.ToBase64String(Encoding.UTF8.GetBytes(PORT));
        public static readonly string encodedUsername = Convert.ToBase64String(Encoding.UTF8.GetBytes(USERNAME));
        public static readonly string encodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(PASSWORD));
        public static readonly string encodedDatabase = Convert.ToBase64String(Encoding.UTF8.GetBytes(DATABASE));

    }
    class Decode
    {
        public static readonly string decodedServer = Encoding.UTF8.GetString(Convert.FromBase64String(Encode.encodedServer));
        public static readonly string decodedPort = Encoding.UTF8.GetString(Convert.FromBase64String(Encode.encodedPort));
        public static readonly string decodedUsername = Encoding.UTF8.GetString(Convert.FromBase64String(Encode.encodedUsername));
        public static readonly string decodedPassword = Encoding.UTF8.GetString(Convert.FromBase64String(Encode.encodedPassword));
        public static readonly string decodedDatabase = Encoding.UTF8.GetString(Convert.FromBase64String(Encode.encodedDatabase));
    }

    internal class DataBase
    {
        public static readonly string conToDB = "server=" + Decode.decodedServer + "port=" + Decode.decodedPort + "username=" + Decode.decodedUsername + 
            "password=" + Decode.decodedPassword + "database=" + Decode.decodedDatabase + ""; 

        //Connection
        MySqlConnection connection = new MySqlConnection(conToDB);

        //Encoded 
        private static readonly string encodedCon = Convert.ToBase64String(Encoding.UTF8.GetBytes(conToDB));

        
        public void openConnection()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }
        }
         
        public void closeConnection()

        { 
            if (connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public MySqlConnection getConnection()
        {
            return connection;
        }


    }
}
