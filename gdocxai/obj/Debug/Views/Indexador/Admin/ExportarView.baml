   M S B A M L   `   `   `  ����B  >indexai, Version=2.4.6.3, Culture=neutral, PublicKeyToken=null!clr-namespace:IndexaiIndexai  5      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5           Indexai.Views.ExportarView   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      j fSyncfusion.SfGridConverter.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfBusyIndicator.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89h dSyncfusion.SfImageEditor.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89`	 \Syncfusion.Linq.Base, Version=18.4460.0.49, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a
 ]Syncfusion.SfGrid.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a ]Syncfusion.Shared.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Tools.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.SfInput.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89@
Syncfusion!http://schemas.syncfusion.com/wpf    	 
    5      Hd2http://schemas.microsoft.com/expression/blend/2008    	 
    5      1indexaiclr-namespace:Indexai    	 
    5      _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a?interactivity+http://schemas.microsoft.com/xaml/behaviors  5      b ^MaterialDesignThemes.Wpf, Version=4.0.0.2422, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5	      Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5
      3system&clr-namespace:System;assembly=mscorlib  5      -   5          >�Name$  ___No_Name_��5      $	��865��5      $	��500��5      .��5      � 
������� � RowDefinitions 5   
   �� 5      $��80 �6   �� 5       � ColumnDefinitions 5   
   �� 5      $	��240 �6   �� 5      .+�55   
   )  #MaterialDesignThemes.Wpf.DialogHost  -   5          Name$ dlgExportar��56      $��0��57      $	��140��58      $��45��59      $��0,15,0,0q�5:      $
��Left=�5;      $	��Top=�5<      {�.    DialogContent 5?      �� 5@      $��16q�6   $��Right=�6)   .+�5A      �� -   5         ��Name$ chkHojaControl��5B      $*��$Active para Exportar Hoja de control��5C       d� 	IsChecked$ FalseY�5D         MaterialDesignDarkCheckBox#��[  5E      �� 5F      -   5        $ chkSobreescribir��5G      $(��"Active para Sobreescribir archivos��5H      $ FalseY�5I      #��[  5J      �� 5K      $��Right=�6!    �� Orientation$ 
Horizontal=�6=   .+�5L      �� $��0,8,8,0q�5M      $2��,materialDesign:DialogHost.CloseDialogCommand��5N       �� 	IsDefault .   MaterialDesignFlatButton#��[ 5P      ��5Q      ��5R   "   True��67   .��5S   7   	ACEPTAR5U      �� 5V      $��0,8,8,0q�5W      $2��,materialDesign:DialogHost.CloseDialogCommand��5X      	 �� IsCancel	 . #��[ 5Z      ��5[      ��5\   "   	False��68   .��5]   7   
CANCELAR5_      5`      5a      .��5c      �� -   5        
 ��Name$
 btnExportar��5d      $��1��5e      $	��140��5f      $��45��5g      $��0,0,0,0q�5h      $��Right=�5i      $	��Top=�5j         OpenDialogCommand#��Z 5k      {�. #��[ 5m      .��5o      �� $ Vertical=�6   .+�5p      '  !MaterialDesignThemes.Wpf.PackIcon  $��20��5q      $��20��5r      $��Center=�5s         Kind$ PdfBox=�5t      �� 5u      $��Center=�5v      $��11(�5w      $��Exportar��5x      5y      5z      5{        5|   
   -   5        $ dlgRechazar��5}      $��2��5~      $	��140��5      $��45��5�      $��0,15,0,0q�5�      $
��Left=�5�      $	��Top=�5�      {�.  5�      �� 5�      $��16q�6   $��Right=�6)   .+�5�      �� 0.Razón por la que desea rechazar las Carpetas?6O   �� 5�      -   5         ��Name$ 
txtRechazo��5�      �� 5�      $��Right=�6!   $ 
Horizontal=�6=   .+�5�      �� $��0,8,8,0q�5�      $2��,materialDesign:DialogHost.CloseDialogCommand��5�       . #��[ 5�      ��5�      ��5�   "   True��67   .��5�   7   	ACEPTAR5�      �� 5�      $��0,8,8,0q�5�      $2��,materialDesign:DialogHost.CloseDialogCommand��5�      	 . #��[ 5�      ��5�      ��5�   "   	False��68   .��5�   7   
CANCELAR5�      5�      5�      .��5�      �� -   5        $
 btnExp��5�      $��1��5�      $	��140��5�      $��45��5�      $��0,0,0,0q�5�      $��Right=�5�      $	��Top=�5�      #��Z 5�      {�. #��[ 5�      .��5�      �� $ Vertical=�6   .+�5�        $��20��5�      $��20��5�      $��Center=�5�      $ HamburgerMenuBack=�5�      �� 5�      $��Center=�5�      $��11(�5�      $��Devolver a Calidad��5�      5�      5�      5�      �� 5�   
   -	   5        $
 btnDatosLote��5�      $��1��5�      $	��140��5�      $��45��5�      $��
150,15,0,0q�5�      $
��Left=�5�      $	��Top=�5�      {�. #��[ 5�      .��5�      �� $ Vertical=�6   .+�5�        $��20��5�      $��20��5�      $��Center=�5�      $ PersonCardDetails=�5�      �� 5�      $��Center=�5�      $��11(�5�      $��Datos Básicos��5�      5�      5�      ( 
 "Syncfusion.UI.Xaml.Grid.SfDataGrid  5�   
   -
   5          Name$ 
exportGrid��5�      $��1��5�      $��1��5�      $��
10,70,0,38q�5�         AllowFiltering . ( 
 "Syncfusion.UI.Xaml.Grid.SfGridBase   AutoGenerateColumns .     ColumnSizer$
 Auto=�5�         SelectionUnit$
 Cell=�5�         ShowToolTip .    ItemsSource 5�      ��  �� ElementName$ exportPager�� �� Path$ PagedSource�   Columns 5�      , 
 &Syncfusion.UI.Xaml.Grid.GridTextColumn  , 
 &Syncfusion.UI.Xaml.Grid.GridColumnBase   
HeaderText$
 Lote��5�         MappingName$ t_lote.nom_lote��5�         TextAlignment$
 Left=�5�        5�      nro_caja��$
 Caja��5�      $ nro_caja��5�      $
 Left=�5�        5�      $ NumExpediente��5�      $ nro_expediente��5�      $
 Left=�5�        5�         Width$	 650Q�5�      $ 
Expediente��5�      $ nom_expediente��5�      $
 Left=�5�        5�      $ Carpeta��5�      $ nro_carpeta��5�      $
 Left=�5�        5�      $ Folios��5�      $ Folios��5�      $
 Left=�5�        5�      $ Paginas��5�      $ Paginas��5�      $
 Left=�5�        5�      $ Asignado��5�      $ Asignado��5�      $
 Left=�5        5     $ Estado��5     $ Estado��5     $
 Left=�5     *  $Microsoft.Xaml.Behaviors.Interaction   	Behaviors 5         Indexai.SfDataGridBehavior  5     5	     �� 5
  
   -   5        $��txtGridTotalitems��5     $��1��5     $��1��5     $	��147��5     $��26��5     $��5q�5     $
��Left=�5     $��Bottom=�5     $��Total registros: 0��5     	   Indexai.BuscadorLotes	  5  
   -   5         	 Name$ buscadorView��5      $��1��66   7
 
 1Syncfusion.UI.Xaml.Controls.DataPager.SfDataPager
  5  
   -   5         
 Name$ exportPager��5     $��1��5     $��1��5     $��28��5     $��10,0,0,5q�5     $��Center=�5     $��Bottom=�5      
  PageSize$ 5��5     �� 5  
   -   5        $ txtItemCount��5     $��1��5      $��1��5!     $	��150��5"     $��38��5#     $��	10,27,0,0q�5$     $
��Left=�5%     $	��Top=�5&     $�� ��5'       �� TextWrapping$
  Wrap=�5(     �� 5)  
   $��1��5*     $��1��5+     $��26��5,     $��10,1,0,0q�5-     $	��Top=�5.     $$��Cantidad máxima de documentos��5/     �� 50  
   -   5        $
 btnMaximoItemsGrid��51     $��1��52     $��1��53     $	��120��54     $��38��55     $��
165,27,0,0q�56     $
��Left=�57     $	��Top=�58     $��Aplicar��5:     � 5;  
   -   5        ! �Name$! 
exportPBar��5<     $��1��5=     $��2��5>     $��5��5?     $��
0,0,-0.2,0q�5@     $��Bottom=�5A     " � SmallChange$" 1Q�5B     5C     5D     