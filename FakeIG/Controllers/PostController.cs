using FakeIG.Managers;
using FakeIG.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
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

            return RedirectToAction("Display", new { postID = post.PostID });
        }

        /// <summary> 上傳圖片/// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateImg(HttpPostedFileBase[] photo)
        {
            // 先檢查檔案是否為圖檔
            foreach (var item in photo)
            {
                if (this.CheckImgFormat(item.FileName) == false)
                {
                    ViewBag.Msg = "請上傳圖檔";
                    return View("Create");
                }
            }

            // Azure 儲存體金鑰
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=fakeiguser;AccountKey=QY0XKemcjtZhdeArIGo4LBoNha2VMdEfGorAIdKv9C+wKYuscYTeK1+6GDkJQHJ+75Gto0kYB6SP+AStcdPcIw==;EndpointSuffix=core.windows.net");

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container. 
            CloudBlobContainer container = blobClient.GetContainerReference("fakeiguser");

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            container.SetPermissions(
                     new BlobContainerPermissions
                     {
                         PublicAccess =
                             BlobContainerPublicAccessType.Blob
                     });

            // ---------------------------------------------
            // 上傳圖檔名稱
            string fileName = "";
            Random random = new Random((int)DateTime.Now.Ticks);
            Member member = (Member)Session["Member"];
            ViewBag.Member = member;

            //string folderPath = Const.consts.folderPath;
            //folderPath = HostingEnvironment.MapPath(folderPath);
            List<string> ImgPath = new List<string>();
            for (int i = 0; i < photo.Length; i++)
            {
                HttpPostedFileBase f = photo[i];
                if (f != null)
                {
                    if (f.ContentLength > 0)
                    {
                        /*if (!Directory.Exists(folderPath)) // 假如資料夾不存在，先建立
                            Directory.CreateDirectory(folderPath);
                        fileName = "P" + DateTime.Now.ToString("yyyyMMdd_HHmmss_FFFFFF") + "_" + member.Account + "_" + random.Next(100000).ToString("00000") + Path.GetExtension(f.FileName);
                        var path = Path.Combine(Server.MapPath($"{folderPath}"), fileName);
                        f.SaveAs(path);

                        var viewPath = Path.Combine(folderPath, fileName);
                        ImgPath.Add(viewPath); */

                        // --------------------------------------------------------------------------

                        fileName = "P" + DateTime.Now.ToString("yyyyMMdd_HHmmss_FFFFFF") + "_" + member.Account + "_" + random.Next(100000).ToString("00000");

                        CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

                        blockBlob.Properties.ContentType = f.ContentType;
                        blockBlob.UploadFromStream(f.InputStream);

                        var ImgblockBlob = blockBlob.Uri.AbsoluteUri;

                        ImgPath.Add(ImgblockBlob);
                    }
                }
            }
            ViewBag.ImgPath = ImgPath;
            return View("Create");
        }

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

        public ActionResult Reply(Guid pointID, string reply, Guid postMemberID)
        {
            Member member = (Member)Session["Member"];

            // 新增回覆
            var pointPost = new Post()
            {
                PostID = Guid.NewGuid(),
                PointID = pointID,
                MemberID = member.MemberID,
                CreateTime = DateTime.Now,
                LastEditTime = DateTime.Now,
                Contents = reply,
            };
            _db.Post.Add(pointPost);

            // 新增回覆通知
            var replyList = new ReplyList()
            {
                PostID = pointPost.PostID,
                MemberID = member.MemberID,
                PostMemberID = postMemberID,
                Readed = false,
            };
            _db.ReplyList.Add(replyList);

            _db.SaveChanges();
            return RedirectToAction("Display", new { postID = pointPost.PointID });
        }

        [HttpPost]
        public string Like(Guid memberID, Guid postID, Guid postMemberID)
        {
            // 查詢是否有nice紀錄
            var kiroku = _db.NiceList.Where(m => m.MemberID == memberID && m.PostID == postID).FirstOrDefault();
            if (kiroku == null)
            {
                // 沒有就新增一筆
                var nice = new NiceList()
                {
                    MemberID = memberID,
                    PostID = postID,
                    PostMemberID = postMemberID,
                    Yes = true,
                    Readed = false,
                };
                _db.NiceList.Add(nice);
                _db.SaveChanges();

                return "Nice";
            }
            else if (kiroku.Yes == false)
            {
                kiroku.Yes = true;
                _db.Entry(kiroku).CurrentValues.SetValues(kiroku);
                _db.SaveChanges();

                return "Nice";
            }
            else
            {
                kiroku.Yes = false;
                _db.Entry(kiroku).CurrentValues.SetValues(kiroku);
                _db.SaveChanges();

                return "NoNice";
            }
        }

        public ActionResult ToastsReply(Guid memberID)
        {
            var replyList = _db.ReplyList.Where(m => m.PostMemberID == memberID && m.Readed == false).ToList();
            var pointList = _db.Post.Where(m => m.PointID != null).ToList();
            List<PostAndMember> toastsList = new List<PostAndMember>();
            if (replyList.Count > 0)
            {
                for (int i = 0; i < replyList.Count; i++)
                {
                    // 找到回文
                    var post = pointList.Find(m => m.PostID == replyList[i].PostID);
                    // 找到回文者暱稱及圖片
                    var postMember = _db.Member.ToList().Where(m => m.MemberID == replyList[i].MemberID).FirstOrDefault();
                    // 整合回文、回文帳號暱稱及圖片
                    var PostPAM = new PostAndMember()
                    {
                        PostID = post.PostID,
                        PointID = post.PointID,
                        MemberID = post.MemberID,
                        CreateTime = post.CreateTime,
                        LastEditTime = post.LastEditTime,
                        Contents = post.Contents,
                        Name = postMember.Name,
                        PicPath = postMember.PicPath,
                    };
                    toastsList.Add(PostPAM);

                    // 未讀的通知改為已通知
                    replyList[i].Readed = true;
                    _db.Entry(replyList[i]).CurrentValues.SetValues(replyList[i]);
                }

                _db.SaveChanges();
                //ViewBag.replyList = toastsList;
                string jsonText = JsonConvert.SerializeObject(toastsList);
                return Json(toastsList);
            }
            return null;
        }

        public ActionResult ToastsNice(Guid memberID)
        {
            var niceList = _db.NiceList.Where(m => m.PostMemberID == memberID && m.Readed == false).ToList();
            List<PostAndMember> toastsList = new List<PostAndMember>();
            if (niceList.Count > 0)
            {
                for (int i = 0; i < niceList.Count; i++)
                {
                    // 找到按讚者暱稱及圖片
                    var postMember = _db.Member.ToList().Where(m => m.MemberID == niceList[i].MemberID).FirstOrDefault();
                    // 整合回文、回文帳號暱稱及圖片
                    var PostPAM = new PostAndMember()
                    {
                        PostID = niceList[i].PostID,
                        Name = postMember.Name,
                        PicPath = postMember.PicPath,
                    };       
                    toastsList.Add(PostPAM);

                    // 未讀的通知改為已通知
                    niceList[i].Readed = true;
                    _db.Entry(niceList[i]).CurrentValues.SetValues(niceList[i]);
                }

                _db.SaveChanges();
                //ViewBag.niceList = toastsList;
                string jsonText = JsonConvert.SerializeObject(toastsList);
                return Json(toastsList);
            }
            return null;
        }

        private Boolean CheckImgFormat(String fileName)
        {
            Boolean flag = false;
            String fileExtension = Path.GetExtension(fileName).ToLower();
            String[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

            for (int i = 0; i < allowedExtensions.Length; i++)
            {
                if (allowedExtensions[i].ToString().Equals(fileExtension))
                {
                    flag = true;
                }
            }

            return flag;
        }
    }
}