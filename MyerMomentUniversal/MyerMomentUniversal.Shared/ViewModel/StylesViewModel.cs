using GalaSoft.MvvmLight;
using MyerMomentUniversal.Helper;
using MyerMomentUniversal.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.Linq;

namespace MyerMomentUniversal.ViewModel
{
    public class StylesViewModel:ViewModelBase
    {
        private ObservableCollection<MomentStyle> _newStyles;
        public ObservableCollection<MomentStyle> NewStyles
        {
            get
            {
                return _newStyles ?? (_newStyles = new ObservableCollection<MomentStyle>());
            }
            set
            {
                if(_newStyles!=value)
                {
                    _newStyles = value;
                    RaisePropertyChanged(() => NewStyles);
                }
            }
        }

        private ObservableCollection<MomentStyle> _packageStyles;
        public ObservableCollection<MomentStyle> PackageStyles
        {
            get
            {
                return _packageStyles ?? (_packageStyles = new ObservableCollection<MomentStyle>());
            }
            set
            {
                if (_packageStyles != value)
                {
                    _packageStyles = value;
                    RaisePropertyChanged(() => PackageStyles);
                }
            }
        }

        public StylesViewModel()
        {
            NewStyles = new ObservableCollection<MomentStyle>();
            PackageStyles = new ObservableCollection<MomentStyle>();
            ConfigPackageStyle();
            //ConfigInstalledStyle();
            NumStyle();
        }

        public void ConfigPackageStyle()
        {
            PackageStyles.Add(new MomentStyle("Alone"));
            PackageStyles.Add(new MomentStyle("Amazing"));
            PackageStyles.Add(new MomentStyle("Brave"));
            PackageStyles.Add(new MomentStyle("Couple"));
            PackageStyles.Add(new MomentStyle("Coffee"));
            PackageStyles.Add(new MomentStyle("Dinner"));
            PackageStyles.Add(new MomentStyle("Food"));
            PackageStyles.Add(new MomentStyle("GTA5"));
            PackageStyles.Add(new MomentStyle("Lumia"));
            PackageStyles.Add(new MomentStyle("Love"));
            PackageStyles.Add(new MomentStyle("Memory"));
            PackageStyles.Add(new MomentStyle("Music"));
            PackageStyles.Add(new MomentStyle("Night"));
            PackageStyles.Add(new MomentStyle("Place"));
            PackageStyles.Add(new MomentStyle("Sad"));
            PackageStyles.Add(new MomentStyle("Scene"));
            PackageStyles.Add(new MomentStyle("Thanks"));
            PackageStyles.Add(new MomentStyle("Time"));

            NewStyles.Add(new MomentStyle("Time"));
        }

        public async void ConfigInstalledStyle()
        {
            var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("WebStyles", CreationCollisionOption.OpenIfExists);
            var files = await folder.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.OrderByName);
            foreach(var file in files)
            {
                var newStyle = new MomentStyle(file.DisplayName, new Uri(file.Path), new Uri(file.Path));
            }
        }

        public void NumStyle()
        {
           for(int i=0;i<=PackageStyles.Count-1;i++)
            {
                var style= PackageStyles.ToList().ElementAt(i);
                style.imageNum = i % 7;
            }
        }

        public async void ConfigWebStyle(string[] styleNames)
        {
            foreach(var name in styleNames)
            {
                if (string.IsNullOrEmpty(name)) continue;
                var style = new MomentStyle(name, HttpHelper.GetUri(name, ".jpg"), HttpHelper.GetUri(name, ".png"));
                await style.CheckStyleExistAndSaveAsync();
                NewStyles.Add(style);
            }
        }
    }
}
