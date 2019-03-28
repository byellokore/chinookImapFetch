using System;
using System.Collections.Generic;
using System.Linq;

namespace chinookImapFetch
{
    public class SalesOrderList : List<SaleOrder>
    {
        public SaleOrder GetById(int saleNumber)
        {
            return this.FirstOrDefault(z => z.SaleNumber == saleNumber);
        }
    }
}
