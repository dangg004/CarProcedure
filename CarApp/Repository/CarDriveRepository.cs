using System;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
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
        private readonly string _connectionString;

        public CarDriveRepository(ApplicationDbContext context)
        {
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

        private async Task ExecuteStoredProcedureWithTVP(string procedureName, object parameters, string tvpName, DataTable tvpData)
        {
            using var connection = new SqlConnection(_connectionString);
            var dynamicParams = new DynamicParameters(parameters);
            dynamicParams.Add("@DT_List", tvpData.AsTableValuedParameter(tvpName));

            await connection.ExecuteAsync(
                procedureName,
                dynamicParams,
                commandType: CommandType.StoredProcedure);
        }

        private async Task<(T1 First, List<T2> Second)> ExecuteStoredProcedureMultiResult<T1, T2>(
            string procedureName, 
            object parameters)
        {
            using var connection = new SqlConnection(_connectionString);
            using var multi = await connection.QueryMultipleAsync(
                procedureName,
                parameters,
                commandType: CommandType.StoredProcedure);

            var first = await multi.ReadFirstOrDefaultAsync<T1>();
            var second = (await multi.ReadAsync<T2>()).ToList();

            return (first, second);
        }

        public async Task InsertCarDriveWithDetails(CarDriveHeader header, List<CarDriveDtList> details)
        {
            var jsonData = new
            {
                Header = header,
                Details = details
            };

            await ExecuteStoredProcedure<int>(
                "sp_CAR_DRIVE_InsertFull",
                new { JsonData = JsonSerializer.Serialize(jsonData) });
        }

        public async Task UpdateCarDriveWithDetails(CarDriveHeader header, List<CarDriveDtList> details)
        {
            var jsonData = new
            {
                Header = header,
                Details = details
            };

            await ExecuteStoredProcedure<int>(
                "sp_CAR_DRIVE_UpdateFull",
                new { JsonData = JsonSerializer.Serialize(jsonData) });
        }

        public async Task<(CarDriveHeader Header, List<CarDriveDtList> Details)> GetCarDriveById(string carDrId)
        {
            var result = await ExecuteStoredProcedureMultiResult<CarDriveHeader, CarDriveDtList>(
                "sp_CAR_DRIVE_GetById",
                new { CAR_DR_ID = carDrId });

            return (result.First, result.Second);
        }

        public async Task DeleteCarDrive(string carDrId)
        {
            await ExecuteStoredProcedure<int>(
                "sp_CAR_DRIVE_DeleteFull",
                new { CAR_DR_ID = carDrId });
        }

        public async Task<PaginatedResult<CarDriveSearchResult>> SearchCarDrivesFull(QueryObject queryObject)
        {
            var jsonParams = JsonSerializer.Serialize(new
            {
                queryObject.carId,
                queryObject.assetId,
                queryObject.invoiceNo,
                queryObject.driveNote,
                pageNumber = queryObject.PageNumber,
                pageSize = queryObject.PageSize
            });
            var result = await ExecuteStoredProcedureMultiResult<int, CarDriveSearchResult>(
                "sp_CAR_DRIVE_SearchFull",
                new { SearchParams = jsonParams });

            return new PaginatedResult<CarDriveSearchResult>
            {
                Items = result.Second,
                PageNumber = queryObject.PageNumber,
                PageSize = queryObject.PageSize,
                TotalCount = result.First
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