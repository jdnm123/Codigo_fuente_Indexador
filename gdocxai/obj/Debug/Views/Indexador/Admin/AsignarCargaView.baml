   M S B A M L   `   `   `  ����B  >indexai, Version=2.4.6.3, Culture=neutral, PublicKeyToken=null!clr-namespace:IndexaiIndexai  5      $    Indexai.Views.AsignarCargaView   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      j fSyncfusion.SfGridConverter.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfBusyIndicator.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89h dSyncfusion.SfImageEditor.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Linq.Base, Version=18.4460.0.49, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a	 ]Syncfusion.SfGrid.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a
 ]Syncfusion.Shared.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Tools.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.SfInput.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89@
Syncfusion!http://schemas.syncfusion.com/wpf     	 
   5      Hd2http://schemas.microsoft.com/expression/blend/2008     	 
   5      1indexaiclr-namespace:Indexai     	 
   5      _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a?interactivity+http://schemas.microsoft.com/xaml/behaviors  5      b ^MaterialDesignThemes.Wpf, Version=4.0.0.2422, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5	      Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5
      $	��995��5      $	��500��5      .��5      � 
�������  � RowDefinitions  5   
   �� 5      $��68 �6   �� 5       � ColumnDefinitions 5   
   �� 5      $	��240 �6   �� 5      .+�5   
   3 
 -Syncfusion.Windows.Tools.Controls.ComboBoxAdv  -   5          Name$ cbx_Usrcontrol��5      $��34��5      $��	10,10,0,0q�5      $	��Top=�5         AllowMultiSelect . 5      ( 	 "Syncfusion.UI.Xaml.Grid.SfDataGrid  5   
   -   5          Name$ 
exportGrid��5      $��1��5       $��1��5!      $��
0,56,10,38q�5"         AllowFiltering .  ( 	 "Syncfusion.UI.Xaml.Grid.SfGridBase   AutoGenerateColumns .     ColumnSizer$
 Auto=�5%         SelectionUnit$
 Cell=�5'      	   ShowToolTip	 . 
   ItemsSource
 5&      ��  �� ElementName$ exportPager�� �� Path$ PagedSource�   Columns 5*      , 	 &Syncfusion.UI.Xaml.Grid.GridTextColumn  ( 	 "Syncfusion.UI.Xaml.Grid.GridColumn   AllowFiltering . , 	 &Syncfusion.UI.Xaml.Grid.GridColumnBase   
HeaderText$
 Lote��5,         MappingName$ t_lote.nom_lote��5-         TextAlignment$
 Left=�5.        5/      $
 Caja��50      $ nro_caja��51      $
 Left=�52        53      $ NumExpediente��54      $ nro_expediente��55      $
 Left=�56        57      $ 
Expediente��58      $ nom_expediente��59      $
 Left=�5:        5;      $ Carpeta��5<      $ nro_carpeta��5=      $
 Left=�5>        5?      $ Folios��5@      $ Folios��5A      $
 Left=�5B        5C      $ Paginas��5D      $ Paginas��5E      $
 Left=�5F        5G      $ Asignado��5H      $ Asignado��5I      $
 Left=�5J        5K      $ Indexado��5L      $ Indexado��5M      $
 Left=�5N        5O      $ Estado��5P      $ Estado��5Q      $
 Left=�5R      *  $Microsoft.Xaml.Behaviors.Interaction   	Behaviors 5T          Indexai.SfDataGridBehavior  5U      5W      	   Indexai.BuscadorLotes	  5Y   
   -   5         	 Name$ buscadorView��5Z      $��1��5[      $��0,7,0,0q�5\      7
 	 1Syncfusion.UI.Xaml.Controls.DataPager.SfDataPager
  5]   
   -   5         
 Name$ exportPager��5^      $��1��5_      $��1��5`      $��33��5a      $��5,0,0,0q�5b      $��Center=�5c      $��Bottom=�5d       
  PageSize$ 5��5e       
  UseOnDemandPaging . 5f      �� 5g   
   -   5         ��Name$ txtItemCount��5h      $��1��5i      $��1��5j      $	��150��5k      $��24��5l      $��	10,27,0,0q�5m      $
��Left=�5n      $	��Top=�5o      $�� ��5p       �� TextWrapping$
 Wrap=�5q      �� 5r   
   $��1��5s      $��1��5t      $��26��5u      $��10,1,-0.2,0q�5v      $	��Top=�5w      $$��Cantidad máxima de documentos��5x      �� 5y   
   -   5         ��Name$ btnMaximoItemsGrid��5z      $��1��5{      $��1��5|      $��86��5}      $��24��5~      $��
165,27,0,0q�5      $
��Left=�5�      $	��Top=�5�      $��Aplicar��5�      $��10(�5�      �� 5�   
   -   5        $ txtPorcientoAsignar��5�      $��1��5�      $��87��5�      $��32��5�      $��
211,10,0,0q�5�      $
��Left=�5�      $	��Top=�5�       J� RenderTransformOrigin$ 1.145,0.516&�5�      $�� ��5�      $
 Wrap=�5�      �� 5�   
   -   5        $ btnAsignarPorciento��5�      $��1��5�      $	��139��5�      $��32��5�      $��
302,10,0,0q�5�      $
��Left=�5�      $	��Top=�5�      $��Asignar��5�      �� 5�   
   -	   5        $ btnActualizar��5�      $��1��5�      $��97��5�      $��34��5�      $��	0,10,10,0q�5�      $��Right=�5�      $	��Top=�5�         MaterialDesignFlatButton#��[  5�      .��5�      ��  �� Orientation$ Vertical=�6   .+�5�      '  !MaterialDesignThemes.Wpf.PackIcon  $��16��5�      $��16��5�      $��Center=�5�         Kind$ Refresh=�5�      �� 5�      $��Center=�5�      $��11(�5�      $��
Actualizar��5�      5�      5�      �� 5�   
   $��1��5�      $	��185��5�      $��
10,16,0,26q�5�      $
��Left=�5�      $4��.Porcentaje de Asignación (1-100)             ��5�      �� 5�   
   -
   5        $��txtGridTotal��5�      $��1��5�      $��1��5�      $	��147��5�      $��26��5�      $��10,0,467.8,7q�5�      $��Center=�5�      $��Bottom=�5�      $��Total registros: 0��5�      �� 5�   
   -   5         ��Name$ chkMostrarTodo��5�      $��1��5�      $��1��5�      $	��150��5�      $��
260,32,0,0q�5�      $
��Left=�5�      $	��Top=�5�      $��Mostrar todo��5�      5�      �� 5�   
   -   5        $ btnCambiarUsuario��5�      $��1��5�      $	��159��5�      $��32��5�      $��
446,10,0,0q�5�      $
��Left=�5�      $	��Top=�5�      $��Cambiar usuario��5�      5�      5�      