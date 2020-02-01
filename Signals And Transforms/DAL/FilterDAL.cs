using Microsoft.Data.Sqlite;
using SignalProcessor.Filters;
using SignalsAndTransforms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SignalsAndTransforms.DAL
{
    public static class FilterDAL
    {
        public static bool Create(WorkBook workBook, Models.CustomFilter filter, SqliteConnection con)
        {
            // For now we just clear and re-create signal entries, we may update values in place later
            // keep it simple for now
            string deleteSQL = $@"DELETE FROM Filters WHERE [Name]=@Name"; // This will cascade to the values table
            SqliteCommand cmd = con.CreateCommand();
            cmd.CommandText = deleteSQL;
            cmd.Parameters.AddWithValue("@Name", filter.Name);
            cmd.ExecuteNonQuery();

            string sql = $@"INSERT INTO Filters (   [Name], 
                                                    [IsActive], 
                                                    [WorkBookId], 
                                                    [FilterType]) 
                                VALUES (@Name, @IsActive, @WorkBookId, @FilterType)";

            cmd = con.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@Name", filter.Name);
            cmd.Parameters.AddWithValue("@IsActive", filter.IsActive);
            cmd.Parameters.AddWithValue("@WorkBookId", workBook.Id);
            cmd.Parameters.AddWithValue("@FilterType", filter.FilterType.ToString());
            cmd.ExecuteNonQuery();

            sql = "SELECT last_insert_rowid();";
            using (SqliteCommand getId = con.CreateCommand())
            {
                getId.CommandText = sql;
                filter.Id = (long)getId.ExecuteScalar();
            }

            if (filter.FrequencyResponse().Count() > 0)
            {
                StringBuilder sqlBuilder = new StringBuilder();

                int sequence = 0;
                foreach (Complex value in filter.FrequencyResponse())
                {
                    sqlBuilder.Append($@"INSERT INTO MagnitudePhase 
                                ([FilterId], [Sequence], [Magnitude], [Phase])
                                VALUES ('{filter.Id}', '{sequence++}', '{value.Magnitude}', '{value.Phase}');");
                }
                cmd = con.CreateCommand();
                cmd.CommandText = sqlBuilder.ToString();
                cmd.ExecuteNonQuery();
            }

            return true;
        }
        public static bool Create(WorkBook workBook, Models.WindowedSyncFilter filter, SqliteConnection con)
        {
            // For now we just clear and re-create signal entries, we may update values in place later
            // keep it simple for now
            string deleteSQL = $@"DELETE FROM Filters WHERE [Name]=@Name"; // This will cascade to the values table
            SqliteCommand cmd = con.CreateCommand();
            cmd.CommandText = deleteSQL;
            cmd.Parameters.AddWithValue("@Name", filter.Name);
            cmd.ExecuteNonQuery();

            string sql = $@"INSERT INTO Filters ([Name], [IsActive], [WorkBookId], [FilterType]) 
                                VALUES (@Name, @IsActive, @WorkBookId, @FilterType)";



            cmd = con.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@Name", filter.Name);
            cmd.Parameters.AddWithValue("@IsActive", filter.IsActive);
            cmd.Parameters.AddWithValue("@FilterType", filter.FilterType.ToString());
            cmd.Parameters.AddWithValue("@WorkBookId", workBook.Id);
            cmd.ExecuteNonQuery();

            sql = "SELECT last_insert_rowid();";
            using (SqliteCommand getId = con.CreateCommand())
            {
                getId.CommandText = sql;
                filter.Id = (long)getId.ExecuteScalar();
            }

            sql = @"INSERT INTO WindowedSyncFilterParameters 
                    ([FilterId], [CutoffFrequencySamplingFrequencyPercentage], [FilterLength]) 
                    VALUES (@FilterId, @CutoffFrequencySamplingFrequencyPercentage, @FilterLength)";

            cmd = con.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@FilterId", filter.Id);
            cmd.Parameters.AddWithValue("@FilterLength", filter.FilterLength);
            cmd.Parameters.AddWithValue("@CutoffFrequencySamplingFrequencyPercentage", filter.CutoffFrequencySamplingFrequencyPercentage);
            cmd.ExecuteNonQuery();

            return true;
        }
    }
}
