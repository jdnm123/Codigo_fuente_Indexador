﻿#pragma checksum "..\..\..\..\..\Views\Indexador\Admin\ProcesoFirma.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "3D9370E32F4F9F4C4D475CE9D86F633E40C0A7D7A80CB0BCA0597B9C80369CDE"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using Syncfusion;
using Syncfusion.UI.Xaml.Controls.DataPager;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.UI.Xaml.Grid.RowFilter;
using Syncfusion.UI.Xaml.ImageEditor;
using Syncfusion.UI.Xaml.TreeGrid;
using Syncfusion.UI.Xaml.TreeGrid.Filtering;
using Syncfusion.Windows;
using Syncfusion.Windows.Controls.Input;
using Syncfusion.Windows.Controls.Notification;
using Syncfusion.Windows.Data;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Indexai.Views {
    
    
    /// <summary>
    /// ProcesoFirma
    /// </summary>
    public partial class ProcesoFirma : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 23 "..\..\..\..\..\Views\Indexador\Admin\ProcesoFirma.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Syncfusion.UI.Xaml.Grid.SfDataGrid exportGrid;
        
        #line default
        #line hidden
        
        
        #line 75 "..\..\..\..\..\Views\Indexador\Admin\ProcesoFirma.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label txtGridTotalitems;
        
        #line default
        #line hidden
        
        
        #line 91 "..\..\..\..\..\Views\Indexador\Admin\ProcesoFirma.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Syncfusion.UI.Xaml.Controls.DataPager.SfDataPager exportPager;
        
        #line default
        #line hidden
        
        
        #line 101 "..\..\..\..\..\Views\Indexador\Admin\ProcesoFirma.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtItemCount;
        
        #line default
        #line hidden
        
        
        #line 121 "..\..\..\..\..\Views\Indexador\Admin\ProcesoFirma.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnMaximoItemsGrid;
        
        #line default
        #line hidden
        
        
        #line 134 "..\..\..\..\..\Views\Indexador\Admin\ProcesoFirma.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnProcesarFirma;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/indexai;component/views/indexador/admin/procesofirma.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Views\Indexador\Admin\ProcesoFirma.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.exportGrid = ((Syncfusion.UI.Xaml.Grid.SfDataGrid)(target));
            return;
            case 2:
            this.txtGridTotalitems = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.exportPager = ((Syncfusion.UI.Xaml.Controls.DataPager.SfDataPager)(target));
            return;
            case 4:
            this.txtItemCount = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.btnMaximoItemsGrid = ((System.Windows.Controls.Button)(target));
            
            #line 129 "..\..\..\..\..\Views\Indexador\Admin\ProcesoFirma.xaml"
            this.btnMaximoItemsGrid.Click += new System.Windows.RoutedEventHandler(this.btnMaximoItemsGrid_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.btnProcesarFirma = ((System.Windows.Controls.Button)(target));
            
            #line 141 "..\..\..\..\..\Views\Indexador\Admin\ProcesoFirma.xaml"
            this.btnProcesarFirma.Click += new System.Windows.RoutedEventHandler(this.btnProcesarFirma_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
