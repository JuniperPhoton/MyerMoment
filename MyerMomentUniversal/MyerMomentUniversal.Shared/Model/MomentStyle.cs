using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MyerMomentUniversal.Model
{
    public class MomentStyle
    {
        private string _nameID;
        public string NameID
        {
            get { return _nameID; }
            set { _nameID = value; }
        }

        private BitmapImage _image;
        public BitmapImage Image
        {
            get { return _image; }
            set { _image = value; }
        }

        private BitmapImage _previewImage;
        public BitmapImage PreviewImge
        {
            get
            {
                return _previewImage;
            }
            set
            {
                _previewImage = value;
            }
        }

        public MomentStyle(string nameID)
        {
            NameID = nameID;

            Image = new BitmapImage();
            Image.UriSource = new Uri("ms-appx:///Asset/"+nameID+".png", UriKind.RelativeOrAbsolute);

            PreviewImge = new BitmapImage();
            PreviewImge.UriSource = new Uri("ms-appx:///Asset/" + nameID + ".jpg", UriKind.RelativeOrAbsolute);
        }

    }
}
