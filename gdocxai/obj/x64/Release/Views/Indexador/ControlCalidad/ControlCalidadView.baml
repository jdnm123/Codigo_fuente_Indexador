   M S B A M L   `   `   `  ���� O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  B >indexai, Version=2.4.6.3, Culture=neutral, PublicKeyToken=null!clr-namespace:IndexaiIndexai -clr-namespace:Indexai.ViewsIndexai.Views &    Indexai.Views.ControlCalidadView   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     j fSyncfusion.SfGridConverter.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfBusyIndicator.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89h dSyncfusion.SfImageEditor.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89`	 \Syncfusion.Linq.Base, Version=18.4460.0.49, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a
 ]Syncfusion.SfGrid.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a ]Syncfusion.Shared.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Tools.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.SfInput.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89@
Syncfusion!http://schemas.syncfusion.com/wpf    	 
    Hd2http://schemas.microsoft.com/expression/blend/2008    	 
    b ^MaterialDesignThemes.Wpf, Version=4.0.0.2422, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  3system&clr-namespace:System;assembly=mscorlib  !utilsclr-namespace:Indexai  'viewsclr-namespace:Indexai.Views  $	��980��$	��675��.��� .+�5  /Syncfusion.Windows.Tools.Controls.TabControlExt  -      Name$   controlCalidadSeleccionTab��   AllowDragDrop .     CloseButtonType$
 Both=�   EnableLabelEdit .     IsNewButtonEnabled .     ShowTabItemContextMenu .     ShowTabListContextMenu .      TabScrollButtonVisibility$
 Auto=�   TabScrollStyle$ Extended=�	 �� TabStripPlacement$		 Top=�.@�2  ,Syncfusion.Windows.Tools.Controls.TabItemExt  -   
  Name$
 
TabCCLista��   CanClose .  $��Carpetas�� �� 
IsSelected .  .���  � ColumnDefinitions �� $	��233 ��� .+�� -    �Name$ buscadorContainer��$��0��.+�  Indexai.BuscadorLotes  -     Name$ buscadorViewCalidad������  �� ElementName$ buscadorContainer�� �� Path$ ActualWidth����� $ buscadorContainer��$ ActualHeight�  -   $��controlCalidadCarpetas��$��1��$��0,36,0,0q� .  $
 Both=� .   .   .   .  $
 Auto=�$ Extended=�$		 Top=�.@�   .     CloseButtonState$ 	Collapsed=�$��	Asignadas�� . .��� .+�( 
 "Syncfusion.UI.Xaml.Grid.SfDataGrid  -     Name$ controlCalidadUsuarioGrid��$��0,0,0,32q�   AllowFiltering .  ( 
 "Syncfusion.UI.Xaml.Grid.SfGridBase   AutoGenerateColumns .     ColumnSizer$ AutoWithLastColumnFill=�   NavigationMode$	 Row=�   ShowToolTip .    ItemsSource �� *PagedSource+$  controlCalidadUsuarioPager��   Columns , 
 &Syncfusion.UI.Xaml.Grid.GridTextColumn  , 
 &Syncfusion.UI.Xaml.Grid.GridColumnBase   Width$	 120Q�   
HeaderText$
 Lote��   MappingName$
 Lote��   TextAlignment$
 Left=�  $	 120Q�$
 Caja��$
 Caja��$
 Left=�  $	 230Q�$ Nro Expediente��$ NroExpediente��$
 Left=�  $	 400Q�$ Nom Expediente��$ NomExpediente��$
 Left=�  $	 120Q�$ Nro Carpeta��$ 
NroCarpeta��$
 Left=�  $ HC_Fin��$ hc_fin��$
 Left=�  $ TotalFolios��$ TotalFolios��$
 Left=�  $ Realizo��$ Realizo��$
 Left=�  $ Fecha indexado��$ FechaIndexado��$
 Left=�7 
 1Syncfusion.UI.Xaml.Controls.DataPager.SfDataPager  -     Name$  controlCalidadUsuarioPager��$��1��$��2��$��27��$��Center=�$��Bottom=�$��Center=�    PageSize$  25��!   UseOnDemandPaging! . �� -   $��txtGridTotalUser��$��1��$��1��$	��147��$��26��$��0,0,0,0q�$
��Left=�$��Bottom=�$��Total registros: 0��   .  $ 	Collapsed=�$��	Públicas�� .  .��� .+�  -	   $ calidadPublicosGrid��$��0,0,0,32q� .   .  $
 Auto=�$	 Row=� .  �� *PagedSource+$ calidadPublicosPager��   $
 Lote��$
 Lote��$
 Left=�  $
 Caja��$
 Caja��$
 Left=�  $ Nro Expediente��$ NroExpediente��$
 Left=�  $ Nom Expediente��$ NomExpediente��$
 Left=�  $ Realizo��$ Realizo��$
 Left=�  $ Nro Carpeta��$ 
NroCarpeta��$
 Left=�  $ TotalFolios��$ TotalFolios��$
 Left=�  $ Fecha indexado��$ FechaIndexado��$
 Left=��� -
   $��txtGridTotalPublicitems��$��1��$��1��$	��147��$��26��$��5q�$
��Left=�$��Bottom=�$��Total registros: 0��  -   $ calidadPublicosPager��$��1��$��2��$��27��$��Center=�$��Bottom=�$��Center=�$  25��! . �� -   " ��Name$" btnActualizarCache��$��1��$	��140��$��48��$��	0,0,265,0q�$��Right=�$	��Top=�   MaterialDesignFlatButton#��[  .���� $��0,4,0,0q�# �� Orientation$# Vertical=�.+�'	  !MaterialDesignThemes.Wpf.PackIcon	  $��16��$��16��$��Center=�$ 	  Kind$$ Refresh=��� $��Center=�$��Center=�$��11(�$��Actualizar Cache��� -   % �Name$% pbarUpdateCache��& � Value$& 50Q�$y�	Collapsed=��� -   $" btnActualizar��$��1��$	��120��$��48��$��	0,0,140,0q�$��Right=�$	��Top=�#��[  .���� $# Vertical=�.+�	  $��16��$��16��$��Center=�$$ Refresh=��� $��Center=�$��11(�$��Actualizar KP��)
  #MaterialDesignThemes.Wpf.DialogHost
  -   ' 
 Name$' dlgEditCaja��$��1��$	��120��$��48��$��0,0,10,0q�$��Right=�$	��Top=�{�. ( 
  DialogContent( �� $��16q�$��Right=�.+��� Ingrese la caja de destino�� -   ) ��Name$) txtCaja���� $��Right=�$# 
Horizontal=�.+��� $��0,8,8,0q�$2��,materialDesign:DialogHost.CloseDialogCommand��* �� 	IsDefault* . #��[  ����True��.��	ACEPTAR�� $��0,8,8,0q�$2��,materialDesign:DialogHost.CloseDialogCommand��+ �� IsCancel+ . #��[  ����	False��.��
CANCELAR.���� -   $" btnEditCaja��$��1��$	��120��$��48��$��0,0,0,-6q�$��Right=�$	��Top=�, 
  OpenDialogCommand#��Z, {�. #��[  .���� $# Vertical=�.+�	  $��16��$��16��$��Center=�$$ FolderEditOutline=��� $��Center=�$��11(�$��
Edita Caja��  -   $
 tabControlCalidadParent�� .  $��Control calidad�� . $y�	Collapsed=�.��� -   $ calidadReviewContainer��.+�.  (Indexai.Views.ControlCalidadRevisionView  -   -  Name$- controCalidadReview������ $ calidadReviewContainer��$ ActualWidth����� $ calidadReviewContainer��$ ActualHeight�