   M S B A M L   `   `   `  ����C  ?zad4_ftp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:zad4_ftpzad4_ftp  5   	       zad4_ftp.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:zad4_ftp     5   	     � Title$  
MainWindow��5   	   $	��450��6   $	��800��6)   .��5	      � .+�5
   
   �� -   5         ��Name$ 
ConnectBtn��5
      $��Connect��6%   $
��Left=�67   $��
700,10,0,0q�6R   $	��Top=�6f   $��75��6~   6�   �� 5   
   -   5         ��Name$ StatusLabel��5      $
��Left=�6%   $��	10,10,0,0q�6@   $	��Top=�6S   $��22��6k   $	��685��6w   $
��Auto=�6�   ��. $��Press connect to start.��6�   $��10(�6�   R� 5   
   -   5         R�Name$ FTPTree��5      $
��Left=�6$   $	��336��6?   $��	10,65,0,0q�6L   $	��Top=�6_   $	��245��6w   {�.  ��5      �� 5       �� 
TargetType" Q�6   .�5      ��  �� Property �@Q�
IsExpanded �� Value . 63   -   5        5      5      �� 5   
   -   5         ��Name$ FTPPath��5      $
��Left=�6#   $��23��6>   $��	10,37,0,0q�6J    �� TextWrapping$
 Wrap=�6]   $��/��6q   $	��Top=�6z   $	��710��6�   {�.  6�   �� 5   
   -   5        	 ��Name$	 FTPList��5      $
��Left=�6$   $	��336��6?   $��
260,65,0,0q�6L   $	��Top=�6`   $	��515��6x   {�.  
 �� View
 5      �� 5      .P�5      �� $
��Name��6%    �� Width$ 80��63    �� DisplayMemberBinding 6>   �� *Name+5      ��5      �� 5      " ��6   .�5       -   5        5!      5#      �� 5$   
   -   5        $ GoBtn��5$      $��Go��6    $
��Left=�6-   $��
725,37,0,0q�6H   $	��Top=�6\   $��50��6t   {�.  6�   5&      5'      