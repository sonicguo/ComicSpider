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
        private const string _IndexerBaseURL = @"http://www.dmzj.com/category/0-0-0-0-0-0-{0}.html";

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

        public List<Uri> GetCategoryIndexerUri()
        {
            List<Uri> uris = new List<Uri>();
            using (var m_DBEntity = new ComicData.ComicSpiderDBEntities())
            {
                var result = GetSiteCategoryIndexerListFromDB();

                foreach (var item in result)
                {
                    uris.Add(new Uri(item));
                }
            }
            return uris;
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
                var sciList = GetSiteCategoryIndexerListFromDB();

                lock (this._syncDBLock)
                {
                    for (int i = 1; i <= 975; i++)
                    {
                        string url = string.Format(_IndexerBaseURL, i);
                        if (!sciList.Contains(url))
                        {
                            m_DBEntity.SiteCategoryIndexer.Add(new SiteCategoryIndexer() { SiteID = SiteInfo.SiteID, URL = url });
                        }
                    }
                    m_DBEntity.SaveChanges();
                }
            }
        }

        private string[] GetSiteCategoryIndexerListFromDB()
        {
            using (var m_DBEntity = new ComicData.ComicSpiderDBEntities())
            {
                return (
                from r in m_DBEntity.SiteCategoryIndexer
                where r.SiteID == SiteInfo.SiteID
                select r.URL
                ).ToArray();
            }
        }

        private void UpdateCategoryInfo(string cateName, string description)
        {

        }

        public Guid UpdateComic(int siteID, string uri, string description)
        {
            Guid commicID = Guid.NewGuid();
            using (var m_DBEntity = new ComicData.ComicSpiderDBEntities())
            {
                Comic res = (from r in m_DBEntity.Comic where r.SiteID == siteID where r.URL == uri select r).FirstOrDefault();

                if (res == null)
                {
                    Comic comic = new Comic();
                    comic.SiteID = siteID;
                    comic.URL = uri;
                    comic.Description = description;
                    comic.ComicID = commicID;
                    m_DBEntity.Comic.Add(comic);
                    m_DBEntity.SaveChanges();
                }
                else
                {
                    commicID = res.ComicID;
                }
            }

            return commicID;
        }

        public void UpdateComicComicDetail(Guid comicID, string comicName, string author, bool status)
        {
            using (var m_DBEntity = new ComicData.ComicSpiderDBEntities())
            {
                try
                {


                    ComicDetails detail = (from r in m_DBEntity.ComicDetails where r.ComicID == comicID select r).FirstOrDefault();

                    if (detail != null)
                    {
                        detail.ComicID = comicID;
                        detail.ComicName = comicName;
                        detail.Author = author;
                        detail.Status = status;
                        m_DBEntity.SaveChanges();
                    }
                    else
                    {
                        detail = new ComicDetails();
                        detail.ComicID = comicID;
                        detail.ComicName = comicName;
                        detail.Author = author;
                        detail.Status = status;

                        m_DBEntity.ComicDetails.Add(detail);
                        m_DBEntity.SaveChanges();
                    }
                }
                catch (Exception)
                {

                    throw;  // TODO : capture and log exception
                }

            }
        }

        public Guid UpdateChapter(Guid comicID, string name, string url, string description, int totalPage )
        {
            using (var m_DBEntity = new ComicData.ComicSpiderDBEntities())
            {
                try
                {


                    var chapter = (from r in m_DBEntity.Chapter where r.ComicID == comicID where r.Name == name select r).FirstOrDefault();

                    if (chapter != null)
                    {
                        chapter.ComicID = comicID;
                        chapter.Name = name;
                        chapter.URL = url;
                        chapter.Description = description;
                        chapter.TotalPage = totalPage;

                        m_DBEntity.SaveChanges();
                    }
                    else
                    {
                        chapter = new Chapter();
                        chapter.ChapterGUID = Guid.NewGuid();
                        chapter.ComicID = comicID;
                        chapter.Name = name;
                        chapter.URL = url;
                        chapter.Description = description;
                        chapter.TotalPage = totalPage;

                        m_DBEntity.Chapter.Add(chapter);
                        m_DBEntity.SaveChanges();
                    }

                    return chapter.ChapterGUID;
                }
                catch (Exception ex)
                {

                    throw ex; // TODO : capture and log excpetion
                }
            }
        }

        #endregion

    }
}
