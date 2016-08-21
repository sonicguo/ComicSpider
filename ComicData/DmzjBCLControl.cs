using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ComicData
{
    public class DmzjBCLControl
    {
        private ComicSpiderDBEntities m_DBEntity = new ComicSpiderDBEntities();

        public DmzjBCLControl()
        {
            var site = from r in m_DBEntity.ComicSiteTable
                       where r.SiteName == "dmzj"
                       select new
                       {
                           SiteID = r.SiteID,
                           SiteName = r.SiteName,
                           URL = r.URL
                       };

        }

    }
}
