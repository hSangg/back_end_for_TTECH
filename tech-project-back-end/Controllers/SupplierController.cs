using Microsoft.AspNetCore.Mvc;
using tech_project_back_end.Data;
using tech_project_back_end.Models;

namespace tech_project_back_end.Controllers
{
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
        public IActionResult AddSupplier(Supplier supplier)
        {

            supplier.supplier_id = Guid.NewGuid().ToString()[..36];

            _appDbContext.Supplier.Add(supplier);
            _appDbContext.SaveChanges();

            return Ok(supplier);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var supplierList = _appDbContext.Supplier.ToList();

            return Ok(supplierList);
        }



        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {

            var isExit = _appDbContext.Supplier.FirstOrDefault(c => c.supplier_id == id);
            if (isExit == null) { return NotFound("Supplier not found"); }
            return Ok(isExit);

        }

        [HttpDelete("DeleteSupplier")]
        public IActionResult DeleteById(string id)
        {
            var isExit = _appDbContext.Supplier.FirstOrDefault(x => x.supplier_id == id);
            if (isExit == null) { return NotFound("Supplier not found"); }
            _appDbContext.Supplier.RemoveRange(isExit);
            _appDbContext.SaveChanges();
            return Ok("deleted");
        }

        [HttpPut("Update")]
        public IActionResult Update(Supplier supplier)
        {
            var isExit = _appDbContext.Supplier.FirstOrDefault(c => c.supplier_id == supplier.supplier_id);
            if (isExit == null) { return NotFound("Supplier not found"); }
            isExit.supplier_name = supplier.supplier_name;
            _appDbContext.SaveChanges();

            return Ok(supplier);
        }




    }
}
