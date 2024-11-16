using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tech_project_back_end.DTO;
using tech_project_back_end.Services.IService;

namespace tech_project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        private readonly ILogger _logger;
        public SupplierController(ISupplierService supplierService, ILogger logger)
        {
            this._supplierService = supplierService;
            this._logger = logger;
        }

        [Authorize(Policy = "ADMIN")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] SupplierDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _supplierService.CreateSupplier(dto);
                return result != null
                    ? Ok(result)
                    : BadRequest("Failed to create supplier");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating supplier");
                return StatusCode(500, "An error occurred while processing the request");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var supplierList = await _supplierService.GetAllSupplier();
                return Ok(supplierList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all the suppliers");
                return StatusCode(500, "An error while processing the request");
            }
        }

        [Authorize(Policy = "ADMIN")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var supplier = await _supplierService.GetSupplierById(id);
                return supplier != null
                    ? Ok(supplier)
                    : NotFound($"Supplier with ID {id} not found");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error while fetching the supplier with id :", id);
                return StatusCode(500, "An error occurred while processing the request");
            }
        }

        [Authorize(Policy = "ADMIN")]
        [HttpDelete]
        public async Task<IActionResult> DeleteById(string id)
        {
            try
            {
                var result = await _supplierService.DeleteSupplierById(id);
                return result != null
                    ? Ok(result)
                    : NotFound($"Supplier with id is {id} not found");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting supplier with id: ", id);
                return StatusCode(500, "Error while processing the request");
            }
        }

        [Authorize(Policy = "ADMIN")]
        [HttpPut]
        public async Task<IActionResult> Update(string id, [FromBody] SupplierDTO dto)
        {
            if (id != dto.SupplierId)
            {
                return BadRequest("Id in url not match id in supplier request body");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _supplierService.UpdateSupplier(dto);
                return result != null
                    ? Ok(result)
                    : NotFound($"Supplier with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating the supplier with id: ", id);
                return StatusCode(500, "An error occurred while processing the request");
            }

        }
    }
}
