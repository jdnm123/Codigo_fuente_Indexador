   M S B A M L   `   `   `  ����   indexai    Indexai.BuscadorLotes   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     j fSyncfusion.SfGridConverter.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfBusyIndicator.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89h dSyncfusion.SfImageEditor.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Linq.Base, Version=18.4460.0.49, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a	 ]Syncfusion.SfGrid.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a
 ]Syncfusion.Shared.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Tools.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.SfInput.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89=editors!http://schemas.syncfusion.com/wpf     	 
   b ^MaterialDesignThemes.Wpf, Version=4.0.0.2422, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  .��� 
�������.+�#  MaterialDesignThemes.Wpf.Card  $��1��$��5q�.��� $��10q�.+��� $
��Auto=�.����   �� Orientation$  Vertical=�.+��� $
��28.5��$	��Top=�$
��Lote���� -    ��Name$ 
txtNomLote��$��22��$
��10,0q�$	��Top=�$�� �� �� TextWrapping$
 Wrap=��� $��28��$	��Top=�$��Grupo / Caja���� -   $ txtNumCaja_parse��$��23��$
��10,0q�$	��Top=�$�� ��$
 Wrap=��� $��28��$	��Top=�$��Nro. Expediente���� -   $ txtCodCarpeta_parse��$��23��$
��10,0q�$	��Top=�$�� ��$
 Wrap=��� $��29��$	��Top=�$��
Expediente���� -   $ txtExpediente_parse��$��60��$
��10,0q�$	��Top=� �� AcceptsReturn . $�� ��$
 Wrap=��� $��28��$	��Top=�$��Nombre de usuario��$y�Visible=��� -   $ 
txtUsuario��$��23��$
��10,0q�$	��Top=�$�� ��$
 Wrap=�$y�Visible=��� $��28��$	��Top=�$��Número Carpeta��$y�Visible=��� -   $ txtNroCarpeta��$��23��$
��10,0q�$	��Top=�$�� ��$
 Wrap=�$y�Visible=��� $��28��$	��Top=�$��
Desde caja��$y�Visible=��� -   $ txtRangoMin��$��23��$
��10,0q�$	��Top=�$�� ��$
 Wrap=�$y�Visible=��� $��28��$	��Top=�$��
Hasta caja��$y�Visible=��� -   $ txtRangoMax��$��23��$
��10,0q�$	��Top=�$�� ��$
 Wrap=�$y�Visible=��� $��28��$��0,4,0,4q�$	��Top=�$��Fecha indexado��$y�Visible=�4  .Syncfusion.Windows.Controls.Input.SfDatePicker  -	     Name$ datePckFechaIndexado��$��23��$
��10,0q�$	��Top=�$y�Visible=�   Value$  X�� $��	0,10,0,10q�$��Bottom=� � ColumnDefinitions �� $	��55* ��� $	��55* �.+��� -
    ��Name$ 	btnBuscar��$��40��$��0,20,5,0q�$	��Top=�$��Buscar���� -   $ 
btnLimpiar��$��1��$��40��$��5,20,0,0q�$	��Top=�$��Limpiar��