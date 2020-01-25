using Dapper;
using Microsoft.Data.Sqlite;
using SignalProcessor.Filters;
using SignalsAndTransforms.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalsAndTransforms.DAL
{
    class SignalValue
    {
        public long Id { get; set; }
        public long SignalId { get; set; }
        public double Value { get; set; }
    }
    public static class WorkBookDAL
    {
        public const string SchemaVersion = "1.2";

        /// <summary>
        /// Create the Workbook file, note this will delete any previously existing file if it exists
        /// </summary>
        /// <param name="workBook"></param>
        /// <returns>true on success</returns>
        public static bool Create(WorkBook workBook)
        {

            SqliteConnectionStringBuilder connectionString = new SqliteConnectionStringBuilder();
            if (workBook.FilePath == null)
            {
                throw new Exception("Filepath not set");
            }

            // Create overwrites any existing file
            if (File.Exists(workBook.FilePath))
            {
                File.Delete(workBook.FilePath);
            }
            connectionString.DataSource = workBook.FilePath;

            using (SqliteConnection sqlLiteConnection = new SqliteConnection(connectionString.ConnectionString))
            {
                sqlLiteConnection.Open();
                DatabaseInitializer.Initialize(sqlLiteConnection);

                string sql = $@"INSERT INTO WorkBook ([Name],[SchemaVersion],[Notes],[CreateDT]) VALUES (@Name, @SchemaVersion, @Notes, datetime('now'))";
               
                using (var transaction = sqlLiteConnection.BeginTransaction())
                {
                    SqliteCommand cmd = sqlLiteConnection.CreateCommand();
                    try
                    {
                        cmd.CommandText = sql;
                        cmd.Parameters.AddWithValue("@Name", workBook.Name);
                        cmd.Parameters.AddWithValue("@SchemaVersion", SchemaVersion);
                        cmd.Parameters.AddWithValue("@Notes", workBook.Notes ?? String.Empty);
                        cmd.ExecuteNonQuery();

                        sql = "SELECT last_insert_rowid();";
                        using (SqliteCommand getId = new SqliteCommand(sql, sqlLiteConnection))
                        {
                            getId.Transaction = transaction;
                            workBook.Id = (long)getId.ExecuteScalar();
                        }
                        transaction.Commit();
                    } catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
               
            }
            
            return true;
        }

        /// <summary>
        /// Persist properties to database
        /// </summary>
        /// <param name="workBook"></param>
        public static bool Update(WorkBook workBook)
        {
            if (workBook.FilePath == null)
            {
                throw new Exception("Filepath not set");
            }

            SqliteConnectionStringBuilder connectionString = new SqliteConnectionStringBuilder();

            connectionString.DataSource = workBook.FilePath;

            using (SqliteConnection sqlLiteConnection = new SqliteConnection(connectionString.ConnectionString))
            {
                sqlLiteConnection.Open();

                string sql = $@"UPDATE WorkBook SET [Name] = @Name,
                                                    [Notes] = @Notes,
                                                    [UpdateDT] = datetime('now') WHERE Id=@Id";

                using (var transaction = sqlLiteConnection.BeginTransaction())
                {
                    try
                    {
                        SqliteCommand cmd = sqlLiteConnection.CreateCommand();
                        cmd.CommandText = sql;

                        cmd.Parameters.AddWithValue("@Name", workBook.Name);
                        cmd.Parameters.AddWithValue("@Notes", workBook.Notes ?? String.Empty);
                        cmd.Parameters.AddWithValue("@Id", workBook.Id);
                        cmd.ExecuteNonQuery();

                        // Save signals
                        foreach (Signal signal in workBook.Signals.Values)
                        {
                            SignalDAL.Create(workBook, signal, sqlLiteConnection);
                        }

                        // Save filters
                        foreach (Models.WindowedSyncFilter filter in workBook.Filters.Values)
                        {
                            FilterDAL.Create(workBook, filter, sqlLiteConnection);
                        }

                        transaction.Commit();
                    } catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
                
            }
            return true;
        }

        public static WorkBook Load(string fromPath)
        {
            WorkBook newWorkBook = new WorkBook("");

            SqliteConnectionStringBuilder connectionString = new SqliteConnectionStringBuilder();
            connectionString.DataSource = fromPath;

            using (var connection = new SqliteConnection(connectionString.ToString()))
            {
                connection.Open();
                // for now only one exists in each file, if we change that approach, we'll need to update this query
                newWorkBook = connection.Query<WorkBook>($@"SELECT [Id], [Name], [Notes] FROM WorkBook").FirstOrDefault();

                var signals = connection.Query<Signal>($"SELECT * from Signals WHERE WorkBookId = '{newWorkBook.Id}'");
                var filters = connection.Query<Models.WindowedSyncFilter>($"SELECT * from Filters WHERE WorkBookId = '{newWorkBook.Id}'");

                foreach (Signal signal in signals)
                {
                    var signalValues = connection.Query<SignalValue>($"SELECT * from SignalValues WHERE SignalId = {signal.Id} ORDER BY Id ASC");
                    signal.Samples = new List<double>(signalValues.Count());

                    foreach (SignalValue value in signalValues)
                    {
                        signal.Samples.Add(value.Value);
                    }

                    newWorkBook.Signals.Add(signal.Name, signal);
                }

                foreach (Models.WindowedSyncFilter filter in filters)
                {
                    newWorkBook.Filters.Add(filter.Name, filter);
                }
            }

            return newWorkBook;
        }
    }
}
