   M S B A M L   `   `   `  ���� B  >indexai, Version=2.4.6.3, Culture=neutral, PublicKeyToken=null!clr-namespace:IndexaiIndexai  O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem      Indexai.Views.FormatosView   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     j fSyncfusion.SfGridConverter.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfBusyIndicator.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89h dSyncfusion.SfImageEditor.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89`	 \Syncfusion.Linq.Base, Version=18.4460.0.49, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a
 ]Syncfusion.SfGrid.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a ]Syncfusion.Shared.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Tools.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.SfInput.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89@
Syncfusion!http://schemas.syncfusion.com/wpf    	 
    Hd2http://schemas.microsoft.com/expression/blend/2008    	 
    1indexaiclr-namespace:Indexai    	 
    b ^MaterialDesignThemes.Wpf, Version=4.0.0.2422, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  3system&clr-namespace:System;assembly=mscorlib  -     >�Name$  ___No_Name_��$	��865��$	��500��.��� 
������� � RowDefinitions �� $��80 ���  � ColumnDefinitions �� $	��240 ��� .+�(  "MaterialDesignThemes.Wpf.ColorZone  $��2��$��75��$	��Top=�
�������   Mode$ PrimaryLight=�.���� $	��860��$��5,5,-0.2,0.2q�$
��Left=� �� Orientation$ 
Horizontal=�.+�)  #MaterialDesignThemes.Wpf.DialogHost  -   $��Center=�$��Center=�   DialogContent �� $��16q�.+��� +)Ingrese un código para exportar la caja.�� -    ��Name$ txtExportCodCarpeta��$��0,8,0,0q�$��Stretch=��� $��Right=�$ 
Horizontal=�.+��� $��0,8,8,0q�$2��,materialDesign:DialogHost.CloseDialogCommand�� �� 	IsDefault .    MaterialDesignFlatButton#��[  ����True��.��
Exportar�� $��0,8,8,0q�$2��,materialDesign:DialogHost.CloseDialogCommand�� �� IsCancel . #��[  ����	False��.��
CANCELAR.���� -   	 ��Name$	 btnExportarCaja��$	��140��$��55��$��0,0,9,0q�
   OpenDialogCommand#��Z
 #��[  .���� $ Vertical=�.+�'  !MaterialDesignThemes.Wpf.PackIcon  $��20��$��20��$��Center=�   Kind$ Inboxes=��� $��Center=�$��Center=�$
��Caja���� -   $	 btnExportarCarpeta��$	��140��$��55��$��0,0,9,0q�#��[  .���� $ Vertical=�.+�  $��20��$��20��$��Center=�$ FolderTableOutline=��� $��Center=�$��Carpeta���� -   $	 btnExportarHojaControl��$	��140��$��55��$��0,0,9,0q�#��[  .���� $ Vertical=�.+�  $��20��$��20��$��Center=�$ NewspaperVariantOutline=��� $��Center=�$��Hoja de control���� -   $	 btnExportarFuid��$	��140��$��55��$��0,0,9,0q�#��[  .���� $ Vertical=�.+�  $��20��$��20��$��Center=�$ PlaylistCheck=��� $��Center=�$
��FUID���� -   $	 btnExportarIndice��$	��140��$��55��$��0,0,9,0q�#��[  .���� $ Vertical=�.+�  $��20��$��20��$��Center=�$ ViewList=��� $��Center=�$��Índice���� -	   $��ImgLogo��$	��100��$��40��$��	100,0,0,0q�$��Center=�$��Center=�$�� ��( 
 "Syncfusion.UI.Xaml.Grid.SfDataGrid  -
     Name$ 
exportGrid��$��1��$��1��$��
10,70,0,38q�   AllowFiltering . ( 
 "Syncfusion.UI.Xaml.Grid.SfGridBase   AutoGenerateColumns .     ColumnSizer$
 Auto=�   NavigationMode$	 Row=�   ShowToolTip .    ItemsSource ��  �� ElementName$ exportPager�� �� Path$ PagedSource�   Columns , 
 &Syncfusion.UI.Xaml.Grid.GridTextColumn  ( 
 "Syncfusion.UI.Xaml.Grid.GridColumn   AllowFiltering . , 
 &Syncfusion.UI.Xaml.Grid.GridColumnBase   
HeaderText$
 Lote��   MappingName$ t_lote.nom_lote��   TextAlignment$
 Left=�  $
 Caja��$ nro_caja��$
 Left=�  $ NumExpediente��$ nro_expediente��$
 Left=�  $ 
Expediente��$ nom_expediente��$
 Left=�  $ Carpeta��$ nro_carpeta��$
 Left=�  $ Folios��$ Folios��$
 Left=�  $ Paginas��$ Paginas��$
 Left=�  $ Asignado��$ Asignado��$
 Left=�  $ Estado��$ Estado��$
 Left=��� -   $��txtGridTotalitems��$��1��$��1��$	��147��$��26��$��5q�$
��Left=�$��Bottom=�$��Total registros: 0��	   Indexai.BuscadorLotes	  -    	 Name$ buscadorView��$��1��7
 
 1Syncfusion.UI.Xaml.Controls.DataPager.SfDataPager
  -    
 Name$ exportPager��$��1��$��1��$��28��$��10,0,-0.2,5q�$��Bottom=�$��Center=� 
  PageSize$ 5���� -   $ txtItemCount��$��1��$��1��$	��150��$��38��$��	10,27,0,0q�$
��Left=�$	��Top=�$�� �� �� TextWrapping$
 Wrap=��� $��1��$��1��$��26��$��10,1,0,0q�$	��Top=�$$��Cantidad máxima de documentos���� -   $	 btnMaximoItemsGrid��$��1��$��1��$	��120��$��38��$��
165,27,0,0q�$
��Left=�$	��Top=�$��Aplicar��� -    �Name$ 
exportPBar��$��1��$��2��$��5��$��
0,0,-0.2,0q�$��Bottom=� � SmallChange$ 1Q�