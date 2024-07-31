﻿#pragma checksum "..\..\..\CustomControls\Selection.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "771FB6FAA97F5852ADED40640012005F4445F82E491FBF5D0E7002F2E096913F"
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
        
        
        #line 12 "..\..\..\CustomControls\Selection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border SelectionBorder;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\CustomControls\Selection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas SelectCan;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\CustomControls\Selection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid SizingGrid;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\CustomControls\Selection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle LeftTop;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\CustomControls\Selection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle CenterTop;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\CustomControls\Selection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle RightTop;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\CustomControls\Selection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle RightCenter;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\CustomControls\Selection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle RightBottom;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\..\CustomControls\Selection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle CenterBottom;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\CustomControls\Selection.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle LeftBottom;
        
        #line default
        #line hidden
        
        
        #line 57 "..\..\..\CustomControls\Selection.xaml"
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
            this.SelectionBorder = ((System.Windows.Controls.Border)(target));
            
            #line 14 "..\..\..\CustomControls\Selection.xaml"
            this.SelectionBorder.SizeChanged += new System.Windows.SizeChangedEventHandler(this.SelectionBorder_SizeChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.SelectCan = ((System.Windows.Controls.Canvas)(target));
            
            #line 17 "..\..\..\CustomControls\Selection.xaml"
            this.SelectCan.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.SelectCan_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 4:
            this.SizingGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 5:
            this.LeftTop = ((System.Windows.Shapes.Rectangle)(target));
            return;
            case 6:
            this.CenterTop = ((System.Windows.Shapes.Rectangle)(target));
            return;
            case 7:
            this.RightTop = ((System.Windows.Shapes.Rectangle)(target));
            return;
            case 8:
            this.RightCenter = ((System.Windows.Shapes.Rectangle)(target));
            return;
            case 9:
            this.RightBottom = ((System.Windows.Shapes.Rectangle)(target));
            return;
            case 10:
            this.CenterBottom = ((System.Windows.Shapes.Rectangle)(target));
            return;
            case 11:
            this.LeftBottom = ((System.Windows.Shapes.Rectangle)(target));
            return;
            case 12:
            this.LeftCenter = ((System.Windows.Shapes.Rectangle)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

