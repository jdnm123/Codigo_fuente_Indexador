   M S B A M L   `   `   `  ����   indexai&     Indexai.Views.BeneficiariosAdmin   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     j fSyncfusion.SfGridConverter.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfBusyIndicator.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89h dSyncfusion.SfImageEditor.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Linq.Base, Version=18.4460.0.49, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a	 ]Syncfusion.SfGrid.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a
 ]Syncfusion.Shared.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Tools.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.SfInput.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89@
Syncfusion!http://schemas.syncfusion.com/wpf     	 
   Hd2http://schemas.microsoft.com/expression/blend/2008     	 
   Rmc;http://schemas.openxmlformats.org/markup-compatibility/2006     	 
   $��814.948��$��634.099��$	��634��
�������.���   � ColumnDefinitions  �� $
��188* ��� $
��101* ��� $
��100* ��� $
��187* � � RowDefinitions �� $	��278 ��� �� $��80 �.+�( 	 "Syncfusion.UI.Xaml.Grid.SfDataGrid  -     Name$ gridBeneficiarios��$��4��$��0,0,0,50q�   AllowDeleting . ( 	 "Syncfusion.UI.Xaml.Grid.SfGridBase   AllowEditing .    AllowFiltering .    AllowResizingColumns .    AutoGenerateColumns .     ColumnSizer$
 Star=�	   NavigationMode$		 Row=�
   ItemsSource
 �� *Beneficiarios+   Columns , 	 &Syncfusion.UI.Xaml.Grid.GridTextColumn  , 	 &Syncfusion.UI.Xaml.Grid.GridColumnBase   
HeaderText$ Nombre��   MappingName$ Nombre��   TextAlignment$
 Left=�  $ 	Apellidos��$ 	Apellidos��$
 Left=�  $
 Tipo��$ TipoDocumento��$
 Left=�  $ Numero documento��$ NumeroDocumento��$
 Left=�  $ 	Principal��$ sol_principal��$
 Left=��� -    ��Name$ 
btnGuardar��$��2��$��2��$��2��$��36��$��
10,0,10,10q�$��Bottom=�$��Guardar Todo��$��9���� -   $ btnEliminarBeneficiario��$��2��$��34��$��	15,0,10,0q�$��Bottom=�$��Eliminar Persona��{�.  �� $��1��$��4��$��
10,5,10,10q�$��Datos beneficiario����Z� .��� $��
5,5,5.2,10q�  �� �� .+��� $��26��$��
11,91,10,0q�$	��Top=�$��NOMBRES���� $��23��$��11,158,10,0q�$	��Top=�$��	APELLIDOS���� $��26��$��	9,26,10,0q�$	��Top=�$��TIPO DE DOCUMENTO���� -    ��Name$ cbxTipoDocumento��$��30��$��
11,52,10,0q�$	��Top=� �� IsTextSearchEnabled . $��1��.@��� $!��CC - 1.Cedula de ciudadania��
 � Tag$ CC���� $!��TI - 2.Tarjeta de identidad��$ TI���� $��RC - 3.Registro civil��$ RC���� $#��CE - 4.Cedula de extranjería��$ CE���� $)��#RD - 5.Registro civil de defunción��$ RD���� $ ��PB - 6.Partida de Bautismo��$ PB���� $"��PD - 7.Partida de defunción��$ PD���� $��PS - 8.Pasaporte��$ PS���� $&�� SI - 9.Menor sin identificación��$ SI���� $+��%RM - 10. Registro Civil de Matrimonio��$ RM���� $��NR - No registra��$ NR���� $��1��$��26��$��
10,10,10,0q�$	��Top=�$��NUMERO DOCUMENTO���� -   $ 
btnAñadir��$��1��$	��247��$��37��$��	0,0,10,10q�$��Right=�$��Bottom=�$��Adicionar / Actualizar��$��8���� $��1��$��	10,41,6,0q�$	��Top=� K� ZIndex$ 90��
�������.+��� -    ��Name$ txtNumeroDocumento��$��33��$	��Top=� �� CharacterCasing$ Upper=�$��2��$�� �� �� TextWrapping$
 Wrap=��� $��2��$	��Top=�$��3��$y�Visible=��� $��11,180,10,0q�$	��Top=�
�������.+��� -   $ txtApellidos��$��1��$��2��$��30��$	��Top=�$ Upper=�$��6��$�� ��$
 Wrap=��� $��2��$	��Top=�
�������$��7��$y�Visible=��� $��11,122,10,0q�$	��Top=�
�������.+��� -   $ 
txtNombres��$��1��$��2��$��30��$	��Top=�$ Upper=�$��4��$�� ��$
 Wrap=��� $��2��$	��Top=�
�������$��5��$y�Visible=��� -	    ��Name$ chkSolicitante��$��15��$��9,6,10,0q�$	��Top=�$��	Principal��$��0���� -
   $ btnNuevoBeneficiario��$��2��$��2��$��34��$
��10,0q�$��Bottom=�$��Nueva Persona��{�.  