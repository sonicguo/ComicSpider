//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ComicData
{
    using System;
    using System.Collections.Generic;
    
    public partial class Chapter
    {
        public int ChapterID { get; set; }
        public System.Guid ComicID { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public string Description { get; set; }
        public System.Guid ChapterGUID { get; set; }
        public int TotalPage { get; set; }
    }
}