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

        public DmzjBCLOperator()
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

    }
}
