using Microsoft.Data.Sqlite;
using SignalsAndTransforms.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalsAndTransforms.DAL
{
    public class WorkBookDAL
    {
        /// <summary>
        /// Create the Workbook file, note this will delete any previously existing file if it exists
        /// </summary>
        /// <param name="workBook"></param>
        /// <returns>true on success</returns>
        public bool Create(WorkBook workBook)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            SqliteConnection sqlLiteConnection;

            SqliteConnectionStringBuilder connectionString = new SqliteConnectionStringBuilder();
            workBook.FilePath = Path.Combine(desktopPath, $"{workBook.Name}.db");

            // Create overwrites any existing file
            if (File.Exists(workBook.FilePath))
            {
                File.Delete(workBook.FilePath);
            }
            connectionString.DataSource = workBook.FilePath;

            using (sqlLiteConnection = new SqliteConnection(connectionString.ConnectionString))
            {
                sqlLiteConnection.Open();
                InitializeWorkBookTable(sqlLiteConnection);

                string sql = $@"INSERT INTO WorkBook ([Name],[Notes]) VALUES (@Name, @Notes)";
                using (SqliteCommand cmd = new SqliteCommand(sql, sqlLiteConnection))
                {
                    int result = 0;
                    using (var transaction = cmd.Connection.BeginTransaction())
                    {
                        cmd.Transaction = transaction;
                        cmd.Parameters.AddWithValue("@Name", workBook.Name);
                        cmd.Parameters.AddWithValue("@Notes", workBook.Notes);
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
            }
            
            return true;
        }


        /// <summary>
        /// Create the WorkBook table in the database
        /// </summary>
        /// <returns></returns>
        private bool InitializeWorkBookTable(SqliteConnection con)
        {
            string sql = $@"
                CREATE TABLE 'WorkBook' (
                    'Id'    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    'Name'  TEXT,
	                'Notes' TEXT,
	                'SourceSignalId'  INTEGER,
                    'OutputSignalId'  INTEGER,
	                'ConvolutionKernelId' INTEGER
                )";

            SqliteCommand cmd = con.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            return true;
        }
    }
}
