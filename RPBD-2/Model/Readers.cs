using System;

namespace RPBD_2
{
    public class Readers
    {
        public virtual int Id { get; set; }
        public virtual string Surname { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string Patronymic { get; set; }
        public virtual string Telephone { get; set; }
        public virtual string Address { get; set; }
        public virtual DateTime RegistrationDate { get; set; }
    }
}
