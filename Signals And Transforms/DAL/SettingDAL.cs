using Microsoft.Data.Sqlite;
using Serilog;
using SignalsAndTransforms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalsAndTransforms.DAL
{
    public static class SettingDAL
    {

        /// <summary>
        /// Create a setting (deleting one if it is already there
        /// </summary>
        /// <param name="workBook"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="con">SQL Connection, it is assumed a transaction is already in process</param>
        /// <returns></returns>
        public static bool Create(WorkBook workBook, string key, string value, SqliteConnection con)
        {
            if (null == workBook) throw new ArgumentNullException(nameof(workBook));
            if (null == key) throw new ArgumentNullException(nameof(key));
            if (null == value) throw new ArgumentNullException(nameof(value));
            if (null == con) throw new ArgumentException(nameof(con));

            if (workBook.FilePath == null)
            {
                throw new Exception(Properties.Resources.ERROR_WORKBOOK_FILEPATHNOTSET);
            }

            string sql = $@"DELETE FROM Settings WHERE Key=@Key AND WorkBookId=@WorkBookId; INSERT INTO Settings ('WorkBookId', 'Key', 'Value') VALUES(@WorkBookId, @Key,@Value);";
            
            try
            {
                SqliteCommand cmd = con.CreateCommand();
                cmd.CommandText = sql;

                cmd.Parameters.AddWithValue("@WorkBookId", workBook.Id);
                cmd.Parameters.AddWithValue("@Key", key);
                cmd.Parameters.AddWithValue("@Value", value);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }
               
            return true;
        }
    }
}
