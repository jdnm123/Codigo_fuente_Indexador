   M S B A M L   `   `   `  ����O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5	      B >indexai, Version=2.4.6.3, Culture=neutral, PublicKeyToken=null!clr-namespace:IndexaiIndexai 5
      -clr-namespace:Indexai.ViewsIndexai.Views 5      &    Indexai.Views.ControlCalidadView   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      j fSyncfusion.SfGridConverter.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfBusyIndicator.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89h dSyncfusion.SfImageEditor.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89`	 \Syncfusion.Linq.Base, Version=18.4460.0.49, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a
 ]Syncfusion.SfGrid.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a ]Syncfusion.Shared.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Tools.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.SfInput.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89@
Syncfusion!http://schemas.syncfusion.com/wpf    	 
    5      Hd2http://schemas.microsoft.com/expression/blend/2008    	 
    5      b ^MaterialDesignThemes.Wpf, Version=4.0.0.2422, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5      Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5      3system&clr-namespace:System;assembly=mscorlib  5	      !utilsclr-namespace:Indexai  5
      'viewsclr-namespace:Indexai.Views  5      $	��980��5      $	��675��5      .��5      � .+�5   
   5  /Syncfusion.Windows.Tools.Controls.TabControlExt  -   5           Name$   controlCalidadSeleccionTab��5         AllowDragDrop .     CloseButtonType$
 Both=�5         EnableLabelEdit .     IsNewButtonEnabled .     ShowTabItemContextMenu .     ShowTabListContextMenu .      TabScrollButtonVisibility$
 Auto=�5         TabScrollStyle$ Extended=�5      	 �� TabStripPlacement$		 Top=�5      .@�5      2  ,Syncfusion.Windows.Tools.Controls.TabItemExt  -   5        
  Name$
 
TabCCLista��5         CanClose .  $��Carpetas��5       �� 
IsSelected .  .��5       �  � ColumnDefinitions 5!      �� 5"      $	��233 �6+   �� 5#      .+�5%      � -   5         �Name$ buscadorContainer��5%      $��0��66   .+�5&        Indexai.BuscadorLotes  -   5          Name$ buscadorViewCalidad��5'      ��5(      ��  �� ElementName$ buscadorContainer�� �� Path$ ActualWidth���5)      �� $ buscadorContainer��$ ActualHeight�5*        5+      -   5        $��controlCalidadCarpetas��5,      $��1��5-      $��0,36,0,0q�5.       .  $
 Both=�50       .   .   .   .  $
 Auto=�55      $ Extended=�56      $		 Top=�57      .@�58         .     CloseButtonState$ 	Collapsed=�5:      $��	Asignadas��5;       . .��5=      � .+�5>   "   ( 
 "Syncfusion.UI.Xaml.Grid.SfDataGrid  -   5          Name$ controlCalidadUsuarioGrid��5?   %   $��0,0,0,32q�5@   %      AllowFiltering .  ( 
 "Syncfusion.UI.Xaml.Grid.SfGridBase   AutoGenerateColumns .     ColumnSizer$ AutoWithLastColumnFill=�5C   %      NavigationMode$	 Row=�5F   %      ShowToolTip .    ItemsSource 5D   %   �� *PagedSource+$  controlCalidadUsuarioPager��   Columns 5J   *   , 
 &Syncfusion.UI.Xaml.Grid.GridTextColumn  , 
 &Syncfusion.UI.Xaml.Grid.GridColumnBase   Width$	 120Q�5K   -      
HeaderText$
 Lote��5L   -      MappingName$
 Lote��5M   -      TextAlignment$
 Left=�5N   -     5O   *   $	 120Q�5P   -   $
 Caja��5Q   -   $
 Caja��5R   -   $
 Left=�5S   -     5T   *   $	 230Q�5U   -   $ Nro Expediente��5V   -   $ NroExpediente��5W   -   $
 Left=�5X   -     5Y   *   $	 400Q�5Z   -   $ Nom Expediente��5[   -   $ NomExpediente��5\   -   $
 Left=�5]   -     5^   *   $	 120Q�5_   -   $ Nro Carpeta��5`   -   $ 
NroCarpeta��5a   -   $
 Left=�5b   -     5c   *   $ HC_Fin��5d   -   $ hc_fin��5e   -   $
 Left=�5f   -     5g   *   $ TotalFolios��5h   -   $ TotalFolios��5i   -   $
 Left=�5j   -     5k   *   $ Realizo��5l   -   $ Realizo��5m   -   $
 Left=�5n   -     5o   *   $ Fecha indexado��5p   -   $ FechaIndexado��5q   -   $
 Left=�5r   -   5t   #   7 
 1Syncfusion.UI.Xaml.Controls.DataPager.SfDataPager  5u   "   -   5          Name$  controlCalidadUsuarioPager��5v   %   $��1��5w   %   $��2��5x   %   $��27��5y   %   $��Center=�5z   %   $��Bottom=�5{   %   $��Center=�5|   %       PageSize$  25��5}   %   !   UseOnDemandPaging! . 5~   %   �� 5   "   -   5        $��txtGridTotalUser��5�   %   $��1��5�   %   $��1��5�   %   $	��147��5�   %   $��26��5�   %   $��0,0,0,0q�5�   %   $
��Left=�5�   %   $��Bottom=�5�   %   $��Total registros: 0��5�   %   5�      5�        5�       .  $ 	Collapsed=�5�      $��	Públicas��5�       .  .��5�      � .+�5�   "     -	   5        $ calidadPublicosGrid��5�   %   $��0,0,0,32q�5�   %    .   .  $
 Auto=�5�   %   $	 Row=�5�   %    .  5�   %   �� *PagedSource+$ calidadPublicosPager�� 5�   *     $
 Lote��5�   -   $
 Lote��5�   -   $
 Left=�5�   -     5�   *   $
 Caja��5�   -   $
 Caja��5�   -   $
 Left=�5�   -     5�   *   $ Nro Expediente��5�   -   $ NroExpediente��5�   -   $
 Left=�5�   -     5�   *   $ Nom Expediente��5�   -   $ NomExpediente��5�   -   $
 Left=�5�   -     5�   *   $ Realizo��5�   -   $ Realizo��5�   -   $
 Left=�5�   -     5�   *   $ Nro Carpeta��5�   -   $ 
NroCarpeta��5�   -   $
 Left=�5�   -     5�   *   $ TotalFolios��5�   -   $ TotalFolios��5�   -   $
 Left=�5�   -     5�   *   $ Fecha indexado��5�   -   $ FechaIndexado��5�   -   $
 Left=�5�   -   5�   #   �� 5�   "   -
   5        $��txtGridTotalPublicitems��5�   %   $��1��5�   %   $��1��5�   %   $	��147��5�   %   $��26��5�   %   $��5q�5�   %   $
��Left=�5�   %   $��Bottom=�5�   %   $��Total registros: 0��5�   %     5�   "   -   5        $ calidadPublicosPager��5�   %   $��1��5�   %   $��2��5�   %   $��27��5�   %   $��Center=�5�   %   $��Bottom=�5�   %   $��Center=�5�   %   $  25��5�   %   ! . 5�   %   5�      5�      5�      �� 5�      -   5        " ��Name$" btnActualizarCache��5�      $��1��5�      $	��140��5�      $��48��5�      $��	0,0,265,0q�5�      $��Right=�5�      $	��Top=�5�         MaterialDesignFlatButton#��[  5�      .��5�      �� $��0,4,0,0q�6%   # �� Orientation$# Vertical=�66   .+�5�      '	  !MaterialDesignThemes.Wpf.PackIcon	  $��16��5�   !   $��16��5�   !   $��Center=�5�   !   $ 	  Kind$$ Refresh=�5�   !   �� 5�      $��Center=�5�   !   $��Center=�5�   !   $��11(�5�   !   $��Actualizar Cache��5�   !   � 5�      -   5        % �Name$% pbarUpdateCache��5�   *   & � Value$& 50Q�6C   $y�	Collapsed=�6N   5�      5�      �� 5�      -   5        $" btnActualizar��5�      $��1��5�      $	��120��5�      $��48��5�      $��	0,0,140,0q�5�      $��Right=�5�      $	��Top=�5�      #��[  5�      .��5�      �� $# Vertical=�6%   .+�5�      	  $��16��5�   !   $��16��5�   !   $��Center=�5�   !   $$ Refresh=�5�   !   �� 5�      $��Center=�5�   !   $��11(�5   !   $��Actualizar KP��5  !   5     5     )
  #MaterialDesignThemes.Wpf.DialogHost
  5     -   5        ' 
 Name$' dlgEditCaja��5     $��1��5     $	��120��5     $��48��5     $��0,0,10,0q�5	     $��Right=�5
     $	��Top=�5     {�. ( 
  DialogContent( 5     �� 5     $��16q�6)   $��Right=�65   .+�5  "   �� Ingrese la caja de destino6H   �� 5  "   -   5        ) ��Name$) txtCaja��5  *   �� 5  "   $��Right=�6-   $# 
Horizontal=�6I   .+�5  &   �� $��0,8,8,0q�5  )   $2��,materialDesign:DialogHost.CloseDialogCommand��5  )   * �� 	IsDefault* . #��[  5  )   ��5  *   ��5  .   True��6C   .��5  C   	ACEPTAR5  '   �� 5  &   $��0,8,8,0q�5  )   $2��,materialDesign:DialogHost.CloseDialogCommand��5  )   + �� IsCancel+ . #��[  5!  )   ��5"  *   ��5#  .   	False��6D   .��5$  C   
CANCELAR5&  '   5'  #   5(     .��5*     �� -   5        $" btnEditCaja��5+     $��1��5,     $	��120��5-     $��48��5.     $��0,0,0,-6q�5/     $��Right=�50     $	��Top=�51     , 
  OpenDialogCommand#��Z, 52     {�. #��[  54     .��55     �� $# Vertical=�6)   .+�56  "   	  $��16��57  %   $��16��58  %   $��Center=�59  %   $$ FolderEditOutline=�5:  %   �� 5;  "   $��Center=�5<  %   $��11(�5=  %   $��
Edita Caja��5>  %   5?     5@     5A     5B     5D       5E     -   5        $
 tabControlCalidadParent��5F      .  $��Control calidad��5H      . $y�	Collapsed=�5J     .��5K     � -   5        $ calidadReviewContainer��5K     .+�5L     .  (Indexai.Views.ControlCalidadRevisionView  -   5        -  Name$- controCalidadReview��5M     ��5N     �� $ calidadReviewContainer��$ ActualWidth���5O     �� $ calidadReviewContainer��$ ActualHeight�5P     5Q     5S     5T     5V     