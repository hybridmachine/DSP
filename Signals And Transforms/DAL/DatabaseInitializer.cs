using Microsoft.Data.Sqlite;
using SignalsAndTransforms.Models;
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
                InitializeSignalsTable(con);
                InitializeSignaTypesTable(con);
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
                    'SchemaVersion' TEXT,
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
        private static bool InitializeSignalsTable(SqliteConnection con)
        {
            string sql = $@"
                CREATE TABLE 'Signals' (
                    'Id'    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    'WorkBookId' INTEGER,
                    'IsActive' INTEGER NOT NULL CHECK (IsActive IN (0,1)),
                    'Name'  TEXT NOT NULL UNIQUE,
                    'Type'  INTEGER NOT NULL,
	                'SamplingHZ' REAL,
	                'SignalHZ'  REAL,
                    'SampleSeconds'  REAL,
	                'Amplitude' REAL
                )";

            SqliteCommand cmd = con.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();

            // Documentation table so readers of the DB file without the app source can see
            // what each signal type is
            sql = $@"CREATE TABLE 'SignalTypes' (
                    'Id'    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    'Name' TEXT NOT NULL UNIQUE,
                    'EnumVal' INTEGER NOT NULL UNIQUE
                    )";

            cmd = con.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            
            return true;
        }

        /// <summary>
        /// Load the enumeration names and values from SignalType into a table, used for reference
        /// for readers of the workbook without access to the source code
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        private static bool InitializeSignaTypesTable(SqliteConnection con)
        {
            SqliteCommand cmd;
            // Load the SignalTypes table
            StringBuilder sqlBuilder = new StringBuilder();
            foreach (var enumName in Enum.GetNames(typeof(SignalType)))
            {
                sqlBuilder.Append($@"INSERT INTO SignalTypes ([Name], [EnumVal]) VALUES('{enumName}', '{(int)(SignalType)Enum.Parse(typeof(SignalType), enumName)}');");
            }

            cmd = con.CreateCommand();
            cmd.CommandText = sqlBuilder.ToString();
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
                        REFERENCES Signals (Id)
                        ON DELETE CASCADE
                )";

            SqliteCommand cmd = con.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            return true;
        }
    }
}
