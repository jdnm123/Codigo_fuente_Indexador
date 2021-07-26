using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xaml.Behaviors;

namespace Indexai
{
    public class SfDataGridBehavior : Behavior<SfDataGrid>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.GridCopyPaste = new CustomGridCopyPaste(this.AssociatedObject);
            this.AssociatedObject.CopyGridCellContent += AssociatedObject_CopyGridCellContent;
        }

        private void AssociatedObject_CopyGridCellContent(object sender, GridCopyPasteCellEventArgs e)
        {
            //Skip to copy contents for all the inactive cells from the selected row 
            SfDataGrid grid = e.OriginalSender is DetailsViewDataGrid ? (SfDataGrid)e.OriginalSender : (SfDataGrid)sender;
            if (grid != null && grid.SelectionController != null
                && grid.SelectionController.CurrentCellManager != null
                && grid.SelectionController.CurrentCellManager.CurrentCell != null
                && e.Column.MappingName != grid.SelectionController.CurrentCellManager.CurrentCell.GridColumn.MappingName)
            {
                e.Handled = true;
            }
        }
    }
}
