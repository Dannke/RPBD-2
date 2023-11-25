using System;
using System.Collections.Generic;


namespace RPBD_2
{
    public class Abonement
    {
        public virtual int Id { get; set; }
        public virtual Readers Reader { get; set; }
        public virtual InventoryList BookNumbers { get; set; }
        public virtual DateTime BookIssueDate { get; set; }
        public virtual DateTime? DeliveryDate { get; set; }
    }
}
