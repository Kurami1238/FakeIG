using FakeIG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FakeIG.Managers
{
    public class PostManager
    {
        private FakeIGEntities2 _db = new FakeIGEntities2();

        public List<PostAndMember> DisplayPostList(Member member, int pageSize)
        {
            var postListMoto = _db.Post
                .Where(m => m.PointID == null)
                .OrderByDescending(m => m.CreateTime)
                .Take(pageSize).ToList();
            List<Post> postList = postListMoto;
            var PAMList = new List<PostAndMember>();
            for (int i = 0; i < postList.Count; i++)
            {
                // 找出文章作者圖片及暱稱 
                var postMember = _db.Member.ToList().Where(m => m.MemberID == postList[i].MemberID).FirstOrDefault();
                //var postMember = (from m in _db.Member
                //                 where m.MemberID == postList[i].MemberID
                //                 select m);
                // 找出文章回文
                var pointList = _db.Post.ToList().Where(m => m.PointID == postList[i].PostID).ToList();
                // 看此登錄帳號有無按讚此文章
                var postYes = _db.NiceList.ToList().Where(m => m.MemberID == member.MemberID && m.PostID == postList[i].PostID).FirstOrDefault();
                // 找此文章圖片
                var postImgPathList = _db.ImagePath.ToList().Where(m => m.PostID == postList[i].PostID).ToList();
                var pointPAMList = new List<PostAndMember>();
                if (pointList.Count > 0)       
                {
                    foreach (var item in pointList)
                    {
                        // 文章回文加上回文作者圖片及暱稱
                        // 回文沒有圖片
                        var pointMember = _db.Member.ToList().Where(m => m.MemberID == item.MemberID).FirstOrDefault();
                        var pointYes = _db.NiceList.ToList().Where(m => m.MemberID == member.MemberID && m.PostID == item.PostID).FirstOrDefault();
                        var pointPAM = new PostAndMember()
                        {
                            PostID = item.PostID,
                            MemberID = item.MemberID,
                            PointID = item.PointID,
                            CreateTime = item.CreateTime,
                            LastEditTime = item.LastEditTime,
                            Contents = item.Contents,
                            Nice = item.Nice,
                            Name = pointMember.Name,
                            PicPath = pointMember.PicPath,
                            Yes = (pointYes != null) ? pointYes.Yes : false
                        };
                        pointPAMList.Add(pointPAM);
                    }
                }

                // 整合文章，要有文章圖片、帳號暱稱及圖片，回文圖片、回文帳號暱稱及圖片
                var PostPAM = new PostAndMember()
                {
                    PostID = postList[i].PostID,
                    MemberID = postList[i].MemberID,
                    CreateTime = postList[i].CreateTime,
                    LastEditTime = postList[i].LastEditTime,
                    Contents = postList[i].Contents,
                    Nice = postList[i].Nice,
                    ImgPathList = postImgPathList,
                    Name = postMember.Name,
                    PicPath = postMember.PicPath,
                    Yes = (postYes != null) ? postYes.Yes : false,
                    PointList = pointPAMList
                };
                PAMList.Add(PostPAM);
            }
            
            return PAMList;
        }
        
        public PostAndMember DisplayPost(Member member, Guid postID)
        {
            var post = _db.Post.Where(m => m.PostID == postID).FirstOrDefault();

            // 找出文章作者圖片及暱稱 
            var postMember = _db.Member.Where(m => m.MemberID == post.MemberID).FirstOrDefault();
            // 找出文章回文
            var pointList = _db.Post.Where(m => m.PointID == post.PostID).ToList();
            // 看此登錄帳號有無按讚此文章
            var postYes = _db.NiceList.ToList().Where(m => m.MemberID == member.MemberID && m.PostID == post.PostID).FirstOrDefault();
            // 找此文章圖片
            var postImgPathList = _db.ImagePath.Where(m => m.PostID == post.PostID).ToList();
            var pointPAMList = new List<PostAndMember>();
            if (pointList.Count > 0)
            {
                foreach (var item in pointList)
                {
                    // 文章回文加上回文作者圖片及暱稱
                    // 回文沒有圖片
                    var pointMember = _db.Member.ToList().Where(m => m.MemberID == item.MemberID).FirstOrDefault();
                    var pointYes = _db.NiceList.ToList().Where(m => m.MemberID == member.MemberID && m.PostID == item.PostID).FirstOrDefault();
                    var pointPAM = new PostAndMember()
                    {
                        PostID = item.PostID,
                        MemberID = item.MemberID,
                        PointID = item.PointID,
                        CreateTime = item.CreateTime,
                        LastEditTime = item.LastEditTime,
                        Contents = item.Contents,
                        Nice = item.Nice,
                        Name = pointMember.Name,
                        PicPath = pointMember.PicPath,
                        Yes = (pointYes != null) ? pointYes.Yes : false
                    };
                    pointPAMList.Add(pointPAM);
                }
            }

            // 整合文章，要有文章圖片、帳號暱稱及圖片，回文圖片、回文帳號暱稱及圖片
            var PostPAM = new PostAndMember()
            {
                PostID = post.PostID,
                MemberID = post.MemberID,
                CreateTime = post.CreateTime,
                LastEditTime = post.LastEditTime,
                Contents = post.Contents,
                Nice = post.Nice,
                ImgPathList = postImgPathList,
                Name = postMember.Name,
                PicPath = postMember.PicPath,
                Yes = (postYes != null) ? postYes.Yes : false,
                PointList = pointPAMList
            };
            return PostPAM;
        }
    }
}