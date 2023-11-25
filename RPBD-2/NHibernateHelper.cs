using NHibernate;
using NHibernate.Cfg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPBD_2
{
    class NHibernateHelper
    {
        private static Configuration _configuration;

        public static Configuration GetConfiguration()
        {
            if (_configuration == null)
            {
                _configuration = new Configuration()
                  .Configure("C:/Users/User/source/repos/RPBD-2/RPBD-2/nhibernate.cfg.xml")
                  .AddAssembly(typeof(BookCollection).Assembly);
            }

            return _configuration;
        }

        public static ISessionFactory GetSessionFactory()
        {
            return GetConfiguration().BuildSessionFactory();
        }
    }
}
