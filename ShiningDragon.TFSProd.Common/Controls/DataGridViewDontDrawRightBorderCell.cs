using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace ShiningDragon.TFSProd.Common.Controls
{
    public class DataGridViewDontDrawRightBorderCell : DataGridViewTextBoxCell
    {
        // By default, enable the button cell. 
        public DataGridViewDontDrawRightBorderCell()
        {

        }

        protected override void Paint(Graphics graphics,
            Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
            DataGridViewElementStates elementState, object value,
            object formattedValue, string errorText,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            DataGridViewAdvancedBorderStyle newborderStyle = new DataGridViewAdvancedBorderStyle()
            {
                All = DataGridViewAdvancedCellBorderStyle.None,
                Top = advancedBorderStyle.Top,
                Bottom = advancedBorderStyle.Bottom,
                Left = advancedBorderStyle.Left,
                Right = DataGridViewAdvancedCellBorderStyle.None
            };
 
            base.Paint(graphics, clipBounds, cellBounds, rowIndex,
                elementState, value, formattedValue, errorText,
                cellStyle, newborderStyle, paintParts);

        }
    }
}
