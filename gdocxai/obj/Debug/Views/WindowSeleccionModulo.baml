   M S B A M L   `   `   `  ����  indexai)    #Indexai.Views.WindowSeleccionModulo   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5        � Title$  Selección de módulos��5      $	��800��5      $	��450��5	       � WindowStartupLocation$ CenterScreen=�5
       � WindowStyle$
 None=�5      .��5      � .+�5   
   �� $��34��5      $��	0,0,0.2,0q�5      $	��Top=�5      ��5      �� 5       �� 
StartPoint$ 0.5,0&�6&    �� EndPoint$ 0.5,1&�69   .��5      �  � Color$ 	#FF2281D1��6#   � 5       � Offset$ 1Q�6#   $ 	#FF34268A��6.   � 5      $ 0.546Q�6#   $ 	#FF33288B��62   5      5      �� 5   
   -   5         ��Name$ 
txtUsuario��5      $��612,4,4.2,396.8q�5      $��Center=�5      $��14(�5      
�������$��Usuario��5        �� TextAlignment$ Right=�5!      �� 5"   
   $��10,2,0,0q�5#      $
��Left=�5$      $	��Top=�5%      $1��+Seleccione el módulo al que desea ingresar��5&      $��16(�5'      ���	#DDFFFFFF	 J� RenderTransformOrigin$	 -0.378,0.004&�5)      &   System.Windows.Controls.DataGrid  5*   
   -   5        
  Name$
 GridModulos��5+      $	��775��5,      $	��374��5-      $��	10,39,0,0q�5.      $
��Left=�5/      $	��Top=�50         
IsReadOnly . 52      53      54      