using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FakeIG.Models
{
    public class MainModel
    {
        public List<PostAndMember> PAMList { get; set; }
        public Member Member { get; set; }
        public PostAndMember PAM { get; set; }
    }

    public class PostAndMember
    {
        public System.Guid PostID { get; set; }
        public System.Guid MemberID { get; set; }
        public Nullable<System.Guid> PointID { get; set; }
        public System.DateTime CreateTime { get; set; }
        public System.DateTime LastEditTime { get; set; }
        public string Contents { get; set; }
        public int Nice { get; set; }
        public List<ImagePath> ImgPathList { get; set; }
        public List<PostAndMember> PointList { get; set; }
        // 帳號資訊
        public string Name { get; set; }
        public string PicPath { get; set; }
        // 是否按讚
        public bool Yes { get; set; }

    }
}