using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;

using Stripe.Checkout;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace BulkyBook.Controllers
{
    public class HomeController : Controller
    {
        private readonly BulkyBookDbContext _context;

        public HomeController(BulkyBookDbContext context)
        {
                _context = context;
        }


        public IActionResult Index()
        {
            List<Product> productList = _context.Product.ToList();
            return View(productList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Register()
        {
            ViewBag.CompanyName = new SelectList(_context.CompanyModels.ToList(),"Id","Name");
            return View();
        }
        [HttpPost]
        public IActionResult Register(RegisterModel regDetail)
        {
            var companyList = _context.CompanyModels.Find(regDetail.CompanyModelId);
            if (ModelState.IsValid && companyList != null && companyList.Id != 2  )
            {
                regDetail.CompanyModelId= companyList.Id;
                _context.RegisterModels.Add(regDetail);
                _context.SaveChanges();
                ViewBag.success = "Message Submited Sucessfully...";
            }
            else if(ModelState.IsValid)
            {
                regDetail.CompanyModelId = 2;
                _context.RegisterModels.Add(regDetail);
                _context.SaveChanges();
                ViewBag.success = "Message Submited Sucessfully...";
            }
            else
            {
                ViewBag.error = "Some Think Went To Wrong...";
            }

            return View();

        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Login logData) {
            RegisterModel regModel = _context.RegisterModels.FirstOrDefault(x => x.Name == logData.UserName && x.Password == logData.Password || x.Email==logData.UserName && x.Password == logData.Password || x.PhoneNumber == logData.UserName && x.Password == logData.Password);

            try
            {
                if ((regModel != null) && (regModel.Name == logData.UserName && regModel.Password == logData.Password) || (regModel.Email == logData.UserName && regModel.Password == logData.Password) || (regModel.PhoneNumber == logData.UserName && regModel.Password == logData.Password))
                {
                    ViewBag.LogInMsg = " ";
                    HttpContext.Session.SetString("LoginType", regModel.Role);
                    HttpContext.Session.SetInt32("LoginId", regModel.Id);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.LogInMsg = "Invalid User Name or Password";
                    ViewBag.RoleData = "";                   
                }
            }
            catch (Exception Ex)
            {

                ViewBag.LogInMsg = Ex.Message;
                ViewBag.RoleData = "";
            }
            return View();

        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("LoginType");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ViewBulkBookProduct_MoreDetails(int id)
        {
            int userId = GetLoginId();
            if (userId != 0)
            {
                Product currentProductList=_context.Product.Find(id);
                currentProductList.Category=_context.Categories.Find(currentProductList.CategoryId);
                currentProductList.CoverType=_context.CoverTypes.Find(currentProductList.CoverTypeId);
                return View(currentProductList);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public IActionResult Add_To_Cart(int count,int Id)
        {
            Cart cartitem = new Cart();
            int userId = GetLoginId();
            if (userId != 0)
            {            
                cartitem.UsersId = userId;
                cartitem.Count = count;
                cartitem.ProductId= Id;
                _context.Carts.Add(cartitem);
                _context.SaveChanges();
                return RedirectToAction("Cart","Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public IActionResult Cart()
        {
            int userId = GetLoginId();
            if (userId != 0)
            {
                List<Cart> cartList = _context.Carts.Include(x => x.ProductItem).ThenInclude(x=>x.Category).Include(x=>x.RegisteredUserslist).ThenInclude(x => x.CompanyModel).Where(x=>x.UsersId == userId).ToList();
                double countItem = 0,cartprice, totalPrice=0;
                foreach(Cart cartitem in cartList) {
                    countItem = (double)Convert.ToDecimal(cartitem.Count);
                    if (cartitem.Count <= 49)
                    {                        
                        cartprice = (double)Convert.ToDecimal(cartitem.ProductItem.Price);
                        cartitem.TotalPrice = countItem * cartprice;
                    }
                    else if (cartitem.Count <= 99)
                    {
                        cartprice = (double)Convert.ToDecimal(cartitem.ProductItem.Price50);
                        cartitem.TotalPrice = countItem * cartprice;
                    }
                    else if (cartitem.Count > 99)
                    {
                        cartprice = (double)Convert.ToDecimal(cartitem.ProductItem.Price100);
                        cartitem.TotalPrice = countItem * cartprice;
                    }

                }
                return View(cartList);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public IActionResult Cart_Count_Plus(int id)
        {
            Cart cartlist = _context.Carts.Find(id);
            int countplus = Convert.ToInt32(cartlist.Count) + 1;
            int userId = GetLoginId();
            if (userId == cartlist.UsersId)
            {
                cartlist.Count = countplus;
                _context.SaveChanges();
                return RedirectToAction("Cart", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }            
        }
        public IActionResult Cart_Count_Minus(int id)
        {
            Cart cartlist = _context.Carts.Find(id);
            int countminus = Convert.ToInt32(cartlist.Count) - 1;
            int userId = GetLoginId();
            if (userId == cartlist.UsersId)
            {
                cartlist.Count = countminus;
                _context.SaveChanges();
                return RedirectToAction("Cart", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }           
        }

        public IActionResult Cart_Count_Delete(int id)
        {
            Cart cartlist=_context.Carts.Find(id);
            if(cartlist != null)
            {
                _context.Carts.Remove(cartlist);
                _context.SaveChanges();
            }
            return RedirectToAction("Cart", "Home");
        }

        public IActionResult Cart_Summary()
        {
            int userId = GetLoginId();
            if (userId != 0)
            {
                List<Cart> cartList = _context.Carts.Include(x => x.ProductItem).ThenInclude(x => x.Category).Include(x => x.RegisteredUserslist).Where(x => x.UsersId == userId).ToList();
                double countItem = 0, cartprice;
                foreach (Cart cartitem in cartList)
                {
                    countItem = (double)Convert.ToDecimal(cartitem.Count);
                    if (cartitem.Count <= 49)
                    {
                        cartprice = (double)Convert.ToDecimal(cartitem.ProductItem.Price);
                        cartitem.TotalPrice = countItem * cartprice;
                    }
                    else if (cartitem.Count <= 99)
                    {
                        cartprice = (double)Convert.ToDecimal(cartitem.ProductItem.Price50);
                        cartitem.TotalPrice = countItem * cartprice;
                    }
                    else if (cartitem.Count > 99)
                    {
                        cartprice = (double)Convert.ToDecimal(cartitem.ProductItem.Price100);
                        cartitem.TotalPrice = countItem * cartprice;
                    }

                }
                return View(cartList);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }

        public IActionResult Place_Order()
        {
            int userId = GetLoginId();
            OrderHeader orderHeader_Item = new OrderHeader();
            int orderQuantity = 0;
            if (userId != 0)
            {
                List<Cart> cat_list = _context.Carts.Include(x => x.ProductItem).Where(x => x.UsersId == userId).ToList();
                double countItem = 0, cartprice, totalAmount = 0;
                foreach (var cartitem in cat_list)
                {
                    countItem = (double)Convert.ToDecimal(cartitem.Count);
                    if (cartitem.Count <= 49)
                    {
                        cartprice = (double)Convert.ToDecimal(cartitem.ProductItem.Price);
                        cartitem.TotalPrice = countItem * cartprice;
                    }
                    else if (cartitem.Count <= 99)
                    {
                        cartprice = (double)Convert.ToDecimal(cartitem.ProductItem.Price50);
                        cartitem.TotalPrice = countItem * cartprice;
                    }
                    else if (cartitem.Count > 99)
                    {
                        cartprice = (double)Convert.ToDecimal(cartitem.ProductItem.Price100);
                        cartitem.TotalPrice = countItem * cartprice;
                    }

                    var totalCartPrice = cartitem.TotalPrice;
                    totalAmount = totalAmount + totalCartPrice;
                }

                orderHeader_Item.Name = Request.Form["Name"];
                orderHeader_Item.Phone = Request.Form["Phone"];
                orderHeader_Item.Address = Request.Form["Address"];
                orderHeader_Item.City = Request.Form["City"];
                orderHeader_Item.State = Request.Form["State"];
                orderHeader_Item.PostalCode = Request.Form["PostalCode"];
                orderHeader_Item.Email = Request.Form["Email"];
                orderHeader_Item.OrderDate = DateTime.Now;
                orderHeader_Item.Carrier = String.Empty;
                orderHeader_Item.ShippingDate = DateTime.Now.AddDays(3);
                orderHeader_Item.TransationId = String.Empty;
                orderHeader_Item.PaymentDueDate = DateTime.Now.AddDays(2);
                orderHeader_Item.PaymentStatus = "Pending";
                orderHeader_Item.Amount = Convert.ToDecimal(totalAmount);
                orderHeader_Item.UserId = userId;
                double totalOrderAmount =Convert.ToDouble(orderHeader_Item.Amount);
                _context.OrderHeaders.Add(orderHeader_Item);
                _context.SaveChanges();
                
                foreach (var cartitem in cat_list)
                {
                    OrderDetail orderDetail = new OrderDetail();
                    orderDetail.OrderId = orderHeader_Item.Id;
                    orderDetail.ProductId = cartitem.ProductId;
                    orderDetail.Quantity = cartitem.Count;
                    _context.OrderDetails.Add(orderDetail);
                    orderQuantity = Convert.ToInt32(cartitem.Count);
                }
                _context.Carts.RemoveRange(cat_list);                
                _context.SaveChanges();
                PayNow(totalOrderAmount, orderQuantity, orderHeader_Item.Id);
                return new StatusCodeResult(303);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }            

        }

        public IActionResult PayNow(double amount,long orderQuantity ,int orderHeader_ItemId)
        {
            var domain = "https://" + HttpContext.Request.Host.Value;
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData=new SessionLineItemPriceDataOptions
                        {
                            UnitAmount= Convert.ToInt32(amount),
                            Currency="usd",
                            ProductData=new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Book",
                            },
                        },
                        Quantity = orderQuantity,
                    },
                },
                Mode = "payment",
                SuccessUrl = domain + "/Home/Success/",
                CancelUrl = domain + "/Home/Cancel/"
            };
            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Create(options);
            TempData["Session"] = session.Id;
            TempData["OrderId"] = orderHeader_ItemId;
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public IActionResult Success()
        {
            var service=new SessionService();
            Session session = service.Get(TempData["Session"].ToString());
            if (session.PaymentStatus == "paid")
            {
                var transactionId=session.PaymentIntentId.ToString();
                OrderHeader orderitem=_context.OrderHeaders.Find(TempData["OrderId"]);
                if (orderitem != null)
                {
                    orderitem.TransationId = transactionId;
                    orderitem.PaymentStatus = "InProcess";
                    _context.SaveChanges();
                }
            }
            return View();
        }

        public IActionResult Cancel()
        {                       
            return View();
        }

        public int GetLoginId()
        {            
            int loginId = Convert.ToInt32(HttpContext.Session.GetInt32("LoginId"));
            if (loginId == null || loginId == 0)
            {
               return 0;
            }
            else
            {
                return loginId;
            }
        }

        //public IActionResult OrderDetails(string status, int pg = 1, int pageSize = 4)
        //{
        //    int userId = GetLoginId();
        //    List<OrderHeader> orderheaderList = new List<OrderHeader>();
        //    if (HttpContext.Session.GetString("LoginType") == "Admin" && userId != 0)
        //    {
        //        if (status == "InProcess")
        //        {
        //            orderheaderList = _context.OrderHeaders.Where(x => x.PaymentStatus == status).ToList();
        //        }
        //        else if (status == "Pending")
        //        {
        //            orderheaderList = _context.OrderHeaders.Where(x => x.PaymentStatus == status).ToList();
        //        }
        //        else if (status == "Completed")
        //        {
        //            orderheaderList = _context.OrderHeaders.Where(x => x.PaymentStatus == status).ToList();
        //        }
        //        else if (status == "Cancel")
        //        {
        //            orderheaderList = _context.OrderHeaders.Where(x => x.PaymentStatus == status).ToList();
        //        }
        //        else
        //        {
        //            orderheaderList = _context.OrderHeaders.ToList();
        //        }
        //    }
        //    else if (userId != 0)
        //    {
        //        if (status == "InProcess")
        //        {
        //            orderheaderList = _context.OrderHeaders.Where(x => x.PaymentStatus == status && x.UserId == userId).ToList();
        //        }
        //        else if (status == "Pending")
        //        {
        //            orderheaderList = _context.OrderHeaders.Where(x => x.PaymentStatus == status && x.UserId == userId).ToList();
        //        }
        //        else if (status == "Completed")
        //        {
        //            orderheaderList = _context.OrderHeaders.Where(x => x.PaymentStatus == status && x.UserId == userId).ToList();
        //        }
        //        else if (status == "Cancel")
        //        {
        //            orderheaderList = _context.OrderHeaders.Where(x => x.PaymentStatus == status && x.UserId == userId).ToList();
        //        }
        //        else
        //        {
        //            orderheaderList = _context.OrderHeaders.Where(x => x.UserId == userId).ToList();
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "Home");
        //    }
        //    var dropdownValues = new List<int> { 5, 10, 15, 20 };
        //    ViewBag.DropDownValues = dropdownValues;
        //    if (pageSize < 4)
        //    {
        //        pageSize = 4;
        //    }
        //    if (pg < 1)
        //    {
        //        pg = 1;
        //    }
        //    int recsCount = orderheaderList.Count();
        //    var pager = new PaginatedList(recsCount, pg, pageSize);
        //    int recSkip = (pg - 1) * pageSize;
        //    var data = orderheaderList.Skip(recSkip).Take(pager.PageSize).ToList();
        //    this.ViewBag.Pager = pager;
        //    return View(data);
        //}
        public IActionResult OrderDetails(string status)
        {
            int userId = GetLoginId();
            if (userId != 0)
            {
                List<OrderHeader> orderheaderList = new List<OrderHeader>();
                return View(orderheaderList);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }

        public IActionResult GetAllOrder(string status)
        {
            int userId = GetLoginId();
            List<OrderHeader> orderheaderList = new List<OrderHeader>();
            if (HttpContext.Session.GetString("LoginType") == "Admin" && userId != 0)
            {
                if (status == "InProcess")
                {
                    orderheaderList = _context.OrderHeaders.Where(x => x.PaymentStatus == status).ToList();
                }
                else if (status == "Pending")
                {
                    orderheaderList = _context.OrderHeaders.Where(x => x.PaymentStatus == status).ToList();
                }
                else if (status == "Completed")
                {
                    orderheaderList = _context.OrderHeaders.Where(x => x.PaymentStatus == status).ToList();
                }
                else if (status == "Cancel")
                {
                    orderheaderList = _context.OrderHeaders.Where(x => x.PaymentStatus == status).ToList();
                }
                else
                {
                    orderheaderList = _context.OrderHeaders.ToList();
                }
            }
            else if (userId != 0)
            {
                if (status == "InProcess")
                {
                    orderheaderList = _context.OrderHeaders.Where(x => x.PaymentStatus == status && x.UserId == userId).ToList();
                }
                else if (status == "Pending")
                {
                    orderheaderList = _context.OrderHeaders.Where(x => x.PaymentStatus == status && x.UserId == userId).ToList();
                }
                else if (status == "Completed")
                {
                    orderheaderList = _context.OrderHeaders.Where(x => x.PaymentStatus == status && x.UserId == userId).ToList();
                }
                else if (status == "Cancel")
                {
                    orderheaderList = _context.OrderHeaders.Where(x => x.PaymentStatus == status && x.UserId == userId).ToList();
                }
                else
                {
                    orderheaderList = _context.OrderHeaders.Where(x => x.UserId == userId).ToList();
                }
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            return Json(orderheaderList);
        }

        public IActionResult OrderDetailsMore(int id)
        {
            int userId = GetLoginId();
            if (userId != 0)
            {
                List<OrderDetail> orderItems = _context.OrderDetails.Include(x => x.Orderlist).Include(x => x.Productlist).Where(x => x.OrderId == id).ToList();
                return View(orderItems);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }

        [HttpPost]
        public IActionResult Order_Update(OrderDetail orderDetail)
        {
            OrderDetail existingOrder = _context.OrderDetails.Include(x => x.Orderlist).First(x => x.Orderlist.Id == orderDetail.Orderlist.Id);
            if (existingOrder.Orderlist.PaymentStatus == "InProcess")
            {
                existingOrder.Orderlist.PaymentStatus = "Completed";
                existingOrder.Orderlist.Carrier = Request.Form["Carrier"];
                existingOrder.Orderlist.Tracking = Convert.ToInt32(Request.Form["Tracking"]);
                _context.SaveChanges();
            }
            
            return RedirectToAction("OrderDetails", "Home");
        }

        public IActionResult Order_Update(string status,int orderid)
        {

            OrderDetail existingOrder = _context.OrderDetails.Include(x => x.Orderlist).First(x => x.Orderlist.Id == orderid);
            if (status == "Cancel")
            {
                existingOrder.Orderlist.PaymentStatus = "Cancel";
                _context.SaveChanges();
            }

            return RedirectToAction("OrderDetails", "Home");
        }
    }
}
