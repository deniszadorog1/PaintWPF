   M S B A M L   `   `   `  ���� C  ?PaintWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:PaintWPFPaintWPF  A%clr-namespace:PaintWPF.CustomControlsPaintWPF.CustomControls  Y1clr-namespace:PaintWPF.CustomControls.BrushesMenu#PaintWPF.CustomControls.BrushesMenu      PaintWPF.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     (localclr-namespace:PaintWPF     X TXceed.Wpf.Toolkit, Version=4.6.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  8UserControls%clr-namespace:PaintWPF.CustomControls  B
BrusheMenu1clr-namespace:PaintWPF.CustomControls.BrushesMenu  -     �Name$  PaintWindow�� � Title$ 
MainWindow��$	��700��$	��700��$	��700��$
��1100��$
��1600��$	��600�����#eef4f9 � 	Resources    RoundButtonStyle �� 
TargetType �� Property �� Value �� 
TargetType A�Name �� Triggers	 P� Property
 P� Value �� 
TargetName  NoStyleButton  ValueGradCircle J� Opacity �� StrokeDashArray  DropDownMenu ��Name �Name � AllowsTransparency � HorizontalOffset �� RelativeSource ��Name �� 
ResourceId �� TypeInTargetAssembly �� ClearTypeHint ��Name  SubMenuItem ��Name �Name �� Triggers��%q
  &	        &	     &	 H    &	 �    &	     '�� :	    �� " ��.���  � �� �� " ��.R�� .+�A� $ outerEllipse��#��z��$	��0.0��A� $ innerEllipse��$��2q�#��z��#��z��$	��0.5���� $��Center=�$��Center=� P� 	 � ��
 . . ��� $ innerEllipse�� � ��
 ��  ���  � ��$ 17����  � ��$ 17����  � ��
 ��ح���  � ��
 �   ���  � ��$ 1q���  � ��$ 2q��� " ��.���  � �� �#f8f9fc��  � �� �#f8f9fc��  � ��$ 1q���  � ��$ 0q���  � �� �� " ��.R��� #��z��#��z��#��z��._��� $��Center=�$��Center=��� " ��.���  � �� �� " ��.R�� .+�A� $ outerEllipse��#��z��$	 0.9Q�$��1��
�������A� $ innerEllipse��$��4q�
����  �$	 5 5R�$	 0.9Q�$	��0.5���� $��Center=�$��Center=���  � ��$ 15����  � ��$ 15����  � ��
 �������  � ��$ 1q���  � ��$ 2q��� " m�.���  � �� �� " m�.R��� $ border��
������ $��0q�$��30��$	��145��._�� .+��� $
��Icon��$
��Left=�$��Center=�$��25��$��25��$��5,0,0,0q��� $��Header��
���   �$��Rubik Medium)�$��13(�$
��Left=�$��Center=�$��27,0,0,5q��� $&�� Assets/img_services_dropdown.png��$
��None=�$��Right=�$��Center=�$��0,0,10,0q�� $ 
PART_Popup�� . $��Bottom=�$
 -150��#��� -����� *IsSubmenuOpen+ �� *TemplatedParent+.���� $ SubMenuBorder�����#f8f9fc���#f8f9fc._��� $ SubMenuScrollViewer����C� *�� $ MenuScrollViewer��" �+.��� $ Enabled=�.+��� $
��Left=�$	��Top=��� $ ItemsPresenter�� P� "	 �@m�IsSuspendingPopupAnimation
 . . ��� $ 
PART_Popup�� � ��$
 None=�P� 	 �@m�IsHighlighted
 . . ��� $ border�� � ��
 ���� �� $ border�� � ��
 ���� �� " m�.���  � �� �� " m�.R��� $ border�����#f9f9f9$��1q�._�� .+��� $ SelectionIcon��$
��Icon��$
��Left=�$��Center=�$��18��$��18��$��8,0,0,0q��� $��Header��
���   �$��Times new Roman)�$��14(�$
��Left=�$��Center=�$��35,0,0,0q�� $ Highlighter��$��35��$��2��
����  �$
��Left=�$��Center=�$y�	Collapsed=� P� 	 �@m�IsHighlighted
 . . ��� $ border�� � �� �#e1e1e1�� $ border�� � �� �#3E3C3D�� $ Highlighter�� � y�$ Visible=��� " ��.���  �@��IsFlicksEnabled .  P� 	 �@��Orientation$
 
Horizontal=�. ���  � ��$ 40����  � ��$ 40��P� 	 �@��Orientation$
 Vertical=�. ���  � ��$ 3����  � ��$ 3����  � ��
 �   �.���  � RowDefinitions �� -    �Name$ SettingsRow��$��20 ��� -   $ ToolsRow��$	��100 ��� -   $ 
DrawingRow���� -   $ 	BottomRow��$��35 �.+��� -   $��FigureCanvasToAdapt��
������  K� ZIndex$ 1������  �� ElementName$ FigurePanel�� �� Path$ ActualWidth����� $ FigurePanel��$ ActualHeight��� $ 1��.+�3   -PaintWPF.CustomControls.BrushesMenu.BrushMenu  -      Name$  BrushesMenu��$y�Hidden=�$	��310��$	��315��� -   $��
UpperPanel��$��0�����#f0f3f9$ 1��! � ColumnDefinitions! �� $	��100 ��� $	��100 ��� $��70 ��� $��30 ��� $��30 ��� .+�� -	   $��0��.+�o� 
������ $��Center=�.@�m� $��
Файлы��#��[ .@�m� -
   $��zeroFile��$��Создать��$��30��$	��400��#��[ " m� Icon" �� $��Images/File/Create.png��m� -   $��oneFile��$��Открыть��$��30��$	��400��#��[ " �� $��Images/File/Open.png��m� -   $��twoFile��$0��*Импортировать на холст��$��30��$	��400��#��[ " �� $��Images/File/Import.png��m� -   $��fourFile��$��Сохранить��$��30��$	��400��#��[ " �� $.��(Images/Pallete/MainPanelTopPart/save.png��m� -   $��fiveFile��$��Сохранить как��$��30��$	��400��#��[ " �� $��Images/File/SaveAs.png��m� -   $��sixFile��$��Печать��$��30��$	��400��#��[ " �� $��Images/File/Print.png��m� -   $��	sevenFile��$��Отправить��$��30��$	��400��#��[ " �� $��Images/File/Send.png��m� -   $��	eightFile��$;��5Сделать фоном рабочего стола��$��30��$	��400��#��[ " �� $*��$Images/File/MakeBgOfWorkingTable.png��m� -   $��nineFile��$-��'Свойства изображения��$��30��$	��400��#��[ " �� $#��Images/File/ImageSettings.png��m� -   $��tenFile��$��
Выйти��$��30��$	��400��#��[ " �� $��Images/File/Exit.png���� $��1��	# �� X1$# 0��	$ �� X2$$ 0��	% �� Y1$% 2��	& �� Y2$& 16��
���   �$��1��$	 0.3Q�� -   $��1��.+�o� 
������ $��0 0 0 0q�.@�m� #��[ ���� $��Изменить��$��14(�$��	-5,-5,0,0q�.@�m� -   $��	CutChange��$��Вырезать��$��30��$	��400��$	 0.5Q�{�.  #��[ " �� $ ��Images/Change/Scissors.png��m� -   $��
CopyChange��$��Копировать��$��30��$	��400��$	 0.5Q�{�.  #��[ " �� $��Images/Change/Copy.png��m� -   $��PasteChange��$��Вставить��$��30��$	��400��$	 0.5Q�{�.  #��[ " �� $��Images/Change/Paste.png���� $��2��$# 0��$$ 0��$% 2��$& 16��
���   �$��1��$	 0.3Q��� -   $
��Save��$��2��#��[ ���#eef4f9$��0q�.���� $.��(Images/Pallete/MainPanelTopPart/save.png���� $��3��$# 0��$$ 0��$% 2��$& 16��
���   �$��1��$	 0.3Q��� -   $��TurnLeft��$��3��#��[ ���#eef4f9$��0q�.���� $2��,Images/Pallete/MainPanelTopPart/turnLeft.png���� -   $��	TurnRight��$��4��#��[ ���#eef4f9$��0q�$��20��$	��Top=�.���� $3��-Images/Pallete/MainPanelTopPart/turnRight.png���� $��5��$# 0��$$ 0��$% 2��$& 16��
���   �$��1��$	 0.3Q��� $��2��
���   �$��1��$# 0��$% 0��$
$ 1100��$& 0��$	 0.3Q��� $��3��
���   �$��1��$# 0��$% 0��$
$ 1100��$& 0��$	 0.3Q�� $��3��! �� $	��130 ��� $	��130 ��� $	��130 ��� .+�� $��0��$��0 5 0 0q�.+��� $3��-Images/Pallete/MainPanelBotomIcons/cursor.png��$��20��$��20���� -   $��	CursCords��$	��110��$��11(�.��123456789012345�� $��1��
���   �$	��0.5��$# 0��$% 5��$$ 0��$& 30��$	 0.3Q�� $��1��$��0 5 0 0q�.+��� $9��3Images/Pallete/MainPanelBotomIcons/dottedSquare.png��$��20��$��20���� -   $��ObjSize��$	��110��$��11(��� $��2��
���   �$	��0.5��$# 0��$% 5��$$ 0��$& 30��$	 0.3Q�� $��2��$��0 5 0 0q�.+��� $1��+Images/Pallete/MainPanelBotomIcons/rect.png��$��20��$��20���� -   $��
CanvasSize��$	��110��$��11(�.��123456789012345�� $��3��
���   �$	��0.5��$# 0��$% 5��$$ 0��$& 30��$	 0.3Q��� -   $��ValueBorder��$��2��$��
10 0 10 10q�$��1��
���   �$	��0.5q�' �� CornerRadius$' 7��$	��320��$��30��$
��Left=�$ 2��
�������._�� .+��� $$��Images/Pallete/Other/lines.png��$	��Top=�$��0 10 0 0q�$��20��$��20��� -   $��ValueProgressBar��$	��270��$��5��
������ ���#8a8a8a( � Minimum$( 0Q�) � Maximum$	) 100Q�$��Bottom=�$��0 0 0 10q��� -    $��ValueCanvas��$	��270��$��0 30 0 0q�.+��� -!   $��draggableButton��#��[ $��20��$��20��* �� Left$* 5��
+ �� Top$	+ 255���� -"   $��paintInBlueCan��
����  �$��Bottom=�$��0��$��5��$��	0 30 0 10q�� -#   $ CenterWindowPanel��$��2��! �� �� -$   $ PaintFieldColumn��$	��21* ���  �� -%   $ TextRow���� -&   $ PaintFieldRow��$	��400 ��� .+�� $��1��$��1��.+��� -'   $��DrawingCanvas��}�. 
�������$	��400��$
��1000��$��Center=�$��0q�.+��� -(   $��RazerMarker��$	��100��$	��100��$��10q�
���   �
�������$ 0��$��1q�$y�Hidden=��� -)   $��BrushMarker��$	��150��$	��150��
���   �
���   �$' 90��$y�Hidden=�]� -*   $��ScrollDrawingCanvasRow��$��1��$��2��.+��� -+   $��DraiwngScroll��$��Bottom=�$��10��
�������$��Disabled=�$
��Auto=����� $ ScrollDrawingCanvasRow��$ ActualWidth�.���� $
��1000��$��10��
������ �� $��1��
���   �$��1��$# 0��$% 0��$
$ 1100��$& 0��$	 0.3Q�� -,   $��	MainPanel��$��1�����#f8f9fc! �� --   $ SelectionPart��$	��100 ��� -.   $ 	ToolsPart��$	��150 ��� -/   $ 	BrushPart��$��75 ��� -0   $ FiguresPart��$	��300 ��� -1   $ 
ColorsPart��$	��350 ��� -2   $ FreePart�� �� $	��100��$��* �.+�� $��0�� �� �� $��25 ��� $��30 �.+��� -3   $��SelectionBut��$��0��#��[ 
������ $��40��$��40��$��Bottom=�$��0 0 5 0q�.���� -4   $��SelectionImg��$9��3Images/Pallete/MainPanelBotomIcons/dottedSquare.png��� -5   $��1��.+�o� -6   $��SelectionMenu��
������ $��Center=�.@�m� -7   $��40��$��25��#��[ " �� -8   $8��2Images/Pallete/MainPanelCaligraphy/showMarkers.png��$��20��$��20��$��Uniform=�.@�m� -9   $��ZeroSelection��$ ��Прямоугольник��$��30��$	��400��#��[ " �� $$��Images/Selection/Rectangle.png��m� -:   $��OneSelection��$)��#Произвольная форма��$��30��$	��400��#��[ " �� $!��Images/Selection/Custom.png��m� -;   $��TwoSelection��$��Выбрать всё��$��30��$	��400��#��[ " �� $$��Images/Selection/ChooseAll.png��m� -<   $��ThreeSelection��$)��#Обратить выдиление��$��30��$	��400��#��[ " �� $*��$Images/Selection/RemoveSelection.png��m� -=   $��FourSelection��$-��'Прозрачное выдиление��$��30��$	��400��#��[ " �� ->   $��TransSelectionIcon��m� -?   $��FiveSelection��$��Удаление��$��30��$	��400��#��[ " �� $!��Images/Selection/Remove.png���� $��2��$��Center=�$��Center=����#5e5f60.��Выдиление�� $��1��$# 0��$% 5��$$ 0��$& 95��
���   �$	��0.5��$	 0.3Q�$	��Top=�� -@   $��	ToolsGrid��$��1�� �� �� $��30 �.+��� -A   $��ToolsAdaptBut��$��70��$y�Hidden=�$��5q�
�������
����� �$	��0.5q�.���� $$��Images/Adaptive/ToolsAdapt.png��� -B   $��ToolsPartPanel��$y�Visible=�$��0��$��5q����#f7fafc$	��140��$��60��.+��� -C   $	��Pen��$��	10 0 10 3q�#��[ .���� $+��%Images/Pallete/MainPanelIcons/Pen.png��$��25��$��25��$��Center=�$��Center=��� -D   $��Bucket��$��	10 0 10 3q�#��[ .���� $��1��$��0��$.��(Images/Pallete/MainPanelIcons/bucket.png��$��25��$��25��$��Center=�$��Center=��� -E   $
��Text��$��5 0 5 3q�#��[ .���� $,��&Images/Pallete/MainPanelIcons/text.png��$��25��$��25��$��Center=�$��Center=��� -F   $��Erazer��$��	10 0 10 0q�#��[ .���� $.��(Images/Pallete/MainPanelIcons/erazer.png��$��25��$��25��$��Center=�$��Center=��� -G   $��	ColorDrop��$��	10 0 10 0q�#��[ .���� $1��+Images/Pallete/MainPanelIcons/colorDrop.png��$��25��$��25��$��Center=�$��Center=��� -H   $��Glass��$��5 0 5 0q�#��[ .���� $��2��$��1��$-��'Images/Pallete/MainPanelIcons/glass.png��$��25��$��25��$��Center=�$��Center=��� $��3��$��2��$	��Top=�$��Center=�$��12(����#5e5f60.��Инструменты�� $��2��$# 0��$% 5��$$ 0��$& 95��
���   �$	��0.5��$	 0.3Q�$	��Top=�� $��2�� �� �� $��25 ��� $��30 �.+��� -I   $��PaintingBut��$��0��#��[ 
������ $��40��$��40��.���� -J   $��BrushTypePic��$%��Images/BrushType/UsualBrush.png��� -K   $��1��.+��� -L   $��
BrushesBut��
������ $��0q�.���� $8��2Images/Pallete/MainPanelCaligraphy/showMarkers.png��$��20��$��20��$��Uniform=��� $��2��$��Center=�$	��Top=����#5e5f60.��
Кисти�� $��3��$# 0��$% 5��$$ 0��$& 95��
���   �$	��0.5��$	 0.3Q�$	��Top=�� -M   $��FigGridPart��$��3�� �� �� $��30 �.+��� -N   $��AdaptFigureBut��$��0��$��5q�$��70��$��Center=�
������ 
����� �$	��0.5q�$y�Hidden=�.���� $&�� Images/Adaptive/FiguresAdapt.png���� -O   $��FiguresBorder��$y�Visible=�$��0��$��1q����#e8ebed$��
10 10 10 0q�._��� -P   $��AdaptFigScroll��$
��Auto=�$��Disabled=�.��� -Q   $��FigurePanel��, � Orientation$, 
Horizontal=����#f7fafc$	��270��.+��� -R   $
��Line��$��22��$��22��#��[ .���� $��Images/Figures/Line.png���� -S   $��Curve��$��22��$��22��#��[ .���� $%��Images/Figures/CroockedLine.png���� -T   $
��Oval��$��22��$��22��#��[ .���� $��Images/Figures/Circle.png���� -U   $��	Rectangle��$��22��$��22��#��[ .���� $"��Images/Figures/Rectangle.png���� -V   $��RoundedRectangle��$��22��$��22��#��[ .���� $(��"Images/Figures/ShapedRectangle.png���� -W   $��Polygon��$��22��$��22��#��[ .���� $ ��Images/Figures/Polygon.png���� -X   $��Triangle��$��22��$��22��#��[ .���� $!��Images/Figures/Triangle.png���� -Y   $��RightTriangle��$��22��$��22��#��[ .���� $&�� Images/Figures/RightTriangle.png���� -Z   $��Rhombus��$��22��$��22��#��[ .���� $ ��Images/Figures/Rhombus.png���� -[   $��Pentagon��$��22��$��22��#��[ .���� $!��Images/Figures/Pentagon.png���� -\   $��Hexagon��$��22��$��22��#��[ .���� $ ��Images/Figures/Hexagon.png���� -]   $��
RightArrow��$��22��$��22��#��[ .���� $#��Images/Figures/RightArrow.png���� -^   $��	LeftArrow��$��22��$��22��#��[ .���� $"��Images/Figures/LeftArrow.png���� -_   $��UpArrow��$��22��$��22��#��[ .���� $ ��Images/Figures/UpArrow.png���� -`   $��	DownArrow��$��22��$��22��#��[ .���� $"��Images/Figures/DownArrow.png���� -a   $��FourPointedStar��$��22��$��22��#��[ .���� $&�� Images/Figures/FourPointStar.png���� -b   $��FivePointedStar��$��22��$��22��#��[ .���� $(��"Images/Figures/FivePointedStar.png���� -c   $��SixPointedStar��$��22��$��22��#��[ .���� $'��!Images/Figures/SixPointedStar.png���� -d   $��RoundedRectangularLeader��$��22��$��22��#��[ .���� $)��#Images/Figures/ShapeRectMeesage.png���� -e   $��OvalCallout��$��22��$��22��#��[ .���� $$��Images/Figures/OvalMessage.png���� -f   $��CalloutCloud��$��22��$��22��#��[ .���� $��Images/Figures/Cloud.png���� -g   $��Heart��$��22��$��22��#��[ .���� $��Images/Figures/Heart.png���� -h   $��	Lightning��$��22��$��22��#��[ .���� $"��Images/Figures/Lightning.png���� $��1��$��Center=�$��Center=����#5e5f60.��Фигуры�� $��4��$# 0��$% 5��$$ 0��$& 95��
���   �$	��0.5��$	 0.3Q�$	��Top=�� -i   $��ColorColGrid��$��4�� �� $��70��$��70 ��� $��30 �.+��� -j   $��PaletteAdaptBut��$y�Hidden=�$��5q�$��70��
�������
����� �$	��0.5q�.���� $'��!Images/Adaptive/PalletteAdapt.png��� -k   $��	ColorPart��$��0��$y�Visible=����#f8f9fc! �� $��45 ��� $	��230 ��� $	��100 �.+��� - �� Orientation$- Vertical=�$��Center=�.+��� -l   $��
FirstColor��#��[  
���   �$��5q�$��25��$��25���� -m   $��SecondColor��#��[  ���#FCFCFC$��	10 6 10 0q�$��25��$��25��� -n   $	��asd��$��1��$��3��$��	0,0,100,0q�.+��� -o   #��[  ���#000000$��5 5 0 0q��� -p   #��[  ���#7f7f7f$��5 5 0 0q��� -q   #��[  ���#880015$��5 5 0 0q��� -r   #��[  ���#ed1c24$��5 5 0 0q��� -s   #��[  ���#ff7f27$��5 5 0 0q��� -t   #��[  ���#fff200$��5 5 0 0q��� -u   #��[  ���#22b14c$��5 5 0 0q��� -v   #��[  ���#00a2e8$��5 5 0 0q��� -w   #��[  ���#3f48cc$��5 5 0 0q��� -x   #��[  ���#a349a4$��5 5 0 0q��� -y   #��[  ���#FCFCFC$��5 5 0 0q��� -z   #��[  ���#c3c3c3$��5 5 0 0q��� -{   #��[  ���#b97a57$��5 5 0 0q��� -|   #��[  ���#ffaec9$��5 5 0 0q��� -}   #��[  ���#ffc90e$��5 5 0 0q��� -~   #��[  ���#efe4b0$��5 5 0 0q��� -   #��[  ���#b5e61d$��5 5 0 0q��� -�   #��[  ���#99d9ea$��5 5 0 0q��� -�   #��[  ���#7092be$��5 5 0 0q��� -�   #��[  ���#c8bfe7$��5 5 0 0q��� -�   $��OnePaletteColor��#��[  ���#f8f9fc$��5 5 0 0q��� -�   $��TwoPaletteColor��#��[  ���#f8f9fc$��5 5 0 0q��� -�   $��ThreePaletteColor��#��[  ���#f8f9fc$��5 5 0 0q��� -�   $��FourPaletteColor��#��[  ���#f8f9fc$��5 5 0 0q��� -�   $��FivePaletteColor��#��[  ���#f8f9fc$��5 5 0 0q��� -�   $��SixPaletteColor��#��[  ���#f8f9fc$��5 5 0 0q��� -�   $��SevenPaletteColor��#��[  ���#f8f9fc$��5 5 0 0q��� -�   $��EightPaletteColor��#��[  ���#f8f9fc$��5 5 0 0q��� -�   $��NinePaletteColor��#��[  ���#f8f9fc$��5 5 0 0q��� -�   $��TenPaletteColor��#��[  ���#f8f9fc$��5 5 0 0q��� -�   $��2��$��0��$��40��$��40��$' 4��$
��Left=�$��5 0 0 0q�$��1q�._��� -�   $��ChooseColorPalette��#��[ 
������ .���� $/��)Images/Pallete/MainPanelIcons/palette.png���� $��3��$��1��$	��Top=�$��Center=�$��14(����#5e5f60.��
Цвета�� $��5��$# 0��$% 5��$$ 0��$& 95��
���   �$	��0.5��$	 0.3Q�$	��Top=�