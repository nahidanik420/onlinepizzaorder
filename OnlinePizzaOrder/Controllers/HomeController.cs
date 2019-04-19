using OnlinePizzaOrder.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlinePizzaOrder.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home

        PizzaOrderEntities db = new PizzaOrderEntities();
        public ActionResult Index()
        {
            
            if (TempData["cart"] != null)
            {
                float x = 0;
                int ct = 0;
                List<cart> li2 = TempData["cart"] as List<cart>;
                foreach (var item in li2)
                {
                    x += item.bill;
                    ct++;

                }

                TempData["total"] = x;
                TempData["cartto"] = ct;
            }
            TempData.Keep();
       
            return View(db.tbl_product.OrderByDescending(x => x.pro_id).ToList());
        }

        public ActionResult AdToCart(int? Id)
        {

            tbl_product p = db.tbl_product.Where(x => x.pro_id == Id).SingleOrDefault();
            return View(p);
        }

        List<cart> li = new List<cart>();

        [HttpPost]
        public ActionResult AdToCart(tbl_product pi, string qty, int Id)
        {
            tbl_product p = db.tbl_product.Where(x => x.pro_id == Id).SingleOrDefault();

            cart c = new cart();
            c.productid = p.pro_id;
            c.price = (float)p.pro_price;
            c.qty = Convert.ToInt32(qty);
            c.bill = c.price * c.qty;
            c.productname = p.pro_name;
            if (TempData["cart"] == null)
            {
                li.Add(c);
                TempData["cart"] = li;

            }
            else
            {
                List<cart> li2 = TempData["cart"] as List<cart>;
                int flag = 0;

                foreach (var item in li2)
                {
                    if (item.productid == c.productid)
                    {
                        item.qty += c.qty;
                        item.bill += c.bill;
                        flag = 1;
                    }
                }

                if (flag == 0)
                {
                    li2.Add(c);
                }

                TempData["cart"] = li2;
            }

            TempData.Keep();

            return RedirectToAction("Features");
        }


        public ActionResult remove(int? id)
        {
            List<cart> li2 = TempData["cart"] as List<cart>;
            cart c = li2.Where(x => x.productid == id).SingleOrDefault();
            li2.Remove(c);
            float h = 0;

            foreach (var item in li2)
            {
                h += item.bill;
            }

            TempData["total"] = h;

            return RedirectToAction("checkout");
        }



       

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(tbl_user avm)
        {
            tbl_user ad = db.tbl_user.Where(x => x.u_name == avm.u_name && x.u_password == avm.u_password).SingleOrDefault();
            if (ad != null)
            {
                if ("admin".Equals(avm.u_name)&& "admin".Equals(avm.u_password))
                {
                    TempData["admin"] = 1;
                    return RedirectToAction("Index");
                }
                else
                {
                    Session["u_id"] = ad.U_id.ToString();
                    return RedirectToAction("Index");
                }
                

            }
            else
            {
                ViewBag.error = "Invalid username or password";

            }

            return View();
        }

        public ActionResult Logout()
        {
            Session["u_id"] = null;
            TempData.Remove("total");
            TempData.Remove("cart");
            TempData.Remove("cartto");
            TempData.Remove("admin");
            return RedirectToAction("Index");

        }

        public ActionResult Features()
        {
            if (TempData["cart"] != null)
            {
                float x = 0;
                int ct = 0;
                List<cart> li2 = TempData["cart"] as List<cart>;
                foreach (var item in li2)
                {
                    x += item.bill;
                    ct++;

                }

                TempData["total"] = x;
                TempData["cartto"] = ct;
            }
            TempData.Keep();
            return View(db.tbl_product.OrderByDescending(x => x.pro_id).ToList());
        }

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(tbl_user reg)
        {
            try
            {
                tbl_user tu = new tbl_user();
                tu.u_name = reg.u_name;
                tu.u_email = reg.u_email;
                tu.u_password = reg.u_password;
                tu.u_contact = reg.u_contact;

                if ((reg.u_name != null) || (reg.u_email!= null) || (reg.u_password!=null) || (reg.u_contact!=null))
                {
                    db.tbl_user.Add(tu);
                    db.SaveChanges();
                    ViewBag.error = "You have been successfully registered";
                }
                else
                {
                    ViewBag.erro = "*Fill all field";
                    
                }
                
            }

            catch (Exception)
            {
                ViewBag.erro = "Fill all field";
                
            }
            return View(); 
        }

        public ActionResult checkout()
        {
            TempData.Keep();
            return View();
        }


        [HttpPost]
        public ActionResult checkout(tbl_order O)
        {
            List<cart> li = TempData["cart"] as List<cart>;
            tbl_invoice iv = new tbl_invoice();
            iv.in_fk_user = Convert.ToInt32(Session["u_id"].ToString());
            iv.in_date = System.DateTime.Now;
            iv.in_totalbill = (float)TempData["total"];
            db.tbl_invoice.Add(iv);
            db.SaveChanges();

            foreach (var item in li)
            {
                tbl_order od = new tbl_order();
                od.o_fk_pro = item.productid;
                od.o_fk_invoice = iv.in_id;
                od.o_date = System.DateTime.Now;
                od.o_qty = item.qty;
                od.o_unitprice = (int)item.price;
                od.o_bill = item.bill;
                db.tbl_order.Add(od);
                db.SaveChanges();
            }

            TempData.Remove("total");
            TempData.Remove("cart");
            TempData.Remove("cartto");

            TempData["msg"] = "Transaction Completed...";
            TempData.Keep();
            return RedirectToAction("Index");

        }


        public ActionResult Admin()
        {                      
                return View();                          
        }


        public ActionResult showuser()
        {
            
            return View(db.tbl_user.OrderByDescending(x => x.U_id).ToList());
        }

        public ActionResult showfood()
        {
            
            return View(db.tbl_product.OrderByDescending(x => x.pro_id).ToList());
        }

        public ActionResult OrderDetails()
        {

            return View();
        }

        public ActionResult AdminFoodAdd()
        {
            
            return View();
        }


        [HttpPost]
        public ActionResult AdminFoodAdd(tbl_product pvm, HttpPostedFileBase imgfile)
        {         
            string path = uploadimgfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded....";
            }
            else
            {
                tbl_product p = new tbl_product();
                p.pro_name = pvm.pro_name;
                p.pro_price = pvm.pro_price;
                p.pro_desc = pvm.pro_desc;
                p.pro_image = path;              
                           
                db.tbl_product.Add(p);
                db.SaveChanges();
                Response.Redirect("AdminFoodAdd");

            }

            return View();
        }

        public string uploadimgfile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {

                        path = Path.Combine(Server.MapPath("~/Content/product"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/product/" + random + Path.GetFileName(file.FileName);

                        //    ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg ,jpeg or png formats are acceptable....'); </script>");
                }
            }

            else
            {
                Response.Write("<script>alert('Please select a file'); </script>");
                path = "-1";
            }
            return path;
        }


        public ActionResult contact()
        {            
            return View();
        }



    }
}
