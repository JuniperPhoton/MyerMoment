using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Media.Imaging;

namespace MyerMomentUniversal.Model
{
    public abstract class AbstractMomentStyle
    {
        private string _nameID;
        public string NameID
        {
            get{return _nameID;}
            set{_nameID=value;}
        }

        private BitmapImage _image;
        public BitmapImage Image
        {
            get{return _image;}
            set{_image=value;}
        }

            }
}
