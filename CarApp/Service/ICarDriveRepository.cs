using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarApp.Helper;
using CarApp.Models;

namespace CarApp.Service
{
    public interface ICarDriveRepository
    {
        Task InsertCarDriveWithDetails(CarDriveHeader header, List<CarDriveDtList> details);
        Task UpdateCarDriveWithDetails(CarDriveHeader header, List<CarDriveDtList> details);
        Task<(CarDriveHeader Header, List<CarDriveDtList> Details)> GetCarDriveById(string carDrId);
        Task DeleteCarDrive(string carDrId);
        
        Task<PaginatedResult<CarDriveSearchResult>> SearchCarDrivesFull(QueryObject queryObject);

    }
}