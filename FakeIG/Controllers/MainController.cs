using FakeIG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FakeIG.Const;
using FakeIG.Managers;

namespace FakeIG.Controllers
{
    [Authorize]
    public class MainController : Controller
    {
        private FakeIGEntities2 _db = new FakeIGEntities2();
        private int _pageSize = consts.pageSize;
        private PostManager _pm = new PostManager();
        // GET: Main
        public ActionResult Index()
        {
            Member member = (Member)Session["Member"];
            if (member == null)
            {
                return View("Login", "Member");
            }
            // 找最近更新的文章
            //var postList = _db.Post
            //    .OrderByDescending(m => m.CreateTime)
            //    .Take(_pageSize).ToList();
            var PAMList = _pm.DisplayPostList(member, _pageSize);
            var mm = new MainModel()
            {
                PAMList = PAMList,
                Member = member
            };
            return View(mm);
        }
    }
}