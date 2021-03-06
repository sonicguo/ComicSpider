﻿using System;
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
                var result = from r in m_DBEntity.SiteCategoryIndexer
                             where r.SiteID == SiteInfo.SiteID
                             select r.URL;

                foreach (var item in result)
                {
                    uris.Add(new Uri(item));
                }
            }
            return uris;
        }

        public List<Uri> GetComicUri()
        {
            List<Uri> uris = new List<Uri>();
            using (var m_DBEntity = new ComicData.ComicSpiderDBEntities())
            {
                var result = from r in m_DBEntity.Comic select r.URL;

                foreach (var item in result)
                {
                    uris.Add(new Uri(item));
                }
            }
            return uris;
        }

        public List<Chapter> GetChapterList()
        {
            List<Chapter> list = new List<Chapter>();
            int siteID = this.SiteInfo.SiteID;
            using (var m_DBEntity = new ComicData.ComicSpiderDBEntities())
            {
                var result = (from r in m_DBEntity.Chapter
                              from l in m_DBEntity.Comic
                              orderby r.ComicID
                              orderby r.OrderValue ascending
                              where l.SiteID == siteID
                              where l.ComicID == r.ComicID
                              select r).ToList<Chapter>();

                list = result;
            }
            return list;
        }

        public void UpdatePage(Guid pageGUID, Guid ChapterGUID, int PageID, string URL, string Description, int OrderValue)
        {
            using (var m_DBEntity = new ComicData.ComicSpiderDBEntities())
            {
                //var pg = (from r
                //         in m_DBEntity.Page
                //          where r.ChapterGUID == ChapterGUID
                //          where r.URL == URL
                //          select r).FirstOrDefault();

                //if (pg != null 
                //    && (pg.PageGUID != null || pg.PageGUID != Guid.Empty)
                //    && pg.PageGUID.ToString() == pageGUID.ToString())
                //{

                //}

                if (pageGUID == null || pageGUID == Guid.Empty)
                {
                    Page page = new Page();

                    page.PageGUID = Guid.NewGuid();
                    page.ChapterGUID = ChapterGUID;
                    page.PageID = PageID;
                    page.Description = Description;
                    page.URL = URL;
                    page.OrderValue = OrderValue;

                    m_DBEntity.Page.Add(page);
                    m_DBEntity.SaveChanges();
                }
                else
                {
                    string pGUID = pageGUID.ToString();
                    var p = (from r in m_DBEntity.Page
                             where r.PageGUID == pageGUID
                             select r).SingleOrDefault();

                    p.ChapterGUID = ChapterGUID;
                    p.PageID = PageID;
                    p.URL = URL;
                    p.Description = Description;
                    p.OrderValue = OrderValue;
                    m_DBEntity.SaveChanges();

                }
            }
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

                    //throw;  // TODO : capture and log exception
                }

            }
        }

        public Guid UpdateChapter(Guid comicID, string name, string url, string description, int orderValue, int totalPage)
        {
           return UpdateChapter(Guid.Empty, comicID, name, url, description, orderValue, totalPage);
        }

        public Guid UpdateChapter(Guid ChapterGUID, Guid comicID, string name, string url, string description, int orderValue, int totalPage)
        {

            using (var m_DBEntity = new ComicData.ComicSpiderDBEntities())
            {
                Chapter chapter = null;
                try
                {
                    if (ChapterGUID == null || ChapterGUID == Guid.Empty)
                    {
                        chapter = (from r in m_DBEntity.Chapter where r.ChapterGUID == ChapterGUID where r.Name == name select r).SingleOrDefault();
                    }

                    if (chapter != null)
                    {
                        chapter.ComicID = comicID;
                        chapter.Name = name;
                        chapter.URL = url;
                        chapter.Description = description;
                        chapter.OrderValue = orderValue;
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
                        chapter.OrderValue = orderValue;
                        chapter.TotalPage = totalPage;

                        m_DBEntity.Chapter.Add(chapter);
                        m_DBEntity.SaveChanges();
                    }

                    ChapterGUID = chapter.ChapterGUID;
                }
                catch (Exception ex)
                {

                    //throw ex; // TODO : capture and log excpetion
                }
            }
            return ChapterGUID;
        }

        public Guid GetComicID(int siteID, Uri uri)
        {
            Guid comicID = Guid.Empty;
            using (var m_DBEntity = new ComicData.ComicSpiderDBEntities())
            {
                string uriStr = uri.ToString();
                var res = (from r in m_DBEntity.Comic
                           where r.URL == uriStr
                           where r.SiteID == siteID
                           select r).FirstOrDefault();

                if (res != null)
                {
                    comicID = res.ComicID;
                }
            }

            return comicID;
        }


        public List<string> GetChapterURL(int siteID)
        {
            List<string> uris = new List<string>();
            using (var m_DBEntity = new ComicData.ComicSpiderDBEntities())
            {
                var result = from r in m_DBEntity.Chapter select r.URL;
            }
            return uris;
        }

        #endregion

    }
}
