﻿#pragma checksum "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "8C2A31AB26774AB4F2017A05B066A878AB91EAEF58AFB1EE33D965B601FB24AE"
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
using Syncfusion.Windows.PdfViewer;
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
    /// BusquedaVozView
    /// </summary>
    public partial class BusquedaVozView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 29 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Expander consultaExpander;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid viewBusquedaDocumental;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Syncfusion.Windows.Controls.Input.SfTextBoxExt documentosFilter;
        
        #line default
        #line hidden
        
        
        #line 85 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnConsultarDocumental;
        
        #line default
        #line hidden
        
        
        #line 95 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox chk_Solicitantes;
        
        #line default
        #line hidden
        
        
        #line 102 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox chk_Titulares;
        
        #line default
        #line hidden
        
        
        #line 149 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txt_lugarExpedicion;
        
        #line default
        #line hidden
        
        
        #line 170 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txt_Lote;
        
        #line default
        #line hidden
        
        
        #line 191 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txt_subserie;
        
        #line default
        #line hidden
        
        
        #line 212 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txt_subdependencia;
        
        #line default
        #line hidden
        
        
        #line 233 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txt_carpeta_parse;
        
        #line default
        #line hidden
        
        
        #line 254 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txt_PrimerApellido;
        
        #line default
        #line hidden
        
        
        #line 275 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txt_Nombre;
        
        #line default
        #line hidden
        
        
        #line 296 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txt_CodCaja_parse;
        
        #line default
        #line hidden
        
        
        #line 319 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txt_noIndetificacion_numerico;
        
        #line default
        #line hidden
        
        
        #line 361 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnLimpiar;
        
        #line default
        #line hidden
        
        
        #line 374 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Expander expanderResultados;
        
        #line default
        #line hidden
        
        
        #line 389 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lbxPdfImages;
        
        #line default
        #line hidden
        
        
        #line 417 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Syncfusion.Windows.Tools.Controls.TabControlExt controlTabConsulta;
        
        #line default
        #line hidden
        
        
        #line 434 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Syncfusion.UI.Xaml.Grid.SfDataGrid gridBusqueda;
        
        #line default
        #line hidden
        
        
        #line 485 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Syncfusion.Windows.PdfViewer.PdfViewerControl pdfviewer;
        
        #line default
        #line hidden
        
        
        #line 490 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnAbrir;
        
        #line default
        #line hidden
        
        
        #line 500 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnExportar;
        
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
            System.Uri resourceLocater = new System.Uri("/indexai;component/views/indexador/busqueda/busquedavozview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
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
            this.consultaExpander = ((System.Windows.Controls.Expander)(target));
            return;
            case 2:
            this.viewBusquedaDocumental = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.documentosFilter = ((Syncfusion.Windows.Controls.Input.SfTextBoxExt)(target));
            return;
            case 4:
            this.btnConsultarDocumental = ((System.Windows.Controls.Button)(target));
            
            #line 92 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
            this.btnConsultarDocumental.Click += new System.Windows.RoutedEventHandler(this.btnConsultarDocumental_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.chk_Solicitantes = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 6:
            this.chk_Titulares = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 7:
            this.txt_lugarExpedicion = ((System.Windows.Controls.TextBox)(target));
            
            #line 153 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
            this.txt_lugarExpedicion.GotFocus += new System.Windows.RoutedEventHandler(this.GotFocus);
            
            #line default
            #line hidden
            
            #line 154 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
            this.txt_lugarExpedicion.LostFocus += new System.Windows.RoutedEventHandler(this.LostFocus);
            
            #line default
            #line hidden
            return;
            case 8:
            this.txt_Lote = ((System.Windows.Controls.TextBox)(target));
            
            #line 174 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
            this.txt_Lote.GotFocus += new System.Windows.RoutedEventHandler(this.GotFocus);
            
            #line default
            #line hidden
            
            #line 175 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
            this.txt_Lote.LostFocus += new System.Windows.RoutedEventHandler(this.LostFocus);
            
            #line default
            #line hidden
            return;
            case 9:
            this.txt_subserie = ((System.Windows.Controls.TextBox)(target));
            
            #line 195 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
            this.txt_subserie.GotFocus += new System.Windows.RoutedEventHandler(this.GotFocus);
            
            #line default
            #line hidden
            
            #line 196 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
            this.txt_subserie.LostFocus += new System.Windows.RoutedEventHandler(this.LostFocus);
            
            #line default
            #line hidden
            return;
            case 10:
            this.txt_subdependencia = ((System.Windows.Controls.TextBox)(target));
            
            #line 216 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
            this.txt_subdependencia.GotFocus += new System.Windows.RoutedEventHandler(this.GotFocus);
            
            #line default
            #line hidden
            
            #line 217 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
            this.txt_subdependencia.LostFocus += new System.Windows.RoutedEventHandler(this.LostFocus);
            
            #line default
            #line hidden
            return;
            case 11:
            this.txt_carpeta_parse = ((System.Windows.Controls.TextBox)(target));
            
            #line 237 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
            this.txt_carpeta_parse.GotFocus += new System.Windows.RoutedEventHandler(this.GotFocus);
            
            #line default
            #line hidden
            
            #line 238 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
            this.txt_carpeta_parse.LostFocus += new System.Windows.RoutedEventHandler(this.LostFocus);
            
            #line default
            #line hidden
            return;
            case 12:
            this.txt_PrimerApellido = ((System.Windows.Controls.TextBox)(target));
            
            #line 258 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
            this.txt_PrimerApellido.GotFocus += new System.Windows.RoutedEventHandler(this.GotFocus);
            
            #line default
            #line hidden
            
            #line 259 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
            this.txt_PrimerApellido.LostFocus += new System.Windows.RoutedEventHandler(this.LostFocus);
            
            #line default
            #line hidden
            return;
            case 13:
            this.txt_Nombre = ((System.Windows.Controls.TextBox)(target));
            
            #line 279 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
            this.txt_Nombre.GotFocus += new System.Windows.RoutedEventHandler(this.GotFocus);
            
            #line default
            #line hidden
            
            #line 280 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
            this.txt_Nombre.LostFocus += new System.Windows.RoutedEventHandler(this.LostFocus);
            
            #line default
            #line hidden
            return;
            case 14:
            this.txt_CodCaja_parse = ((System.Windows.Controls.TextBox)(target));
            
            #line 302 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
            this.txt_CodCaja_parse.GotFocus += new System.Windows.RoutedEventHandler(this.GotFocus);
            
            #line default
            #line hidden
            
            #line 303 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
            this.txt_CodCaja_parse.LostFocus += new System.Windows.RoutedEventHandler(this.LostFocus);
            
            #line default
            #line hidden
            return;
            case 15:
            this.txt_noIndetificacion_numerico = ((System.Windows.Controls.TextBox)(target));
            
            #line 325 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
            this.txt_noIndetificacion_numerico.GotFocus += new System.Windows.RoutedEventHandler(this.GotFocus);
            
            #line default
            #line hidden
            
            #line 326 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
            this.txt_noIndetificacion_numerico.LostFocus += new System.Windows.RoutedEventHandler(this.LostFocus);
            
            #line default
            #line hidden
            return;
            case 16:
            this.btnLimpiar = ((System.Windows.Controls.Button)(target));
            
            #line 368 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
            this.btnLimpiar.Click += new System.Windows.RoutedEventHandler(this.btnLimpiar_Click);
            
            #line default
            #line hidden
            return;
            case 17:
            this.expanderResultados = ((System.Windows.Controls.Expander)(target));
            return;
            case 18:
            this.lbxPdfImages = ((System.Windows.Controls.ListView)(target));
            return;
            case 19:
            this.controlTabConsulta = ((Syncfusion.Windows.Tools.Controls.TabControlExt)(target));
            return;
            case 20:
            this.gridBusqueda = ((Syncfusion.UI.Xaml.Grid.SfDataGrid)(target));
            return;
            case 21:
            this.pdfviewer = ((Syncfusion.Windows.PdfViewer.PdfViewerControl)(target));
            return;
            case 22:
            this.btnAbrir = ((System.Windows.Controls.Button)(target));
            return;
            case 23:
            this.btnExportar = ((System.Windows.Controls.Button)(target));
            
            #line 507 "..\..\..\..\..\..\Views\Indexador\Busqueda\BusquedaVozView.xaml"
            this.btnExportar.Click += new System.Windows.RoutedEventHandler(this.btnExportar_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
