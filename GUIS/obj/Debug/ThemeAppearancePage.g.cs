﻿#pragma checksum "..\..\ThemeAppearancePage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "78F6771AA90B21B9FC8C7CAFF458681B"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4214
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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


namespace GUIProj1 {
    
    
    /// <summary>
    /// ThemeAppearancePage
    /// </summary>
    public partial class ThemeAppearancePage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 23 "..\..\ThemeAppearancePage.xaml"
        internal System.Windows.Controls.Label label3;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\ThemeAppearancePage.xaml"
        internal System.Windows.Controls.Label label1;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\ThemeAppearancePage.xaml"
        internal System.Windows.Controls.Label label2;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/iPlan;component/themeappearancepage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ThemeAppearancePage.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.label3 = ((System.Windows.Controls.Label)(target));
            
            #line 23 "..\..\ThemeAppearancePage.xaml"
            this.label3.MouseEnter += new System.Windows.Input.MouseEventHandler(this.label3_MouseEnter);
            
            #line default
            #line hidden
            
            #line 23 "..\..\ThemeAppearancePage.xaml"
            this.label3.MouseLeave += new System.Windows.Input.MouseEventHandler(this.label3_MouseLeave);
            
            #line default
            #line hidden
            return;
            case 2:
            this.label1 = ((System.Windows.Controls.Label)(target));
            
            #line 24 "..\..\ThemeAppearancePage.xaml"
            this.label1.MouseEnter += new System.Windows.Input.MouseEventHandler(this.label1_MouseEnter);
            
            #line default
            #line hidden
            
            #line 24 "..\..\ThemeAppearancePage.xaml"
            this.label1.MouseLeave += new System.Windows.Input.MouseEventHandler(this.label1_MouseLeave);
            
            #line default
            #line hidden
            return;
            case 3:
            this.label2 = ((System.Windows.Controls.Label)(target));
            
            #line 25 "..\..\ThemeAppearancePage.xaml"
            this.label2.MouseEnter += new System.Windows.Input.MouseEventHandler(this.label2_MouseEnter);
            
            #line default
            #line hidden
            
            #line 25 "..\..\ThemeAppearancePage.xaml"
            this.label2.MouseLeave += new System.Windows.Input.MouseEventHandler(this.label2_MouseLeave);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
