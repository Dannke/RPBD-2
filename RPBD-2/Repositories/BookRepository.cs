using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;


namespace RPBD_2
{
    public class BookRepository
    {
        private readonly ISession session;

        public BookRepository(ISession session)
        {
            this.session = session;
        }

        public bool AddBookCollection(string bookTitle, string authors, int yearOfCreation, int placeOfCreation, int publishingHouse, int udc)
        {
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    var newBookCollection = new BookCollection
                    {
                        BookTitle = bookTitle,
                        Authors = authors,
                        YearOfCreation = yearOfCreation,
                        PlaceOfCreation = session.Load<ListCountries>(placeOfCreation),
                        PublishingHouse = session.Load<ListPublishers>(publishingHouse),
                        UDC = session.Load<SystematicCatalog>(udc)
                    };

                    session.Save(newBookCollection);
                    transaction.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public bool UpdateBookInformation(BookCollection bookToUpdate, string bookTitleToUpdate, int fieldNumber, string newValue)
        {
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    switch (fieldNumber)
                    {
                        case 1:
                            bookToUpdate.BookTitle = newValue;
                            break;
                        case 2:
                            bookToUpdate.Authors = newValue;
                            break;
                        case 3:
                            if (int.TryParse(newValue, out var newYearOfCreation))
                            {
                                bookToUpdate.YearOfCreation = newYearOfCreation;
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        case 4:
                            if (int.TryParse(newValue, out var newCountryId))
                            {
                                var newCountry = session.Load<ListCountries>(newCountryId);
                                bookToUpdate.PlaceOfCreation = newCountry;
                            }
                            else
                            {
                                Console.WriteLine("Ошибка ввода места написания книги.");
                                return false;
                            }
                            break;
                        case 5:
                            if (int.TryParse(newValue, out var newPublisher))
                            {
                                var newHouse = session.Load<ListPublishers>(newPublisher);
                                bookToUpdate.PublishingHouse = newHouse;
                            }
                            else
                            {
                                Console.WriteLine("Ошибка ввода места написания книги.");
                                return false;
                            }

                            break;
                        case 6:
                            if (int.TryParse(newValue, out var newUdc))
                            {
                                bookToUpdate.UDC.UDC = newUdc;
                            }
                            else
                            {
                                Console.WriteLine("Ошибка ввода UDC.");
                                return false;
                            }
                            break;
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

        public bool DeleteBookByTitle(BookCollection bookToDelete, string bookTitleToDelete)
        {
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    session.Delete(bookToDelete);
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

        public IList<BookCollection> SearchBooks(
            string bookTitle = null,
            string authors = null,
            int? yearOfCreation = null,
            int? placeOfCreation = null,
            int? publishingHouse = null,
            int? udc = null
            )
        {
            var criteria = DetachedCriteria.For<BookCollection>();

            if (!string.IsNullOrEmpty(bookTitle))
            {
                criteria.Add(Restrictions.Like("BookTitle", $"%{bookTitle}%"));
            }

            if (!string.IsNullOrEmpty(authors))
            {
                criteria.Add(Restrictions.Like("Authors", $"%{authors}%"));
            }

            if (yearOfCreation.HasValue)
            {
                criteria.Add(Restrictions.Eq("YearOfCreation", yearOfCreation.Value));
            }

            if (placeOfCreation.HasValue)
            {
                var placeOfCreationObj = session.Load<ListCountries>(placeOfCreation.Value);
                criteria.Add(Restrictions.Eq("PlaceOfCreation", placeOfCreationObj));
            }           

            if (publishingHouse.HasValue)
            {
                var publishingHouseObj = session.Load<ListPublishers>(publishingHouse.Value);
                criteria.Add(Restrictions.Eq("PublishingHouse", publishingHouseObj));
            }

            if (udc.HasValue)
            {
                criteria.Add(Restrictions.Eq("UDC", udc.Value));
            }

            var result = criteria.GetExecutableCriteria(session).List<BookCollection>();

            return result;
        }

        public int GetBooksQuantity(int bookId)
        {
            using (var transaction = session.BeginTransaction())
            {
                var hql = $"SELECT COUNT(*) FROM InventoryList WHERE Book.Id = {bookId}";
                var query = session.CreateQuery(hql);
                return Convert.ToInt32(query.UniqueResult());
            }
        }
    }
}
