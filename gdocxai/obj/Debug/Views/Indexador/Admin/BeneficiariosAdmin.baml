   M S B A M L   `   `   `  ����  indexai&     Indexai.Views.BeneficiariosAdmin   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      j fSyncfusion.SfGridConverter.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfBusyIndicator.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89h dSyncfusion.SfImageEditor.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Linq.Base, Version=18.4460.0.49, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a	 ]Syncfusion.SfGrid.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a
 ]Syncfusion.Shared.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Tools.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.SfInput.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89@
Syncfusion!http://schemas.syncfusion.com/wpf     	 
   5      Hd2http://schemas.microsoft.com/expression/blend/2008     	 
   5      Rmc;http://schemas.openxmlformats.org/markup-compatibility/2006     	 
   5      $��814.948��5      $��634.099��5	      $	��634��5
      
�������.��5      �   � ColumnDefinitions  5   
   �� 5      $
��188* �6   �� 5      $
��101* �6   �� 5      $
��100* �6   �� 5      $
��187* �6    � RowDefinitions 5   
   �� 5      $	��278 �6   �� 5      �� 5      $��80 �6   .+�5   
   ( 	 "Syncfusion.UI.Xaml.Grid.SfDataGrid  -   5          Name$ gridBeneficiarios��5      $��4��5      $��0,0,0,50q�5         AllowDeleting . ( 	 "Syncfusion.UI.Xaml.Grid.SfGridBase   AllowEditing .    AllowFiltering .    AllowResizingColumns .    AutoGenerateColumns .     ColumnSizer$
 Star=�5"      	   NavigationMode$		 Row=�5$      
   ItemsSource
 5#      �� *Beneficiarios+   Columns 5&      , 	 &Syncfusion.UI.Xaml.Grid.GridTextColumn  , 	 &Syncfusion.UI.Xaml.Grid.GridColumnBase   
HeaderText$ Nombre��5'         MappingName$ Nombre��5(         TextAlignment$
 Left=�5)        5*      $ 	Apellidos��5+      $ 	Apellidos��5,      $
 Left=�5-        5.      $
 Tipo��5/      $ TipoDocumento��50      $
 Left=�51        52      $ Numero documento��53      $ NumeroDocumento��54      $
 Left=�55        56      $ 	Principal��57      $ sol_principal��58      $
 Left=�59      5;      �� 5<   
   -   5         ��Name$ 
btnGuardar��5=      $��2��5>      $��2��5?      $��2��5@      $��36��5A      $��
10,0,10,10q�5B      $��Bottom=�5C      $��Guardar Todo��5E      $��9��5F      �� 5G   
   -   5        $ btnEliminarBeneficiario��5H      $��2��5I      $��34��5J      $��	15,0,10,0q�5K      $��Bottom=�5L      $��Eliminar Persona��5N      {�.  5O      �� 5P   
   $��1��5Q      $��4��5R      $��
10,5,10,10q�5S      $��Datos beneficiario��5T      ��5U      Z� .��5V      � $��
5,5,5.2,10q�6     5W      �� 5X      �� 5Y      .+�5[      �� $��26��5\      $��
11,91,10,0q�5]      $	��Top=�5^      $��NOMBRES��5_      �� 5a      $��23��5b      $��11,158,10,0q�5c      $	��Top=�5d      $��	APELLIDOS��5e      �� 5f      $��26��5g      $��	9,26,10,0q�5h      $	��Top=�5i      $��TIPO DE DOCUMENTO��5j      �� 5k      -   5         ��Name$ cbxTipoDocumento��5l      $��30��5m      $��
11,52,10,0q�5n      $	��Top=�5o       �� IsTextSearchEnabled . $��1��5q      .@�5r      �� $!��CC - 1.Cedula de ciudadania��6#   
 � Tag$ CC��6I   �� 5s      $!��TI - 2.Tarjeta de identidad��6#   $ TI��6I   �� 5t      $��RC - 3.Registro civil��6#   $ RC��6C   �� 5u      $#��CE - 4.Cedula de extranjería��6#   $ CE��6J   �� 5v      $)��#RD - 5.Registro civil de defunción��6#   $ RD��6P   �� 5w      $ ��PB - 6.Partida de Bautismo��6#   $ PB��6H   �� 5x      $"��PD - 7.Partida de defunción��6#   $ PD��6I   �� 5y      $��PS - 8.Pasaporte��6#   $ PS��6>   �� 5z      $&�� SI - 9.Menor sin identificación��6#   $ SI��6M   �� 5{      $+��%RM - 10. Registro Civil de Matrimonio��6#   $ RM��6S   �� 5|      $��NR - No registra��6#   $ NR��6>   5}      �� 5~      $��1��5      $��26��5�      $��
10,10,10,0q�5�      $	��Top=�5�      $��NUMERO DOCUMENTO��5�      �� 5�      -   5        $ 
btnAñadir��5�      $��1��5�      $	��247��5�      $��37��5�      $��	0,0,10,10q�5�      $��Right=�5�      $��Bottom=�5�      $��Adicionar / Actualizar��5�      $��8��5�      �� 5�      $��1��5�      $��	10,41,6,0q�5�      $	��Top=�5�       K� ZIndex$ 90��5�      
�������.+�5�      �� -   5         ��Name$ txtNumeroDocumento��5�      $��33��5�      $	��Top=�5�       �� CharacterCasing$ Upper=�5�      $��2��5�      $�� ��5�       �� TextWrapping$
 Wrap=�5�      �� 5�      $��2��5�      $	��Top=�5�      $��3��5�      $y�Visible=�5�      5�      �� 5�      $��11,180,10,0q�5�      $	��Top=�5�      
�������.+�5�      �� -   5        $ txtApellidos��5�      $��1��5�      $��2��5�      $��30��5�      $	��Top=�5�      $ Upper=�5�      $��6��5�      $�� ��5�      $
 Wrap=�5�      �� 5�      $��2��5�      $	��Top=�5�      
�������$��7��5�      $y�Visible=�5�      5�      �� 5�      $��11,122,10,0q�5�      $	��Top=�5�      
�������.+�5�      �� -   5        $ 
txtNombres��5�      $��1��5�      $��2��5�      $��30��5�      $	��Top=�5�      $ Upper=�5�      $��4��5�      $�� ��5�      $
 Wrap=�5�      �� 5�      $��2��5�      $	��Top=�5�      
�������$��5��5�      $y�Visible=�5�      5�      �� 5�      -	   5         ��Name$ chkSolicitante��5�      $��15��5�      $��9,6,10,0q�5�      $	��Top=�5�      $��	Principal��5�      $��0��5�      5�      5�      �� 5�   
   -
   5        $ btnNuevoBeneficiario��5�      $��2��5�      $��2��5�      $��34��5�      $
��10,0q�5�      $��Bottom=�5�      $��Nueva Persona��5�      {�.  5�      5�      5�      