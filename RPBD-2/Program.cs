using NHibernate;
using NHibernate.Cfg;
using System;

namespace RPBD_2
{
    class Program
    {
        static void Main(string[] args)
        {
            var sessionFactory = NHibernateHelper.GetSessionFactory();
            using (var session = sessionFactory.OpenSession())
            {
                var bookRepository = new BookRepository(session);
                var readerRepository = new ReaderRepository(session);
                var abonementRepository = new AbonementRepository(session);

                var consoleUI = new ConsoleUI(bookRepository, readerRepository, abonementRepository, session);

                Console.WriteLine("Wellcome to Library management system");

                consoleUI.Run();

            }   
        }
    }
}
