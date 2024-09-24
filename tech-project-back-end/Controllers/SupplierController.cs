using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tech_project_back_end.Data;
using tech_project_back_end.DTO;
using tech_project_back_end.Models;
using tech_project_back_end.Services;

namespace tech_project_back_end.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class SupplierController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly SupplierService supplierService;
        public SupplierController(AppDbContext appDbContext, IMapper mapper)
        {
            this._appDbContext = appDbContext;
            this._mapper = mapper;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Supplier>> Add(SupplierDTO dto)
        {
            var supplierDTO = supplierService.CreateSupplier(dto);
            if (supplierDTO == null) {
                return BadRequest("Error while creating supplier");
            }
            return Ok(dto);
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetAll()
        {
            var supplierList = _appDbContext.Supplier.ToList();
            var supplierDTOList = _mapper.Map<List<Supplier>>(supplierList);

            return Ok(supplierDTOList);
        }



        [HttpGet("{id}")]
        public IActionResult GetSupplierById(string id)
        {

            var isExit = _appDbContext.Supplier.FirstOrDefault(c => c.SupplierId == id);
            if (isExit == null) { return NotFound("Supplier not found"); }

            var supplierDTO = _mapper.Map<SupplierDTO>(isExit);

            return Ok(supplierDTO);

        }

        [HttpDelete]
        public IActionResult DeleteById(string id)
        {
            var isExit = _appDbContext.Supplier.FirstOrDefault(x => x.SupplierId == id);
            if (isExit == null) { return NotFound("Supplier not found"); }

            _appDbContext.Supplier.RemoveRange(isExit);
            _appDbContext.SaveChanges();

            return Ok(isExit);
        }

        [HttpPut]
        public IActionResult Update(SupplierDTO dto)
        {
            var isExit = _appDbContext.Supplier.FirstOrDefault(c => c.SupplierId == dto.SupplierId);
            if (isExit == null) { return NotFound("Supplier not found"); }

            _appDbContext.Supplier.Update(isExit);
            _appDbContext.SaveChanges();

            return Ok(dto);
        }




    }
}
