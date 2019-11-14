using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalsAndTransforms.DAL
{
    /// <summary>
    /// Setup the table structure for the workbook file
    /// </summary>
    public static class DatabaseInitializer
    {
        public static void Initialize(SqliteConnection con)
        {
            var transaction = con.BeginTransaction();
            try
            {
                InitializeWorkBookTable(con);
                InitializeSignalTable(con);
                InitializeSignalValuesTable(con);
                transaction.Commit();
            } catch (Exception ex)
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Create the WorkBook table in the database
        /// </summary>
        /// <returns></returns>
        private static bool InitializeWorkBookTable(SqliteConnection con)
        {
            string sql = $@"
                CREATE TABLE 'WorkBook' (
                    'Id'    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    'Name'  TEXT,
	                'Notes' TEXT,
                    'CreateDT' TEXT,
                    'UpdateDT' TEXT,
	                'SourceSignalId'  INTEGER,
                    'OutputSignalId'  INTEGER,
	                'ConvolutionKernelId' INTEGER
                )";

            SqliteCommand cmd = con.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            return true;
        }

        /// <summary>
        /// Create the Signal table in the database
        /// </summary>
        /// <returns></returns>
        private static bool InitializeSignalTable(SqliteConnection con)
        {
            string sql = $@"
                CREATE TABLE 'Signal' (
                    'Id'    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    'WorkBookId' INTEGER,
                    'Name'  TEXT NOT NULL UNIQUE,
	                'SamplingHZ' REAL,
	                'SignalHZ'  REAL,
                    'SampleSeconds'  REAL,
	                'Amplitude' REAL
                )";

            SqliteCommand cmd = con.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            return true;
        }

        /// <summary>
        /// Create the Signal table in the database
        /// </summary>
        /// <returns></returns>
        private static bool InitializeSignalValuesTable(SqliteConnection con)
        {
            string sql = $@"
                CREATE TABLE 'SignalValues' (
                    'Id'    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    'SignalID' INTEGER,
                    'Value'  REAL,
                    CONSTRAINT fk_signalid
                        FOREIGN KEY (SignalID)
                        REFERENCES Signal (Id)
                        ON DELETE CASCADE
                )";

            SqliteCommand cmd = con.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            return true;
        }
    }
}
