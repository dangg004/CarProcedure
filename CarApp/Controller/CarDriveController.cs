using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarApp.Helper;
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
                return Ok(new { 
                    message = "Car Drive record inserted successfully",
                    data = new {
                        header = request.Header,
                        details = request.Details
                    }
                });
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
                return Ok(new
                {
                    message = "Car Drive record updated successfully",
                    data = new
                    {
                        header = request.Header,
                        details = request.Details
                    }
                });
                
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
        public async Task<IActionResult> SearchCarDrivesFull([FromQuery] QueryObject query)
        {
            try
            {
                var result = await _repository.SearchCarDrivesFull(query);
                return Ok(new
                {
                    items = result.Items,
                    pageNumber = result.PageNumber,
                    pageSize = result.PageSize,
                    totalCount = result.TotalCount,
                    totalPages = result.TotalPages,
                    hasPreviousPage = result.HasPreviousPage,
                    hasNextPage = result.HasNextPage
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}