using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.bll
{
    public  class sv_bks_Order
    {

        public static db.view.sv_bks_Order getEntryByRowID(string rowID, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc); dc = ef.dc;

            db.view.sv_bks_Order entry = dc.sv_bks_Order.Single(a => a.rowID == rowID);
            return entry;
        }
    }
}
