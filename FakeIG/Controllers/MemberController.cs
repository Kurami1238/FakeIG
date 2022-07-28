using FakeIG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FakeIG.Controllers
{
    public class MemberController : Controller
    {
        private FakeIGEntities2 _db = new FakeIGEntities2();
        // GET: Member
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Member member)
        {

            if (ModelState.IsValid)
            {
                // 檢查是否重複帳號
                var acc = _db.Member.Where(m => m.Account == member.Account).FirstOrDefault();
                if (acc == null)
                {
                    member.MemberID = Guid.NewGuid();
                    member.CreateTime = DateTime.Now;
                    member.PicPath = "/Images/defaltpic.jpg";
                    _db.Member.Add(member);
                    _db.SaveChanges();
                    ViewBag.MemberAcc = member.Account;
                    return RedirectToAction("Login");

                }
                else
                {
                    ViewBag.Msg = "已有此帳號";
                    return View(member);
                }

            }
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Member member)
        {
            var result = _db.Member.Where(m => m.Account == member.Account && m.Password == member.Password).FirstOrDefault();
            if (result != null)
              {
                Session["Member"] = result;
                FormsAuthentication.RedirectFromLoginPage("User", true);
                return RedirectToAction("Index","Main");
            }
            else
            {
                ViewBag.Msg = "帳號密碼錯誤";
                return View();
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session["Member"] = null;
            return RedirectToAction("Login");
        }

        
    }
}