   M S B A M L   `   `   `  ����F  Bzad1_base64, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:zad1_base64zad1_base64  5   	       zad1_base64.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:zad1_base64     5   	     � Title$  
MainWindow��5   	   $	��637��6   $	��525��6)   .��5	      � .+�5
   
   �� -   5        $��fileNameTextBox��5
   K   $��32��6   $
��Left=�6   $��6,10,0,0q�69   $	��Top=�5      $	��393��6*    �� 
IsReadOnly . 66   �� 5   
   -   5        $��	browseBtn��5      $��Browse��5      $��32��6"   $
��Left=�6.   $��
405,10,0,0q�6I   $	��Top=�5   "   $��88��6:   6E   �� 5   
   $��400,40,10,0q�6   $	��Top=�6)   �� 5      .F�5      �� $��Compare results:��6   $	��107��63   $��25��6?   $��
-4,0,0,-10q�6K   5      �� 6"   �� 6(   �� 5      $6��0https://www.browserling.com/tools/file-to-base64?�6   .�6X   �� $�- file encoder��6\   6u   �� 6�   �� 5      $0��*https://cryptii.com/pipes/base64-to-binary?�6   .�6R   �� $�- file decoder��6V   6o   �� 6z   �� 5      $0��*https://cryptii.com/pipes/base64-to-binary?�6   .�6R   �� $�- text en-/decoder��6V   6s   5      	� 5   
   -   5         	�Name$ encodeRadio��5      $��Encode Base64��6+   $
��Left=�6C   $��6,58,0,0q�6^   $	��Top=�6p    J� RenderTransformOrigin$ -0.34,0.028&�6�    	� 	GroupName$ 
codeToWhat��6�   {�.  6�   	� 5   
   -   5        $ decodeRadio��5      $��Decode Base64��6+   $
��Left=�6C   $��
118,58,0,0q�6^   $	��Top=�6r   $ -0.34,0.028&�6�   $ 
codeToWhat��6�   {�.  6�   �� 5   
   -   5         ��Name$ startBtn��5      $��START��6#   $
��Left=�63   $��
234,56,0,0q�6N   $	��Top=�6b   $��75��6z   {�.  6�   �� 5   
   $��0,85,0,0q�6   $��30��6&   $	��Top=�62   �� 5      .F�5      �� $��Save as:��6   $
��Left=�6+   $��6,0,0,0q�6F   $	��Top=�6W   5       �� 5      .F�5      �� -   5         ��Name$ newFileName��5      $
��Left=�6/   $��23��6J   $��0q�6V    �� TextWrapping$
 Wrap=�6a   $	��Top=�6u   $	��120��6�   {�.  6�   5       �� 5       .F�5!      �� $��With extension:��6   $
��Left=�62   $��6,0,0,0q�6M   $	��Top=�6^   5"       �� 5#      .F�5$      �� -   5        $ 
newFileExt��5$      $
��Left=�6.   $��23��6I   $��0q�6U   $
 Wrap=�6`   $	��Top=�6t   $��50��6�   {�.  6�   5%      5&      �� 5'   
   -   5        $��binaryTxtBox��5'      $
��Left=�6&   $��10,145,10,0q�6A   $	��Top=�6V   $
 Wrap=�6n    . $��0q�6�   $
��Auto=�6�   $	��246��6�   �� 5(   
   -	   5        $��base64TxtBox��5(      $
��Left=�6&   $��260,145,10,0q�6A   $	��Top=�6W   $
 Wrap=�6o    . $��0q�6�   $
��Auto=�6�   �� 5)   
   $��Bytes (hex):��6   $
��Left=�6'   $��	6,115,0,0q�6B   $	��Top=�6U   $
��Bold"�6m   �� 5*   
   $��Base64:��6   $
��Left=�6"   $��256,115,0,0q�6=   $	��Top=�6R   $
��Bold"�6j   5+      5,      