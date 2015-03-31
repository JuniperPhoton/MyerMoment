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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MyerMomentUniversal
{ 
    
    public sealed partial class CropControl : UserControl
    {
        double lefttopAllX = 0;
        double lefttopAllY = 0;

        double leftdownAllX = 0;
        double leftdownAllY = 0;

        double righttopAllX = 0;
        double righttopAllY = 0;

        double rightdownAllX = 0;
        double rightdownAllY = 0;

        private TranslateTransform _translateTransformLeftTop = new TranslateTransform();
        private TranslateTransform _translateTransformLeftDown = new TranslateTransform();
        private TranslateTransform _translateTransformRightDown = new TranslateTransform();
        private TranslateTransform _translateTransformRightTop = new TranslateTransform();

        public CropControl()
        {
            this.InitializeComponent();
            
            leftdown.RenderTransform = _translateTransformLeftDown;
            righttop.RenderTransform = _translateTransformRightTop;
            rightdown.RenderTransform = _translateTransformRightDown;
        }

        private void lefttop_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (0<= lefttopAllX && lefttopAllX<=rootBorder.ActualWidth)
            {
                //rootBorder.Width -= e.Delta.Translation.X;
                if (lefttopAllX >= 0)
                {
                    lefttopAllX += e.Delta.Translation.X;
                    _translateTransformLeftTop.X += e.Delta.Translation.X;
                }
               
                if (lefttopAllX < 0) lefttopAllX = 0;
                
                alllefttb.Text = lefttopAllX.ToString();
            }

            if (0<=lefttopAllY && lefttopAllY<= rootBorder.ActualHeight)
            {
                //rootBorder.Height -= e.Delta.Translation.Y;
                if (lefttopAllY >= 0)
                {
                    lefttopAllY += e.Delta.Translation.Y;
                    
                    _translateTransformLeftTop.Y += e.Delta.Translation.Y;
                }
                if (lefttopAllY < 0) lefttopAllY = 0;
            }

        }

        private void righttop_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {

        }

        private void leftdown_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {

        }

        private void rightdown_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {

        }

        private void lefttop_ManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
        {
            lefttop.RenderTransform = _translateTransformLeftTop;
            lefttop.ManipulationDelta -= lefttop_ManipulationDelta;
            lefttop.ManipulationDelta += lefttop_ManipulationDelta;
        }
    }
}
