using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ComicData;

namespace ComicBCL
{
    public class DmzjBCLOperator
    {
        private ComicSiteTable dmzjSite = null;

        #region Properties

        public ComicSiteTable SiteInfo
        {
            get { return dmzjSite; }
        }

        #endregion

        #region Constructor


        public DmzjBCLOperator()
        {
            Initialize();
        }

        #endregion


        #region Public Method

        public void Initialize()
        {
            if (dmzjSite == null)
            {
                InitializeSiteInfo();
            }
        }

        public void UpdateCategoary(string cateName, string cateURL)
        {
            using (var m_DBEntity = new ComicData.ComicSpiderDBEntities())
            {
                var cateInfo = (from c in m_DBEntity.Category where c.CategoryName == cateName select c).FirstOrDefault();
                if (cateInfo == null)
                {
                    cateInfo = new Category()
                    {
                        CategoryName = cateName,
                        Description = "动漫之家 - " + cateName
                    };
                    m_DBEntity.Category.Add(cateInfo);
                    m_DBEntity.SaveChanges();

                    //cateInfo = (from c in m_DBEntity.Category where c.CategoryName == cateName select c).FirstOrDefault();
                }

                var siteCateInfo = (from r in m_DBEntity.SiteCategory
                              where r.SiteID == SiteInfo.SiteID
                              where r.CategoryID == cateInfo.CategoryID
                              select r).FirstOrDefault();

                if (siteCateInfo == null)
                {
                    siteCateInfo = new SiteCategory()
                    {
                        CategoryID = cateInfo.CategoryID,
                        SiteID = SiteInfo.SiteID,
                        CategoryName = cateName,
                        URL = cateURL,
                        Description = "动漫之家 - " + cateName
                    };
                }
                siteCateInfo.URL = cateURL;

                m_DBEntity.SiteCategory.Add(siteCateInfo);
                m_DBEntity.SaveChanges();


            }
        }

        #endregion


        #region Private Method

        private void InitializeSiteInfo()
        {
            using (var m_DBEntity = new ComicData.ComicSpiderDBEntities())
            {

                var result = (from r in m_DBEntity.ComicSiteTable
                              where r.SiteName == "dmzj"
                              select r).FirstOrDefault();

                ;
                if (result == null)
                {
                    dmzjSite = new ComicSiteTable()
                    {
                        SiteName = "dmzj",
                        Description = @"动漫之家",
                        URL = @"http://www.dmzj.com"
                    };
                    m_DBEntity.ComicSiteTable.Add(dmzjSite);
                    m_DBEntity.SaveChanges();
                }
                else
                {
                    dmzjSite = result;
                }
            }
        }

        private void InsertCategoryInfo(string cateName, string description)
        {

        }

        #endregion

    }
}
