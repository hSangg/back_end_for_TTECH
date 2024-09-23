using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tech_project_back_end.Data;
using tech_project_back_end.Models;

namespace tech_project_back_end.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class SupplierController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public SupplierController(AppDbContext appDbContext)
        {
            this._appDbContext = appDbContext;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Supplier>> AddSupplier(Supplier supplier)
        {

            supplier.SupplierId = Guid.NewGuid().ToString()[..36];

            _appDbContext.Supplier.Add(supplier);
            await _appDbContext.SaveChangesAsync();

            return CreatedAtAction("AddSupplier", new { id = supplier.SupplierId }, supplier);
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetAll()
        {
            var supplierList = _appDbContext.Supplier.ToList();

            return Ok(supplierList);
        }



        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {

            var isExit = _appDbContext.Supplier.FirstOrDefault(c => c.SupplierId == id);
            if (isExit == null) { return NotFound("Supplier not found"); }
            return Ok(isExit);

        }

        [HttpDelete("DeleteSupplier")]
        public IActionResult DeleteById(string id)
        {
            var isExit = _appDbContext.Supplier.FirstOrDefault(x => x.SupplierId == id);
            if (isExit == null) { return NotFound("Supplier not found"); }
            _appDbContext.Supplier.RemoveRange(isExit);
            _appDbContext.SaveChanges();
            return Ok("deleted");
        }

        [HttpPut("Update")]
        public IActionResult Update(Supplier supplier)
        {
            var isExit = _appDbContext.Supplier.FirstOrDefault(c => c.SupplierId == supplier.SupplierId);
            if (isExit == null) { return NotFound("Supplier not found"); }
            isExit.SupplierName = supplier.SupplierName;
            _appDbContext.SaveChanges();

            return Ok(supplier);
        }




    }
}
