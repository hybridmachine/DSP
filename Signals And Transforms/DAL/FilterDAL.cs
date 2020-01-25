using Microsoft.Data.Sqlite;
using SignalProcessor.Filters;
using SignalsAndTransforms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalsAndTransforms.DAL
{
    public static class FilterDAL
    {
        public static bool Create(WorkBook workBook, Models.WindowedSyncFilter filter, SqliteConnection con)
        {
            // For now we just clear and re-create signal entries, we may update values in place later
            // keep it simple for now
            string deleteSQL = $@"DELETE FROM Filters WHERE [Name]=@Name"; // This will cascade to the values table
            SqliteCommand cmd = con.CreateCommand();
            cmd.CommandText = deleteSQL;
            cmd.Parameters.AddWithValue("@Name", filter.Name);
            cmd.ExecuteNonQuery();

            string sql = $@"INSERT INTO Filters ([Name], [IsActive], [WorkBookId], [CutoffFrequencySamplingFrequencyPercentage], [FilterLength], [FilterType]) 
                                VALUES (@Name, @IsActive, @WorkBookId, @CutoffFrequencySamplingFrequencyPercentage, @FilterLength, @FilterType)";



            cmd = con.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@Name", filter.Name);
            cmd.Parameters.AddWithValue("@IsActive", filter.IsActive);
            cmd.Parameters.AddWithValue("@FilterType", filter.FilterType.ToString());
            cmd.Parameters.AddWithValue("@FilterLength", filter.FilterLength);
            cmd.Parameters.AddWithValue("@CutoffFrequencySamplingFrequencyPercentage", filter.CutoffFrequencySamplingFrequencyPercentage);
            cmd.Parameters.AddWithValue("@WorkBookId", workBook.Id);
            cmd.ExecuteNonQuery();

            return true;
        }
    }
}
