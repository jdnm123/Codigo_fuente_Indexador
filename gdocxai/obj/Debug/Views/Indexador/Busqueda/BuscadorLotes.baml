   M S B A M L   `   `   `  ����  indexai    Indexai.BuscadorLotes   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      j fSyncfusion.SfGridConverter.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfBusyIndicator.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89h dSyncfusion.SfImageEditor.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Linq.Base, Version=18.4460.0.49, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a	 ]Syncfusion.SfGrid.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a
 ]Syncfusion.Shared.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Tools.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.SfInput.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89=editors!http://schemas.syncfusion.com/wpf     	 
   5      b ^MaterialDesignThemes.Wpf, Version=4.0.0.2422, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5      Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5      .��5
      � 
�������.+�5   
   #  MaterialDesignThemes.Wpf.Card  $��1��6   $��5q�6+   .��5      � $��10q�6   .+�5      �� $
��Auto=�6   .��5      ��   �� Orientation$  Vertical=�6!   .+�5      �� $
��28.5��5      $	��Top=�5      $
��Lote��5      �� 5      -   5         ��Name$ 
txtNomLote��5      $��22��5      $
��10,0q�5      $	��Top=�5      $�� ��5       �� TextWrapping$
 Wrap=�5      �� 5      $��28��5      $	��Top=�5      $��Grupo / Caja��5      �� 5!      -   5        $ txtNumCaja_parse��5"      $��23��5#      $
��10,0q�5$      $	��Top=�5%      $�� ��5(      $
 Wrap=�5)      �� 5+      $��28��5,      $	��Top=�5-      $��Nro. Expediente��5.      �� 5/      -   5        $ txtCodCarpeta_parse��50      $��23��51      $
��10,0q�52      $	��Top=�53      $�� ��56      $
 Wrap=�57      �� 58      $��29��59      $	��Top=�5:      $��
Expediente��5;      �� 5<      -   5        $ txtExpediente_parse��5=      $��60��5>      $
��10,0q�5?      $	��Top=�5@       �� AcceptsReturn . $�� ��5D      $
 Wrap=�5E      �� 5F      $��28��5G      $	��Top=�5H      $��Nombre de usuario��5I      $y�Visible=�5J      �� 5K      -   5        $ 
txtUsuario��5L      $��23��5M      $
��10,0q�5N      $	��Top=�5O      $�� ��5R      $
 Wrap=�5S      $y�Visible=�5T      �� 5U      $��28��5V      $	��Top=�5W      $��Número Carpeta��5X      $y�Visible=�5Y      �� 5Z      -   5        $ txtNroCarpeta��5[      $��23��5\      $
��10,0q�5]      $	��Top=�5^      $�� ��5a      $
 Wrap=�5b      $y�Visible=�5c      �� 5d      $��28��5e      $	��Top=�5f      $��
Desde caja��5g      $y�Visible=�5h      �� 5i      -   5        $ txtRangoMin��5j      $��23��5k      $
��10,0q�5l      $	��Top=�5m      $�� ��5p      $
 Wrap=�5q      $y�Visible=�5r      �� 5s      $��28��5t      $	��Top=�5u      $��
Hasta caja��5v      $y�Visible=�5w      �� 5x      -   5        $ txtRangoMax��5y      $��23��5z      $
��10,0q�5{      $	��Top=�5|      $�� ��5      $
 Wrap=�5�      $y�Visible=�5�      �� 5�      $��28��5�      $��0,4,0,4q�5�      $	��Top=�5�      $��Fecha indexado��5�      $y�Visible=�5�      4  .Syncfusion.Windows.Controls.Input.SfDatePicker  5�      -	   5          Name$ datePckFechaIndexado��5�      $��23��5�      $
��10,0q�5�      $	��Top=�5�      $y�Visible=�5�         Value$  X�5�      � 5�      $��	0,10,0,10q�6   $��Bottom=�62    � ColumnDefinitions 5�      �� 5�   "   $	��55* �63   �� 5�   "   $	��55* �63   .+�5�      �� -
   5         ��Name$ 	btnBuscar��5�   !   $��40��5�   !   $��0,20,5,0q�5�   !   $	��Top=�5�   !   $��Buscar��5�   !   �� 5�      -   5        $ 
btnLimpiar��5�   !   $��1��5�   !   $��40��5�   !   $��5,20,0,0q�5�   !   $	��Top=�5�   !   $��Limpiar��5�   !   5�      5�      5�      5�      5�      5�      5�      