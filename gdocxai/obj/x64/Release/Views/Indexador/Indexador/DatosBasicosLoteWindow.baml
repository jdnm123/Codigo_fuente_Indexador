   M S B A M L   `   `   `  ���� Q  MWPFTextBoxAutoComplete, Version=1.0.0.2, Culture=neutral, PublicKeyToken=null_Dclr-namespace:WPFTextBoxAutoComplete;assembly=WPFTextBoxAutoCompleteWPFTextBoxAutoComplete   indexai$   Indexai.DatosBasicosLoteWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Z	behaviorsDclr-namespace:WPFTextBoxAutoComplete;assembly=WPFTextBoxAutoComplete     @d2http://schemas.microsoft.com/expression/blend/2008     b ^MaterialDesignThemes.Wpf, Version=4.0.0.2422, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  -     � Title$  Datos básicos del lote��$	��640��$��519.145�� � WindowStartupLocation$ CenterScreen=� � WindowStyle$ SingleBorderWindow=�.��� $	��417��$��
-2,0,1.2,0q�$	��Top=� � ColumnDefinitions �� $	��35* ��� $	��35* �.+��� $��26��$��	10,1,14,0q�$	��Top=�$��Numero caja���� -    ��Name$ 
txtNumCaja��$��1��$��23��$��10,4,8,0q�$	��Top=�{�.  $��7��$�� �� �� TextWrapping$
 Wrap=�1   +WPFTextBoxAutoComplete.AutoCompleteBehavior   AutoCompleteItemsSource �� *Transcriptions+�� $��24��$��
10,32,13,0q�$	��Top=�$��Total de folios���� -   $ txtTotalFolios_numerico��$��1��$��24��$��	10,34,8,0q�$	��Top=�$�� ��$
 Wrap=� �� *Transcriptions+�� $��32��$��
10,60,14,0q�$	��Top=�$��Número carpeta���� -   $ txtNumeroCarpeta��$��1��$��24��$��	10,68,8,0q�$	��Top=�$�� ��$
 Wrap=� �� *Transcriptions+�� $��26��$��
11,90,12,0q�$
��Left=�$	��Top=�$��No. Expediente���� $��2��$��
15,115,5,0q�$	��Top=� K� ZIndex$ 92��.+��� -   $ txtExpediente��$��25��$��	0,0,0.2,0q�$	��Top=� �� AcceptsReturn . 	 �� CharacterCasing$	 Upper=�$�� ��$
 Wrap=��� $��2��$	��Top=�
�������$y�Visible=��� $��2��$��26��$��
11,145,9,0q�$	��Top=�$��
Expediente��
 J� RenderTransformOrigin$
 0.516,-0.496&��� $��2��$��
15,165,5,0q�$	��Top=�$ 91��.+��� -   $ txtNomExpediente��$��40��$��	0,0,0.2,0q�$	��Top=� . $	 Upper=�$��0��$�� ��$
 Wrap=��� $��2��$	��Top=�
�������$��11(�$y�Visible=�����  �� 
TargetType" ��.���  �� Property � �� �� Value$ 0,0,0,0q��� $��2��$��61��$��15,290,10,0q�$	��Top=�$ 60��.+��� $��37��$��0,5,0,0q�$	��Top=�$ 70�� �� Orientation$ 
Horizontal=�.+��� $��92��$��24��$��0,6,1,0q�$
��Left=�$	��Top=�$��Hoja de control ���� $��0,6,0,0q�.+��� -   $ txthcinio_numerico��$��80��$��25��$��1,0,1,1q�$	 Upper=�$��4��$�� ��$
 Wrap=��� $��80��$��0,-1,0,0q�$��Bottom=�
�������$y�Visible=��� $��24��$��24��$��2,6,2,0q�$
��Left=�$	��Top=�$��de���� $��0,6,12,0q�.+��� -   $ txthcfin_numerico��$��2��$��80��$��25��$��1,0,1,1q�$	 Upper=�$��5��$�� ��$
 Wrap=��� $��2��$��80��$��0,-1,0,2q�$��Bottom=�
�������$y�Visible=��� $��39��$��24��$��20,6,0,2q�$
��Left=�$	��Top=�$��Tomo ���� $��0,6,0,0q�.+��� -	   $ 
txtTomoIni��$��2��$��80��$��25��$��1,0,1,1q�$	 Upper=�$��6��$�� ��$
 Wrap=��� $��2��$��80��$��0,-1,0,2q�$��Bottom=�
�������$y�Visible=��� $��24��$��24��$��2,6,2,0q�$
��Left=�$	��Top=�$��de���� $��0,6,0,0q�.+��� -
   $ 
txtTomoFin��$��2��$��80��$��25��$��1,0,1,1q�$	 Upper=�$��7��$�� ��$
 Wrap=��� $��2��$��80��$��0,-1,0,2q�$��Bottom=�
�������$y�Visible=��� $��2��$��23��$��
12,216,8,0q�$	��Top=�$��Observaciones���� $��2��$��
15,235,8,0q�$	��Top=�$ 90��.+��� -   $ txtObservaciones��$��40��$��	0,0,0.6,0q�$	��Top=� . $	 Upper=�$��1��$�� ��$
 Wrap=��� $��2��$	��Top=�
�������$��11(�$y�Visible=����� " ��.���  � ��$ 0,0,0,0q��� -    ��Name$ 
btnAceptar��$��30��$��
17,0,4,-40q�$��Bottom=�$��Aceptar��$��9���� -   $ btnCancelar��$��1��$��30��$��10,0,10,-40q�$��Bottom=�$��Cancelar��$
 0.51,-0.727&�$��10��