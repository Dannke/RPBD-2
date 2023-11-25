namespace RPBD_2
{
    public class InventoryList
    {
        public virtual int Id { get; set; }
        public virtual BookCollection Book { get; set; }
        public virtual int BookNumber { get; set; }
    }
}
