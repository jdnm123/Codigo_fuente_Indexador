   M S B A M L   `   `   `  ����  indexai'    !Indexai.InformacionComplementaria   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      j fSyncfusion.SfGridConverter.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89j fSyncfusion.SfBusyIndicator.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89h dSyncfusion.SfImageEditor.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Linq.Base, Version=18.4460.0.49, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a	 ]Syncfusion.SfGrid.WPF, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89a
 ]Syncfusion.Shared.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89` \Syncfusion.Tools.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89b ^Syncfusion.SfInput.Wpf, Version=19.1460.0.66, Culture=neutral, PublicKeyToken=3d67ed1f87d44c89@
Syncfusion!http://schemas.syncfusion.com/wpf     	 
   5      Hd2http://schemas.microsoft.com/expression/blend/2008     	 
   5      b ^MaterialDesignThemes.Wpf, Version=4.0.0.2422, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5      Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5      -   5          � Title$*  $Registro información complementaria��5	      $	��440��5
      $��668.255��5       � WindowStartupLocation$ Manual=�5      .��5      �  � ColumnDefinitions 5   
   �� 5      $
��188* �6   �� 5      $
��101* �6   �� 5      $
��100* �6   �� 5      $
��187* �6    � RowDefinitions 5   
   �� 5      $	��280 �6   �� 5      �� 5      $��60 �6   .+�5   
   ( 	 "Syncfusion.UI.Xaml.Grid.SfDataGrid  -   5          Name$ gridBeneficiarios��5      $��4��5      $��0,0,0,50q�5         AllowDeleting . ( 	 "Syncfusion.UI.Xaml.Grid.SfGridBase   AllowEditing .    AllowFiltering .    AllowResizingColumns . 	   AutoGenerateColumns	 .  
   NavigationMode$	
 Row=�5%         ItemsSource 5$      �� *BeneficiariosColl+   Columns 5'      , 	 &Syncfusion.UI.Xaml.Grid.GridTextColumn  , 	 &Syncfusion.UI.Xaml.Grid.GridColumnBase   Width$	 110Q�5(         
HeaderText$ Nombre��5)         MappingName$ Nombre��5*         TextAlignment$
 Left=�5+        5,      $	 110Q�5-      $ 	Apellidos��5.      $ 	Apellidos��5/      $
 Left=�50        51      $ 50Q�52      $ Tipo de documento��53      $ TipoDocumento��54      $
 Left=�55        56      $	 110Q�57      $ Numero documento��58      $ NumeroDocumento��59      $
 Left=�5:        5;      $ 50Q�5<      $ 	Principal��5=      $ sol_principal��5>      $
 Left=�5?      5A      �� 5B   
   -   5         ��Name$ btnNuevoBeneficiario��5C      $��2��5D      $��2��5E      $��34��5F      $
��10,0q�5G      $��Bottom=�5H      $��Nueva Persona��5J      {�.  5K      �� 5L   
   -   5        $ btnEliminarBeneficiario��5M      $��2��5N      $��34��5O      $��	15,0,10,0q�5P      $��Bottom=�5Q      $��Eliminar beneficiario��5S      {�.  5T      �� 5U   
   $��1��5V      $��4��5W      $��3,5,3,0q�5X      $��Datos beneficiario��5Y      ��5Z      Z� .��5[      � $��2,2,2,2q�6    5\      �� 5]      �� 5^      .+�5`      �� -   5         ��Name$ chkSolicitante��5a      $��18��5b      $��9,5,10,0q�5c      $	��Top=�5d      $��	Principal��5e      $��0��5f      �� 5g      $��26��5h      $��	9,26,10,0q�5i      $	��Top=�5j      $��TIPO DE DOCUMENTO��5k      �� 5l      -   5         ��Name$ cbxTipoDocumento��5m      $��30��5n      $��3,45,3,0q�5o      $	��Top=�5p       �� IsTextSearchEnabled . $��1��5r      .@�5s      �� $!��CC - 1.Cedula de ciudadania��6#   
 � Tag$ CC��6I   �� 5t      $!��TI - 2.Tarjeta de identidad��6#   $ TI��6I   �� 5u      $��RC - 3.Registro civil��6#   $ RC��6C   �� 5v      $#��CE - 4.Cedula de extranjería��6#   $ CE��6J   �� 5w      $)��#RD - 5.Registro civil de defunción��6#   $ RD��6P   �� 5x      $ ��PB - 6.Partida de Bautismo��6#   $ PB��6H   �� 5y      $"��PD - 7.Partida de defunción��6#   $ PD��6I   �� 5z      $��PS - 8.Pasaporte��6#   $ PS��6>   �� 5{      $&�� SI - 9.Menor sin identificación��6#   $ SI��6M   �� 5|      $+��%RM - 10. Registro Civil de Matrimonio��6#   $ RM��6S   �� 5}      $��NR - No registra��6#   $ NR��6>   5~      �� 5      $��1��5�      $��26��5�      $��
10,26,10,0q�5�      $	��Top=�5�      $��NUMERO DOCUMENTO��5�      �� 5�      $��1��5�      $��3,50,3,0q�5�      $	��Top=�5�       K� ZIndex$	 200��5�      
�������.+�5�      �� -   5         ��Name$ txtNumeroDocumento��5�      $��25��5�      $	��Top=�5�       �� CharacterCasing$ Upper=�5�      $��2��5�      $�� ��5�       �� TextWrapping$
 Wrap=�5�      �� 5�      $��2��5�      $	��Top=�5�      $	 201��5�      
�������$��11(�5�      $��3��5�      $y�Visible=�5�      ��5�      �� 5�       �� 
TargetType" ��6$   .�5�   "   ��  �� Property � �� �� Value$ 0,0,0,0q�6<   5�      5�      5�      �� 5�      $��26��5�      $��
10,80,10,0q�5�      $	��Top=�5�      $��NOMBRES��5�      �� 5�      $��	3,100,3,0q�5�      $	��Top=�5�      
�������.+�5�      �� -   5        $ 
txtNombres��5�      $��1��5�      $��2��5�      $��25��5�      $	��Top=�5�      $ Upper=�5�      $��4��5�      $�� ��5�      $
 Wrap=�5�      �� 5�      $��2��5�      $	��Top=�5�      
�������$��11(�5�      $y�Visible=�5�      ��5�      �� 5�      " ��6$   .�5�   "   ��  � ��$ 0,0,0,0q�6<   5�      5�      5�      �� 5�      $��1��5�      $��26��5�      $��10.4,80,10.2,0q�5�      $	��Top=�5�      $��	APELLIDOS��5�      �� 5�      $��1��5�      $��	3,100,3,0q�5�      $	��Top=�5�      $ 50��5�      
�������.+�5�      �� -	   5        $ txtApellidos��5�      $��1��5�      $��2��5�      $��25��5�      $	��Top=�5�      $ Upper=�5�      $��6��5�      $�� ��5�      $
 Wrap=�5�      �� 5�      $��2��5�      $	��Top=�5�      $ 51��5�      
�������$��11(�5�      $y�Visible=�5�      ��5�      �� 5�      " ��6$   .�5�   "   ��  � ��$ 0,0,0,0q�6<   5�      5�      5�      �� 5�      -
   5        $ 
btnAñadir��5�      $��1��5�      $	��165��5�      $��32��5�      $��0,0,5,0q�5�      $��Right=�5�      $��Bottom=�5�      $ 15��5�      $��Adicionar / Actualizar��5�      $��8��5�      5�      5�      �� 5�   
   -   5        $ 
btnGuardar��5�      $��2��5�      $��2��5�      $��2��5�      $��36��5�      $��
10,0,10,10q�5      $��Bottom=�5     $��9��5     .��5     �� $��0,4,0,0q�6    �� Orientation$ 
Horizontal=�6*   .+�5     '  !MaterialDesignThemes.Wpf.PackIcon  $��16��5     $��16��5     $
��Left=�5        Kind$ ContentSave=�5	     �� 5
     $��Center=�5     $��Center=�5     $��12(�5     $��  GUARDAR TODO��5     5     5     5     5     