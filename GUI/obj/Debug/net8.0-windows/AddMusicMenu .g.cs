﻿#pragma checksum "..\..\..\AddMusicMenu .xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "48C0C8C2A07B0B10B78366C927C8D32374E1EBFE"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using KostPostMusic;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
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


namespace KostPostMusic {
    
    
    /// <summary>
    /// AddMusicMenu
    /// </summary>
    public partial class AddMusicMenu : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 12 "..\..\..\AddMusicMenu .xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton SingleSongRadioButton;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\AddMusicMenu .xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BrowseFileButton;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\AddMusicMenu .xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock SelectedFileTextBlock;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\AddMusicMenu .xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox MusicNameTextBox;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\AddMusicMenu .xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock DurationTextBlock;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\AddMusicMenu .xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button AddMusicButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.3.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/GUI;component/addmusicmenu%20.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\AddMusicMenu .xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.3.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.SingleSongRadioButton = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 2:
            this.BrowseFileButton = ((System.Windows.Controls.Button)(target));
            
            #line 15 "..\..\..\AddMusicMenu .xaml"
            this.BrowseFileButton.Click += new System.Windows.RoutedEventHandler(this.BrowseFileButton_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.SelectedFileTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.MusicNameTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.DurationTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.AddMusicButton = ((System.Windows.Controls.Button)(target));
            
            #line 23 "..\..\..\AddMusicMenu .xaml"
            this.AddMusicButton.Click += new System.Windows.RoutedEventHandler(this.AddMusicButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

