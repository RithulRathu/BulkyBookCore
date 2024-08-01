using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;


namespace BulkyBook.Controllers
{
    public class AdminController : Controller
    {
        private readonly BulkyBookDbContext _context;
        private readonly IHostingEnvironment environment;
        
        public AdminController(BulkyBookDbContext context, IHostingEnvironment _environment)
        {
                _context = context;
               environment = _environment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CategoryList()
        {
            List<Category> categoryList = _context.Categories.ToList(); 
            return View(categoryList);
        }
        public IActionResult CreateCategory() 
        {
            return View(); 
        }

        [HttpPost]
        public IActionResult CreateCategory(Category cateItem)
        {
            if(ModelState.IsValid) {
                _context.Categories.Add(cateItem);
                _context.SaveChanges();
            }
            return RedirectToAction("CategoryList");
            
        }

        public IActionResult EditCategory(int id)
        {
            Category editCategoryList = _context.Categories.Find(id);
            return View(editCategoryList);
        }

        [HttpPost]
        public IActionResult EditCategory(Category cateItem) {
            Category editCategoryList = _context.Categories.Find(cateItem.Id);
            if(editCategoryList != null)
            {
                editCategoryList.Name= cateItem.Name;
                _context.SaveChanges();
                ViewBag.editedMsg = "Update Category Succesfully Completed....";
            }
            else
            {
                ViewBag.editedMsg = "Somethink Went Wrong....";
            }
            return RedirectToAction("CategoryList");
        }

        public IActionResult DeleteCategory(int id)
        {
            Category deletCategoryList = _context.Categories.Find(id);
            if(deletCategoryList != null)
            {
                _context.Categories.Remove(deletCategoryList);
                _context.SaveChanges();
            }
            return RedirectToAction("CategoryList");
        }

        public IActionResult CoverTypeList()
        {
            List<CoverType> coverTypelist=_context.CoverTypes.ToList();
            return View(coverTypelist);
        }

        public IActionResult CreateCovertype()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCovertype(CoverType covertypeItem)
        {
            if(ModelState.IsValid)
            {
                _context.CoverTypes.Add(covertypeItem);
                _context.SaveChanges();
                ViewBag.CoverTypeAddedMsg = "Cover Type Added";                
            }
            else
            {
                ViewBag.CoverTypeAddedMsg = "Somethink went to wrong";                
            }
            return RedirectToAction("CoverTypeList");
        }

        public IActionResult EditCovertype(int id)
        {
            CoverType editCoverTypelist = _context.CoverTypes.Find(id);
            return View(editCoverTypelist);
        }

        [HttpPost]
        public IActionResult EditCovertype(CoverType editCoverTypeItem)
        {
            CoverType editCoverTypeList = _context.CoverTypes.Find(editCoverTypeItem.Id);
            if(editCoverTypeList != null)
            {
                editCoverTypeList.Name = editCoverTypeItem.Name;
                _context.SaveChanges();
                ViewBag.EditedScucessMsg = "Cover Type Updated....";                
            }
            else
            {
                ViewBag.EditedScucessMsg = "Cover Type Updated....";

            }
            return RedirectToAction("CoverTypeList");
        }

        public IActionResult DeleteCovertype(int id)
        {
            CoverType deleteCOvertype = _context.CoverTypes.Find(id);
            if(deleteCOvertype != null)
            {
                _context.CoverTypes.Remove(deleteCOvertype);
                _context.SaveChanges();
                ViewBag.DeleteCovertypeMsg = "Cover Type Removed....";
            }
            else
            {
                ViewBag.DeleteCovertypeMsg = "Cover Type Removed....";
            }
            return RedirectToAction("CoverTypeList");
        }

        public IActionResult CreateProductList()
        {
            ProductVM productVM=new ProductVM();
            productVM.CategoryList = new SelectList(_context.Categories.ToList(),"Id","Name");
            productVM.CoverTypeList = new SelectList(_context.CoverTypes.ToList(),"Id","Name");
            return View(productVM);
        }

        [HttpPost]
        public IActionResult AddProductList(IFormFile postedFile, ProductVM productVM)
        {
            try {
                productVM.Product.Images = "dummy";
                _context.Product.Add(productVM.Product);
                _context.SaveChanges();
                int imgId = productVM.Product.Id;
                string wwwPath = this.environment.WebRootPath;
                string contentPath = this.environment.ContentRootPath;
                string path = Path.Combine(wwwPath, "images");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileName = imgId + "_" + Path.GetFileName(postedFile.FileName);
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                    productVM.Product.Images = fileName;

                }
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return RedirectToAction("ShowProductList");
        }

        public IActionResult ShowProductList()
        {            
            List<Product> productList = _context.Product.Include(x=>x.Category).ToList();                           
            return View(productList);
        }

        public IActionResult EditProductList(int id)
        {
            ProductVM productVM = new ProductVM();            
            Product ProductList = _context.Product.Find(id);
            productVM.CategoryList = new SelectList(_context.Categories.ToList(), "Id", "Name");
            productVM.CoverTypeList = new SelectList(_context.CoverTypes.ToList(), "Id", "Name");
            productVM.Product = ProductList;
            return View(productVM);
        }

        public IActionResult UpdateProductList(IFormFile postedFile, ProductVM updatedProduct)
        {
            Product productList = _context.Product.Find(updatedProduct.Product.Id);
            if (productList != null)
            {
                if ((productList.Images != null && updatedProduct.Product.Images == productList.Images) || (productList.Images != null && updatedProduct.Product.Images == "dummy") || (productList.Images != null))
                {
                    string wwwPath = this.environment.WebRootPath;
                    string contentPath = this.environment.ContentRootPath;
                    string path = Path.Combine(wwwPath, "images\\" + productList.Images);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    int imgId = updatedProduct.Product.Id;
                    string fileName = imgId + "_" + Path.GetFileName(postedFile.FileName);
                    string updatedpath = Path.Combine(wwwPath, "images");
                    using (FileStream stream = new FileStream(Path.Combine(updatedpath, fileName), FileMode.Create))
                    {
                        postedFile.CopyTo(stream);
                        productList.Images = fileName;

                    }
                }
                productList.Title = updatedProduct.Product.Title;
                productList.Isbn = updatedProduct.Product.Isbn;
                productList.Author = updatedProduct.Product.Author;
                productList.Description = updatedProduct.Product.Description;
                productList.ListPrice = updatedProduct.Product.ListPrice;
                productList.Price = updatedProduct.Product.Price;
                productList.Price50 = updatedProduct.Product.Price50;
                productList.Price100 = updatedProduct.Product.Price100;
                productList.CategoryId = updatedProduct.Product.CategoryId;
                productList.CoverTypeId = updatedProduct.Product.CoverTypeId;
                _context.SaveChanges();
            }            
            return RedirectToAction("ShowProductList");
        }

        public IActionResult DeleteProductList(Product deleteproductList)
        {
            Product productList = _context.Product.Find(deleteproductList.Id);
            if (productList != null)
            {
                string wwwPath = this.environment.WebRootPath;
                string contentPath = this.environment.ContentRootPath;
                string path = Path.Combine(wwwPath, "images\\" + productList.Images);
                if (productList.Images != null)
                {
                    System.IO.File.Delete(path);
                }
                _context.Product.Remove(productList);
                _context.SaveChanges();
                ViewBag.DeleteProductMsg = "Product Removed....";
            }
            else
            {
                ViewBag.DeleteProductMsg = "Cover Type Removed....";
            }
            return RedirectToAction("ShowProductList");
        }

        public IActionResult CreateCompany()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCompany(CompanyModel companyItem)
        {
            if(ModelState.IsValid)
            {
                _context.CompanyModels.Add(companyItem);
                _context.SaveChanges();
                ViewBag.CreateCompanyMsg = "Company Details Added Sucessfully...";
            }
            else
            {
                ViewBag.CreateCompanyMsg = "Some think went to wrong...";
            }
            return View(nameof(CreateCompany));
        }

        public IActionResult ShowCompanyList()
        {
            List <CompanyModel> companyList = _context.CompanyModels.ToList();
            return View(companyList);
        }

        public IActionResult EditCompanyList(int id)
        {
            CompanyModel companyList=_context.CompanyModels.Find(id);
            return View(companyList);
        }

        public IActionResult UpdateCompanyDetails(CompanyModel companyDetails)
        {
            CompanyModel companyList=_context.CompanyModels.Find(companyDetails.Id);
            if(companyList != null)
            {
                companyList.Name= companyDetails.Name;
                companyList.StreetAddress= companyDetails.StreetAddress;
                companyList.City= companyDetails.City;
                companyList.PostalCode= companyDetails.PostalCode;
                companyList.State= companyDetails.State;
                companyList.Country= companyDetails.Country;
                companyList.Phone= companyDetails.Phone;
                companyList.IsAuthorizesd= companyDetails.IsAuthorizesd;
                _context.SaveChanges();
            }
            return RedirectToAction("ShowCompanyList");
        }

        public IActionResult DeleteCompany(int id)
        {
            CompanyModel companydetails = _context.CompanyModels.Find(id);
            if(companydetails != null)
            {
                _context.CompanyModels.Remove(companydetails);
                _context.SaveChanges();
            }
            return RedirectToAction("ShowCompanyList");
        }

        public IActionResult ShowUsersList()
        {
            string loginRole = HttpContext.Session.GetString("LoginType");
            List<RegisterModel> usersList = new List<RegisterModel>();
            if (loginRole == "Admin")
            {
                usersList = _context.RegisterModels.Include(x => x.CompanyModel).ToList();
            }else if(loginRole == "Employee")
            {
                usersList = _context.RegisterModels.Include(x => x.CompanyModel).Where(x=>x.Role == "Employee").ToList();
            }else if(loginRole == "CompanyCustomer")
            {
                usersList = _context.RegisterModels.Include(x => x.CompanyModel).Where(x => x.Role == "CompanyCustomer").ToList();
            }
                       
            return View(usersList);
        }

        public IActionResult LockAndUnlockUser(int id)
        {
            RegisterModel uesrlist=_context.RegisterModels.Find(id);
            if(uesrlist != null && uesrlist.AccountStatus=="Lock")
            {
                uesrlist.AccountStatus = "Unlock";
            }
            else if(uesrlist != null && uesrlist.AccountStatus == "Unlock"){
                uesrlist.AccountStatus = "Lock";
            }
            _context.SaveChanges();
            return RedirectToAction("ShowUsersList");
        }

    }
}
