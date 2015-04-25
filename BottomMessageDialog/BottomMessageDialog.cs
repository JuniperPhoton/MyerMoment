using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls.Primitives;

namespace Dialog
{
    public class BottomMessageDialog : ContentControl
    {
        public SolidColorBrush DialogBackground
        {
            get { return GetValue(DialogBackgroundProperty) as SolidColorBrush; }
            set { SetValue(DialogBackgroundProperty, value); }
        }
        public static DependencyProperty DialogBackgroundProperty = DependencyProperty.Register("DialogBackground",
            typeof(SolidColorBrush), typeof(BottomMessageDialog), new PropertyMetadata(null));

        public SolidColorBrush DialogForeground
        {
            get { return GetValue(DialogForegroundProperty) as SolidColorBrush; }
            set { SetValue(DialogForegroundProperty, null); }
        }

        public static DependencyProperty DialogForegroundProperty = DependencyProperty.Register("DialogForeground",
            typeof(SolidColorBrush), typeof(BottomMessageDialog), new PropertyMetadata(null));

        public SolidColorBrush LeftButtonBackground
        {
            get { return GetValue(LeftButtonBackgroundProperty) as SolidColorBrush; }
            set { SetValue(LeftButtonBackgroundProperty, null); }
        }

        public static DependencyProperty LeftButtonBackgroundProperty = DependencyProperty.Register("LeftButtonBackground",
            typeof(SolidColorBrush), typeof(BottomMessageDialog), new PropertyMetadata(null));

        public SolidColorBrush RightButtonBackground
        {
            get { return GetValue(RightButtonBackgroundProperty) as SolidColorBrush; }
            set { SetValue(RightButtonBackgroundProperty, null); }
        }

        public static DependencyProperty RightButtonBackgroundProperty = DependencyProperty.Register("RightButtonBackground",
            typeof(SolidColorBrush), typeof(BottomMessageDialog), new PropertyMetadata(null));

        public SolidColorBrush LeftButtonForeground
        {
            get { return GetValue(LeftButtonForegroundProperty) as SolidColorBrush; }
            set { SetValue(LeftButtonForegroundProperty, null); }
        }

        public static DependencyProperty LeftButtonForegroundProperty = DependencyProperty.Register("LeftButtonForeground",
            typeof(SolidColorBrush), typeof(BottomMessageDialog), new PropertyMetadata(null));

        public SolidColorBrush RightButtonForeground
        {
            get { return GetValue(RightButtonForegroundProperty) as SolidColorBrush; }
            set { SetValue(RightButtonForegroundProperty, null); }
        }

        public static DependencyProperty RightButtonForegroundProperty = DependencyProperty.Register("RightButtonForeground",
            typeof(SolidColorBrush), typeof(BottomMessageDialog), new PropertyMetadata(null));

        private bool _isOnFrame = false;
        private TextBlock _titleTextBlock;
        private TextBlock _contentTextBlock;
        private Button _leftButton;
        private Button _rightButton;

        //public EventHandler<DialogHandleEventArgs> LeftCallback { get; set; }
        //public EventHandler<DialogHandleEventArgs> RightCallback { get; set; }

        public BottomMessageDialog()
        {
            DefaultStyleKey = typeof(BottomMessageDialog);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _titleTextBlock = GetTemplateChild("Title") as TextBlock;
            _contentTextBlock = GetTemplateChild("Content") as TextBlock;
            _leftButton = GetTemplateChild("LeftButton") as Button;
            _rightButton = GetTemplateChild("RightButton") as Button;

            _leftButton.Click += (s, e) =>
                {
                    //LeftCallback.Invoke(this, new DialogHandleEventArgs());
                };
            _rightButton.Click += (s, e) =>
                {
                    //RightCallback.Invoke(this, new DialogHandleEventArgs());
                };
        }

        public void ShowDialog(string title, string content, 
            EventHandler<DialogHandleEventArgs> leftAct, 
            EventHandler<DialogHandleEventArgs> rightAct, string leftBtnContent = "OK", string rightBtnContent = "CANCEL")
        {
            if(_isOnFrame)
            {
                HideDialog();
            }
            
            this._titleTextBlock.Text = title;
            this._contentTextBlock.Text = content;
            this._leftButton.Content = leftBtnContent;
            this._rightButton.Content = rightBtnContent;

           // LeftCallback = leftAct;
           // RightCallback = rightAct;

            _isOnFrame = true;
        }

        public void HideDialog()
        {

        }

    }
}
