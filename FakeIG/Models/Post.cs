//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace FakeIG.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Post
    {
        public System.Guid PostID { get; set; }
        public System.Guid MemberID { get; set; }
        public Nullable<System.Guid> PointID { get; set; }
        public System.DateTime CreateTime { get; set; }
        public System.DateTime LastEditTime { get; set; }
        public string Contents { get; set; }
        public int Nice { get; set; }
    }
}