using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Reflection;

namespace GameGrid.Models
{
    public class SQLConnector
    {
        public static string connectionString = @"Data Source=DESKTOP-U0C7J25\SQLEXPRESS;Initial Catalog=GameGrid;Integrated Security=True";

        /// <summary>
        /// Load all DBObjects from a table.
        /// </summary>
        /// <param name="dbo">An instance of the type of DBObject to load</param>
        public static List<DBObject> LoadTable(DBObject dbo)
        {
            string queryString = "SELECT * FROM " + dbo.TableName;
            return LoadList(dbo, queryString);
        }

        public static List<DBObject> LoadWhere(DBObject dbo, string column, string val)
        {
            string queryString = $"SELECT * FROM {dbo.TableName} WHERE {column} = '{val}'";
            return LoadList(dbo, queryString);
        }

        public static List<DBObject> LoadList(DBObject dbo, string sqlQuery)
        {
            var objects = new List<DBObject>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            objects.Add(dbo.GetFromReader(reader));
                    }
                }
                connection.Close();
            }
            return objects;
        }

        /// <summary>
        /// Update every field on the DBObject except for the ID
        /// </summary>
        /// <param name="id">Name of ID column</param>
        public static void Update(DBObject dbo, string id)
        {
            DoubleApostraphies(dbo);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = GetUpdateStatement(dbo, id);
                using (SqlCommand command = new SqlCommand(sql, connection))
                    command.ExecuteNonQuery();
                connection.Close();
            }
        }

        /// <summary>
        /// Returns the SQL statement for updating a DBObject
        /// </summary>
        /// <param name="dbo">Object to update</param>
        /// <param name="id">Name of ID column</param>
        private static string GetUpdateStatement(DBObject dbo, string id)
        {
            Type t = dbo.GetType();
            List<string> columns = GetColumns(dbo.TableName);
            columns.Remove("Updated");
            columns.Remove(id);

            string updateStatement = $"UPDATE {dbo.TableName} SET ";

            string values = "";
            foreach (string col in columns)
                values += (col + $" = '{t.GetProperty(col).GetValue(dbo)}', ");
            values += $"Updated = '{GetCurrentDate()}' "; // Table must have Updated column

            string whereClause = $"WHERE {id} = '{t.GetProperty(id).GetValue(dbo)}'";

            return $"{updateStatement}{values}{whereClause}";
        }

        /// <summary>
        /// Insert a list of DBObjects to the database (must all have same type)
        /// </summary>
        public static void Insert(List<DBObject> objects)
        {
            string tableName = objects[0].TableName;
            List<string> columns = GetColumns(tableName);
            string insertStatement = GetInsertStatement(tableName, columns);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (DBObject o in objects)
                {
                    string values = "VALUES (";
                    foreach (string col in columns)
                        values = GetValueStatement(o, col, values);
                    string sql = String.Format("{0}{1})", insertStatement, values.TrimEnd().TrimEnd(','));
                    using (SqlCommand command = new SqlCommand(sql, connection))
                        command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        /// <summary>
        /// Returns INSERT statement with the columns specified
        /// </summary>
        private static string GetInsertStatement(string table, List<string> columns)
        {
            string s = "INSERT INTO " + table + " (";
            foreach (string c in columns)
                s += c + ", ";
            return s = s.Remove(s.Length - 2) + ") ";
        }

        /// <summary>
        /// Appends the value of the specified column to the VALUES statement
        /// </summary>
        private static string GetValueStatement(DBObject o, string col, string values)
        {
            if (col == "Updated")
                return values + "'" + GetCurrentDate() + "', ";

            Type t = o.GetType();
            PropertyInfo pi = t.GetProperty(col);

            if (pi.GetValue(o) == null)
                return values += "'', ";

            string val = pi.GetValue(o).ToString();
            string prop = pi.ToString();

            if (prop.Contains("DateTime"))
                val = val.Split(' ')[0]; // Remove time from date

            if (prop.Contains("String") || prop.Contains("DateTime"))
                values += "'" + val.Replace("'", "''") + "', ";
            else if (prop.Contains("Double") || prop.Contains("Int32") || prop.Contains("Decimal"))
                values += val + ", ";
            else if (prop.Contains("Boolean"))
                values += (val == "False" ? "0" : "1") + ", ";

            return values;
        }

        /// <summary>
        /// Return list of fields names
        /// </summary>
        /// <param name="o"></param>
        public static List<string> GetFields(DBObject o, string id)
        {
            List<string> fields = new List<string>();
            foreach (PropertyInfo pi in o.GetType().GetProperties())
            {
                if (pi.Name != "Updated" && pi.Name != "TableName" && pi.Name != id)
                    fields.Add(pi.Name);
            }
            return fields;
        }

        /// <summary>
        /// Returns a list of all columns names from a table
        /// </summary>
        public static List<string> GetColumns(string table)
        {
            List<string> columns = new List<string>();
            string sql = "SELECT * FROM " + table;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                            columns.Add(reader.GetName(i).Replace("_", ""));
                    }
                }
                connection.Close();
            }
            return columns;
        }

        /// <summary>
        /// Doubles up the single quotes contained in every string field on the object.
        /// So the sql statment doesn't brake
        /// </summary>
        public static void DoubleApostraphies(DBObject dbo)
        {
            PropertyInfo[] properties = dbo.GetType().GetProperties();
            foreach (PropertyInfo pi in properties)
            {
                if (pi.ToString().Contains("String") && pi.GetSetMethod() != null)
                    pi.SetValue(dbo, pi.GetValue(dbo)?.ToString().Replace("'", "''"));
            }
        }

        public static List<string> GameFaqsUrls()
        {
            var list = new List<string>();
            //sql = "SELECT UrlGameFaqs FROM GameRankings";
            //StartReader();
            //int i = 0;
            //while (reader.Read())
            //{
            //    string s = reader.GetValue(i).ToString();
            //    if (s != "" && s != " ")
            //        list.Add(s);
            //}
            //dbConnection.Close();
            return list;
        }

        private static string GetCurrentDate()
        {
            return DateTime.Now.ToString().Split(' ')[0]; // Remove time from date
        }

        public static List<Game> LoadMaster()
        {
            return new List<Game>();
        }

        #region old

        //public static List<Game> LoadTable(WebScraper source)
        //{
        //    string queryString = "SELECT * FROM " + source.GetType().Name;

        //    var games = new List<Game>();

        //    Type type = Type.GetType("GameGrid.Models.Game");

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();

        //        using (SqlCommand command = new SqlCommand(queryString, connection))
        //        {
        //            using (SqlDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                    games.Add(source.GetGameFromReader(reader));
        //            }
        //        }

        //        connection.Close();
        //    }

        //    return games;
        //}

        //public static List<Game> LoadMaster()
        //{
        //    string queryString = "SELECT * FROM Master WHERE System LIKE 'Nintendo 64'";

        //    var games = new List<Game>();

        //    Type type = Type.GetType("GameGrid.Models.Game");

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();

        //        using (SqlCommand command = new SqlCommand(queryString, connection))
        //        {
        //            using (SqlDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    Game g = new Game(reader.GetString(0), reader.GetString(1))
        //                    {
        //                        Genre = reader.GetString(2),
        //                        Developer = reader.GetString(3),
        //                        Publisher = reader.GetString(4),
        //                        DateNA = reader.GetDateTime(5),
        //                        DatePAL = reader.GetDateTime(6),
        //                        DateJAP = reader.GetDateTime(7),
        //                        ESRB = reader.GetString(8),
        //                        Description = reader.GetString(9),
        //                        Directors = reader.GetString(10),
        //                        Producers = reader.GetString(11),
        //                        Programmers = reader.GetString(12),
        //                        Artists = reader.GetString(13),
        //                        Composers = reader.GetString(14),
        //                        Rank = Convert.ToDouble(reader.GetDecimal(15)),
        //                        Reviews = reader.GetInt32(16),
        //                        MRank = Convert.ToInt32(reader.GetDecimal(17)),
        //                        MReviews = reader.GetInt32(18),
        //                        LoosePrice = Convert.ToDouble(reader.GetDecimal(19)),
        //                        CompletePrice = Convert.ToDouble(reader.GetDecimal(20)),
        //                        NewPrice = Convert.ToDouble(reader.GetDecimal(21)),
        //                        GradedPrice = Convert.ToDouble(reader.GetDecimal(22)),
        //                        BoxPrice = Convert.ToDouble(reader.GetDecimal(23)),
        //                        ManualPrice = Convert.ToDouble(reader.GetDecimal(24)),
        //                        ASIN = reader.GetString(25),
        //                        EPID = reader.GetInt32(26),
        //                        Variants = reader.GetString(27),
        //                        DiscCount = reader.GetInt32(28),
        //                        AlsoOn = reader.GetString(29),
        //                        PCID = reader.GetInt32(30),
        //                        NAID = reader.GetInt32(31),
        //                        UPC = reader.GetString(32),
        //                        SoldNA = Convert.ToDouble(reader.GetDecimal(33)),
        //                        SoldPAL = Convert.ToDouble(reader.GetDecimal(34)),
        //                        SoldJAP = Convert.ToDouble(reader.GetDecimal(35)),
        //                        SoldRest = Convert.ToDouble(reader.GetDecimal(36)),
        //                        SoldTotal = Convert.ToDouble(reader.GetDecimal(37)),
        //                        LengthMain = Convert.ToDouble(reader.GetDecimal(38)),
        //                        LengthMainExtras = Convert.ToDouble(reader.GetDecimal(39)),
        //                        LengthComplete = Convert.ToDouble(reader.GetDecimal(40)),
        //                        LengthAllPlayStyles = Convert.ToDouble(reader.GetDecimal(41)),
        //                        BoxText = reader.GetString(42),
        //                        AltName = reader.GetString(43),
        //                        BuyPrice = reader.GetString(44),
        //                        EstimatedValue = reader.GetString(45),
        //                        Barcode = reader.GetString(46),
        //                        ItemNumber = reader.GetString(47),
        //                        LocalPlayers = reader.GetInt32(48),
        //                        OnlinePlayers = reader.GetInt32(49),
        //                        Modes = reader.GetString(50),
        //                        //Story = reader.GetBoolean(51),
        //                        //Coop = reader.GetBoolean(52),
        //                        Notes = reader.GetString(53),
        //                        UrlCoverProject = reader.GetString(54),
        //                        UrlGameRankings = reader.GetString(55),
        //                        UrlGameFaqs = reader.GetString(56),
        //                        UrlHowLongToBeat = reader.GetString(57),
        //                        UrlMetaCritic = reader.GetString(58),
        //                        UrlPriceCharts = reader.GetString(59),
        //                        UrlVGChartz = reader.GetString(60),
        //                        UrlVGCollect = reader.GetString(61),
        //                        UrlWikipedia = reader.GetString(62)
        //                    };
        //                    games.Add(g);
        //                }
        //            }
        //        }

        //        connection.Close();
        //    }

        //    return games;
        //}

        //public static void Insert(List<Game> games, string table)
        //{
        //    List<string> columns = GetColumns(table);
        //    Type t = Type.GetType("GameGrid.Models.Game");
        //    string insertStatement = GetInsertStatement(table, columns);
        //    int count = 0;

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();

        //        foreach (Game g in games)
        //        {
        //            count++;
        //            DoubleApostraphies(g);
        //            string values = "VALUES (";
        //            foreach (string col in columns)
        //                values = AddValueStatement(g, col, values, t);
        //            string sql = String.Format("{0}{1})", insertStatement, values.TrimEnd().TrimEnd(','));
        //            using (SqlCommand command = new SqlCommand(sql, connection))
        //            {
        //                command.ExecuteNonQuery();
        //            }
        //        }

        //        connection.Close();
        //    }
        //}

        //private static string AddValueStatement(Game g, string col, string values, Type t)
        //{
        //    if (col == "Updated")
        //        return values + "'" + GetCurrentDate() + "', ";

        //    if (col.Contains("Own") || col.Contains("Completed") || col == "Wanted" || col == "Story")
        //        return values + "'', ";

        //    PropertyInfo pi = t.GetProperty(col);

        //    if (pi.GetValue(g) == null)
        //    {
        //        values += "'', ";
        //    }
        //    else
        //    {
        //        string val = pi.GetValue(g).ToString();
        //        string prop = pi.ToString();

        //        if (prop.Contains("DateTime"))
        //            val = val.Split(' ')[0];

        //        if (prop.Contains("String") || prop.Contains("DateTime"))
        //            values += "'" + val + "', ";
        //        else if (prop.Contains("Double") || prop.Contains("Int32"))
        //            values += val + ", ";
        //        else if (prop.Contains("Boolean"))
        //            values += (val == "False" ? "0" : "1") + ", ";
        //    }

        //    return values;
        //}

        //public static void Update(string name, string system, string value, string field)
        //{
        //    string sql;

        //    if (field.Contains("Completed") || field.Contains("Owned") || field == "Wanted" || field == "Story")
        //        value = value == "True" ? "1" : "0";

        //    name = name.Replace("'", "''");
        //    if (field == "Name")
        //        sql = String.Format("UPDATE Master SET Name = '{0}' WHERE Name = '{1}' AND System = '{2}'", value, name, system);
        //    else if (field == "System")
        //        sql = String.Format("UPDATE Master SET System = '{0}' WHERE Name = '{1}' AND System = '{2}'", value, name, system);
        //    else
        //        sql = String.Format("UPDATE Master SET {0} = '{1}' WHERE Name = '{2}' AND System = '{3}'", field, value, name, system);
        //}

        //public static void DoubleApostraphies(Game g)
        //{
        //    //var info = typeof(Game).GetTypeInfo();
        //    //var list = new List<string>();
        //    //foreach (FieldInfo f in info.DeclaredFields)
        //    //{
        //    //    var array = f.ToString().Split(' ');
        //    //    list.Add(array[1]);
        //    //}

        //    if (g.Name != null)
        //        g.Name = g.Name.Replace("'", "''");
        //    if (g.Developer != null)
        //        g.Developer = g.Developer.Replace("'", "''");
        //    if (g.Publisher != null)
        //        g.Publisher = g.Publisher.Replace("'", "''");
        //    if (g.Genre != null)
        //        g.Genre = g.Genre.Replace("'", "''");
        //    if (g.Programmers != null)
        //        g.Programmers = g.Programmers.Replace("'", "''");
        //    if (g.DateString != null)
        //        g.DateString = g.DateString.Replace("'", "''");
        //    if (g.Composers != null)
        //        g.Composers = g.Composers.Replace("'", "''");
        //    if (g.Artists != null)
        //        g.Artists = g.Artists.Replace("'", "''");
        //    if (g.Directors != null)
        //        g.Directors = g.Directors.Replace("'", "''");
        //    if (g.Producers != null)
        //        g.Producers = g.Producers.Replace("'", "''");
        //    if (g.BoxText != null)
        //        g.BoxText = g.BoxText.Replace("'", "''");
        //    if (g.Description != null)
        //        g.Description = g.Description.Replace("'", "''");
        //    if (g.AltName != null)
        //        g.AltName = g.AltName.Replace("'", "''");
        //    if (g.BuyPrice != null)
        //        g.BuyPrice = g.BuyPrice.Replace("'", "''");
        //    if (g.System != null)
        //        g.System = g.System.Replace("'", "''");
        //    if (g.ESRB != null)
        //        g.ESRB = g.ESRB.Replace("'", "''");
        //    if (g.Modes != null)
        //        g.Modes = g.Modes.Replace("'", "''");
        //    if (g.Notes != null)
        //        g.Notes = g.Notes.Replace("'", "''");
        //    if (g.Note != null)
        //        g.Note = g.Note.Replace("'", "''");
        //    if (g.UrlWikipedia != null)
        //        g.UrlWikipedia = g.UrlWikipedia.Replace("'", "''");
        //}

        #endregion
    }
}