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
        private ComicSite dmzjSite = null;
        private object _syncDBLock = new object();
        private const string _IndexerBaseURL = @"http://http://www.dmzj.com/category/0-0-0-0-0-0-{0}.html";

        #region Properties

        public ComicSite SiteInfo
        {
            get { return dmzjSite; }
        }

        #endregion

        #region Constructor


        public DmzjBCLOperator()
        {
            Initialize();
            InitializeSiteCategoryIndexerTable();
        }

        #endregion


        #region Public Method

        public void Initialize()
        {
            if (dmzjSite == null)
            {
                InitializeSiteInfo();
            }
            InitializeSiteCategoryIndexerTable();
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


        public void UpdateComic()
        {

        }

        #endregion


        #region Private Method

        private void InitializeSiteInfo()
        {
            using (var m_DBEntity = new ComicData.ComicSpiderDBEntities())
            {

                ComicSite result = (from r in m_DBEntity.ComicSite
                              where r.SiteName == "dmzj"
                              select r)
                              .FirstOrDefault();

                if (result == null)
                {
                    dmzjSite = new ComicSite()
                    {
                        SiteName = "dmzj",
                        Description = @"动漫之家",
                        URL = @"http://www.dmzj.com"
                    };
                    m_DBEntity.ComicSite.Add(dmzjSite);
                    m_DBEntity.SaveChanges();
                }
                else
                {
                    dmzjSite = result;
                }
            }
        }

        private void InitializeSiteCategoryIndexerTable()
        {
            using (var m_DBEntity = new ComicData.ComicSpiderDBEntities())
            {

                lock (this._syncDBLock)
                {
                    for (int i = 1; i <= 975; i++)
                    {
                        string url = string.Format(_IndexerBaseURL, i);

                        var sciInfo = (
                        from r in m_DBEntity.SiteCategoryIndexer
                        where r.SiteID == SiteInfo.SiteID
                        select r
                        ).FirstOrDefault();

                        if (sciInfo == null || !(sciInfo.SiteID == SiteInfo.SiteID && sciInfo.URL == url))
                        {
                            m_DBEntity.SiteCategoryIndexer.Add(new SiteCategoryIndexer() { SiteID = SiteInfo.SiteID, URL = url });
                            m_DBEntity.SaveChanges();
                        }
                    }

                }
            }
        }

        private void UpdateCategoryInfo(string cateName, string description)
        {

        }

        #endregion

    }
}
