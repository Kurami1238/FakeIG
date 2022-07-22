using FakeIG.Managers;
using FakeIG.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace FakeIG.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private FakeIGEntities2 _db = new FakeIGEntities2();
        private PostManager _pm = new PostManager();

        // GET: Post
        public ActionResult Create()
        {
            Member member = (Member)Session["Member"];
            ViewBag.Member = member;

            return View();
        }

        /// <summary> 上傳圖片後新增文章 /// </summary>
        /// <param name="contents"></param>
        /// <param name="imgpath"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(string contents, string imgpath)
        {
            Member member = (Member)Session["Member"];
            ViewBag.Member = member;

            var post = new Post()
            {
                PostID = Guid.NewGuid(),
                MemberID = member.MemberID,
                PointID = null,
                CreateTime = DateTime.Now,
                LastEditTime = DateTime.Now,
                Contents = contents,
                Nice = 0
            };
            _db.Post.Add(post);

            string[] path = imgpath.Split(';');
            foreach (var item in path)
            {
                var imagepath = new ImagePath()
                {
                    ImgPath = item,
                    PostID = post.PostID,
                };
                _db.ImagePath.Add(imagepath);
            }

            _db.SaveChanges();

            return RedirectToAction("Display", post.PostID);
        }

        /// <summary> 上傳圖片/// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateImg(HttpPostedFileBase[] photo)
        {
            // 上傳圖檔名稱
            string fileName = "";
            Random random = new Random((int)DateTime.Now.Ticks);
            Member member = (Member)Session["Member"];
            ViewBag.Member = member;

            string folderPath = Const.consts.folderPath;
            folderPath = HostingEnvironment.MapPath(folderPath);
            List<string> ImgPath = new List<string>();
            for (int i = 0; i < photo.Length; i++)
            {
                HttpPostedFileBase f = photo[i];
                if (f != null)
                {
                    if (f.ContentLength > 0)
                    {
                        if (!Directory.Exists(folderPath)) // 假如資料夾不存在，先建立
                            Directory.CreateDirectory(folderPath);
                        fileName = "P" + DateTime.Now.ToString("yyyyMMdd_HHmmss_FFFFFF") + "_" + member.Account + "_" + random.Next(100000).ToString("00000") + Path.GetExtension(f.FileName);
                                               
                        var path = Path.Combine(folderPath, fileName);
                        f.SaveAs(path);
                        ImgPath.Add(path);
                    }
                }
            }
            ViewBag.ImgPath = ImgPath;
            return View();
        }

        ///// <summary> 上傳圖片/// </summary>
        ///// <param name="photo"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult CreateImg(HttpPostedFileBase photo)
        //{
        //    // 上傳圖檔名稱
        //    string fileName = "";
        //    Random random = new Random((int)DateTime.Now.Ticks);
        //    Member member = (Member)Session["Member"];
        //    ViewBag.Member = member;
        //    string folderPath = Const.consts.folderPath;
        //    folderPath = HostingEnvironment.MapPath(folderPath);
        //    List<string> ImgPath = new List<string>();
            
        //    if (photo.ContentLength > 0)
        //    {
        //        if (!Directory.Exists(folderPath)) // 假如資料夾不存在，先建立
        //            Directory.CreateDirectory(folderPath);
        //        fileName = "P" + DateTime.Now.ToString("yyyyMMdd_HHmmss_FFFFFF") + "_" + member.Account + "_" + random.Next(100000).ToString("00000") + Path.GetExtension(photo.FileName);

        //        var path = Path.Combine(folderPath, fileName);
        //        photo.SaveAs(path);
        //        ImgPath.Add(path);
        //    }
              
        //    ViewBag.ImgPath = ImgPath;
        //    return View();
        //}

        public ActionResult Display(Guid postID)
        {
            Member member = (Member)Session["Member"];
            ViewBag.Member = member;

            var PAM = _pm.DisplayPost(member, postID);
            MainModel mm = new MainModel()
            {
                PAM = PAM,
                Member = member
            };
            return View(mm);
        }
    }
}