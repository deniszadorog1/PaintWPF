   M S B A M L   `   `   `  ���� C  ?PaintWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:PaintWPF.CustomControlsPaintWPF.CustomControls  4    .PaintWPF.CustomControls.LeftClickSelectionMenu   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     7local%clr-namespace:PaintWPF.CustomControls     $	��300��$	��300��  � 	Resources     Storyboard2 �� 
TargetName �� TargetProperty9  3System.Windows.Media.Animation.EasingDoubleKeyFrame O� KeyTime O� Value��%�  &	        �� .��U� $ listBox��$ (FrameworkElement.Height)�.\�  $ 00:00:00��$ 10Q�  $ 00:00:00.1000000��$ 50Q�  $ 00:00:00.2000000��$	 100Q�  $ 00:00:00.3000000��$	 150Q�  $ 00:00:00.4000000��$	 200Q�  $ 00:00:00.5000000��$	 250Q�  $ 00:00:00.6000000��$	 300Q�U� $ listBox��$U O(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)�.\�  $ 00:00:00��$
 -150Q�  $ 00:00:00.1000000��$
 -125Q�  $ 00:00:00.2000000��$
 -100Q�  $ 00:00:00.3000000��$	 -75Q�  $ 00:00:00.4000000��$	 -50Q�  $ 00:00:00.5000000��$	 -25Q�  $ 00:00:00.6000000��$ 0Q� � Triggers :�  :� RoutedEvent$ UserControl.Loaded��.[��� #��[  .��� $	��300��.+��� -    ��Name$ listBox��$��Hidden=�$��Hidden=�$	��300�� J� RenderTransformOrigin$ 0.5,0.5&�$	��300��	 J� Opacity$		 0.9Q�
�������z�V� .~��� 
 �� ScaleY$
 1Q��� �� T� .@��� -   $	��Cut�����	#FFD0D0D0
�������$��30��$	��285��$
��Left=�$��0q�$
��Left=�.��� $
��Left=�$��Center=�$	��285��.+��� $��20��$��20��$��Center=�$!��/Images/Change/Scissors.png��$��0q��� $��Вырезать��$��Center=�$��12(�$��0q��� $��Ctrl + x��$��10(�$	��202��$��Right=�$��25���� -    ��Name$
 Copy�����	#FFD0D0D0
�������$��30��$	��285��$��0q�$
��Left=�.��� $
��Left=�$��Center=�$	��285��.+��� $��20��$��20��$��Center=�$��/Images/Change/Copy.png��$��0q��� $��Копировать��$��Center=�$��12(�$��0q��� $��Ctrl + c��$��10(�$	��189��$��Right=�$��25���� -   $ Paste�����	#FFD0D0D0
�������$��30��$	��285��$��0q�$
��Left=�.��� $
��Left=�$��Center=�$	��285��.+��� $��20��$��20��$��Center=�$��/Images/Change/Paste.png��$��0q��� $��Вставить��$��Center=�$��12(�$��0q��� $��Ctrl + v��$��10(�$	��206��$��Right=�$��25���� �� -   $ 	ChooseAll�����	#FFD0D0D0
�������$��30��$	��285��$��0q�$
��Left=�.��� $
��Left=�$��Center=�$	��285��.+��� $��20��$��20��$��Center=�$%��/Images/Selection/ChooseAll.png��$��0q��� $��Выбрать все��$��Center=�$��12(�$��0q��� $��Ctrl + f��$��10(�$	��186��$��Right=�$��25���� -   $ InvertSelection�����	#FFD0D0D0
�������$��30��$	��285��$��0q�$
��Left=�.��� $
��Left=�$��Center=�$	��285��.+��� $��20��$��20��$��Center=�$+��%/Images/Selection/RemoveSelection.png��$��0q��� $)��#Обратить выдиление��$��Center=�$��12(�$��0q��� -   $ Delete�����	#FFD0D0D0
�������$��30��$	��285��$��0q�$
��Left=�.��� $
��Left=�$��Center=�$	��285��.+��� $��20��$��20��$��Center=�$"��/Images/Selection/Remove.png��$��0q��� $��Удалить��$��Center=�$��12(�$��0q��� $��Delete��$��10(�$	��211��$��Right=�$��25���� �� -   $
 Swap�����	#FFD0D0D0
�������$��30��$	��285��$��0q�$
��Left=�.��� $
��Left=�$��Center=�$	��285��.+��� $��20��$��20��$��Center=�$ ��/Images/ClickMenu/Swap.png��$��0q��� $��Перевернуть��$��Center=�$��12(�$��0q��� $��20��$	��183��$%��/Images/ClickMenu/TurnRight.png��$��Right=��� -	   $ ToTurn�����	#FFD0D0D0
�������$��30��$	��285��$��0q�$
��Left=�.��� $
��Left=�$��Center=�$	��285��.+��� $��20��$��20��$��Center=�$$��/Images/ClickMenu/TurnOver.png��$��0q��� $��Повернуть��$��Center=�$��12(�$��0q��� $��20��$	��196��$%��/Images/ClickMenu/TurnRight.png��$��Right=�