   M S B A M L   `   `   `  ���� d  `Syncfusion.PdfViewer.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89mLclr-namespace:Syncfusion.Windows.PdfViewer;assembly=Syncfusion.PdfViewer.WPFSyncfusion.Windows.PdfViewer  O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem B >indexai, Version=2.4.6.3, Culture=neutral, PublicKeyToken=null!clr-namespace:IndexaiIndexai .   (Indexai.Views.ControlCalidadRevisionView   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     b	PdfViewerLclr-namespace:Syncfusion.Windows.PdfViewer;assembly=Syncfusion.PdfViewer.WPF     j fSyncfusion.SfGridConverter.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfBusyIndicator.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89h	 dSyncfusion.SfImageEditor.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89`
 \Syncfusion.Linq.Base, Version=18.4460.0.49, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a ]Syncfusion.SfGrid.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a ]Syncfusion.Shared.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Tools.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.SfInput.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89@
Syncfusion!http://schemas.syncfusion.com/wpf   	 
     Hd2http://schemas.microsoft.com/expression/blend/2008   	 
     b ^MaterialDesignThemes.Wpf, Version=4.0.0.2422, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  3system&clr-namespace:System;assembly=mscorlib  !utilsclr-namespace:Indexai  $
��1409��  � 	Resources     
SpaceStyle �� 
TargetType �� Property �� Value��%2   &	        �� " �.���  � ��$ 8,3,0,0q�.���  � RowDefinitions �� �� $��91 ��� $	��220 � � ColumnDefinitions �� �� $	��149 ��� $	��138 ��� $	��137 �.+��� $��3��$��2��$��2��$��
6,57,10,10q�$��Indices��.��� -    �Name$ rowDetailGroup��$��5q�.+�  Indexai.IndexarDocumentos  -     Name$ rowDetailEdit��$
��Left=�$y�Hidden=�����  �� ElementName$ rowDetailGroup��	 �� Path$	 ActualWidth����� $ rowDetailGroup��$	 ActualHeight�5  /Syncfusion.Windows.Tools.Controls.TabControlExt  -   $��controlCalidadTab��$��3��$��0,0,0,45q�
   AllowDragDrop
 .     CloseButtonType$
 Both=�   EnableLabelEdit .     IsNewButtonEnabled .     ShowTabItemContextMenu .     ShowTabListContextMenu .      TabScrollButtonVisibility$
 Auto=�   TabScrollStyle$ Extended=� �� TabStripPlacement$	 Top=�.@�2  ,Syncfusion.Windows.Tools.Controls.TabItemExt     CanClose .     CloseButtonState$ 	Collapsed=�$��	Indexados�� �� 
IsSelected .  .���  �� $
��169* �.+�>  8Syncfusion.Windows.Controls.Notification.SfBusyIndicator  -     Name$ BusyIndicator��   AnimationType$ DualRing=�   Header$ Cargando PDF��   IsBusy .  � $	��142��$��45��$��
355,10,0,0q�$
��Left=�$	��Top=�.+�)  #MaterialDesignThemes.Wpf.DialogHost  -   $��0,0,0,0q�   DialogContent �� $��16q�.+��� $��4q�Ordenar de forma:�� $��4q�.+�	� -    	�Name$ rdbtnAscendente�� 	� 	GroupName$ orden�� d� 	IsChecked$
 TrueY�.��
Ascendente	� -   $ rdbtnDescendente��$ orden��.��Descendente�� $��0,8,0,0q�Iniciar desde:�� -    ��Name$ txtInicioOrden��$��0,8,0,0q�$��Stretch=�$��1���� $��Right=� �� Orientation$ 
Horizontal=�.+��� $��0,8,8,0q�$2��,materialDesign:DialogHost.CloseDialogCommand��  �� 	IsDefault  .   MaterialDesignFlatButton#��[ ����True��.��	ORDENAR�� $��0,8,8,0q�$2��,materialDesign:DialogHost.CloseDialogCommand��! �� IsCancel! . #��[ ����	False��.��
CANCELAR.���� -	   " ��Name$" 
btnOrdenar��$��0��$��45��$��0,0,0,0q�#   OpenDialogCommand#��Z# #��[ .���� $��0,4,0,0q�$ Vertical=�.+�'  !MaterialDesignThemes.Wpf.PackIcon  $��16��$��16��$��Center=�$   Kind$
$ Sort=��� $��Center=�$��Center=�$��12(�$��ORDENAR���� -
   $" btnBorrarCache��$��1��$��97��$��42��$��
0,10,110,0q�$��Right=�$	��Top=�#��[ .���� $ Vertical=�.+�  $��16��$��16��$��Center=�$$ FileCancelOutline=��� $��Center=�$��11(�$��Borrar Cache���� -   $" btnActualizar��$��1��$	��110��$��42��$��0,10,10.2,0q�$��Right=�$	��Top=�#��[ .���� $ Vertical=�.+�  $��16��$��16��$��Center=�$$ Refresh=��� $��Center=�$��11(�$��Actualizar Docs��(  "Syncfusion.UI.Xaml.Grid.SfDataGrid  -   %  Name$% controlCalidadGrid��$��	0,57,0,40q�&   AllowFiltering& .  (  "Syncfusion.UI.Xaml.Grid.SfGridBase'   AutoGenerateColumns' .  (   ColumnSizer$( AutoWithLastColumnFill=�)   NavigationMode$	) Row=�*   ShowToolTip* . +   ItemsSource+ �� $ calidadPager��$	 PagedSource�,   Columns, ,	  &Syncfusion.UI.Xaml.Grid.GridTextColumn	  ,
  &Syncfusion.UI.Xaml.Grid.GridColumnBase- 
  Width$- 60Q�(  "Syncfusion.UI.Xaml.Grid.GridColumn.   AllowGrouping. . / 
  AllowSorting/ .  0 
  
HeaderText$0 Id��1 
  MappingName$1 Id��2 
  TextAlignment$
2 Left=�	  $- 60Q�. . / .  $0 Pers.��$1 TotalTerceros��$
2 Left=�	  $	- 200Q�/ .  $0 
Expediente��$1 NumExpediente��$
2 Left=�	  $	- 280Q�. . / .  $0 
Archivador��$1 
Archivador��$
2 Left=�	  / .  $0 	Folio Ini��$1 FolioIni��$
2 Left=�	  / .  $0 	Folio Fin��$1 FolioFin��$
2 Left=�	  / .  $0 Pág Inicio��$1 PagIni��$
2 Left=�	  / .  $0 Pág Fin��$1 PagFin��$
2 Left=�7  1Syncfusion.UI.Xaml.Controls.DataPager.SfDataPager  -   3  Name$3 calidadPager��$��40��$��Center=�$��Bottom=�$��Center=�4   NumericButtonCount$4 10��5   PageSize$5 30��6   UseOnDemandPaging6 . �� -   $" btnEditarBeneficiarios��$	��116��$��42��$��	10,10,0,0q�$
��Left=�$	��Top=�{�.  #��[ .���� $ Vertical=�.+�  $��16��$��16��$��Center=�$$ 
UsersGroup=��� $��Center=�$��11(�$��Ver beneficiarios���� -   $" btnDatosLote��$	��107��$��42��$��
131,10,0,0q�$
��Left=�$	��Top=�#��[ .���� $��40��$��0,4,0,0q�$ Vertical=�.+�  $��16��$��16��$��Center=�$$ PersonCardDetails=��� $��Center=�$��11(�$��Datos básicos���� -   $��txtGridTotalitems��$��1��$��1��$	��147��$��26��$��0,0,0,0q�$
��Left=�$��Bottom=�$��Total registros: 0���� -   $" btnInsertarPdf��$	��107��$��42��$��
243,10,0,0q�$
��Left=�$	��Top=�{�.  #��[ .���� $��40��$��0,4,0,0q�$ Vertical=�.+�  $��16��$��16��$��Center=�$	$ Add=��� $��Center=�$��11(�$��Insertar PDF��   .  $ 	Collapsed=�$��Imagen�� . .��3   -Syncfusion.Windows.PdfViewer.PdfViewerControl  -   $��	pdfviewer��7   EnableNotificationBar7 .  8   RenderingEngine$8 Pdfium=��� -   9 ��Name$9 lbxPdfImages��$��3��$��1��$��10q����� .R��� 
������ $ Vertical=����� .R��� $ Vertical=�.+��� $	��100��$	��135��$��Center=�$	��Top=�$
��Fill=����� *Source+�� $��Stretch=�$��Bottom=�: �� TextAlignment$: Center=����� *Index+�� -   $" btnAceptarCarpeta��$��2��$��2��$��42��$��
10,10,10,0q�$	��Top=�$��Aceptar carpeta���� $��2��$��45��$��Center=�$��Bottom=�$ 
Horizontal=�  ; �� BasedOn��%   '�       7   �� 8;   " �.+�� .+�� $��0,0,0,0q�.+�  -    �� $��16q�.+��� �� $3�-Ingrese la cantidad para aumentar el índice:���� -   $ txtAumentar���� $��Right=�$ 
Horizontal=�.+��� $��0,8,8,0q�$2��,materialDesign:DialogHost.CloseDialogCommand��$��ACEPTAR��  . #��[ ����True���� $��0,8,8,0q�$2��,materialDesign:DialogHost.CloseDialogCommand��$��CANCELAR��! . #��[ ����	False��.���� -   $" btnAumentarIndex��$	��150��#��Z# {�.  .���� $��0,4,0,0q�$ 
Horizontal=�.+�  $��16��$��16��$
��Left=�$$ 
PlusCircle=��� $��Center=�$��Center=�$��12(�$��  Aumentar índice��� $��2��$
��Left=�$��Bottom=�.+��� $��0,0,0,0q�$ Vertical=�.+�  -   $��44��$��Center=� �� $��16q�.+��� 0.Ingrese la cantidad para disminuir el índice:�� -   $ txtDisminuir���� $��Right=�$ 
Horizontal=�.+��� $��0,8,8,0q�$2��,materialDesign:DialogHost.CloseDialogCommand��  . #��[ ����True��.��	ACEPTAR�� $��0,8,8,0q�$2��,materialDesign:DialogHost.CloseDialogCommand��! . #��[ ����	False��.��
CANCELAR.���� -   $" btnDisminuirIndex��$	��150��#��Z# {�.  .���� $��0,4,0,0q�$ 
Horizontal=�.+�  $��16��$��16��$
��Left=�$$ MinusCircleOutline=��� $��Center=�$��Center=�$��12(�$��  Disminuir índice��� .+�� $��0,0,0,0q�.+�  -    �� $��16q�.+��� �� $�Página inicial:���� -   $ 	txtPagIni���� �� $�Página final:���� -   $ 	txtPagFin���� $��Right=�$ 
Horizontal=�.+��� $��0,8,8,0q�$2��,materialDesign:DialogHost.CloseDialogCommand��$��ACEPTAR��  . #��[ ����True���� $��0,8,8,0q�$2��,materialDesign:DialogHost.CloseDialogCommand��$��CANCELAR��! . #��[ ����	False��.���� -   $" btnCambiaRango��$	��140��#��Z# {�.  .���� $��0,4,0,0q�$ 
Horizontal=�.+�  $��16��$��16��$
��Left=�$$ ContainStart=�  $��16��$��16��$
��Left=�$$ 
ContainEnd=��� $��Center=�$��Center=�$��12(�$��Editar Rango��� .+�  -   $��0,0,0,0q� �� $��16q�.+��� !Observación para el indexador.�� -    $ txtMensajeTranscriptores��$��0,8,0,0q�$��Stretch=��� -!   < ��Name$< chkEliminarTodo��$��10q�$#��Eliminar todos los documentos���� $��Right=�$ 
Horizontal=�.+��� $��0,8,8,0q�$2��,materialDesign:DialogHost.CloseDialogCommand��  . #��[ ����True��.��
RECHAZAR�� $��0,8,8,0q�$2��,materialDesign:DialogHost.CloseDialogCommand��! . #��[ ����	False��.��
CANCELAR.���� -"   $" btnAdminDeleteDocumento��$��0��$	��140��$��0,0,0,0q�#��Z# {�.  .���� $��0,4,0,0q�$ 
Horizontal=�.+�  $��16��$��16��$
��Left=�$$ DeleteSweep=��� $��Center=�$��Center=�$��12(�$��
  Eliminar��� .+�  -#   $��0,0,0,0q� �� $��16q�.+��� Dividir en:�� -$   $ txtDividirEn��$��0,8,0,0q�$��Stretch=��� $��Right=�$ 
Horizontal=�.+��� $��0,8,8,0q�$2��,materialDesign:DialogHost.CloseDialogCommand��  . #��[ ����True��.��	DIVIDIR�� $��0,8,8,0q�$2��,materialDesign:DialogHost.CloseDialogCommand��! . #��[ ����	False��.��
CANCELAR.���� -%   $" 
btnDividir��$��0��$	��140��$��0,0,0,0q�#��Z# {�.  .���� $��0,4,0,0q�$ 
Horizontal=�.+�  $��16��$��16��$
��Left=�$$ FormatAlignMiddle=��� $��6,0,0,0q�$��Center=�$��Center=�$��12(�$��Dividir��