   M S B A M L   `   `   `  ���� B  >indexai, Version=2.4.6.3, Culture=neutral, PublicKeyToken=null!clr-namespace:IndexaiIndexai  $    Indexai.Views.AsignarCargaView   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     j fSyncfusion.SfGridConverter.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfBusyIndicator.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89h dSyncfusion.SfImageEditor.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Linq.Base, Version=18.4460.0.49, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a	 ]Syncfusion.SfGrid.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a
 ]Syncfusion.Shared.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Tools.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.SfInput.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89@
Syncfusion!http://schemas.syncfusion.com/wpf     	 
   Hd2http://schemas.microsoft.com/expression/blend/2008     	 
   1indexaiclr-namespace:Indexai     	 
   _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a?interactivity+http://schemas.microsoft.com/xaml/behaviors  b ^MaterialDesignThemes.Wpf, Version=4.0.0.2422, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  $	��995��$	��500��.��� 
�������  � RowDefinitions  �� $��68 ���  � ColumnDefinitions �� $	��240 ��� .+�3 
 -Syncfusion.Windows.Tools.Controls.ComboBoxAdv  -     Name$ cbx_Usrcontrol��$��34��$��	10,10,0,0q�$	��Top=�   AllowMultiSelect . ( 	 "Syncfusion.UI.Xaml.Grid.SfDataGrid  -     Name$ 
exportGrid��$��1��$��1��$��
0,56,10,38q�   AllowFiltering .  ( 	 "Syncfusion.UI.Xaml.Grid.SfGridBase   AutoGenerateColumns .     ColumnSizer$
 Auto=�   SelectionUnit$
 Cell=�	   ShowToolTip	 . 
   ItemsSource
 ��  �� ElementName$ exportPager�� �� Path$ PagedSource�   Columns , 	 &Syncfusion.UI.Xaml.Grid.GridTextColumn  ( 	 "Syncfusion.UI.Xaml.Grid.GridColumn   AllowFiltering . , 	 &Syncfusion.UI.Xaml.Grid.GridColumnBase   
HeaderText$
 Lote��   MappingName$ t_lote.nom_lote��   TextAlignment$
 Left=�  $
 Caja��$ nro_caja��$
 Left=�  $ NumExpediente��$ nro_expediente��$
 Left=�  $ 
Expediente��$ nom_expediente��$
 Left=�  $ Carpeta��$ nro_carpeta��$
 Left=�  $ Folios��$ Folios��$
 Left=�  $ Paginas��$ Paginas��$
 Left=�  $ Asignado��$ Asignado��$
 Left=�  $ Indexado��$ Indexado��$
 Left=�  $ Estado��$ Estado��$
 Left=�*  $Microsoft.Xaml.Behaviors.Interaction   	Behaviors     Indexai.SfDataGridBehavior  	   Indexai.BuscadorLotes	  -    	 Name$ buscadorView��$��1��$��0,7,0,0q�7
 	 1Syncfusion.UI.Xaml.Controls.DataPager.SfDataPager
  -    
 Name$ exportPager��$��1��$��1��$��33��$��5,0,0,0q�$��Center=�$��Bottom=� 
  PageSize$ 5�� 
  UseOnDemandPaging . �� -    ��Name$ txtItemCount��$��1��$��1��$	��150��$��24��$��	10,27,0,0q�$
��Left=�$	��Top=�$�� �� �� TextWrapping$
 Wrap=��� $��1��$��1��$��26��$��10,1,-0.2,0q�$	��Top=�$$��Cantidad máxima de documentos���� -    ��Name$ btnMaximoItemsGrid��$��1��$��1��$��86��$��24��$��
165,27,0,0q�$
��Left=�$	��Top=�$��Aplicar��$��10(��� -   $ txtPorcientoAsignar��$��1��$��87��$��32��$��
211,10,0,0q�$
��Left=�$	��Top=� J� RenderTransformOrigin$ 1.145,0.516&�$�� ��$
 Wrap=��� -   $ btnAsignarPorciento��$��1��$	��139��$��32��$��
302,10,0,0q�$
��Left=�$	��Top=�$��Asignar���� -	   $ btnActualizar��$��1��$��97��$��34��$��	0,10,10,0q�$��Right=�$	��Top=�   MaterialDesignFlatButton#��[  .����  �� Orientation$ Vertical=�.+�'  !MaterialDesignThemes.Wpf.PackIcon  $��16��$��16��$��Center=�   Kind$ Refresh=��� $��Center=�$��11(�$��
Actualizar���� $��1��$	��185��$��
10,16,0,26q�$
��Left=�$4��.Porcentaje de Asignación (1-100)             ���� -
   $��txtGridTotal��$��1��$��1��$	��147��$��26��$��10,0,467.8,7q�$��Center=�$��Bottom=�$��Total registros: 0���� -    ��Name$ chkMostrarTodo��$��1��$��1��$	��150��$��
260,32,0,0q�$
��Left=�$	��Top=�$��Mostrar todo���� -   $ btnCambiarUsuario��$��1��$	��159��$��32��$��
446,10,0,0q�$
��Left=�$	��Top=�$��Cambiar usuario��