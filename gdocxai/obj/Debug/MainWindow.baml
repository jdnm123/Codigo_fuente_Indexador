   M S B A M L   `   `   `  ����B  >indexai, Version=2.4.6.3, Culture=neutral, PublicKeyToken=nullclr-namespace:Login2Login2  5      !clr-namespace:IndexaiIndexai  5          Indexai.MainWindow   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      &localclr-namespace:Login2     5      (local1clr-namespace:Indexai     5      b ^MaterialDesignThemes.Wpf, Version=4.0.0.2422, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5      Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5	        � Title$  Login��5
      $	��350��5      $	��500��5      $��Segoe UI Emoji)�5       � 
ResizeMode$ NoResize=�5       � WindowStartupLocation$ CenterScreen=�5       � WindowStyle$
 None=�5      .��5      � $	��350��6   $��Right=�6   .+�5   
   �� $	��280��6   $	��Top=�6!   ��5      �� 5       �� 
StartPoint$ 0.5,0&�6&    �� EndPoint$ 0.5,1&�69   .��5      �  � Color$ 	#FF2281D1��6#   � 5       � Offset$ 1Q�6#   $ 	#FF34268A��6.   � 5      $ 0.546Q�6#   $ 	#FF33288B��62   5      5      �� 5   
   -   5         ��Name$ 	BtnCerrar��5      $��301,0,0,468q�5      	 � ToolTip$	 Cerrar��5!      ��5       0  *MaterialDesignThemes.Wpf.PackIconExtension  *close+5!      �� 5"   
   $	��280��5#      $	��240��5$      $
��0,80q�5%      $��Bottom=�5&      
�������
 �� RadiusX$
 10��5(       �� RadiusY$ 10��5)       J� Effect 5*      3  -System.Windows.Media.Effects.DropShadowEffect  5+         
BlurRadius$ 15Q�5,         	Direction$ 0Q�5-         RenderingBias$ Quality=�5.         ShadowDepth$ 1Q�5/         Color$ 	#FFBBBBBB��50      52      � 53   
   $	��240��54      $��35,80q�55      $��Bottom=�56      .+�57      �� $��5q�58      $��Center=�59      $	��Top=�5:      $��INICIO DE SESIÓN��5;      $��18(�5<      
�������5=      �� 5>      $��15q�6   $��Center=�6%   .+�5?      �� -   5         ��Name$ 
txtUsuario��5@      $
��0,10q�5A      )  #MaterialDesignThemes.Wpf.HintAssist   Hint$ USUARIO��5B      $��Champagne & Limousines)�5C      $��18(�5D       %  !MaterialDesignFloatingHintTextBox#��[  5E      G� 5F      -   5         G�Name$ pasClave��5G      $
��0,10q�5H      $ CONTRASEÑA��5I      $��Champagne & Limousines)�5J      $��18(�5K       ) %MaterialDesignFloatingHintPasswordBox#��[ 5L      5M      5N      �� 5O   
   -   5        $��btnIngresar��5P      $	��150��5Q      $
��0,65q�5R      $��Center=�5S      $��Bottom=�5T      $��INICIAR SESIÓN��5V      �� 5X   
   $	��100��5Y      $	��100��5Z      $��30q�5[      $	��Top=�5\      $��Assets/home.png��5]      �� 5^   
   -   5         ��Name$ lblError��5_      $	��186��5`      $��75,365,89,0q�5a      $��Center=�5b      $	��Top=�5c      $��Center=�5d      ���	#FFFCFCFC$'��!Debe ingresar email y contraseña��5f      ���	#DDFF00005g      �� 5h   
   -   5        $ BtnMinimiza��5i      $��252,0,49,468q�5j      $	 Cerrar��5m      ��5l        *Minus+5m      �� 5n   
   -   5        $ 
lblVersion��5o      $	��186��5p      $��75,466,89,0q�5q      $��Center=�5r      $	��Top=�5s      $��Center=�5t      ���	#FFFCFCFC$��Versión��5v      ���	#DD001FB85w         Indexai.LoadingAnimation  5x   
   -   5          Name$ loadingAnimation��5y      $��20��5z      $��
55,0,45,40q�5{      $��Bottom=�5|      $y�	Collapsed=�5}      5~      5      