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
            string sql = $@"INSERT INTO Signal ([Name], [WorkBookId], [SamplingHZ], [SignalHZ], [SampleSeconds]) 
                                VALUES (@Name, @WorkBookId, @SamplingHZ, @SignalHZ, @SampleSeconds)";

            
            SqliteCommand cmd = con.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@Name", signal.Name);
            cmd.Parameters.AddWithValue("@SamplingHZ", signal.SamplingHZ);
            cmd.Parameters.AddWithValue("@SignalHZ", signal.SignalHZ);
            cmd.Parameters.AddWithValue("@SampleSeconds", signal.SampleSeconds);

            cmd.Parameters.AddWithValue("@WorkBookId", workBook.Id);
            cmd.ExecuteNonQuery();

            return true;
        }
    }
}
