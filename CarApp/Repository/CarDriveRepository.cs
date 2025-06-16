using System;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CarApp.Data;
using CarApp.Helper;
using CarApp.Models;
using CarApp.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CarApp.Repository
{
    public class CarDriveRepository : ICarDriveRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public CarDriveRepository(ApplicationDbContext context)
        {
            _context = context;
            _connectionString = context.Database.GetConnectionString();
        }

        private async Task<T> ExecuteStoredProcedure<T>(string procedureName, object parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<T>(
                procedureName,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task InsertCarDriveWithDetails(CarDriveHeader header, List<CarDriveDtList> details)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters(header);
            parameters.Add("@DT_List", CreateDetailTable(details).AsTableValuedParameter("CAR_DRIVE_DT_List"));

            await connection.ExecuteAsync(
                "sp_CAR_DRIVE_InsertFull",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateCarDriveWithDetails(CarDriveHeader header, List<CarDriveDtList> details)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters(header);
            parameters.Add("@DT_List", CreateDetailTable(details).AsTableValuedParameter("CAR_DRIVE_DT_List"));

            await connection.ExecuteAsync(
                "sp_CAR_DRIVE_UpdateFull",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<(CarDriveHeader Header, List<CarDriveDtList> Details)> GetCarDriveById(string carDrId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var multi = await connection.QueryMultipleAsync(
                "sp_CAR_DRIVE_GetById",
                new { CAR_DR_ID = carDrId },
                commandType: CommandType.StoredProcedure);

            var header = await multi.ReadFirstOrDefaultAsync<CarDriveHeader>();
            var details = (await multi.ReadAsync<CarDriveDtList>()).ToList();

            return (header, details);
        }

        public async Task DeleteCarDrive(string carDrId)
        {
            await ExecuteStoredProcedure<int>(
                "sp_CAR_DRIVE_DeleteFull",
                new { CAR_DR_ID = carDrId });
        }

        public async Task<PaginatedResult<CarDriveSearchResult>> SearchCarDrivesFull(QueryObject queryObject)
        {
            using var connection = new SqlConnection(_connectionString);
            using var multi = await connection.QueryMultipleAsync(
                "sp_CAR_DRIVE_SearchFull",
                new
                {
                    queryObject.carId,
                    queryObject.assetId,
                    queryObject.invoiceNo,
                    queryObject.driveNote,
                    queryObject.PageNumber,
                    queryObject.PageSize
                },
                commandType: CommandType.StoredProcedure);

            var totalCount = await multi.ReadFirstAsync<int>();
            var results = await multi.ReadAsync<CarDriveSearchResult>();

            return new PaginatedResult<CarDriveSearchResult>
            {
                Items = results.ToList(),
                PageNumber = queryObject.PageNumber,
                PageSize = queryObject.PageSize,
                TotalCount = totalCount
            };
        }

        private DataTable CreateDetailTable(List<CarDriveDtList> details)
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