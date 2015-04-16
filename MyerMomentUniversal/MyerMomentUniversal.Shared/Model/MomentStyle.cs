using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;
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

        /// <summary>
        /// 初始化Style类
        /// </summary>  
        /// <param name="nameID">图片的名字</param>
        /// <param name="isFromUser">是否来自用户的导入</param>
        public MomentStyle(string nameID,bool isFromUser)
        {
            NameID = nameID;

            if(!isFromUser)
            {
                Image = new BitmapImage();
                Image.UriSource = new Uri("ms-appx:///Asset/Style/" + nameID + ".png", UriKind.RelativeOrAbsolute);

                PreviewImge = new BitmapImage();
                PreviewImge.UriSource = new Uri("ms-appx:///Asset/Style/" + nameID + ".jpg", UriKind.RelativeOrAbsolute);
            }
            else
            {
                ImportStyle(nameID);
            }
            
        }

        private async void ImportStyle(string nameID)
        {
            var folder = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync("MyerMomentStyles", CreationCollisionOption.OpenIfExists);
            var file = await folder.GetFileAsync(nameID);
            if(file!=null)
            {
                using(var fileStream=await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    this.Image.SetSource(fileStream);
                }
            }
        }

    }
}
