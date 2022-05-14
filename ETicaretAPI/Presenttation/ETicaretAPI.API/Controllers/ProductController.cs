﻿using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.RequestParameters;
using ETicaretAPI.Application.ViewModels.Products;
using ETicaretAPI.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ETicaretAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        readonly private IProductWriteRepository _productWriteRepository;
        readonly private IProducReadRepository   _producReadRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IProductWriteRepository productWriteRepository, 
            IProducReadRepository producReadRepository,
            IWebHostEnvironment webHostEnvironment
            )
        {
            _productWriteRepository = productWriteRepository;
            _producReadRepository = producReadRepository;
            _webHostEnvironment = webHostEnvironment;
            
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]Pagination pagination)
        {
            await Task.Delay(5000);
            var totalCount = _producReadRepository.GetAll(false).Count();
           var products = _producReadRepository.GetAll(false).Select(p => new
            {
                p.Id,
                p.Name,
                p.Price,
                p.Stock,
                p.CreatedDate,
                p.UpdatedDate,
            }).Skip(pagination.Page * pagination.Size).Take(pagination.Size);

            return Ok(new
            {
                totalCount,
                products
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(await _producReadRepository.GetByIdAsync(id,false));
        }

        [HttpPost]
        public async Task<IActionResult> Post(VM_Create_Product model)
        {
            await _productWriteRepository.AddAsync(new() { 
                Name = model.Name,
                Price = model.Price,
                Stock = model.Stock,
            });
            await _productWriteRepository.SaveAsync();
            return StatusCode((int)HttpStatusCode.Created);
        }

        [HttpPut]
        public async Task<IActionResult> Put(VM_Update_Product model)
        {
            Product product = await _producReadRepository.GetByIdAsync(model.Id);
            product.Stock = model.Stock;
            product.Name = model.Name;
            product.Price = model.Price;
            await _productWriteRepository.SaveAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _productWriteRepository.RemoveAsync(id);
            await _productWriteRepository.SaveAsync();
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Upload()
        {
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath,"resource/product-images");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            Random r = new Random();
            foreach (IFormFile file in Request.Form.Files)
            {
                string fullPath = Path.Combine(uploadPath, $"{r.Next()}{Path.GetExtension(file.Name)}");

                using FileStream fileStream = new (fullPath,FileMode.Create,FileAccess.Write,
                    FileShare.None,1024 * 1024,useAsync: false);
                await file.CopyToAsync(fileStream);
                fileStream.FlushAsync();
            }

            return Ok();
        }

    }
}
