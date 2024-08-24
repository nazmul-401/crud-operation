using Microsoft.AspNetCore.Mvc;
using Store.Models;
using Store.Services;
using System;

namespace Store.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        public IActionResult Index()
        {
            var products = context.Products.ToList();
            return View(products);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ProductDT productDT)
        {
            if (productDT.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is required");
            }
            if (!ModelState.IsValid)
            {
                return View(productDT);
            }

            //Save the Image file.....
            // save the image file
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productDT.ImageFile!.FileName);

            string imageFullPath = environment.WebRootPath + "/Products/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDT.ImageFile.CopyTo(stream);
            }

            // save the new product in the database
            Product product = new Product()
            {
                Name = productDT.Name,
                Brand = productDT.Brand,
                Category = productDT.Category,
                Price = productDT.Price,
                Description = productDT.Description,
                ImageFileName = newFileName,
                CreatedAt = DateTime.Now,
            };

            // save this object in the database
            context.Products.Add(product);
            //save the modification
            context.SaveChanges();
            return RedirectToAction("Index", "Products");
        }

        //edit action
        public IActionResult Edit(int id)
        {
            var product = context.Products.Find(id);

            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            // create productDto from product
            var ProductDT = new ProductDT()
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description,
            };
            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreateAt"] = product.CreatedAt;

            return View(ProductDT);
        }
        //update database

        [HttpPost]
        public IActionResult Edit(int Id, ProductDT productDT)

		{
			var product = context.Products.Find(Id);

            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");

                return View(productDT);
            }
			string newFileName = product.ImageFileName;
			if (productDT.ImageFile != null)
			{
				newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
				newFileName += Path.GetExtension(productDT.ImageFile.FileName);

				string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
				using (var stream = System.IO.File.Create(imageFullPath))
				{
					productDT.ImageFile.CopyTo(stream);
				}

				// delete the old image
				string oldImageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName;
				System.IO.File.Delete(oldImageFullPath);

				// Update the product's image file name to the new one
				product.ImageFileName = newFileName;


			}
			
            //update the product in the database
            product.Name = productDT.Name;
            product.Brand = productDT.Brand;
            product.Category = productDT.Category;
            product.Price = productDT.Price;
            product.Description = productDT.Description;
            product.ImageFileName = newFileName;

			context.SaveChanges();

			return RedirectToAction("Index", "Products");


		}
        //delete
		public IActionResult Delete(int id)
		{
			var product = context.Products.Find(id);
			if (product == null)
			{
				return RedirectToAction("Index", "Products");
			}

			string imageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName;
			System.IO.File.Delete(imageFullPath);

			context.Products.Remove(product);
			context.SaveChanges(true);

			return RedirectToAction("Index", "Products");
		}

	}
}

