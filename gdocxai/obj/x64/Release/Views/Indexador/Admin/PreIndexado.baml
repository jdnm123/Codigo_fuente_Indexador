   M S B A M L   `   `   `  ���� B  >indexai, Version=2.4.6.3, Culture=neutral, PublicKeyToken=null!clr-namespace:IndexaiIndexai  #    Indexai.Views.PreIndexadoView   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     j fSyncfusion.SfGridConverter.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfBusyIndicator.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89h dSyncfusion.SfImageEditor.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Linq.Base, Version=18.4460.0.49, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a	 ]Syncfusion.SfGrid.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a
 ]Syncfusion.Shared.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Tools.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.SfInput.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89@
Syncfusion!http://schemas.syncfusion.com/wpf     	 
   Hd2http://schemas.microsoft.com/expression/blend/2008     	 
   1indexaiclr-namespace:Indexai     	 
   Rmc;http://schemas.openxmlformats.org/markup-compatibility/2006     	 
   $	��865��$	��500��.��� 
�������  � RowDefinitions  �� $��68 ���  � ColumnDefinitions �� $	��240 ��� .+�( 	 "Syncfusion.UI.Xaml.Grid.SfDataGrid  -     Name$ 
exportGrid��$��1��$��1��$��	0,56,0,38q�   AllowFiltering . ( 	 "Syncfusion.UI.Xaml.Grid.SfGridBase   AutoGenerateColumns .     ColumnSizer$
 Star=�   NavigationMode$	 Row=�   ItemsSource ��  �� ElementName$ exportPager��	 �� Path$	 PagedSource�
   Columns
 , 	 &Syncfusion.UI.Xaml.Grid.GridTextColumn  , 	 &Syncfusion.UI.Xaml.Grid.GridColumnBase   
HeaderText$
 Lote��   MappingName$ t_lote.nom_lote��   TextAlignment$
 Left=�  nro_caja��$
 Caja��$ nro_caja��$
 Left=�  $ NumExpediente��$ nro_expediente��$
 Left=�     Width$	 650Q�$ 
Expediente��$ nom_expediente��$
 Left=�  $ Carpeta��$ nro_carpeta��$
 Left=�  $ Folios��$ Folios��$
 Left=�  $ Paginas��$ Paginas��$
 Left=�  $ Asignado��$ Asignado��$
 Left=�  $ Estado��$ Estado��$
 Left=��� -   $��txtGridTotalitems��$��1��$��1��$	��147��$��26��$��5q�$
��Left=�$��Bottom=�$��Total registros: 0��7 	 1Syncfusion.UI.Xaml.Controls.DataPager.SfDataPager  -     Name$ exportPager��$��1��$��1��$��33��$��10,0,-0.2,0q�$��Bottom=�$��Center=�   PageSize$ 5���� -    ��Name$ txtItemCount��$��1��$��1��$	��150��$��24��$��	10,27,0,0q�$
��Left=�$	��Top=�$�� �� �� TextWrapping$
 Wrap=��� $��1��$��1��$��26��$��10,1,-0.2,0q�$	��Top=�$$��Cantidad máxima de documentos���� -    ��Name$ btnMaximoItemsGrid��$��1��$��1��$��86��$��24��$��
165,27,0,0q�$
��Left=�$	��Top=�$��Aplicar��$��10(��� -   $ btnPreIndexar��$��1��$	��120��$��38��$��	10,0,0,10q�$
��Left=�$��Bottom=�$��Procesar��{�.     Indexai.BuscadorLotes  -     Name$ buscador��$��1���� -    ��Name$ cbxTablaPreindexado��$��23��$��
10,35,10,0q�$	��Top=� �� DisplayMemberPath$ 
nombre_trd���� $��	10,5,10,0q�$	��Top=�$��Tabla preindexado��