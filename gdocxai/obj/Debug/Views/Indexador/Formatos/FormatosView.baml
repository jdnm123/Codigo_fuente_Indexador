   M S B A M L   `   `   `  ����B  >indexai, Version=2.4.6.3, Culture=neutral, PublicKeyToken=null!clr-namespace:IndexaiIndexai  5      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5
           Indexai.Views.FormatosView   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      j fSyncfusion.SfGridConverter.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfBusyIndicator.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89h dSyncfusion.SfImageEditor.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89`	 \Syncfusion.Linq.Base, Version=18.4460.0.49, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a
 ]Syncfusion.SfGrid.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a ]Syncfusion.Shared.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Tools.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.SfInput.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89@
Syncfusion!http://schemas.syncfusion.com/wpf    	 
    5      Hd2http://schemas.microsoft.com/expression/blend/2008    	 
    5      1indexaiclr-namespace:Indexai    	 
    5      b ^MaterialDesignThemes.Wpf, Version=4.0.0.2422, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5      Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5	      3system&clr-namespace:System;assembly=mscorlib  5
      -   5          >�Name$  ___No_Name_��5      $	��865��5      $	��500��5      .��5      � 
������� � RowDefinitions 5   
   �� 5      $��80 �6   �� 5       � ColumnDefinitions 5   
   �� 5      $	��240 �6   �� 5      .+�5   
   (  "MaterialDesignThemes.Wpf.ColorZone  $��2��5      $��75��5      $	��Top=�5      
�������   Mode$ PrimaryLight=�5      .��5      �� $	��860��5      $��5,5,-0.2,0.2q�5       $
��Left=�5!       �� Orientation$ 
Horizontal=�5"      .+�5#      )  #MaterialDesignThemes.Wpf.DialogHost  -   5        $��Center=�5$      $��Center=�5%         DialogContent 5'      �� 5(      $��16q�6%   .+�5)      �� +)Ingrese un código para exportar la caja.6R   �� 5*      -   5         ��Name$ txtExportCodCarpeta��5+   !   $��0,8,0,0q�5,   !   $��Stretch=�5-   !   �� 5.      $��Right=�6)   $ 
Horizontal=�6E   .+�5/   "   �� $��0,8,8,0q�50   %   $2��,materialDesign:DialogHost.CloseDialogCommand��51   %    �� 	IsDefault .    MaterialDesignFlatButton#��[  53   %   ��54   &   ��55   *   True��6?   .��56   ?   
Exportar58   #   �� 59   "   $��0,8,8,0q�5:   %   $2��,materialDesign:DialogHost.CloseDialogCommand��5;   %    �� IsCancel . #��[  5=   %   ��5>   &   ��5?   *   	False��6@   .��5@   ?   
CANCELAR5B   #   5C      5D      .��5F      �� -   5        	 ��Name$	 btnExportarCaja��5G      $	��140��5H      $��55��5I      $��0,0,9,0q�5J      
   OpenDialogCommand#��Z
 5K      #��[  5L      .��5M      �� $ Vertical=�6%   .+�5N      '  !MaterialDesignThemes.Wpf.PackIcon  $��20��5O   !   $��20��5P   !   $��Center=�5Q   !      Kind$ Inboxes=�5R   !   �� 5S      $��Center=�5T   !   $��Center=�5U   !   $
��Caja��5V   !   5W      5X      5Y      �� 5Z      -   5        $	 btnExportarCarpeta��5[      $	��140��5\      $��55��5]      $��0,0,9,0q�5^      #��[  5`      .��5a      �� $ Vertical=�6!   .+�5b        $��20��5c      $��20��5d      $��Center=�5e      $ FolderTableOutline=�5f      �� 5g      $��Center=�6$   $��Carpeta��6?   5h      5i      �� 5j      -   5        $	 btnExportarHojaControl��5k      $	��140��5l      $��55��5m      $��0,0,9,0q�5n      #��[  5p      .��5q      �� $ Vertical=�6!   .+�5r        $��20��5s      $��20��5t      $��Center=�5u      $ NewspaperVariantOutline=�5v      �� 5w      $��Center=�6$   $��Hoja de control��6?   5x      5y      �� 5z      -   5        $	 btnExportarFuid��5{      $	��140��5|      $��55��5}      $��0,0,9,0q�5~      #��[  5�      .��5�      �� $ Vertical=�6!   .+�5�        $��20��5�      $��20��5�      $��Center=�5�      $ PlaylistCheck=�5�      �� 5�      $��Center=�6$   $
��FUID��6?   5�      5�      �� 5�      -   5        $	 btnExportarIndice��5�      $	��140��5�      $��55��5�      $��0,0,9,0q�5�      #��[  5�      .��5�      �� $ Vertical=�6!   .+�5�        $��20��5�      $��20��5�      $��Center=�5�      $ ViewList=�5�      �� 5�      $��Center=�6$   $��Índice��6?   5�      5�      �� 5�      -	   5        $��ImgLogo��5�      $	��100��5�      $��40��5�      $��	100,0,0,0q�5�      $��Center=�5�      $��Center=�5�      $�� ��5�      5�      5�      ( 
 "Syncfusion.UI.Xaml.Grid.SfDataGrid  5�   
   -
   5          Name$ 
exportGrid��5�      $��1��5�      $��1��5�      $��
10,70,0,38q�5�         AllowFiltering . ( 
 "Syncfusion.UI.Xaml.Grid.SfGridBase   AutoGenerateColumns .     ColumnSizer$
 Auto=�5�         NavigationMode$	 Row=�5�         ShowToolTip .    ItemsSource 5�      ��  �� ElementName$ exportPager�� �� Path$ PagedSource�   Columns 5�      , 
 &Syncfusion.UI.Xaml.Grid.GridTextColumn  ( 
 "Syncfusion.UI.Xaml.Grid.GridColumn   AllowFiltering . , 
 &Syncfusion.UI.Xaml.Grid.GridColumnBase   
HeaderText$
 Lote��5�         MappingName$ t_lote.nom_lote��5�         TextAlignment$
 Left=�5�        5�      $
 Caja��5�      $ nro_caja��5�      $
 Left=�5�        5�      $ NumExpediente��5�      $ nro_expediente��5�      $
 Left=�5�        5�      $ 
Expediente��5�      $ nom_expediente��5�      $
 Left=�5�        5�      $ Carpeta��5�      $ nro_carpeta��5�      $
 Left=�5�        5�      $ Folios��5�      $ Folios��5�      $
 Left=�5�        5�      $ Paginas��5�      $ Paginas��5�      $
 Left=�5�        5�      $ Asignado��5�      $ Asignado��5�      $
 Left=�5�        5�      $ Estado��5�      $ Estado��5�      $
 Left=�5�      5�      �� 5�   
   -   5        $��txtGridTotalitems��5�      $��1��5�      $��1��5�      $	��147��5�      $��26��5�      $��5q�5�      $
��Left=�5�      $��Bottom=�5�      $��Total registros: 0��5�      	   Indexai.BuscadorLotes	  5�   
   -   5         	 Name$ buscadorView��5�       $��1��66   7
 
 1Syncfusion.UI.Xaml.Controls.DataPager.SfDataPager
  5�   
   -   5         
 Name$ exportPager��5�      $��1��5�      $��1��5�      $��28��5�      $��10,0,-0.2,5q�5�      $��Bottom=�5�      $��Center=�5�       
  PageSize$ 5��5�      �� 5�   
   -   5        $ txtItemCount��5�      $��1��5�      $��1��5�      $	��150��5�      $��38��5�      $��	10,27,0,0q�5�      $
��Left=�5�      $	��Top=�5�      $�� ��5�       �� TextWrapping$
 Wrap=�5�      �� 5�   
   $��1��5�      $��1��5�      $��26��5�      $��10,1,0,0q�5�      $	��Top=�5�      $$��Cantidad máxima de documentos��5�      �� 5�   
   -   5        $	 btnMaximoItemsGrid��5�      $��1��5�      $��1��5      $	��120��5     $��38��5     $��
165,27,0,0q�5     $
��Left=�5     $	��Top=�5     $��Aplicar��5     � 5  
   -   5         �Name$ 
exportPBar��5	     $��1��5
     $��2��5     $��5��5     $��
0,0,-0.2,0q�5     $��Bottom=�5      � SmallChange$ 1Q�5     5     5     