using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarApp.Models;
using CarApp.Service;
using Microsoft.AspNetCore.Mvc;

namespace CarApp.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarDriveController : ControllerBase
    {
        private readonly ICarDriveRepository _repository;

        public CarDriveController(ICarDriveRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("insert")]
        public async Task<IActionResult> InsertCarDrive([FromBody] CarDriveRequest request)
        {
            try
            {
                await _repository.InsertCarDriveWithDetails(request.Header, request.Details);
                return Ok(new { message = "Car Drive record inserted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCarDrive([FromBody] CarDriveRequest request)
        {
            try
            {
                await _repository.UpdateCarDriveWithDetails(request.Header, request.Details);
                return Ok(new { message = "Car Drive record updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var (header, details) = await _repository.GetCarDriveById(id);
                if (header == null)
                    return NotFound(new { message = $"Car Drive with ID {id} not found" });

                return Ok(new { header, details });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCarDrive(string id)
        {
            try
            {
                await _repository.DeleteCarDrive(id);
                return Ok(new { message = $"Car Drive record {id} deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("search-full")]
        public async Task<IActionResult> SearchCarDrivesFull(
            [FromQuery] string? carId = null,
            [FromQuery] string? assetId = null,
            [FromQuery] string? invoiceNo = null,
            [FromQuery] string? driveNote = null)
        {
            try
            {
                var results = await _repository.SearchCarDrivesFull(carId, assetId, invoiceNo, driveNote);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}