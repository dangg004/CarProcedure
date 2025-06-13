using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CarApp.Data;
using CarApp.Models;
using CarApp.Service;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CarApp.Repository
{
    public class CarDriveRepository : ICarDriveRepository
    {
        private readonly ApplicationDbContext _context;

        public CarDriveRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task InsertCarDriveWithDetails(CarDriveHeader header, List<CarDriveDtList> details)
        {
            using var conn = new SqlConnection(_context.Database.GetConnectionString());
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_CAR_DRIVE_InsertFull", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // Add scalar params from 'header'
            cmd.Parameters.AddWithValue("@CAR_DR_ID", header.CAR_DR_ID);
            cmd.Parameters.AddWithValue("@CAR_ID", header.CAR_ID ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ASSET_ID", header.ASSET_ID ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ASSET_NAME", header.ASSET_NAME ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@OLD_INDEX_NUMBER", header.OLD_INDEX_NUMBER ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@NEW_INDEX_NUMBER", header.NEW_INDEX_NUMBER ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@INDEX_NUMBER", header.INDEX_NUMBER ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@POWER_RATE", header.POWER_RATE ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@POWER_RATE_INDEX", header.POWER_RATE_INDEX ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CURR_POWER_RATE", header.CURR_POWER_RATE ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@INPUT_DT", header.INPUT_DT ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ISLEAF", header.ISLEAF ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PARENT_ID", header.PARENT_ID ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@NOTES", header.NOTES ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@RECORD_STATUS", header.RECORD_STATUS ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@MAKER_ID", header.MAKER_ID ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CREATE_DT", header.CREATE_DT ?? DateTime.Now);
            cmd.Parameters.AddWithValue("@AUTH_STATUS", header.AUTH_STATUS ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CHECKER_ID", header.CHECKER_ID ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@APPROVE_DT", header.APPROVE_DT ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CAR_AMT", header.CAR_AMT ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@INVOICE_NO", header.INVOICE_NO ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DR_ROUTE", header.DR_ROUTE ?? (object)DBNull.Value);

            // Add TVP param
            var tvp = new SqlParameter("@DT_List", SqlDbType.Structured)
            {
                TypeName = "CAR_DRIVE_DT_List",
                Value = ConvertToDataTable(details)
            };
            cmd.Parameters.Add(tvp);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdateCarDriveWithDetails(CarDriveHeader header, List<CarDriveDtList> details)
        {
            using var conn = new SqlConnection(_context.Database.GetConnectionString());
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_CAR_DRIVE_UpdateFull", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // Add scalar params from 'header'
            cmd.Parameters.AddWithValue("@CAR_DR_ID", header.CAR_DR_ID);
            cmd.Parameters.AddWithValue("@CAR_ID", header.CAR_ID ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ASSET_ID", header.ASSET_ID ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ASSET_NAME", header.ASSET_NAME ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@OLD_INDEX_NUMBER", header.OLD_INDEX_NUMBER ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@NEW_INDEX_NUMBER", header.NEW_INDEX_NUMBER ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@INDEX_NUMBER", header.INDEX_NUMBER ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@POWER_RATE", header.POWER_RATE ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@POWER_RATE_INDEX", header.POWER_RATE_INDEX ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CURR_POWER_RATE", header.CURR_POWER_RATE ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@INPUT_DT", header.INPUT_DT ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ISLEAF", header.ISLEAF ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PARENT_ID", header.PARENT_ID ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@NOTES", header.NOTES ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@RECORD_STATUS", header.RECORD_STATUS ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@MAKER_ID", header.MAKER_ID ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CREATE_DT", header.CREATE_DT ?? DateTime.Now);
            cmd.Parameters.AddWithValue("@AUTH_STATUS", header.AUTH_STATUS ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CHECKER_ID", header.CHECKER_ID ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@APPROVE_DT", header.APPROVE_DT ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CAR_AMT", header.CAR_AMT ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@INVOICE_NO", header.INVOICE_NO ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DR_ROUTE", header.DR_ROUTE ?? (object)DBNull.Value);

            // Add TVP parameter
            var tvp = new SqlParameter("@DT_List", SqlDbType.Structured)
            {
                TypeName = "CAR_DRIVE_DT_List",
                Value = ConvertToDataTable(details)
            };
            cmd.Parameters.Add(tvp);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<(CarDriveHeader Header, List<CarDriveDtList> Details)> GetCarDriveById(string carDrId)
        {
            using var conn = new SqlConnection(_context.Database.GetConnectionString());
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_CAR_DRIVE_GetById", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CAR_DR_ID", carDrId);

            using var reader = await cmd.ExecuteReaderAsync();
            
            CarDriveHeader header = null;
            var details = new List<CarDriveDtList>();

            if (await reader.ReadAsync())
            {
                header = new CarDriveHeader
                {
                    CAR_DR_ID = reader["CAR_DR_ID"].ToString(),
                    CAR_ID = reader["CAR_ID"]?.ToString(),
                    ASSET_ID = reader["ASSET_ID"]?.ToString(),
                    ASSET_NAME = reader["ASSET_NAME"]?.ToString(),
                    OLD_INDEX_NUMBER = reader["OLD_INDEX_NUMBER"] as decimal?,
                    NEW_INDEX_NUMBER = reader["NEW_INDEX_NUMBER"] as decimal?,
                    INDEX_NUMBER = reader["INDEX_NUMBER"] as decimal?,
                    POWER_RATE = reader["POWER_RATE"] as decimal?,
                    POWER_RATE_INDEX = reader["POWER_RATE_INDEX"] as decimal?,
                    CURR_POWER_RATE = reader["CURR_POWER_RATE"] as decimal?,
                    INPUT_DT = reader["INPUT_DT"] as DateTime?,
                    ISLEAF = reader["ISLEAF"]?.ToString(),
                    PARENT_ID = reader["PARENT_ID"]?.ToString(),
                    NOTES = reader["NOTES"]?.ToString(),
                    RECORD_STATUS = reader["RECORD_STATUS"]?.ToString(),
                    MAKER_ID = reader["MAKER_ID"]?.ToString(),
                    CREATE_DT = reader["CREATE_DT"] as DateTime?,
                    AUTH_STATUS = reader["AUTH_STATUS"]?.ToString(),
                    CHECKER_ID = reader["CHECKER_ID"]?.ToString(),
                    APPROVE_DT = reader["APPROVE_DT"] as DateTime?,
                    CAR_AMT = reader["CAR_AMT"] as decimal?,
                    INVOICE_NO = reader["INVOICE_NO"]?.ToString(),
                    DR_ROUTE = reader["DR_ROUTE"]?.ToString()
                };
            }

            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    details.Add(new CarDriveDtList
                    {
                        DRDT_ID = reader["DRDT_ID"].ToString(),
                        INVOICE_NO = reader["INVOICE_NO"]?.ToString(),
                        INVOICE_DT = reader["INVOICE_DT"] as DateTime?,
                        INVOICE_AMT = reader["INVOICE_AMT"] as decimal?,
                        CAR_DR_TYPE = reader["CAR_DR_TYPE"]?.ToString(),
                        NOTES = reader["NOTES"]?.ToString()
                    });
                }
            }

            return (header, details);
        }

        public async Task DeleteCarDrive(string carDrId)
        {
            using var conn = new SqlConnection(_context.Database.GetConnectionString());
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_CAR_DRIVE_DeleteFull", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CAR_DR_ID", carDrId);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<CarDriveSearchResult>> SearchCarDrivesFull(
            string? carId = null,
            string? assetId = null,
            string? invoiceNo = null,
            string? driveNote = null)
        {
            using var conn = new SqlConnection(_context.Database.GetConnectionString());
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_CAR_DRIVE_SearchFull", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@CarId", carId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@AssetId", assetId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@InvoiceNo", invoiceNo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DriveNote", driveNote ?? (object)DBNull.Value);

            using var reader = await cmd.ExecuteReaderAsync();
            var results = new List<CarDriveSearchResult>();

            while (await reader.ReadAsync())
            {
                results.Add(new CarDriveSearchResult
                {
                    // Header properties
                    CAR_DR_ID = reader["CAR_DR_ID"].ToString(),
                    CAR_ID = reader["CAR_ID"]?.ToString(),
                    ASSET_ID = reader["ASSET_ID"]?.ToString(),
                    ASSET_NAME = reader["ASSET_NAME"]?.ToString(),
                    OLD_INDEX_NUMBER = reader["OLD_INDEX_NUMBER"] as decimal?,
                    NEW_INDEX_NUMBER = reader["NEW_INDEX_NUMBER"] as decimal?,
                    INDEX_NUMBER = reader["INDEX_NUMBER"] as decimal?,
                    POWER_RATE = reader["POWER_RATE"] as decimal?,
                    POWER_RATE_INDEX = reader["POWER_RATE_INDEX"] as decimal?,
                    CURR_POWER_RATE = reader["CURR_POWER_RATE"] as decimal?,
                    INPUT_DT = reader["INPUT_DT"] as DateTime?,
                    ISLEAF = reader["ISLEAF"]?.ToString(),
                    PARENT_ID = reader["PARENT_ID"]?.ToString(),
                    NOTES = reader["NOTES"]?.ToString(),
                    RECORD_STATUS = reader["RECORD_STATUS"]?.ToString(),
                    MAKER_ID = reader["MAKER_ID"]?.ToString(),
                    CREATE_DT = reader["CREATE_DT"] as DateTime?,
                    AUTH_STATUS = reader["AUTH_STATUS"]?.ToString(),
                    CHECKER_ID = reader["CHECKER_ID"]?.ToString(),
                    APPROVE_DT = reader["APPROVE_DT"] as DateTime?,
                    CAR_AMT = reader["CAR_AMT"] as decimal?,
                    INVOICE_NO = reader["INVOICE_NO"]?.ToString(),
                    DR_ROUTE = reader["DR_ROUTE"]?.ToString(),

                    // Detail properties
                    DRDT_ID = reader["DRDT_ID"]?.ToString(),
                    DT_INVOICE_NO = reader["INVOICE_NO"]?.ToString(),
                    INVOICE_DT = reader["INVOICE_DT"] as DateTime?,
                    INVOICE_AMT = reader["INVOICE_AMT"] as decimal?,
                    CAR_DR_TYPE = reader["CAR_DR_TYPE"]?.ToString(),
                    DT_NOTES = reader["DT_NOTES"]?.ToString()
                });
            }

            return results;
        }

        private DataTable ConvertToDataTable(List<CarDriveDtList> details)
        {
            var table = new DataTable();
            table.Columns.Add("DRDT_ID", typeof(string));
            table.Columns.Add("INVOICE_NO", typeof(string));
            table.Columns.Add("INVOICE_DT", typeof(DateTime));
            table.Columns.Add("INVOICE_AMT", typeof(decimal));
            table.Columns.Add("CAR_DR_TYPE", typeof(string));
            table.Columns.Add("NOTES", typeof(string));

            foreach (var item in details)
            {
                table.Rows.Add(
                    item.DRDT_ID,
                    item.INVOICE_NO,
                    item.INVOICE_DT,
                    item.INVOICE_AMT,
                    item.CAR_DR_TYPE,
                    item.NOTES
                );
            }

            return table;
        }
    }
}