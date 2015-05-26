using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MicroMsg;


namespace MyerMomentUniversal
{

    public sealed partial class EntryPage : WXEntryBasePage
    {
        public EntryPage()
        {
            this.InitializeComponent();
            
        }

        public override void On_SendMessageToWX_Response(SendMessageToWX.Resp response)
        {
            if(Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
}
