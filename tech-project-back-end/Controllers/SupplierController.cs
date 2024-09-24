using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tech_project_back_end.Data;
using tech_project_back_end.DTO;
using tech_project_back_end.Models;
using tech_project_back_end.Services;
using tech_project_back_end.Services.IService;

namespace tech_project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SupplierController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly ISupplierService supplierService;
        private readonly ILogger _logger;
        public SupplierController(AppDbContext appDbContext, IMapper mapper, ISupplierService supplierService)
        {
            this._appDbContext = appDbContext;
            this._mapper = mapper;
            this.supplierService = supplierService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] SupplierDTO dto)
        {
            var supplierDTO = await supplierService.CreateSupplier(dto);
            if (supplierDTO == null) 
            {
                return BadRequest("Error while creating supplier");
            }
            return Ok(supplierDTO);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var supplierDTOList = await supplierService.GetAllSupplier();
            return Ok(supplierDTOList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var supplierDTO = await supplierService.GetSupplierById(id);
            return Ok(supplierDTO);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteById(string id)
        {
            var supplierDTO = await supplierService.DeleteSupplierById(id);

            return Ok(supplierDTO);
        }

        [HttpPut]
        public async Task<IActionResult> Update(SupplierDTO dto)
        {
            var supplierDTO = await supplierService.UpdateSupplier(dto);

            return Ok(supplierDTO);
        }
    }
}
