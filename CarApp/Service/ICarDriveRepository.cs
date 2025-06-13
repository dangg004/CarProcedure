using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarApp.Models;

namespace CarApp.Service
{
    public interface ICarDriveRepository
    {
        Task InsertCarDriveWithDetails(CarDriveHeader header, List<CarDriveDtList> details);
        Task UpdateCarDriveWithDetails(CarDriveHeader header, List<CarDriveDtList> details);
        Task<(CarDriveHeader Header, List<CarDriveDtList> Details)> GetCarDriveById(string carDrId);
        Task DeleteCarDrive(string carDrId);
        
    // ... existing methods ...
        Task<List<CarDriveSearchResult>> SearchCarDrivesFull(string? carId = null, string? assetId = null, string? invoiceNo = null, string? driveNote = null);

    }
}