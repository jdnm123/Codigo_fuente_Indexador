﻿#pragma checksum "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "4FBCFFC94C4A13DAE045670DDED0EC4BC4C7CFDEA0BBFC50A81A82498B736621"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Converters;
using MaterialDesignThemes.Wpf.Transitions;
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


namespace Indexai {
    
    
    /// <summary>
    /// BuscadorLotes
    /// </summary>
    public partial class BuscadorLotes : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 20 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtNomLote;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtNumCaja_parse;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtCodCarpeta_parse;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtExpediente_parse;
        
        #line default
        #line hidden
        
        
        #line 76 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtUsuario;
        
        #line default
        #line hidden
        
        
        #line 91 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtNroCarpeta;
        
        #line default
        #line hidden
        
        
        #line 106 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtRangoMin;
        
        #line default
        #line hidden
        
        
        #line 121 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtRangoMax;
        
        #line default
        #line hidden
        
        
        #line 137 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Syncfusion.Windows.Controls.Input.SfDatePicker datePckFechaIndexado;
        
        #line default
        #line hidden
        
        
        #line 151 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnBuscar;
        
        #line default
        #line hidden
        
        
        #line 158 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnLimpiar;
        
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
            System.Uri resourceLocater = new System.Uri("/indexai;component/views/indexador/busqueda/buscadorlotes.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
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
            this.txtNomLote = ((System.Windows.Controls.TextBox)(target));
            
            #line 24 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
            this.txtNomLote.GotFocus += new System.Windows.RoutedEventHandler(this.GotFocus);
            
            #line default
            #line hidden
            
            #line 25 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
            this.txtNomLote.LostFocus += new System.Windows.RoutedEventHandler(this.LostFocus);
            
            #line default
            #line hidden
            return;
            case 2:
            this.txtNumCaja_parse = ((System.Windows.Controls.TextBox)(target));
            
            #line 38 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
            this.txtNumCaja_parse.GotFocus += new System.Windows.RoutedEventHandler(this.GotFocus);
            
            #line default
            #line hidden
            
            #line 39 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
            this.txtNumCaja_parse.LostFocus += new System.Windows.RoutedEventHandler(this.LostFocus);
            
            #line default
            #line hidden
            return;
            case 3:
            this.txtCodCarpeta_parse = ((System.Windows.Controls.TextBox)(target));
            
            #line 52 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
            this.txtCodCarpeta_parse.GotFocus += new System.Windows.RoutedEventHandler(this.GotFocus);
            
            #line default
            #line hidden
            
            #line 53 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
            this.txtCodCarpeta_parse.LostFocus += new System.Windows.RoutedEventHandler(this.LostFocus);
            
            #line default
            #line hidden
            return;
            case 4:
            this.txtExpediente_parse = ((System.Windows.Controls.TextBox)(target));
            
            #line 66 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
            this.txtExpediente_parse.GotFocus += new System.Windows.RoutedEventHandler(this.GotFocus);
            
            #line default
            #line hidden
            
            #line 67 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
            this.txtExpediente_parse.LostFocus += new System.Windows.RoutedEventHandler(this.LostFocus);
            
            #line default
            #line hidden
            return;
            case 5:
            this.txtUsuario = ((System.Windows.Controls.TextBox)(target));
            
            #line 80 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
            this.txtUsuario.GotFocus += new System.Windows.RoutedEventHandler(this.GotFocus);
            
            #line default
            #line hidden
            
            #line 81 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
            this.txtUsuario.LostFocus += new System.Windows.RoutedEventHandler(this.LostFocus);
            
            #line default
            #line hidden
            return;
            case 6:
            this.txtNroCarpeta = ((System.Windows.Controls.TextBox)(target));
            
            #line 95 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
            this.txtNroCarpeta.GotFocus += new System.Windows.RoutedEventHandler(this.GotFocus);
            
            #line default
            #line hidden
            
            #line 96 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
            this.txtNroCarpeta.LostFocus += new System.Windows.RoutedEventHandler(this.LostFocus);
            
            #line default
            #line hidden
            return;
            case 7:
            this.txtRangoMin = ((System.Windows.Controls.TextBox)(target));
            
            #line 110 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
            this.txtRangoMin.GotFocus += new System.Windows.RoutedEventHandler(this.GotFocus);
            
            #line default
            #line hidden
            
            #line 111 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
            this.txtRangoMin.LostFocus += new System.Windows.RoutedEventHandler(this.LostFocus);
            
            #line default
            #line hidden
            return;
            case 8:
            this.txtRangoMax = ((System.Windows.Controls.TextBox)(target));
            
            #line 125 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
            this.txtRangoMax.GotFocus += new System.Windows.RoutedEventHandler(this.GotFocus);
            
            #line default
            #line hidden
            
            #line 126 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
            this.txtRangoMax.LostFocus += new System.Windows.RoutedEventHandler(this.LostFocus);
            
            #line default
            #line hidden
            return;
            case 9:
            this.datePckFechaIndexado = ((Syncfusion.Windows.Controls.Input.SfDatePicker)(target));
            
            #line 141 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
            this.datePckFechaIndexado.GotFocus += new System.Windows.RoutedEventHandler(this.GotFocus);
            
            #line default
            #line hidden
            
            #line 142 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
            this.datePckFechaIndexado.LostFocus += new System.Windows.RoutedEventHandler(this.LostFocus);
            
            #line default
            #line hidden
            return;
            case 10:
            this.btnBuscar = ((System.Windows.Controls.Button)(target));
            
            #line 155 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
            this.btnBuscar.Click += new System.Windows.RoutedEventHandler(this.btnBuscar_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.btnLimpiar = ((System.Windows.Controls.Button)(target));
            
            #line 163 "..\..\..\..\..\Views\Indexador\Busqueda\BuscadorLotes.xaml"
            this.btnLimpiar.Click += new System.Windows.RoutedEventHandler(this.btnLimpiar_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
