using App.Business.Abstract;
using App.Entities.Models;
using ECommerce.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ECommerce.WebUI.Controllers
{

    public class AdminController : Controller
    {
        private IProductService _productService;
        private ICategoryService _categoryService;

        public AdminController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }
        public static bool FilterState { get; set; } = false;
        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Index(int page = 1, int category = 0, bool filterAZ = false)
        {
            int pageSize = 10;
            var products = _productService.GetAllByCategory(category);
            products = _productService.GetAllByFilterAZ(products, filterAZ);
            FilterState = !FilterState;
            var model = new ProductListViewModel
            {
                UpdatedProduct = new Product(),
                CurrentFilterState = FilterState,
                Products = products.ToList(),
                CurrentCategory = category,
                PageCount = (int)Math.Ceiling(products.Count / (double)pageSize),
                PageSize = pageSize,
                CurrentPage = page
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var model = new ProductAddViewModel();
            model.Product = new Product();
            model.Categories = _categoryService.GetAll();
            return View(model);
        }



        [HttpPost]
        public IActionResult Add(ProductAddViewModel model)
        {
            _productService.Add(model.Product);
            return RedirectToAction("index");
        }

        public JsonResult Product(int id)
        {
            var item = _productService.GetById(id);
            return Json(item);
        }



        public IActionResult Update(string id)
        {
            var product = JsonConvert.DeserializeObject<Product>(id);
            if (product != null)
			{
                _productService.Update(product);
            }
            return RedirectToAction("index");
        }

        [HttpGet("ProductDelete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _productService.Delete(id);
                TempData.Add("message", String.Format("Your product, {0} was deleted successfully!", id));
            }
            catch (Exception ex)
            {
                TempData.Add("message", "Not Found");

            }
            return RedirectToAction("index");
        }
    }
}
