using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;

namespace RPBD_2
{
    public class ReaderRepository
    {
        private readonly ISession session;

        public ReaderRepository(ISession session)
        {
            this.session = session;
        }

        public bool AddReader(string surname, string firstName, string patronymic, string telephone, string address, DateTime registrationDate)
        {
            using (var transaction = session.BeginTransaction())
            {
                var newReader = new Readers
                {
                    Surname = surname,
                    FirstName = firstName,
                    Patronymic = patronymic,
                    Telephone = telephone,
                    Address = address,
                    RegistrationDate = registrationDate
                };

                try
                {
                    session.Save(newReader);
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

        public bool DeleteReaderByName(Readers readerToDelete, string surnameToDelete, string firstNameToDelete, string patronymicToDelete)
        {
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    session.Delete(readerToDelete);
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

        public bool UpdateReaderInformation(Readers readerToUpdate, int fieldNumber, string newValue)
        {
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    if (fieldNumber >= 1 && fieldNumber <= 5)
                    {
                        // Обновление выбранного поля
                        switch (fieldNumber)
                        {
                            case 1:
                                readerToUpdate.Surname = newValue;
                                break;
                            case 2:
                                readerToUpdate.FirstName = newValue;
                                break;
                            case 3:
                                readerToUpdate.Patronymic = newValue;
                                break;
                            case 4:
                                readerToUpdate.Telephone = newValue;
                                break;
                            case 5:
                                readerToUpdate.Address = newValue;
                                break;
                        }
                    }
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

        public Readers FindReader(string surname, string firstName, string patronymic)
        {
            var hql = "FROM Readers WHERE Surname = :surname AND FirstName = :firstName AND Patronymic = :patronymic";
            var query = session.CreateQuery(hql)
                .SetParameter("surname", surname)
                .SetParameter("firstName", firstName)
                .SetParameter("patronymic", patronymic);

            return query.UniqueResult<Readers>();
        }

        public bool AddBookToReader(Readers reader, int bookNumber, DateTime bookIssueDate)
        {
            var newAbonement = new Abonement
            {
                Reader = reader,
                BookNumbers = session.Load<InventoryList>(bookNumber),
                BookIssueDate = bookIssueDate,
            };
            using (var transaction = session.BeginTransaction())
            {
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

        public bool RemoveBookFromReader(Readers reader, int bookNumber, DateTime deliveryDate)
        {
            // Поиск записи в таблице Abonement
            var hql = "FROM Abonement WHERE Reader.Id = :readerId AND BookNumbers = :bookNumber";
            var query = session.CreateQuery(hql);
            query.SetParameter("readerId", reader.Id);
            query.SetParameter("bookNumber", bookNumber);

            var abonementToRemove = query.UniqueResult<Abonement>();

            if (abonementToRemove == null)
            {
                return false;
            }

            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    abonementToRemove.DeliveryDate = deliveryDate;
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

        public IList<Readers> SearchReaders(
            string surname = null,
            string firstName = null,
            string patronymic = null,
            string telephone = null,
            string address = null,
            DateTime? registrationDate = null)
        {
            var criteria = DetachedCriteria.For<Readers>();

            if (!string.IsNullOrEmpty(surname))
            {
                criteria.Add(Restrictions.Like("Surname", $"%{surname}%"));
            }

            if (!string.IsNullOrEmpty(firstName))
            {
                criteria.Add(Restrictions.Like("FirstName", $"%{firstName}%"));
            }

            if (!string.IsNullOrEmpty(patronymic))
            {
                criteria.Add(Restrictions.Like("Patronymic", $"%{patronymic}%"));
            }

            if (!string.IsNullOrEmpty(telephone))
            {
                criteria.Add(Restrictions.Like("Telephone", $"%{telephone}%"));
            }

            if (!string.IsNullOrEmpty(address))
            {
                criteria.Add(Restrictions.Like("Address", $"%{address}%"));
            }

            if (registrationDate.HasValue)
            {
                criteria.Add(Restrictions.Eq("RegistrationDate", registrationDate.Value));
            }

            var result = criteria.GetExecutableCriteria(session).List<Readers>();
          
            return result;
        }

        public int GetBorrowedBooksCount(int readerId)
        {
            var hql = $"SELECT COUNT(*) FROM Abonement WHERE Reader.Id = {readerId}";
            var query = session.CreateQuery(hql);
            return Convert.ToInt32(query.UniqueResult());
        }

        public int GetReturnedBooksCount(int readerId)
        {
            var hql = $"SELECT COUNT(*) FROM Abonement WHERE Reader.Id = {readerId} AND DeliveryDate IS NOT NULL";
            var query = session.CreateQuery(hql);
            return Convert.ToInt32(query.UniqueResult());
        }
    }
}
