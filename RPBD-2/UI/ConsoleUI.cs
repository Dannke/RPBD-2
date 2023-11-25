using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPBD_2
{
    public class ConsoleUI
    {
        private readonly BookRepository bookRepository;
        private readonly ReaderRepository readerRepository;
        private readonly AbonementRepository abonementRepository;
        private readonly ISession session;

        public ConsoleUI(BookRepository bookRepository, ReaderRepository readerRepository, AbonementRepository abonementRepository, ISession session)
        {
            this.bookRepository = bookRepository;
            this.readerRepository = readerRepository;
            this.abonementRepository = abonementRepository;
            this.session = session;
        }

        public void Run()
        {
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("\n1. Вывести информацию о книгах");
                Console.WriteLine("2. Добавить новую книгу");
                Console.WriteLine("3. Обновить информацию о книге");
                Console.WriteLine("4. Удалить книгу");
                Console.WriteLine("5. Найти книгу");
                Console.WriteLine("6. Вывести информацию о читателях");
                Console.WriteLine("7. Добавить читателя");
                Console.WriteLine("8. Удалить читателя");
                Console.WriteLine("9. Обновить информацию о читателе");
                Console.WriteLine("10. Найти читателя");
                Console.WriteLine("11. Добавить книгу читателю");
                Console.WriteLine("12. Удалить книгу у читателя");
                Console.WriteLine("0. Выйти");

                Console.Write("Выберите действие: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        DisplayBooksInformation();
                        break;
                    case "2":
                        AddBooKInformationUI();
                        break;
                    case "3":
                        UpdateBookInformationUI();
                        break;
                    case "4":
                        DeleteBooKByTitleUI();
                        break;
                    case "5":
                        PerformBookSearchUI();
                        break;
                    case "6":
                        DisplayReadersInformation();
                        break;
                    case "7":
                        AddReaderInformationUI();
                        break;
                    case "8":
                        DeleteReaderByNameUI();
                        break;
                    case "9":
                        UpdateReaderInformationUI();
                        break;
                    case "10":
                        PerfomReaderSearchUI();
                        break;
                    case "11":
                        AddAbonementInformationUI();
                        break;
                    case "12":
                        DeleteAbonementInformationUI();
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Некорректный выбор. Попробуйте еще раз.");
                        break;
                }
            }
        }

        public void DisplayBooksInformation()
        {
            using (var transaction = session.BeginTransaction())
            {
                var hql = "FROM BookCollection";
                var query = session.CreateQuery(hql);

                var bookCollections = query.List<BookCollection>();

                Console.WriteLine("\nИнформация о книгах:\n");

                foreach (var bookCollection in bookCollections)
                {
                    Console.WriteLine($"Название книги: {bookCollection.BookTitle}");
                    Console.WriteLine($"Авторы: {bookCollection.Authors}");
                    Console.WriteLine($"Год создания: {bookCollection.YearOfCreation}");
                    Console.WriteLine($"Место создания: {bookCollection.PlaceOfCreation.Country}");
                    Console.WriteLine($"Издательство: {bookCollection.PublishingHouse.PublishingHouse}");
                    Console.WriteLine($"UDC: {bookCollection.UDC.UDC}");

                    // Получение количества книг по каждой записи в таблице inventory_list
                    var bookQuantity = bookRepository.GetBooksQuantity(bookCollection.Id);
                    Console.WriteLine($"Количество: {bookQuantity}");

                    Console.WriteLine();
                }
            }
        }

        private void PerformBookSearchUI()
        {
            Console.WriteLine("\n1. По названию книги");
            Console.WriteLine("2. По автору");
            Console.WriteLine("3. По году написания");
            Console.WriteLine("4. По месту написания");
            Console.WriteLine("5. По издательству");
            Console.WriteLine("0. Назад");

            Console.Write("Выберите критерий поиска: ");
            string attribute = Console.ReadLine();

            switch (attribute)
            {
                case "1":
                    Console.Write("Введите название книги: ");
                    string bookTitle = Console.ReadLine();
                    var resultTitle = bookRepository.SearchBooks(bookTitle: bookTitle);
                    DisplayBooksSearch(resultTitle);
                    break;
                case "2":
                    Console.Write("Введите Имя автора книги: ");
                    string author = Console.ReadLine();
                    var resultAuthor = bookRepository.SearchBooks(authors: author);
                    DisplayBooksSearch(resultAuthor);
                    break;
                case "3":
                    Console.Write("Введите год написания книги: ");
                    if (int.TryParse(Console.ReadLine(), out int yearOfCreation))
                    {
                        var resultYear = bookRepository.SearchBooks(yearOfCreation: yearOfCreation);
                        DisplayBooksSearch(resultYear);
                    }
                    else
                    {
                        Console.WriteLine("Некорректный ввод. Год создания должен быть целым числом.");
                    }
                    break;
                case "4":
                    Console.Write("Введите место написания книги: ");
                    if (int.TryParse(Console.ReadLine(), out int placeOfCreation))
                    {
                        var resultPlace = bookRepository.SearchBooks(placeOfCreation: placeOfCreation);
                        DisplayBooksSearch(resultPlace);
                    }
                    break;
                case "5":
                    Console.Write("Введите название издательства книги: ");
                    if (int.TryParse(Console.ReadLine(), out int publishingHouse))
                    {
                        var resultHouse = bookRepository.SearchBooks(publishingHouse: publishingHouse);
                        DisplayBooksSearch(resultHouse);
                    }
                    break;
                case "0":
                    return; // Возврат в предыдущее меню
                default:
                    Console.WriteLine("Некорректный выбор. Попробуйте еще раз.");
                    break;
            }
        }

        public void DisplayBooksSearch(IList<BookCollection> result)
        {
            Console.WriteLine("\nРезультаты поиска:");
            foreach (var book in result)
            {
                Console.WriteLine($"1.Название книги: {book.BookTitle}");
                Console.WriteLine($"2.Авторы: {book.Authors}");
                Console.WriteLine($"3.Год создания: {book.YearOfCreation}");
                Console.WriteLine($"4.Место создания: {book.PlaceOfCreation.Country}");
                Console.WriteLine($"5.Издательство: {book.PublishingHouse.PublishingHouse}");
                Console.WriteLine($"6.UDC: {book.UDC.Id}");

                var bookQuantity = bookRepository.GetBooksQuantity(book.Id);
                Console.WriteLine($"Количество: {bookQuantity}");

                Console.WriteLine();
            }
        }

        public void DisplayReadersSearch(IList<Readers> result)
        {
            Console.WriteLine("\nРезультаты поиска читателей:");
            foreach (var reader in result)
            {
                Console.WriteLine($"Фамилия: {reader.Surname}");
                Console.WriteLine($"Имя: {reader.FirstName}");
                Console.WriteLine($"Отчество: {reader.Patronymic}");
                Console.WriteLine($"Телефон: {reader.Telephone}");
                Console.WriteLine($"Адрес: {reader.Address}");
                Console.WriteLine($"Дата регистрации: {reader.RegistrationDate:yyyy-MM-dd}");

                var borrowedBooks = readerRepository.GetBorrowedBooksCount(reader.Id);
                Console.WriteLine($"Количество взятых книг: {borrowedBooks}");

                var returnedBooks = readerRepository.GetReturnedBooksCount(reader.Id);
                Console.WriteLine($"Количество возвращенных книг: {returnedBooks}");

                Console.WriteLine();
            }
        }

        public string GetAvailableUDCRange()
        {
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    var hql = "SELECT MIN(Id), MAX(Id) FROM SystematicCatalog";
                    var query = session.CreateQuery(hql);
                    var result = query.List<object[]>().FirstOrDefault();

                    if (result != null && result.Length == 2)
                    {
                        var minUDC = (int)result[0];
                        var maxUDC = (int)result[1];
                        return $"Доступные UDC: {minUDC}-{maxUDC}";
                    }

                    return "Доступные UDC не найдены";
                }
                catch (Exception ex)
                {
                    // Обработка ошибки
                    Console.WriteLine($"Ошибка при выполнении запроса: {ex.Message}");
                    transaction.Rollback();
                    return "Ошибка при выполнении запроса";
                }
            }
        }

        public void UpdateBookInformationUI()
        {
            Console.Write("Введите название книги для обновления информации: ");
            var bookTitleToUpdate = Console.ReadLine();

            // Поиск книги по названию
            var hql = "FROM BookCollection WHERE BookTitle = :bookTitle";
            var query = session.CreateQuery(hql);
            query.SetParameter("bookTitle", bookTitleToUpdate);

            var bookToUpdate = query.List<BookCollection>().FirstOrDefault();

            if (bookToUpdate == null)
            {
                Console.WriteLine("Книга не найдена.");
                return;
            }

            Console.WriteLine("\nТекущая информация о книге:");

            Console.WriteLine($"1.Название книги: {bookToUpdate.BookTitle}");
            Console.WriteLine($"2.Авторы: {bookToUpdate.Authors}");
            Console.WriteLine($"3.Год создания: {bookToUpdate.YearOfCreation}");
            Console.WriteLine($"4.Место создания: {bookToUpdate.PlaceOfCreation.Country}");
            Console.WriteLine($"5.Издательство: {bookToUpdate.PublishingHouse.PublishingHouse}");
            Console.WriteLine($"6.UDC: {bookToUpdate.UDC.Id}");
            Console.WriteLine("Выберите номер поля для редактирования (0 - завершить): ");

            if (!int.TryParse(Console.ReadLine(), out var fieldNumber) || fieldNumber < 0 || fieldNumber > 6)
            {
                Console.WriteLine("Некорректный выбор.");
                return;
            }
            if (fieldNumber == 6)
            {
                var availableUDCRange = GetAvailableUDCRange();
                Console.WriteLine(availableUDCRange);
            }

            if (fieldNumber == 0)
            {
                Console.WriteLine("Редактирование завершено.");
                return;
            }

            Console.Write("Введите новое значение: ");
            var newValue = Console.ReadLine();

            if (bookRepository.UpdateBookInformation(bookToUpdate, bookTitleToUpdate, fieldNumber, newValue))
            {
                Console.WriteLine("Книга успешно обновлена");
            }
            else { Console.WriteLine("Произошла ошибка при обновлении данных о книги"); }
        }

        public void AddBooKInformationUI()
        {
            Console.Write("Название книги: ");
            var bookTitle = Console.ReadLine();

            Console.Write("Авторы: ");
            var authors = Console.ReadLine();

            Console.Write("Год создания: ");
            if (!int.TryParse(Console.ReadLine(), out var yearOfCreation))
            {
                Console.WriteLine("Ошибка ввода года.");
                return;
            }

            Console.Write("Место создания: ");
            string input = Console.ReadLine();

            if (!int.TryParse(input, out int placeOfCreation))
            {
                Console.WriteLine("Ошибка ввода. Введите корректное целое число.");
            }

            Console.Write("Издательство: ");
            if (!int.TryParse(Console.ReadLine(), out int publishingHouse))
            {
                Console.WriteLine("Ошибка ввода. Введите корректное целое число.");
            }

            var availableUDCRange = GetAvailableUDCRange();
            Console.WriteLine(availableUDCRange);

            Console.Write("UDC: ");
            if (!int.TryParse(Console.ReadLine(), out var udc))
            {
                Console.WriteLine("Ошибка ввода UDC.");
                return;
            }

            if (bookRepository.AddBookCollection(bookTitle, authors, yearOfCreation, placeOfCreation, publishingHouse, udc))
            {
                Console.WriteLine("Книга успешно добавлена!");
            }
            else { Console.WriteLine("Ошибка при добавлении книги"); }
        }

        public void DeleteBooKByTitleUI()
        {
            Console.Write("Введите название книги для удаления: ");
            var bookTitleToDelete = Console.ReadLine();

            // Поиск книги по названию
            var hql = "FROM BookCollection WHERE BookTitle = :title";
            var query = session.CreateQuery(hql);
            query.SetParameter("title", bookTitleToDelete);

            var bookToDelete = query.UniqueResult<BookCollection>();

            if (bookToDelete == null)
            {
                Console.Write("Книга не найдена");
                return;
            }

            if (bookRepository.DeleteBookByTitle(bookToDelete, bookTitleToDelete))
            {
                Console.Write("\nКнига успешна удалена");
            }
            else { Console.Write("\nОшибка при удалении книги"); }
        }

        public void AddReaderInformationUI()
        {
            // Запрос данных у пользователя
            Console.WriteLine("Введите данные для новой записи в таблице Readers:");

            Console.Write("Фамилия: ");
            var surname = Console.ReadLine();

            Console.Write("Имя: ");
            var firstName = Console.ReadLine();

            Console.Write("Отчество: ");
            var patronymic = Console.ReadLine();

            Console.Write("Телефон: ");
            var telephone = Console.ReadLine();

            Console.Write("Адрес: ");
            var address = Console.ReadLine();

            Console.Write("Дата регистрации (гггг-мм-дд): ");
            if (!DateTime.TryParse(Console.ReadLine(), out var registrationDate))
            {
                Console.WriteLine("Ошибка ввода даты регистрации.");
                return;
            }

            if (readerRepository.AddReader(surname, firstName, patronymic, telephone, address, registrationDate))
            {
                Console.WriteLine("Запись успешно добавлена в таблицу Readers.");
            }
            else { Console.WriteLine("Ошибка при добавлении записи в таблицу Readers."); }
        }

        public void DeleteReaderByNameUI()
        {
            Console.Write("Введите Фамилию читателя для удаления: ");
            var surnameToDelete = Console.ReadLine();

            Console.Write("Введите Имя читателя для удаления: ");
            var firstNameToDelete = Console.ReadLine();

            Console.Write("Введите Отчество читателя для удаления: ");
            var patronymicToDelete = Console.ReadLine();

            // Поиск читателя по ФИО
            var hql = "FROM Readers WHERE Surname = :surname AND FirstName = :firstName AND Patronymic = :patronymic";
            var query = session.CreateQuery(hql);
            query.SetParameter("surname", surnameToDelete);
            query.SetParameter("firstName", firstNameToDelete);
            query.SetParameter("patronymic", patronymicToDelete);
            var readerToDelete = query.UniqueResult<Readers>();

            if (readerToDelete == null)
            {
                Console.WriteLine("Читатель не найден.");
                return;
            }

            if (readerRepository.DeleteReaderByName(readerToDelete, surnameToDelete, firstNameToDelete, patronymicToDelete))
            {
                Console.WriteLine("Читатель успешно удален.");
            }
            else { Console.WriteLine("Ошибка при удалении читателя."); }
        }

        public void UpdateReaderInformationUI()
        {
            Console.Write("Введите Фамилию читателя для обновления информации: ");
            var surnameToUpdate = Console.ReadLine();

            Console.Write("Введите Имя читателя: ");
            var firstNameToUpdate = Console.ReadLine();

            Console.Write("Введите Отчество читателя: ");
            var patronymicToUpdate = Console.ReadLine();

            // Поиск читателя по введенным данным
            var readerToUpdate = readerRepository.FindReader(surnameToUpdate, firstNameToUpdate, patronymicToUpdate);

            if (readerToUpdate == null)
            {
                Console.WriteLine("Читатель не найден.");
                return;
            }

            while (true)
            {
                Console.WriteLine("\nТекущая информация о читателе:");

                Console.WriteLine($"1. Фамилия: {readerToUpdate.Surname}");
                Console.WriteLine($"2. Имя: {readerToUpdate.FirstName}");
                Console.WriteLine($"3. Отчество: {readerToUpdate.Patronymic}");
                Console.WriteLine($"4. Телефон: {readerToUpdate.Telephone}");
                Console.WriteLine($"5. Адрес: {readerToUpdate.Address}");
                Console.WriteLine("6. Добавить книгу читателю");
                Console.WriteLine("7. Удалить книгу у читателя");
                Console.WriteLine("Выберите номер поля для редактирования (0 - завершить): ");

                if (!int.TryParse(Console.ReadLine(), out var fieldNumber) || fieldNumber < 0 || fieldNumber > 7)
                {
                    Console.WriteLine("Некорректный ввод.");
                    return;
                }
                if (fieldNumber == 0)
                {
                    Console.WriteLine("Редактирование завершено.");
                    return;
                }

                if (fieldNumber >= 1 && fieldNumber <= 5)
                {
                    Console.Write("Введите новое значение: ");
                    var newValue = Console.ReadLine();

                    if (readerRepository.UpdateReaderInformation(readerToUpdate, fieldNumber, newValue))
                    {
                        Console.WriteLine("Информация о читателе успешно обновлена.");
                    }
                    else { Console.WriteLine("Ошибка при обновлении информации о читателе."); }
                }
                else if (fieldNumber > 5 && fieldNumber < 8)
                {
                    switch (fieldNumber)
                    {
                        case 6:
                            Console.WriteLine("Добавление книги читателю:");

                            Console.Write("Введите номер книги (book_numbers): ");
                            if (!int.TryParse(Console.ReadLine(), out var bookNumber))
                            {
                                Console.WriteLine("Ошибка ввода номера книги.");
                                return;
                            }

                            Console.Write("Введите дату выдачи книги (гггг-мм-дд): ");
                            if (!DateTime.TryParse(Console.ReadLine(), out var bookIssueDate))
                            {
                                Console.WriteLine("Ошибка ввода даты выдачи книги.");
                                return;
                            }

                            if (readerRepository.AddBookToReader(readerToUpdate, bookNumber, bookIssueDate))
                            {
                                Console.WriteLine("Книга успешно добавлена читателю.");
                            }
                            else { Console.WriteLine("Ошибка при добавлении книги читателю."); }
                            break;
                        case 7:
                            Console.WriteLine("Возвращение книги читателем:");

                            Console.Write("Введите номер книги (book_numbers): ");
                            if (!int.TryParse(Console.ReadLine(), out var bookNumberDelete))
                            {
                                Console.WriteLine("Ошибка ввода номера книги.");
                                return;
                            }

                            Console.Write("Введите дату возврата книги (гггг-мм-дд): ");
                            if (!DateTime.TryParse(Console.ReadLine(), out var deliveryDate))
                            {
                                Console.WriteLine("Ошибка ввода даты возврата книги.");
                                return;
                            }

                            if (readerRepository.RemoveBookFromReader(readerToUpdate, bookNumberDelete, deliveryDate))
                            {
                                Console.WriteLine("Книга успешно возвращена читателем.");
                            }
                            else { Console.WriteLine("Ошибка возврата книги."); }
                            break;
                    }
                }
            }
        }

        public void PerfomReaderSearchUI()
        {
            Console.WriteLine("\n1. Найти читателя по фамилии");
            Console.WriteLine("2. Найти читателя по имени");
            Console.WriteLine("3. Найти читателя по отчеству");
            Console.WriteLine("4. Найти читателя по телефону");
            Console.WriteLine("5. Найти читателя по адресу");
            Console.WriteLine("6. Найти читателя по дате регистрации");
            Console.WriteLine("0. Назад");

            Console.Write("Выберите пункт меню: ");
            string attribute = Console.ReadLine();

            switch (attribute)
            {
                case "1":
                    Console.Write("Введите фамилию читателя: ");
                    string surname = Console.ReadLine();
                    var resultSurname = readerRepository.SearchReaders(surname: surname);
                    DisplayReadersSearch(resultSurname);
                    break;
                case "2":
                    Console.Write("Введите имя читателя: ");
                    string firstName = Console.ReadLine();
                    var resName = readerRepository.SearchReaders(firstName: firstName);
                    DisplayReadersSearch(resName);
                    break;
                case "3":
                    Console.Write("Введите отчество читателя: ");
                    string patronymic = Console.ReadLine();
                    var resPatronymic = readerRepository.SearchReaders(patronymic: patronymic);
                    DisplayReadersSearch(resPatronymic);
                    break;
                case "4":
                    Console.Write("Введите телефон читателя: ");
                    string telephone = Console.ReadLine();
                    var resTelephone = readerRepository.SearchReaders(telephone: telephone);
                    DisplayReadersSearch(resTelephone);
                    break;
                case "5":
                    Console.Write("Введите адрес читателя: ");
                    string address = Console.ReadLine();
                    var resAddress = readerRepository.SearchReaders(address: address);
                    DisplayReadersSearch(resAddress);
                    break;
                case "6":
                    Console.Write("Введите дату регистрации читателя (гггг-мм-дд): ");
                    if (DateTime.TryParse(Console.ReadLine(), out DateTime registrationDate))
                    {
                        var resDate = readerRepository.SearchReaders(registrationDate: registrationDate);
                        DisplayReadersSearch(resDate);
                    }
                    else
                    { Console.WriteLine("Некорректный ввод. Попробуйте еще раз."); }
                    break;
                case "0":
                    return; // Возврат в предыдущее меню
                default:
                    Console.WriteLine("Некорректный выбор. Попробуйте еще раз.");
                    break;
            }
        }

        public void AddAbonementInformationUI()
        {
            // Запрос данных у пользователя для определения читателя
            Console.WriteLine("Введите данные читателя для добавления записи в таблицу Abonement:");

            Console.Write("Фамилия: ");
            var surname = Console.ReadLine();

            Console.Write("Имя: ");
            var firstName = Console.ReadLine();

            Console.Write("Отчество: ");
            var patronymic = Console.ReadLine();

            // Поиск читателя по введенным данным
            var reader = readerRepository.FindReader(surname, firstName, patronymic);

            if (reader == null)
            {
                Console.WriteLine("Читатель не найден.");
                return;
            }

            // Запрос данных у пользователя для добавления записи в таблицу Abonement
            Console.WriteLine("Введите данные для новой записи в таблице Abonement:");

            Console.Write("Номер книги (book_numbers): ");
            if (!int.TryParse(Console.ReadLine(), out var bookNumber))
            {
                Console.WriteLine("Ошибка ввода номера книги.");
                return;
            }

            Console.Write("Дата выдачи книги (гггг-мм-дд): ");
            if (!DateTime.TryParse(Console.ReadLine(), out var bookIssueDate))
            {
                Console.WriteLine("Ошибка ввода даты выдачи книги.");
                return;
            }

            Console.Write("Дата возврата книги (гггг-мм-дд, оставьте пустым, если не возвращено): ");
            DateTime? deliveryDate = null;
            string deliveryDateInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(deliveryDateInput))
            {
                if (!DateTime.TryParse(deliveryDateInput, out var tempDeliveryDate))
                {
                    Console.WriteLine("Ошибка ввода даты возврата книги.");
                    return;
                }

                deliveryDate = tempDeliveryDate;
            }
            if (abonementRepository.AddAbonement(reader, bookNumber, bookIssueDate, deliveryDate))
            {
                Console.WriteLine("Запись успешно добавлена в таблицу Abonement.");
            }
            else { Console.WriteLine("Ошибка добавления записи в таблицу Abonement"); }
        }

        public void DeleteAbonementInformationUI()
        {
            Console.Write("Введите номер книги для удаления: ");
            string input = Console.ReadLine();

            if (int.TryParse(input, out int bookNumberToDelete))
            {
                var hql = "FROM Abonement WHERE BookNumbers = :book_number_delete";
                var query = session.CreateQuery(hql);
                query.SetParameter("book_number_delete", bookNumberToDelete);

                var abonementToDelete = query.UniqueResult<Abonement>();

                if (abonementToDelete == null)
                {
                    Console.WriteLine("Книгу с таким номером никто не брал.");
                    return;
                }

                if (abonementRepository.DeleteAbonement(abonementToDelete, bookNumberToDelete))
                {
                    Console.WriteLine("Запись в таблице Abonement успешно удалены.");
                }
                else { Console.WriteLine("Ошибка при удалении записи в таблице Abonement"); }
            }
            else
            {
                Console.WriteLine("Ошибка ввода. Введите корректное целое число.");
            }

        }

        public void DisplayReadersInformation()
        {
            var hql = "FROM Readers";
            var query = session.CreateQuery(hql);

            var readers = query.List<Readers>();

            Console.WriteLine("\nИнформация о читателях:\n");

            foreach (var reader in readers)
            {
                Console.WriteLine($"Фамилия: {reader.Surname}");
                Console.WriteLine($"Имя: {reader.FirstName}");
                Console.WriteLine($"Отчество: {reader.Patronymic}");
                Console.WriteLine($"Телефон: {reader.Telephone}");
                Console.WriteLine($"Адрес: {reader.Address}");
                Console.WriteLine($"Дата регистрации: {reader.RegistrationDate:yyyy-MM-dd}");

                // Получение количества взятых книг
                var borrowedBooks = readerRepository.GetBorrowedBooksCount(reader.Id);
                Console.WriteLine($"Количество взятых книг: {borrowedBooks}");

                // Получение количества возвращенных книг
                var returnedBooks = readerRepository.GetReturnedBooksCount(reader.Id);
                Console.WriteLine($"Количество возвращенных книг: {returnedBooks}");

                Console.WriteLine();
            }
        }

    }

}
