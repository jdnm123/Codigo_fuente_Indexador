   M S B A M L   `   `   `  ����   indexai'    !Indexai.InformacionComplementaria   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     j fSyncfusion.SfGridConverter.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfBusyIndicator.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89h dSyncfusion.SfImageEditor.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Linq.Base, Version=18.4460.0.49, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a	 ]Syncfusion.SfGrid.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a
 ]Syncfusion.Shared.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Tools.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.SfInput.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89@
Syncfusion!http://schemas.syncfusion.com/wpf     	 
   Hd2http://schemas.microsoft.com/expression/blend/2008     	 
   b ^MaterialDesignThemes.Wpf, Version=4.0.0.2422, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  -     � Title$*  $Registro información complementaria��$	��440��$��668.255�� � WindowStartupLocation$ Manual=�.���  � ColumnDefinitions �� $
��188* ��� $
��101* ��� $
��100* ��� $
��187* � � RowDefinitions �� $	��280 ��� �� $��60 �.+�( 	 "Syncfusion.UI.Xaml.Grid.SfDataGrid  -     Name$ gridBeneficiarios��$��4��$��0,0,0,50q�   AllowDeleting . ( 	 "Syncfusion.UI.Xaml.Grid.SfGridBase   AllowEditing .    AllowFiltering .    AllowResizingColumns . 	   AutoGenerateColumns	 .  
   NavigationMode$	
 Row=�   ItemsSource �� *BeneficiariosColl+   Columns , 	 &Syncfusion.UI.Xaml.Grid.GridTextColumn  , 	 &Syncfusion.UI.Xaml.Grid.GridColumnBase   Width$	 110Q�   
HeaderText$ Nombre��   MappingName$ Nombre��   TextAlignment$
 Left=�  $	 110Q�$ 	Apellidos��$ 	Apellidos��$
 Left=�  $ 50Q�$ Tipo de documento��$ TipoDocumento��$
 Left=�  $	 110Q�$ Numero documento��$ NumeroDocumento��$
 Left=�  $ 50Q�$ 	Principal��$ sol_principal��$
 Left=��� -    ��Name$ btnNuevoBeneficiario��$��2��$��2��$��34��$
��10,0q�$��Bottom=�$��Nueva Persona��{�.  �� -   $ btnEliminarBeneficiario��$��2��$��34��$��	15,0,10,0q�$��Bottom=�$��Eliminar beneficiario��{�.  �� $��1��$��4��$��3,5,3,0q�$��Datos beneficiario����Z� .��� $��2,2,2,2q� �� �� .+��� -    ��Name$ chkSolicitante��$��18��$��9,5,10,0q�$	��Top=�$��	Principal��$��0���� $��26��$��	9,26,10,0q�$	��Top=�$��TIPO DE DOCUMENTO���� -    ��Name$ cbxTipoDocumento��$��30��$��3,45,3,0q�$	��Top=� �� IsTextSearchEnabled . $��1��.@��� $!��CC - 1.Cedula de ciudadania��
 � Tag$ CC���� $!��TI - 2.Tarjeta de identidad��$ TI���� $��RC - 3.Registro civil��$ RC���� $#��CE - 4.Cedula de extranjería��$ CE���� $)��#RD - 5.Registro civil de defunción��$ RD���� $ ��PB - 6.Partida de Bautismo��$ PB���� $"��PD - 7.Partida de defunción��$ PD���� $��PS - 8.Pasaporte��$ PS���� $&�� SI - 9.Menor sin identificación��$ SI���� $+��%RM - 10. Registro Civil de Matrimonio��$ RM���� $��NR - No registra��$ NR���� $��1��$��26��$��
10,26,10,0q�$	��Top=�$��NUMERO DOCUMENTO���� $��1��$��3,50,3,0q�$	��Top=� K� ZIndex$	 200��
�������.+��� -    ��Name$ txtNumeroDocumento��$��25��$	��Top=� �� CharacterCasing$ Upper=�$��2��$�� �� �� TextWrapping$
 Wrap=��� $��2��$	��Top=�$	 201��
�������$��11(�$��3��$y�Visible=�����  �� 
TargetType" ��.���  �� Property � �� �� Value$ 0,0,0,0q��� $��26��$��
10,80,10,0q�$	��Top=�$��NOMBRES���� $��	3,100,3,0q�$	��Top=�
�������.+��� -   $ 
txtNombres��$��1��$��2��$��25��$	��Top=�$ Upper=�$��4��$�� ��$
 Wrap=��� $��2��$	��Top=�
�������$��11(�$y�Visible=����� " ��.���  � ��$ 0,0,0,0q��� $��1��$��26��$��10.4,80,10.2,0q�$	��Top=�$��	APELLIDOS���� $��1��$��	3,100,3,0q�$	��Top=�$ 50��
�������.+��� -	   $ txtApellidos��$��1��$��2��$��25��$	��Top=�$ Upper=�$��6��$�� ��$
 Wrap=��� $��2��$	��Top=�$ 51��
�������$��11(�$y�Visible=����� " ��.���  � ��$ 0,0,0,0q��� -
   $ 
btnAñadir��$��1��$	��165��$��32��$��0,0,5,0q�$��Right=�$��Bottom=�$ 15��$��Adicionar / Actualizar��$��8���� -   $ 
btnGuardar��$��2��$��2��$��2��$��36��$��
10,0,10,10q�$��Bottom=�$��9��.���� $��0,4,0,0q� �� Orientation$ 
Horizontal=�.+�'  !MaterialDesignThemes.Wpf.PackIcon  $��16��$��16��$
��Left=�   Kind$ ContentSave=��� $��Center=�$��Center=�$��12(�$��  GUARDAR TODO��