﻿#pragma checksum "..\..\..\CustomControls\Selection.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "2D1F09D22C76B3ADC228540956F6D8E2BC2B7E07463C479FADF9140E9CC421A6"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using PaintWPF.CustomControls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace PaintWPF.CustomControls {
    
    
    /// <summary>
    /// Selection
    /// </summary>
    public partial class Selection : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 14 "..\..\..\CustomControls\Selection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle DashedBorder;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\CustomControls\Selection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border SelectionBorder;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\CustomControls\Selection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas SelectCan;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\CustomControls\Selection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid SizingGrid;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\CustomControls\Selection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle LeftTop;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\CustomControls\Selection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle CenterTop;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\CustomControls\Selection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle RightTop;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\..\CustomControls\Selection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle RightCenter;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\..\CustomControls\Selection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle RightBottom;
        
        #line default
        #line hidden
        
        
        #line 76 "..\..\..\CustomControls\Selection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle CenterBottom;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\..\CustomControls\Selection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle LeftBottom;
        
        #line default
        #line hidden
        
        
        #line 92 "..\..\..\CustomControls\Selection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle LeftCenter;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/PaintWPF;component/customcontrols/selection.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\CustomControls\Selection.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 9 "..\..\..\CustomControls\Selection.xaml"
            ((PaintWPF.CustomControls.Selection)(target)).SizeChanged += new System.Windows.SizeChangedEventHandler(this.UserControl_SizeChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.DashedBorder = ((System.Windows.Shapes.Rectangle)(target));
            return;
            case 3:
            this.SelectionBorder = ((System.Windows.Controls.Border)(target));
            
            #line 23 "..\..\..\CustomControls\Selection.xaml"
            this.SelectionBorder.SizeChanged += new System.Windows.SizeChangedEventHandler(this.SelectionBorder_SizeChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.SelectCan = ((System.Windows.Controls.Canvas)(target));
            
            #line 28 "..\..\..\CustomControls\Selection.xaml"
            this.SelectCan.MouseEnter += new System.Windows.Input.MouseEventHandler(this.MoveCursor_MouseEnter);
            
            #line default
            #line hidden
            
            #line 29 "..\..\..\CustomControls\Selection.xaml"
            this.SelectCan.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.MoveCursor_MouseDown);
            
            #line default
            #line hidden
            
            #line 30 "..\..\..\CustomControls\Selection.xaml"
            this.SelectCan.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.SelectCan_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 5:
            this.SizingGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 6:
            this.LeftTop = ((System.Windows.Shapes.Rectangle)(target));
            
            #line 41 "..\..\..\CustomControls\Selection.xaml"
            this.LeftTop.MouseEnter += new System.Windows.Input.MouseEventHandler(this.LeftTopCursor_MouseEnter);
            
            #line default
            #line hidden
            
            #line 42 "..\..\..\CustomControls\Selection.xaml"
            this.LeftTop.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.LeftTopCurosor_MouseDown);
            
            #line default
            #line hidden
            return;
            case 7:
            this.CenterTop = ((System.Windows.Shapes.Rectangle)(target));
            
            #line 49 "..\..\..\CustomControls\Selection.xaml"
            this.CenterTop.MouseEnter += new System.Windows.Input.MouseEventHandler(this.VerticalCursor_MouseEnter);
            
            #line default
            #line hidden
            
            #line 50 "..\..\..\CustomControls\Selection.xaml"
            this.CenterTop.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.VerticalCursor_MouseDown);
            
            #line default
            #line hidden
            return;
            case 8:
            this.RightTop = ((System.Windows.Shapes.Rectangle)(target));
            
            #line 57 "..\..\..\CustomControls\Selection.xaml"
            this.RightTop.MouseEnter += new System.Windows.Input.MouseEventHandler(this.RightTopCursor_MouseEnter);
            
            #line default
            #line hidden
            
            #line 58 "..\..\..\CustomControls\Selection.xaml"
            this.RightTop.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.RightTopCursor_MouseDown);
            
            #line default
            #line hidden
            return;
            case 9:
            this.RightCenter = ((System.Windows.Shapes.Rectangle)(target));
            
            #line 65 "..\..\..\CustomControls\Selection.xaml"
            this.RightCenter.MouseEnter += new System.Windows.Input.MouseEventHandler(this.HorizontalCursor_MouseEnter);
            
            #line default
            #line hidden
            
            #line 66 "..\..\..\CustomControls\Selection.xaml"
            this.RightCenter.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.HorizontalCursor_MouseDown);
            
            #line default
            #line hidden
            return;
            case 10:
            this.RightBottom = ((System.Windows.Shapes.Rectangle)(target));
            
            #line 73 "..\..\..\CustomControls\Selection.xaml"
            this.RightBottom.MouseEnter += new System.Windows.Input.MouseEventHandler(this.LeftTopCursor_MouseEnter);
            
            #line default
            #line hidden
            
            #line 74 "..\..\..\CustomControls\Selection.xaml"
            this.RightBottom.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.LeftTopCurosor_MouseDown);
            
            #line default
            #line hidden
            return;
            case 11:
            this.CenterBottom = ((System.Windows.Shapes.Rectangle)(target));
            
            #line 81 "..\..\..\CustomControls\Selection.xaml"
            this.CenterBottom.MouseEnter += new System.Windows.Input.MouseEventHandler(this.VerticalCursor_MouseEnter);
            
            #line default
            #line hidden
            
            #line 82 "..\..\..\CustomControls\Selection.xaml"
            this.CenterBottom.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.VerticalCursor_MouseDown);
            
            #line default
            #line hidden
            return;
            case 12:
            this.LeftBottom = ((System.Windows.Shapes.Rectangle)(target));
            
            #line 89 "..\..\..\CustomControls\Selection.xaml"
            this.LeftBottom.MouseEnter += new System.Windows.Input.MouseEventHandler(this.RightTopCursor_MouseEnter);
            
            #line default
            #line hidden
            
            #line 90 "..\..\..\CustomControls\Selection.xaml"
            this.LeftBottom.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.RightTopCursor_MouseDown);
            
            #line default
            #line hidden
            return;
            case 13:
            this.LeftCenter = ((System.Windows.Shapes.Rectangle)(target));
            
            #line 97 "..\..\..\CustomControls\Selection.xaml"
            this.LeftCenter.MouseEnter += new System.Windows.Input.MouseEventHandler(this.HorizontalCursor_MouseEnter);
            
            #line default
            #line hidden
            
            #line 98 "..\..\..\CustomControls\Selection.xaml"
            this.LeftCenter.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.HorizontalCursor_MouseDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

