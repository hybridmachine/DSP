using Microsoft.Data.Sqlite;
using SignalsAndTransforms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalsAndTransforms.DAL
{
    public static class SignalDAL
    {
        /// <summary>
        /// Create a signal with given name for the given workBook
        /// </summary>
        /// <param name="workBook"></param>
        /// <param name="name">Name of signal, must be unique</param>
        /// <param name="con">SQL Connection, it is assumed a transaction is already in process</param>
        /// <returns></returns>
        public static bool Create(WorkBook workBook, Signal signal, SqliteConnection con)
        {
            // For now we just clear and re-create signal entries, we may update values in place later
            // keep it simple for now
            string deleteSQL = $@"DELETE FROM Signals WHERE [Name]=@Name"; // This will cascade to the values table
            SqliteCommand cmd = con.CreateCommand();
            cmd.CommandText = deleteSQL;
            cmd.Parameters.AddWithValue("@Name", signal.Name);
            cmd.ExecuteNonQuery();

            string sql = $@"INSERT INTO Signals ([Name], [Type], [WorkBookId], [SamplingHZ], [SignalHZ], [SampleSeconds], [Amplitude]) 
                                VALUES (@Name, @Type, @WorkBookId, @SamplingHZ, @SignalHZ, @SampleSeconds, @Amplitude)";



            cmd = con.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@Name", signal.Name);
            cmd.Parameters.AddWithValue("@Type", signal.Type);
            cmd.Parameters.AddWithValue("@SamplingHZ", signal.SamplingHZ);
            cmd.Parameters.AddWithValue("@SignalHZ", signal.SignalHZ);
            cmd.Parameters.AddWithValue("@SampleSeconds", signal.SampleSeconds);
            cmd.Parameters.AddWithValue("@Amplitude", signal.Amplitude);
            cmd.Parameters.AddWithValue("@WorkBookId", workBook.Id);
            cmd.ExecuteNonQuery();

            sql = "SELECT last_insert_rowid();";
            using (SqliteCommand getId = con.CreateCommand())
            {
                getId.CommandText = sql;
                signal.Id = (long)getId.ExecuteScalar();
            }

            if (signal.Samples.Count > 0)
            {
                StringBuilder sqlblder = new StringBuilder();
                foreach (double sample in signal.Samples)
                {
                    sqlblder.Append($@"INSERT INTO SignalValues ([SignalID], [Value]) VALUES ('{signal.Id}', '{sample}');");
                }
                cmd = con.CreateCommand();
                cmd.CommandText = sqlblder.ToString();
                cmd.ExecuteNonQuery();
            }

            return true;
        }
    }
}
