#pragma checksum "..\..\..\..\Views\ConfiguracionMicro.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "F6FA180123CA9353459F9A9B0FD22D16E9BF627AD14D4E0BEEA0C9798BDDF9C2"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

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
    /// ConfiguracionMicro
    /// </summary>
    public partial class ConfiguracionMicro : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 21 "..\..\..\..\Views\ConfiguracionMicro.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cbxSelectedMicro;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\..\Views\ConfiguracionMicro.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnCancelar;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\..\Views\ConfiguracionMicro.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnGuardar;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\..\..\Views\ConfiguracionMicro.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblResult;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\..\..\Views\ConfiguracionMicro.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox chkShow;
        
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
            System.Uri resourceLocater = new System.Uri("/indexai;component/views/configuracionmicro.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\ConfiguracionMicro.xaml"
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
            
            #line 10 "..\..\..\..\Views\ConfiguracionMicro.xaml"
            ((Indexai.ConfiguracionMicro)(target)).LostKeyboardFocus += new System.Windows.Input.KeyboardFocusChangedEventHandler(this.Window_LostKeyboardFocus);
            
            #line default
            #line hidden
            return;
            case 2:
            this.cbxSelectedMicro = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 3:
            this.btnCancelar = ((System.Windows.Controls.Button)(target));
            
            #line 34 "..\..\..\..\Views\ConfiguracionMicro.xaml"
            this.btnCancelar.Click += new System.Windows.RoutedEventHandler(this.BtnCancelar_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnGuardar = ((System.Windows.Controls.Button)(target));
            
            #line 43 "..\..\..\..\Views\ConfiguracionMicro.xaml"
            this.btnGuardar.Click += new System.Windows.RoutedEventHandler(this.BtnGuardar_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.lblResult = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.chkShow = ((System.Windows.Controls.CheckBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

