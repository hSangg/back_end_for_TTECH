using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Runtime.CompilerServices;
using tech_project_back_end.Data;
using tech_project_back_end.Models;

namespace tech_project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment eviroment;
        public ProductController(AppDbContext appDbContext, IWebHostEnvironment eviroment)
        {
            this._appDbContext = appDbContext;
            this.eviroment = eviroment;
        }

        [HttpPut("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile formFile, string product_id)
        {
            try
            {
                string FilePath = GetFilePath(product_id);

                if (!System.IO.Directory.Exists(FilePath))
                {
                    System.IO.Directory.CreateDirectory(FilePath);
                }

                string fileName = $"{product_id}_{DateTime.Now.Ticks}.png";
                string ImagePath = Path.Combine(FilePath, fileName);

                using (FileStream stream = System.IO.File.Create(ImagePath))
                {
                    await formFile.CopyToAsync(stream);
                }

                // Generate the URL using Url.Action method
                string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";
                string relativePath = $"/Upload/product/{product_id}/{fileName}";
                string imageUrl = baseUrl + relativePath;

                _appDbContext.Image.Add(new Image
                {
                    image_id = Guid.NewGuid().ToString()[..36],
                    product_id = product_id,
                    image_href = imageUrl
                }) ;

                await _appDbContext.SaveChangesAsync();

                return Ok(imageUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Failed to upload image.");
            }
        }

        [HttpGet("GetMultiImage")]
        public async Task<IActionResult> GetMultiImage(string product_id)
        {
            List<string> Imageurl = new List<string>();
            string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            try
            {
                string Filepath = GetFilePath(product_id);

                if (System.IO.Directory.Exists(Filepath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(Filepath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles();
                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        string filename = fileInfo.Name;
                        string imagepath = Filepath + "\\" + filename;
                        if (System.IO.File.Exists(imagepath))
                        {
                            string _Imageurl = hosturl + "/Upload/product/" + product_id + "/" + filename;
                            Imageurl.Add(_Imageurl);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
            }
            return Ok(Imageurl);

        }



       


        [NonAction]
        private string GetFilePath(string product_id)
        {
            return this.eviroment.WebRootPath + "\\Upload\\product\\" + product_id;
        }

    }
}
