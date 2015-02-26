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
    public class DataGridViewInvisibleButtonCell : DataGridViewButtonCell
    {
        private bool isVisibleValue;
        public bool IsVisible
        {
            get
            {
                return isVisibleValue;
            }
            set
            {
                isVisibleValue = value;
            }
        }

        // Override the Clone method so that the Enabled property is copied. 
        public override object Clone()
        {
            DataGridViewInvisibleButtonCell cell =
                (DataGridViewInvisibleButtonCell)base.Clone();
            cell.IsVisible = this.IsVisible;
            return cell;
        }

        // By default, enable the button cell. 
        public DataGridViewInvisibleButtonCell()
        {
            this.isVisibleValue = true;
        }

        protected override void Paint(Graphics graphics,
            Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
            DataGridViewElementStates elementState, object value,
            object formattedValue, string errorText,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            // The button cell is invisible, so paint the border and background only            
            if (!this.isVisibleValue)
            {
                // Draw the cell background, if specified
                if ((paintParts & DataGridViewPaintParts.Background) ==
                    DataGridViewPaintParts.Background)
                {
                    SolidBrush cellBackground =
                        new SolidBrush(cellStyle.BackColor);
                    graphics.FillRectangle(cellBackground, cellBounds);
                    cellBackground.Dispose();
                }

                // Draw the cell borders, if specified
                DataGridViewAdvancedBorderStyle newborderStyle = new DataGridViewAdvancedBorderStyle()
                {
                    All  = DataGridViewAdvancedCellBorderStyle.None,
                    Top = advancedBorderStyle.Top,
                    Bottom = advancedBorderStyle.Bottom,
                    Left = DataGridViewAdvancedCellBorderStyle.None,
                    Right = DataGridViewAdvancedCellBorderStyle.None
                };
                if ((paintParts & DataGridViewPaintParts.Border) ==
                    DataGridViewPaintParts.Border)
                {
                    PaintBorder(graphics, clipBounds, cellBounds, cellStyle,
                        newborderStyle);
                }
            }
            else
            {
                // The button cell is enabled, so let the base class  
                // handle the painting
                base.Paint(graphics, clipBounds, cellBounds, rowIndex,
                    elementState, value, formattedValue, errorText,
                    cellStyle, advancedBorderStyle, paintParts);
            }
        }
    }
}
