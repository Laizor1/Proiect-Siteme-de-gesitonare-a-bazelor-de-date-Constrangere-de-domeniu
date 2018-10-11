using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace SGBDProiect
{
    class SGBD
    {
        public static MySqlConnection conn { get; set; }

        public static bool Connect(string addr, uint port, string user, string pass, string db)
        {
            MySqlConnectionStringBuilder connStr = new MySqlConnectionStringBuilder();
            connStr.Server = addr;
            connStr.UserID = user;
            connStr.Password = pass;
            connStr.Port = port;
            connStr.Database = db;

            try
            {
                conn = new MySqlConnection(connStr.ToString());
                conn.Open();
                conn.Close();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static bool HasDomainConstraint(string table, string field)
        {
            MySqlCommand queryHasInsertTrigger = conn.CreateCommand();

            queryHasInsertTrigger.CommandText =
                @"SELECT COUNT(*) AS Triggers FROM information_schema.TRIGGERS 
                WHERE TRIGGER_SCHEMA = (SELECT DATABASE())
                AND EVENT_OBJECT_TABLE = '" + table + "'\n" +
                "AND TRIGGER_NAME='domain_insert_trigger_" + field + "';";

            conn.Open();

            MySqlDataReader reader = queryHasInsertTrigger.ExecuteReader();


            reader.Read();

            int triggers = reader.GetInt32("Triggers");

            if (triggers >= 1)
            {
                conn.Close();
                return true;
            }

            conn.Close();


            return false;
        }

        public static bool AddDomainConstraint(string table, string field, string min, string max)
        {
            MySqlCommand queryOnInsert = conn.CreateCommand();
            MySqlCommand queryOnUpdate = conn.CreateCommand();

            queryOnInsert.CommandText =
                    "create trigger `domain_insert_trigger_" + field + "` before insert on " + table + "\n" +
                    "for each row\n" +
                    "begin\n" +
                        "if new." + field + " < '" + min + "' then\n" +
                        "set new." + field + " = '" + min + "';\n" +
                        "elseif new." + field + " > '" + max + "' then\n" +
                        "set new." + field + " = '" + max + "';\n" + 
                        "end if;\n" +
                    "end\n";

            queryOnUpdate.CommandText =
                    "create trigger `domain_update_trigger_" + field + "` before update on " + table + "\n" +
                    "for each row\n" +
                    "begin\n" +
                        "if new." + field + " < '" + min + "' then\n" +
                        "set new." + field + " = '" + min + "';\n" +
                        "elseif new." + field + " > '" + max + "' then\n" +
                        "set new." + field + " = '" + max + "';\n" +
                        "end if;\n" +
                    "end\n";

            conn.Open();

            try
            {
                queryOnInsert.ExecuteNonQuery();
                queryOnUpdate.ExecuteNonQuery();
            }
            catch
            {
                return false;
            }
            conn.Close();

            return true;
        }
    }
}
