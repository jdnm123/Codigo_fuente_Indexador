   M S B A M L   `   `   `  ����B  >indexai, Version=2.4.6.3, Culture=neutral, PublicKeyToken=null!clr-namespace:IndexaiIndexai  5      #    Indexai.Views.PreIndexadoView   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      j fSyncfusion.SfGridConverter.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfBusyIndicator.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89h dSyncfusion.SfImageEditor.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Linq.Base, Version=18.4460.0.49, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a	 ]Syncfusion.SfGrid.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a
 ]Syncfusion.Shared.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Tools.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.SfInput.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89@
Syncfusion!http://schemas.syncfusion.com/wpf     	 
   5      Hd2http://schemas.microsoft.com/expression/blend/2008     	 
   5      1indexaiclr-namespace:Indexai     	 
   5      Rmc;http://schemas.openxmlformats.org/markup-compatibility/2006     	 
   5      $	��865��5	      $	��500��5
      .��5      � 
�������  � RowDefinitions  5   
   �� 5      $��68 �6   �� 5       � ColumnDefinitions 5   
   �� 5      $	��240 �6   �� 5      .+�5   
   ( 	 "Syncfusion.UI.Xaml.Grid.SfDataGrid  -   5          Name$ 
exportGrid��5      $��1��5      $��1��5      $��	0,56,0,38q�5         AllowFiltering . ( 	 "Syncfusion.UI.Xaml.Grid.SfGridBase   AutoGenerateColumns .     ColumnSizer$
 Star=�5         NavigationMode$	 Row=�5          ItemsSource 5      ��  �� ElementName$ exportPager��	 �� Path$	 PagedSource�
   Columns
 5"      , 	 &Syncfusion.UI.Xaml.Grid.GridTextColumn  , 	 &Syncfusion.UI.Xaml.Grid.GridColumnBase   
HeaderText$
 Lote��5#         MappingName$ t_lote.nom_lote��5$         TextAlignment$
 Left=�5%        5&      nro_caja��$
 Caja��5(      $ nro_caja��5)      $
 Left=�5*        5+      $ NumExpediente��5,      $ nro_expediente��5-      $
 Left=�5.        5/         Width$	 650Q�50      $ 
Expediente��51      $ nom_expediente��52      $
 Left=�53        54      $ Carpeta��55      $ nro_carpeta��56      $
 Left=�57        58      $ Folios��59      $ Folios��5:      $
 Left=�5;        5<      $ Paginas��5=      $ Paginas��5>      $
 Left=�5?        5@      $ Asignado��5A      $ Asignado��5B      $
 Left=�5C        5D      $ Estado��5E      $ Estado��5F      $
 Left=�5G      5I      �� 5J   
   -   5        $��txtGridTotalitems��5K      $��1��5L      $��1��5M      $	��147��5N      $��26��5O      $��5q�5P      $
��Left=�5Q      $��Bottom=�5R      $��Total registros: 0��5S      7 	 1Syncfusion.UI.Xaml.Controls.DataPager.SfDataPager  5Z   
   -   5          Name$ exportPager��5[      $��1��5\      $��1��5]      $��33��5^      $��10,0,-0.2,0q�5_      $��Bottom=�5`      $��Center=�5a         PageSize$ 5��5b      �� 5c   
   -   5         ��Name$ txtItemCount��5d      $��1��5e      $��1��5f      $	��150��5g      $��24��5h      $��	10,27,0,0q�5i      $
��Left=�5j      $	��Top=�5k      $�� ��5l       �� TextWrapping$
 Wrap=�5m      �� 5n   
   $��1��5o      $��1��5p      $��26��5q      $��10,1,-0.2,0q�5r      $	��Top=�5s      $$��Cantidad máxima de documentos��5t      �� 5u   
   -   5         ��Name$ btnMaximoItemsGrid��5v      $��1��5w      $��1��5x      $��86��5y      $��24��5z      $��
165,27,0,0q�5{      $
��Left=�5|      $	��Top=�5}      $��Aplicar��5      $��10(�5�      �� 5�   
   -   5        $ btnPreIndexar��5�      $��1��5�      $	��120��5�      $��38��5�      $��	10,0,0,10q�5�      $
��Left=�5�      $��Bottom=�5�      $��Procesar��5�      {�.  5�         Indexai.BuscadorLotes  5�   
   -   5          Name$ buscador��5�       $��1��62   �� 5�   
   -   5         ��Name$ cbxTablaPreindexado��5�      $��23��5�      $��
10,35,10,0q�5�      $	��Top=�5�       �� DisplayMemberPath$ 
nombre_trd��5�      �� 5�   
   $��	10,5,10,0q�5�      $	��Top=�5�      $��Tabla preindexado��5�      5�      5�      