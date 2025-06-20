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
using Microsoft.Extensions.Caching.Distributed;

namespace CarApp.Repository
{
    public class CarDriveRepository : ICarDriveRepository
    {
        private const string CACHE_KEY_PREFIX = "carDrive:";
        private readonly string _connectionString;
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly ILogger<CarDriveRepository> _logger;

        // Cache key generators
        private string GenerateCacheKey(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Attempted to generate cache key with null or empty id");
                return string.Empty;
            }
            return $"{CACHE_KEY_PREFIX}entity:{id}";
        }

        private string GenerateSearchCacheKey(QueryObject query)
        {
            if (query == null)
            {
                _logger.LogWarning("Attempted to generate search cache key with null query");
                return string.Empty;
            }

            return $"{CACHE_KEY_PREFIX}list:{query.carId}:{query.assetId}:{query.invoiceNo}:{query.driveNote}:{query.PageNumber}:{query.PageSize}";
        }

        private async Task InvalidateSearchCache()
        {
            var searchKeysKey = $"{CACHE_KEY_PREFIX}activeSearchKeys";
            var activeKeys = await _cache.GetStringAsync(searchKeysKey);
            if (!string.IsNullOrEmpty(activeKeys))
            {
                var keys = JsonSerializer.Deserialize<List<string>>(activeKeys);
                foreach (var key in keys)
                {
                    await _cache.RemoveAsync(key);
                }
            }
            await _cache.RemoveAsync(searchKeysKey);
        }

        private async Task AddSearchCacheKey(string cacheKey)
        {
            var searchKeysKey = $"{CACHE_KEY_PREFIX}activeSearchKeys";
            var activeKeys = await _cache.GetStringAsync(searchKeysKey);
            var keys = string.IsNullOrEmpty(activeKeys) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(activeKeys);
            if (!keys.Contains(cacheKey))
            {
                keys.Add(cacheKey);
                await _cache.SetStringAsync(searchKeysKey, JsonSerializer.Serialize(keys));
            }
        }

        public CarDriveRepository(ApplicationDbContext context, IDistributedCache cache, ILogger<CarDriveRepository> logger)
        {
            _connectionString = context.Database.GetConnectionString();
            _cache = cache;
            _context = context;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

            await InvalidateSearchCache();
        }

        public async Task UpdateCarDriveWithDetails(CarDriveHeader header, List<CarDriveDtList> details)
        {
            var result = await ExecuteStoredProcedureMultiResult<CarDriveHeader, CarDriveDtList>(
                "sp_CAR_DRIVE_GetById",
                new { CAR_DR_ID = header.CAR_DR_ID });

            if (result.First == null)
            {
                throw new Exception($"Car Drive with ID {header.CAR_DR_ID} does not exist.");
            }
            var jsonData = new
            {
                Header = header,
                Details = details
            };

            await ExecuteStoredProcedure<int>(
                "sp_CAR_DRIVE_UpdateFull",
                new { JsonData = JsonSerializer.Serialize(jsonData) });

            
            if (!string.IsNullOrEmpty(header.CAR_DR_ID))
            {
                var entityCacheKey = GenerateCacheKey(header.CAR_DR_ID);
                await _cache.RemoveAsync(entityCacheKey);

                // Also invalidate any search results
                await InvalidateSearchCache();
            }
        }

        public async Task<(CarDriveHeader Header, List<CarDriveDtList> Details)> GetCarDriveById(string carDrId)
        {
            var cacheKey = GenerateCacheKey(carDrId);
            var cached = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cached))
            {
                var cachedData = JsonSerializer.Deserialize<CachedCarDrive>(cached);
                _logger.LogInformation($"Cache HIT: {cached}");
                return (cachedData.Header, cachedData.Details);
            }
            var result = await ExecuteStoredProcedureMultiResult<CarDriveHeader, CarDriveDtList>(
                "sp_CAR_DRIVE_GetById",
                new { CAR_DR_ID = carDrId });

            if (result.First != null)
            {
                var cacheEntry = new CachedCarDrive
                {
                    Header = result.First,
                    Details = result.Second
                };

                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                };

                await _cache.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(cacheEntry),
                    cacheOptions
                );
            }
            return (result.First, result.Second);
        }

        public async Task DeleteCarDrive(string carDrId)
        {

            await ExecuteStoredProcedure<int>(
                "sp_CAR_DRIVE_DeleteFull",
                new { CAR_DR_ID = carDrId });

            if (!string.IsNullOrEmpty(carDrId))
            {
                var entityCacheKey = GenerateCacheKey(carDrId);
                await _cache.RemoveAsync(entityCacheKey);

                // Also invalidate any search results
                await InvalidateSearchCache();
            }
        }

        public async Task<PaginatedResult<CarDriveSearchResult>> SearchCarDrivesFull(QueryObject queryObject)
        {
            // Generate cache key based on search parameters
            var cacheKey = GenerateSearchCacheKey(queryObject);

            // Try get from cache
            var cached = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cached))
            {
                _logger.LogInformation($"Cache HIT: {cached}");
                return JsonSerializer.Deserialize<PaginatedResult<CarDriveSearchResult>>(cached);
            }
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

            var paginatedResult = new PaginatedResult<CarDriveSearchResult>
            {
                Items = result.Second,
                PageNumber = queryObject.PageNumber,
                PageSize = queryObject.PageSize,
                TotalCount = result.First
            };

            // Cache the result
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(paginatedResult),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),  // Shorter cache time for search results
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                });

            await AddSearchCacheKey(cacheKey);
            return paginatedResult;
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