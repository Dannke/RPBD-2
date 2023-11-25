using NHibernate;
using System;


namespace RPBD_2
{
    public class AbonementRepository
    {
        private readonly ISession session;

        public AbonementRepository(ISession session)
        {
            this.session = session;
        }

        public bool AddAbonement(Readers reader, int bookNumber, DateTime bookIssueDate, DateTime? deliveryDate)
        {
            using (var transaction = session.BeginTransaction())
            {
                var newAbonement = new Abonement
                {
                    Reader = reader,
                    BookNumbers = session.Load<InventoryList>(bookNumber),
                    BookIssueDate = bookIssueDate,
                    DeliveryDate = deliveryDate
                };

                try
                {
                    session.Save(newAbonement);
                    transaction.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public bool DeleteAbonement(Abonement abonementToDelete, int bookNumberToDelete)
        {
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    session.Delete(abonementToDelete);
                    transaction.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }
    }
}
