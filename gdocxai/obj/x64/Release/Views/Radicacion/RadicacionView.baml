   M S B A M L   `   `   `  ���� d  `Syncfusion.PdfViewer.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89mLclr-namespace:Syncfusion.Windows.PdfViewer;assembly=Syncfusion.PdfViewer.WPFSyncfusion.Windows.PdfViewer  O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem B >indexai, Version=2.4.6.3, Culture=neutral, PublicKeyToken=null!clr-namespace:IndexaiIndexai "   Indexai.Views.RadicacionView   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     b	PdfViewerLclr-namespace:Syncfusion.Windows.PdfViewer;assembly=Syncfusion.PdfViewer.WPF     j fSyncfusion.SfGridConverter.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfBusyIndicator.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89h	 dSyncfusion.SfImageEditor.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89`
 \Syncfusion.Linq.Base, Version=18.4460.0.49, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a ]Syncfusion.SfGrid.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a ]Syncfusion.Shared.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Tools.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.SfInput.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89@
Syncfusion!http://schemas.syncfusion.com/wpf   	 
     Hd2http://schemas.microsoft.com/expression/blend/2008   	 
     b ^MaterialDesignThemes.Wpf, Version=4.0.0.2422, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  3system&clr-namespace:System;assembly=mscorlib  !utilsclr-namespace:Indexai  $	��760��  � 	Resources   �� MergedDictionaries+  %MaterialDesignThemes.Wpf.BundledTheme   	BaseTheme   PrimaryColor   SecondaryColor �� Source��    $ LightX�$ 
DeepPurpleX�$ PurpleX��� $h bpack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml?�%    .��� -    �Name$
 root��$��0,0,0,0q�.+�� .+�)  #MaterialDesignThemes.Wpf.DialogHost     MaterialDesignDivider#���      DialogContent � $	��500��$	��500��$��Center=�.+��  � ColumnDefinitions �� $
��0.1* ��� $��* �	 � RowDefinitions	 �� $��80 ��� $��* �.+�'  !MaterialDesignThemes.Wpf.PackIcon  $��40��$��40��$��Center=�$��Center=�  PrimaryHueMidBrush#��[ 
   Kind$
 UserAdd=��� $��1��$��Center=�$��Center=� $  MaterialDesignHeadline6TextBlock#��[ $
��Wrap=�CREAR REMITENTE� $��1��$��1��$��0,0,60,0q� � ShowGridLines .  �� $��1* ��� $��1* �	 �� $��50 ��� $��6* ��� $��6* ��� $��6* ��� $��6* ��� $��6* ��� $��6* �.+���  �� Orientation$ 
Horizontal=�.+�  $��25��$��25��$��	0,20,12,0q�$	��Top=�$
 	AlphaNBox=�����  �� ElementName$ NameTextBox�� �� Path$ BorderBrush��� -   $��txtInputRemNombre��$	��150��$	��Top=�)  #MaterialDesignThemes.Wpf.HintAssist   Hint$ Nombres��   
IsFloating . .������ $ Nombre del remitente� �� UpdateSourceTrigger$ PropertyChanged=�.���� $��2��$ 
Horizontal=�.+�  $��25��$��25��$��	0,20,12,0q�$	��Top=�$
 	AlphaABox=����� $ NameTextBox��$ BorderBrush��� -   $��txtInputRemApellido��$	��150��$	��Top=�$ 	Apellidos�� . .������ $ Apellido del remitente�$ PropertyChanged=�.���� $��1��$��0��$ 
Horizontal=�.+�  $��25��$��25��$��	0,20,12,0q�$	��Top=�$
 IdCard=����� $ NameTextBox��$ BorderBrush��� -   $��selectTipoDocumento��$	��150��.@���  �� 
IsSelected . .��Tipo de documento�� .��Cédula de Ciudadanía�� .��	Pasaporte�� $��1��$��1��$ 
Horizontal=�.+�  $��25��$��25��$��	0,20,12,0q�$	��Top=�$
 IdCard=����� $ NameTextBox��$ BorderBrush��� -   $��txtInputRemIdentificacion��$	��150��$	��Top=�$ Identificación�� . .������ $# Identificación del remitente�$ PropertyChanged=�.���� $��2��$��0��$ 
Horizontal=�.+�  $��25��$��25��$��	0,20,12,0q�$	��Top=�$
 	ChessKing=����� $ NameTextBox��$ BorderBrush��� -   $��selectTipoTercero��$	��150�� . .@��� $	��150�� . .��Tipo de remitente�� .��	PERSONA�� .��
EMPLEADO�� .��	USUARIO�� $��2��$��1��$ 
Horizontal=�.+�  $��25��$��25��$��	0,20,12,0q�$	��Top=�$
 CardAccountDetails=����� $ NameTextBox��$ BorderBrush��� -   $��selectLugarExp��$	��150�� . .@��� $	��150�� . .��Lugar de Expedición�� .��
Bogota D.C�� .��Cali�� .��	Medellín�� $��3��$��0��$ 
Horizontal=�.+�  $��25��$��25��$��	0,20,12,0q�$	��Top=�$
 Phone=����� $ NameTextBox��$ BorderBrush��� -   $��txtInputRemTelefono��$	��150��$	��Top=�$ 	Teléfono�� . .������ $ Teléfono del remitente�$ PropertyChanged=�.���� $��3��$��1��$ 
Horizontal=�.+�  $��25��$��25��$��	0,20,12,0q�$	��Top=�$
 MapAdd=����� $ NameTextBox��$ BorderBrush��� -	   $��txtInputRemDireccion��$	��150��$	��Top=�$ 
Dirección�� . .������ $ Dirección del remitente�$ PropertyChanged=�.���� $��4��$��0��$ 
Horizontal=�.+�  $��25��$��25��$��	0,20,12,0q�$	��Top=�$
 At=����� $ NameTextBox��$ BorderBrush��� -
   $��txtInputRemEmail��$	��150��$	��Top=�$ Email�� . .������ $ Email del remitente�$ PropertyChanged=�.���� $��4��$��1��$ 
Horizontal=�.+�  $��25��$��25��$��	0,20,12,0q�$	��Top=�$
 BriefcaseOutline=����� $ NameTextBox��$ BorderBrush��� -   $��selectRemCargo��$	��150��.@���  . .��Cargo�� -   $��btnNuevoGuardarRemitente��$��7��$��4q�$��Center=�   CloseDialogCommand#��Z   MaterialDesignFlatButton#���  .��	GUARDAR�� $��7��$��2��$��4q�$��Center=�#��Z #���  .��
CANCELAR.��� .+��  �� $	��270 ��� .+��� $��2,0,0,2q����� -   $��txtRemitente��$	��250��$��	0,0,0.4,0q�$��Right=�$
��Bold"�
�������$��	Remitente��.��� $��1��$��0,10,0,0q�.+��� $ Vertical=�.+��� $ 
Horizontal=�.+�  $��0,0,12,0q�$��Bottom=�$

 User=����� $ NameTextBox��$ BorderBrush��� -   $��selectUsuarioAsignado��$	��200��$ Seleccionar..��   HintOpacity$	 .26Q� .  �� 
IsEditable . �� $��50,40,10,10q�$ 
Horizontal=�.+��� .+��� $	��120��$��0,0,0,0q�   OpenDialogCommand#��Z $��12(� " MaterialDesignRaisedDarkButton#��[  � ToolTip$ Agregar Usuario��.��Agregar usuario�� $��	10,20,0,0q�$ 
Horizontal=�.+�&   MaterialDesignThemes.Wpf.Flipper     FrontContent � $	��220��$
��Auto��$��0,40,0,0q�	 �� $
��Auto ��� $��* �.+�(  "MaterialDesignThemes.Wpf.ColorZone  $��Stretch=�   Mode$ PrimaryLight=�.��  $	��128��$	��128��$��Center=�$��Center=�$
 AccountCircle=��� $��1��$��Center=�$��Center=�.+��� $��0,20,0,0q�.+�#  MaterialDesignThemes.Wpf.Chip  -   $��chipNombreUsuario��$��Junior Parra Gamarra��$��10(�   Icon$ JP���� $��	0,20,0,20q�$��Center=�.+�  -   $��chipTipoUsuario��$��Tipo Usuario��$��10(�$ T���� .+��� $��0,4,0,0q�   FlipCommand#��Z #��[ .��EDITAR   BackContent � $	��220��$
��Auto��	 �� $
��Auto ��� $��* �.+�  $��6q�$ PrimaryLight=�.���� $ 
Horizontal=�.+��� $
��Left=�#��Z  & "MaterialDesignToolForegroundButton#��[ .��  $��Right=�$
 	ArrowLeft=��� $��8,0,0,0q�$��Center=�Información� $��1��$	��172��$��0,6,0,0q�$��Center=�$	��Top=�	 �� �� �� �� .+��� -   $��txtTelefono��$��0,12,0,0q�$ Telefono�� . �� -   $��txtDireccion��$��1��$��0,12,0,0q�$ 
Dirección�� . �� $��2��$	��172��$��0,6,0,0q�$��Center=�$	��Top=�.+��� -   $��selectTipoUsuario��$��1��$��0,12,0,0q�$ Tipo de Usuario��$	 .26Q� . .@��� .��Funcionario�� .��
Dignatario�� -   $��txtEmail��$��2��$��0,12,0,0q�$ Email�� . �� $��3��$	��172��$��0,6,0,0q�$��Center=�$	��Top=�.+��� -   $��selectDepartamento��$��1��$��0,12,0,0q�$ Departamento��$	 .26Q� . �� -   $��selectCiudad��$��1��$��0,12,0,0q�$ Ciudad��$	 .26Q� . �� -   $��btnActualizarRemitente��$��0,12,0,0q�$ Actualizar Usuario����0  *MaterialDesignThemes.Wpf.PackIconExtension  *ContentSave+� $��	272,0,2,2q�.+��  �� $
��1.5* ��� $
��0.5* �.+��� $��0q�$
��Bold"�$��Radicado��.��� $��	20,0,0,10q� �� $��* ��� $��* �	 �� $��* ��� $��2* ��� $��* ��� $��* ��� $��* ��� $��* ��� $
��3.5* ��� $
��1.5* �.+��� $��Center=�$��Center=�$ 
Horizontal=�.+�  $��25��$��25��$��	0,20,12,0q�$	��Top=�$
 AndroidMessages=����� $" selectAsuntosPredeterminados��$ BorderBrush��� -   $"��selectAsuntosPredeterminados��$	��230��$��0,15,0,0q�$ Asuntos Predeterminados��$	 .26Q� . .@��� .��Exoneración de Cuentas�� $��1��$��Center=�$��Center=�$ 
Horizontal=�.+�  $��25��$��25��$��	0,25,12,0q�$	��Top=�$
 TextSubject=����� $ txtBoxDetalleAsunto��$ BorderBrush��� -   $��txtBoxDetalleAsunto��$	��230��$��50��$��	0,15,12,0q�$	��Top=�$ Detalle Asunto�� �� AcceptsReturn .  * &MaterialDesignOutlinedTextFieldTextBox#��[  �� TextWrapping$
 Wrap=�" �� VerticalScrollBarVisibility$
 Auto=��� $��2��$��Center=�$��Center=�$ 
Horizontal=�.+�  $��25��$��25��$��	0,20,12,0q�$	��Top=�$
 TruckDelivery=����� $ selectMedioRecepcion��$ BorderBrush��� -   $��selectMedioRecepcion��$	��230��$��0,15,0,0q�$ Medio de Recepción��$	 .26Q� . .@��� .��Fisico�� .��	Digital�� $��0��$��1��$��Center=�$��Center=�$ 
Horizontal=�.+�  $��25��$��25��$��	0,20,12,0q�$	��Top=�$
 
Attachment=����� $ txtRefDesAnexos��$ BorderBrush��� -   $��txtRefDesAnexos��$	��230��$ Descripción de anexos�� . .������ $ Descripción de anexos�$ PropertyChanged=�.���� $��1��$��1��$��Center=�$��Center=�$ 
Horizontal=�.+�  $��25��$��25��$��	0,20,12,0q�$	��Top=�$
 	FaceAgent=����� $ textBoxReferenciaCliente��$ BorderBrush��� -   $��textBoxReferenciaCliente��$	��230��$��50��$��	0,15,12,0q�$	��Top=�$) #Referencia radicado cliente externo�� . #��[ $
 Wrap=�$
 Auto=��� $��2��$��1��$��Center=�$��Center=�$ 
Horizontal=�.+�  $��25��$��25��$��0,20,0,0q�$	��Top=�$
 	DateRange=����� $ calendarFechaVencimiento��$ BorderBrush�(	  "System.Windows.Controls.DatePicker	  -   $��calendarFechaVencimiento��$	��230��$��	15,20,0,0q�$ Fecha de Vencimento��  	  SelectedDate  �� $ FutureValidatingDate�$ PropertyChanged=��� $��3��$��0��$��Center=�$��Center=�$ 
Horizontal=�.+�  $��25��$��25��$��	0,20,12,0q�$	��Top=�$
 Counter=����� $ selectSerie��$ BorderBrush��� -   $��selectSerie��$	��230��$��0,15,0,0q�$ Serie��$	 .26Q� . �� $��4��$��0��$��Center=�$��Center=�$ 
Horizontal=�.+�  $��25��$��25��$��	0,20,12,0q�$	��Top=�$
 ScatterPlot=����� $ selectTDocumental��$ BorderBrush��� -   $��selectTDocumental��$	��230��$��0,15,0,0q�$ Tipo Documental��$	 .26Q� . �� $��5��$��0��$��Center=�$��Center=�$ 
Horizontal=�.+�  $��25��$��25��$��	0,20,12,0q�$	��Top=�$
 MapMarkerDistance=����� $ selectRegional��$ BorderBrush��� -    $��selectRegional��$	��230��$��0,15,0,0q�$ Regional��$	 .26Q� . .@��� .��CARIBE�� $��3��$��1��$��Center=�$��Center=�$ 
Horizontal=�.+�  $��25��$��25��$��	0,20,12,0q�$	��Top=�$ 
 Numeric2BoxMultipleOutline=����� $ selectSubSerie��$ BorderBrush��� -!   $��selectSubSerie��$	��230��$��0,15,0,0q�$ SubSerie��$	 .26Q� . �� $��4��$��1��$��Center=�$��Center=�$ 
Horizontal=�.+�  $��25��$��25��$��	0,20,12,0q�$	��Top=�$
 Calendar=����� $ txtDiasTermino��$ BorderBrush��� -"   $��txtDiasTermino��$	��230��$ Dias de termino�� . .������ $ Dias de termino�$ PropertyChanged=�.���� $��5��$��1��$��Center=�$��Center=�$ 
Horizontal=�.+�  $��25��$��25��$��	0,20,12,0q�$	��Top=�$
 Buffer=����� $ selectClasificacion��$ BorderBrush��� -#   $��selectClasificacion��$	��230��$��0,15,0,0q�$ Clasificación��$	 .26Q� . .@��� .��
FÁBRICA�� $��6��$��0��$��Center=�$��Center=�$ 
Horizontal=�.+�  $��Center=�$��Center=�!   DialogTheme$! Inherit=� �� $��16q�.+��� Copiar radicado a:�� -$   $��selectCopiarUsuarios��$	��230��$��0,15,0,0q�$ Usuarios��$	 .26Q� . �� $��Right=�$ 
Horizontal=�.+��� $��0,8,8,0q�$2��,materialDesign:DialogHost.CloseDialogCommand��" �� 	IsDefault" . #��[ ��1&clr-namespace:System;assembly=mscorlibSystem ��3system&clr-namespace:System;assembly=mscorlib  True��.��	Aceptar�� $��0,8,8,0q�$2��,materialDesign:DialogHost.CloseDialogCommand��# �� IsCancel# . #��[ ��1&clr-namespace:System;assembly=mscorlibSystem ��3system&clr-namespace:System;assembly=mscorlib  	False��.��
Cancelar.���� $	��270��$	��130��#���  $��1q�}�. ._�� 	 �� $��* ��� $
��Auto �.+��� -%   $��listBoxUsuarios��  $��1��  MaterialDesignShadowDepth5$ J� Effect#$ �  $ 
PrimaryMid=�.���� $��10q�Agregar usuarios�� $��0��$��
0,0,28,-20q�$��Right=�$��Bottom=�#��Z  0 ,MaterialDesignFloatingActionMiniAccentButton#��[ .��  $��22��$��22��$

 Plus=�� $��6��$��1��	 �� $��* ��� $��* �.+��� $��Center=�$��Center=�$ 
Horizontal=�.+�  $��25��$��25��$��	0,20,12,0q�$	��Top=�$
 BriefcaseAccountOutline=����� $ NameTextBox��$ BorderBrush��� -&   $"��selectDependenciaResponsable��$	��230��$��0,15,0,0q�$ Dependencia Responsable��$	 .26Q� . �� $��1��$��Center=�$��Center=�$ 
Horizontal=�.+�  $��25��$��25��$��	0,20,12,0q�$	��Top=�$
 AccountSupervisorCircle=����� $ selectUsuarioResponsable��$ BorderBrush��� -'   $��selectUsuarioResponsable��$	��230��$��0,15,0,0q�$ Usuario Responsable��$	 .26Q� . � $��7��$��1��.+�'
  !MaterialDesignThemes.Wpf.PopupBox
  $��Right=�$��Center=�% 
  PlacementMode$% LeftAndAlignMiddles=� -	 )MaterialDesignMultiFloatingActionPopupBox#��[	 $ Menú de Opciones��& 
  UnfurlOrientation$& 
Horizontal=�' 
  PopupContent.' �� $ 
Horizontal=�.+��� -(   $��
btnRadicar��$ Radicar����  *Send+(   Size$( 20X��� -)   $��btnBorrador��$ Guardar Radicado������ *Trash+��  *ContentSaveSettingsOutline+$( 20X��� $��1��$��2,0,0,0q�$
��Bold"�$��Adjuntos��.���  �� $��* �	 �� $
��0.1* ��� $
��0.7* ��� $
��0.3* ��� $��1* �.+�� .+��� $ Vertical=�.+��� $ 
Horizontal=�.+���  $
  MaterialDesignSubtitle2TextBlock#��[
 Vista previa�� $��1��
�������� $��2��.+��� $ Vertical=�.+��� $ 
Horizontal=�.+��� -*   $��FileNameTextBox��$	��200��$��43��$��	30,10,0,0q�$
��Left=�$	��Top=�$��Seleccione los archivos��$
 Wrap=��� -+   ) ��Name$) BrowseButton��$��3��$	��170��$��40��$��Center=�$	��Top=�$��Adjuntar archivos��* J� RenderTransformOrigin$* 1.047,0.821&�