using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace APIBackend
{
    public class Database
    {
        private static MySqlConnection connection;
        private static string server;
        private static string database;
        private static string uid;
        private static string password;

        public static int Testing()
        {
            return 42;
        }

        public static int Connect(string server, string database, string uid, string password)
        {
            string connectionString;
            connectionString = string.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};", server, database, uid, password);

            connection = new MySqlConnection(connectionString);

            return OpenConnection() ? 1 : 0;
        }

        private static bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {

                return false;
            }   
        }

        public static void Insert(string query)
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.ExecuteNonQuery();
                }
            } catch(Exception ex)
            {
                return;
            }
        }

        public static void Update(string query)
        {
            try { 
                if(connection.State == System.Data.ConnectionState.Open)
                {
                    MySqlCommand cmd = new MySqlCommand();

                    cmd.CommandText = query;

                    cmd.Connection = connection;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public static List<List<string>> Select(string query)
        {
            try
            {
                List<List<string>> list = new List<List<string>>();

                if (connection.State == System.Data.ConnectionState.Open)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    while (dataReader.Read())
                    {
                        var columns = Enumerable.Range(0, dataReader.FieldCount).Select(dataReader.GetName).ToList();
                        List<string> info = new List<string>();
                        foreach (var s in columns)
                        {
                            info.Add(dataReader[s] + "");
                        }
                        list.Add(info);
                    }

                    dataReader.Close();

                    return list;
                }
                else
                {
                    return list;
                }
            } catch(Exception ex)
            {
                return new List<List<string>>();
            }
}

        public static void Delete(string query)
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}
