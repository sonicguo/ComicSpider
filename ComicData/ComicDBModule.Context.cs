﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ComicSpiderDBEntities : DbContext
    {
        public ComicSpiderDBEntities()
            : base("name=ComicSpiderDBEntities") 
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        public void FixEfProviderServicesProblem()
        {
            //The Entity Framework provider type 'System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer'
            //for the 'System.Data.SqlClient' ADO.NET provider could not be loaded. 
            //Make sure the provider assembly is available to the running application. 
            //See http://go.microsoft.com/fwlink/?LinkId=260882 for more information.

            var instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }

        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<SiteCategory> SiteCategory { get; set; }
        public virtual DbSet<Chapter> Chapter { get; set; }
        public virtual DbSet<Comic> Comic { get; set; }
        public virtual DbSet<ComicDetails> ComicDetails { get; set; }
        public virtual DbSet<ComicSite> ComicSite { get; set; }
        public virtual DbSet<Page> Page { get; set; }
        public virtual DbSet<SiteCategoryIndexer> SiteCategoryIndexer { get; set; }
    }
}
