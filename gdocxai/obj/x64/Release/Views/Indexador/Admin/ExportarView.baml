   M S B A M L   `   `   `  ���� B  >indexai, Version=2.4.6.3, Culture=neutral, PublicKeyToken=null!clr-namespace:IndexaiIndexai  O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem      Indexai.Views.ExportarView   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     j fSyncfusion.SfGridConverter.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfBusyIndicator.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89h dSyncfusion.SfImageEditor.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89`	 \Syncfusion.Linq.Base, Version=18.4460.0.49, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a
 ]Syncfusion.SfGrid.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a ]Syncfusion.Shared.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Tools.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.SfInput.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89@
Syncfusion!http://schemas.syncfusion.com/wpf    	 
    Hd2http://schemas.microsoft.com/expression/blend/2008    	 
    1indexaiclr-namespace:Indexai    	 
    _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a?interactivity+http://schemas.microsoft.com/xaml/behaviors  b ^MaterialDesignThemes.Wpf, Version=4.0.0.2422, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  3system&clr-namespace:System;assembly=mscorlib  -     >�Name$  ___No_Name_��$	��865��$	��500��.��� 
������� � RowDefinitions �� $��80 ���  � ColumnDefinitions �� $	��240 ��� .+�)  #MaterialDesignThemes.Wpf.DialogHost  -     Name$ dlgExportar��$��0��$	��140��$��45��$��0,15,0,0q�$
��Left=�$	��Top=�{�.    DialogContent �� $��16q�$��Right=�.+��� -    ��Name$ chkHojaControl��$*��$Active para Exportar Hoja de control�� d� 	IsChecked$ FalseY�   MaterialDesignDarkCheckBox#��[  �� -   $ chkSobreescribir��$(��"Active para Sobreescribir archivos��$ FalseY�#��[  �� $��Right=� �� Orientation$ 
Horizontal=�.+��� $��0,8,8,0q�$2��,materialDesign:DialogHost.CloseDialogCommand�� �� 	IsDefault .   MaterialDesignFlatButton#��[ ����True��.��	ACEPTAR�� $��0,8,8,0q�$2��,materialDesign:DialogHost.CloseDialogCommand��	 �� IsCancel	 . #��[ ����	False��.��
CANCELAR.���� -   
 ��Name$
 btnExportar��$��1��$	��140��$��45��$��0,0,0,0q�$��Right=�$	��Top=�   OpenDialogCommand#��Z {�. #��[ .���� $ Vertical=�.+�'  !MaterialDesignThemes.Wpf.PackIcon  $��20��$��20��$��Center=�   Kind$ PdfBox=��� $��Center=�$��11(�$��Exportar��  -   $ dlgRechazar��$��2��$	��140��$��45��$��0,15,0,0q�$
��Left=�$	��Top=�{�.  �� $��16q�$��Right=�.+��� 0.Razón por la que desea rechazar las Carpetas?�� -    ��Name$ 
txtRechazo���� $��Right=�$ 
Horizontal=�.+��� $��0,8,8,0q�$2��,materialDesign:DialogHost.CloseDialogCommand�� . #��[ ����True��.��	ACEPTAR�� $��0,8,8,0q�$2��,materialDesign:DialogHost.CloseDialogCommand��	 . #��[ ����	False��.��
CANCELAR.���� -   $
 btnExp��$��1��$	��140��$��45��$��0,0,0,0q�$��Right=�$	��Top=�#��Z {�. #��[ .���� $ Vertical=�.+�  $��20��$��20��$��Center=�$ HamburgerMenuBack=��� $��Center=�$��11(�$��Devolver a Calidad���� -	   $
 btnDatosLote��$��1��$	��140��$��45��$��
150,15,0,0q�$
��Left=�$	��Top=�{�. #��[ .���� $ Vertical=�.+�  $��20��$��20��$��Center=�$ PersonCardDetails=��� $��Center=�$��11(�$��Datos Básicos��( 
 "Syncfusion.UI.Xaml.Grid.SfDataGrid  -
     Name$ 
exportGrid��$��1��$��1��$��
10,70,0,38q�   AllowFiltering . ( 
 "Syncfusion.UI.Xaml.Grid.SfGridBase   AutoGenerateColumns .     ColumnSizer$
 Auto=�   SelectionUnit$
 Cell=�   ShowToolTip .    ItemsSource ��  �� ElementName$ exportPager�� �� Path$ PagedSource�   Columns , 
 &Syncfusion.UI.Xaml.Grid.GridTextColumn  , 
 &Syncfusion.UI.Xaml.Grid.GridColumnBase   
HeaderText$
 Lote��   MappingName$ t_lote.nom_lote��   TextAlignment$
 Left=�  nro_caja��$
 Caja��$ nro_caja��$
 Left=�  $ NumExpediente��$ nro_expediente��$
 Left=�     Width$	 650Q�$ 
Expediente��$ nom_expediente��$
 Left=�  $ Carpeta��$ nro_carpeta��$
 Left=�  $ Folios��$ Folios��$
 Left=�  $ Paginas��$ Paginas��$
 Left=�  $ Asignado��$ Asignado��$
 Left=�  $ Estado��$ Estado��$
 Left=�*  $Microsoft.Xaml.Behaviors.Interaction   	Behaviors     Indexai.SfDataGridBehavior  �� -   $��txtGridTotalitems��$��1��$��1��$	��147��$��26��$��5q�$
��Left=�$��Bottom=�$��Total registros: 0��	   Indexai.BuscadorLotes	  -    	 Name$ buscadorView��$��1��7
 
 1Syncfusion.UI.Xaml.Controls.DataPager.SfDataPager
  -    
 Name$ exportPager��$��1��$��1��$��28��$��10,0,0,5q�$��Center=�$��Bottom=� 
  PageSize$ 5���� -   $ txtItemCount��$��1��$��1��$	��150��$��38��$��	10,27,0,0q�$
��Left=�$	��Top=�$�� ��  �� TextWrapping$
  Wrap=��� $��1��$��1��$��26��$��10,1,0,0q�$	��Top=�$$��Cantidad máxima de documentos���� -   $
 btnMaximoItemsGrid��$��1��$��1��$	��120��$��38��$��
165,27,0,0q�$
��Left=�$	��Top=�$��Aplicar��� -   ! �Name$! 
exportPBar��$��1��$��2��$��5��$��
0,0,-0.2,0q�$��Bottom=�" � SmallChange$" 1Q�