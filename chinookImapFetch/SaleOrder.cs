using System;
using System.Collections.Generic;

namespace chinookImapFetch
{
    public class SaleOrder
    {
        public int SaleNumber { get; set; }
        public string BillLading { get; set; }
        public string ContainerNumber { get; set; }
        public List<string> Attachments { get; set; }

        public SaleOrder(int salenumber, string bol, string container)
        {
            SaleNumber = salenumber;
            BillLading = bol;
            ContainerNumber = container;
            Attachments = new List<string>();
        }

        public override int GetHashCode()
        {
            return this.SaleNumber;
        }

        public override bool Equals(object obj)
        {   
            if (obj == null || GetType() != obj.GetType())
                return false;

            SaleOrder other = obj as SaleOrder;
            return this.SaleNumber == other.SaleNumber;
        }


    }
}
