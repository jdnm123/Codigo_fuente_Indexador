using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexai
{
    public class CustomGridCopyPaste : GridCutCopyPaste
    {
        public CustomGridCopyPaste(SfDataGrid dataGrid):base(dataGrid)
        {

        }
        protected override void CopyCell(object record, GridColumn column, ref StringBuilder text)
        {
            base.CopyCell(record, column, ref text);
            text.Replace("\t", string.Empty);
        }
    }
}
