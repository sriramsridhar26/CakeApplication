using CakeApplication.Data;
using CakeApplication.DTO;
using CakeApplication.Model;
using Microsoft.AspNetCore.Mvc;

namespace CakeApplication.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost("/Login")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody] LoginDTO userd)
        {
            if (userd.emailId != null)
            {
                LoginResDTO userc = new LoginResDTO(_context.users
                                                  .Where(x => x.emailId == userd.emailId && x.password == userd.password).SingleOrDefault());
                return Ok(userc);
            }
            else
            {
                return BadRequest();
            }

        }
        [HttpGet("/CheckUser")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkuser([FromQuery] string emailId)
        {
            if (emailId != null)
            {
                LoginResDTO userc = new LoginResDTO(_context.users
                                                  .Where(x => x.emailId == emailId).SingleOrDefault());
                if(userc != null)
                {
                    return Ok(userc.customerName);
                }
                else
                {
                    return BadRequest();
                }
                
            }
            else
            {
                return BadRequest();
            }

        }

        [HttpPost("/Register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (user != null)
            {
                _context.users.Add(user);
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }
        [HttpGet("/Getcategory")]
        public async Task<IActionResult> Getcategory()
        {
            return Ok(_context.items.Select(x => x.itemType).Distinct());
        }
        [HttpGet("/GetItems")]
        public async Task<IActionResult> GetItems()
        {
            List<Item> itemd = _context.items.ToList();
            foreach (Item item in itemd)
            {
                byte[] bytes = System.IO.File.ReadAllBytes(item.imglink);
                item.imglink = Convert.ToBase64String(bytes);

            }
            if (itemd != null)
            {
                return Ok(itemd);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("/GetOrders")]
        public async Task<IActionResult> GetOrder([FromQuery] string emailId)
        {
            if (emailId != null)
            {
                // IEnumerable<Order> orders = _context.orders.Where(x => x.emailId == emailId);
                return Ok(_context.orders.Where(x => x.emailId == emailId));

            }
            else
            {
                return BadRequest("Please provide emailId! or Cart Empty");
            }

        }

        [HttpPost("/AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] AddtocartDTO addtocart)
        {
            //var temp1 = _context.orders.Where(x => x.itemId == addtocart.itemId
            //                               && x.purchased == false
            //                               && x.Lspec == addtocart.Lspec
            //                               && x.Rspec == addtocart.Rspec)
            //                           .FirstOrDefault();
            if (addtocart != null)
            {
                Item item = _context.items
                                          .Where(x => x.Id == addtocart.itemId)
                                          .SingleOrDefault();
                if (item != null)
                {
                    if(addtocart.size=="1 Kg")
                    {
                        int tempcost = item.cost * 2;
                        var t = new Order
                        {
                            emailId = addtocart.emailId,
                            itemId = addtocart.itemId,
                            itemName = item.itemName,
                            cost = tempcost,
                            purchased = false,
                            size = addtocart.size,
                            quantity = 1

                        };
                        _context.orders.Add(t);
                        _context.SaveChanges();
                    }
                    if (addtocart.size == "2 Kg")
                    {
                        int tempcost = item.cost * 4;
                        var t = new Order
                        {
                            emailId = addtocart.emailId,
                            itemId = addtocart.itemId,
                            itemName = item.itemName,
                            cost = tempcost,
                            purchased = false,
                            size = addtocart.size,
                            quantity = 1

                        };
                        _context.orders.Add(t);
                        _context.SaveChanges();
                    }
                    else
                    {
                        var t = new Order
                        {
                            emailId = addtocart.emailId,
                            itemId = addtocart.itemId,
                            itemName = item.itemName,
                            cost = item.cost,
                            purchased = false,
                            size = addtocart.size,
                            quantity = 1

                        };
                        _context.orders.Add(t);
                        _context.SaveChanges();
                    }
                    
                    
                    return Ok();
                }
            }

            return BadRequest("item does not exist");
        }

        [HttpDelete("/Deletecartitem")]
        public async Task<IActionResult> Deletecartitem([FromQuery] int orderid)
        {
            if (orderid != null)
            {
                var items = _context.orders.Where(x => x.orderId == orderid).FirstOrDefault();
                if (items != null)
                {

                    var t = _context.orders.Remove(items);
                    _context.SaveChangesAsync();
                    return Ok(Json("success"));
                }
                else
                {
                    return BadRequest("Item not present in cart");
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("/PlaceOrder")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDTO placeorder)
        {

            if (placeorder.emailId != null)
            {
                using (_context)
                {
                    var temp = _context.orders.Where(x => x.emailId == placeorder.emailId).ToList();
                    temp.ForEach(a =>
                    {
                        a.purchased = true;
                        a.purchaseDT = DateTime.Now;
                        a.paymentMode = placeorder.paymentMode;
                    });
                    _context.SaveChanges();
                }

                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete("/Reduce")]
        public async Task<IActionResult> Reduce([FromQuery] int orderid)
        {
            if (orderid != null)
            {
                var items = _context.orders.Where(x => x.orderId == orderid).FirstOrDefault();
                if (items != null)
                {
                    using (_context)
                    {
                        items.quantity -= 1;
                        _context.SaveChanges();
                    }

                    return Ok(Json("success"));
                }
                else
                {
                    return BadRequest("Item not present in cart");
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("/Addanother")]
        public async Task<IActionResult> Addanother([FromBody] int orderid)
        {
            var temp1 = _context.orders.Where(x => x.orderId == orderid
                                           && x.purchased == false)
                                       .FirstOrDefault();
            if (temp1 != null)
            {
                using (_context)
                {
                    temp1.quantity += 1;
                    _context.SaveChanges();
                }
                return Ok();
            }
            else
            {

                return BadRequest("item does not exist");

            }

        }

        [HttpGet("/Item")]
        public async Task<IActionResult> Item([FromQuery] int id)
        {
            if (id != null)
            {
                Item item = _context.items.Where(p => p.Id == id).SingleOrDefault();

                byte[] bytes = System.IO.File.ReadAllBytes(item.imglink);
                item.imglink = Convert.ToBase64String(bytes);
                return Ok(item);
            }
            else
            {
                return BadRequest();
            }


        }

    }
}
