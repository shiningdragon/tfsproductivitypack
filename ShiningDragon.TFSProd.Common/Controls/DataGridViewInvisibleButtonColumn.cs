using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShiningDragon.TFSProd.Common.Controls
{
    public class DataGridViewInvisibleButtonColumn : DataGridViewButtonColumn
    {
        public DataGridViewInvisibleButtonColumn()
        {
            this.CellTemplate = new DataGridViewInvisibleButtonCell();
        }
    }
}
