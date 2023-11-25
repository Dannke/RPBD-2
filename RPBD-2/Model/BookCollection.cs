namespace RPBD_2
{
    public class BookCollection
    {
        public virtual int Id { get; set; }
        public virtual string BookTitle { get; set; }
        public virtual string Authors { get; set; }
        public virtual int YearOfCreation { get; set; }
        public virtual ListCountries PlaceOfCreation { get; set; }
        public virtual ListPublishers PublishingHouse { get; set; }
        public virtual SystematicCatalog UDC { get; set; }
    }
}
