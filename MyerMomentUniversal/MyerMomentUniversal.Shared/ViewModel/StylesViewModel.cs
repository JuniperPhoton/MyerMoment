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
using Windows.UI.Xaml;
using JP.Utils.Debug;
using GalaSoft.MvvmLight.Command;
using Windows.UI.Xaml.Controls;

namespace MyerMomentUniversal.ViewModel
{
    public class StylesViewModel:ViewModelBase
    {
        private const  int IMAGE_NUM=13;

        private Visibility _noItemsVisibility;
        public Visibility NoItemsVisibility
        {
            get
            {
                return _noItemsVisibility;
            }
            set
            {
                _noItemsVisibility = value;
                RaisePropertyChanged(() => NoItemsVisibility);
            }

        }

        private Visibility _isLoadingVisibility;
        public Visibility IsLoadingVisibility
        {
            get
            {
                return _isLoadingVisibility;
            }
            set
            {
                _isLoadingVisibility = value;
                RaisePropertyChanged(() => IsLoadingVisibility);
            }
        }

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
            ConfigInstalledStyle();
            NumStyle(PackageStyles);
        }

        /// <summary>
        /// 加载预安装的样式
        /// </summary>
        private void ConfigPackageStyle()
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
        }

        /// <summary>
        /// 加载来自Web的已经安装的样式
        /// </summary>
        private void ConfigInstalledStyle()
        {
            ExceptionHelper.TryExecute(async() =>
            {
                var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("WebStyles", CreationCollisionOption.OpenIfExists);
                var files = await folder.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.OrderByName);
                if (files.Count == 0) return;
                foreach (var file in files)
                {
                    var newStyle = new MomentStyle(file.DisplayName, new Uri(file.Path), new Uri(file.Path));
                }
            });
           
        }

        /// <summary>
        /// 为样式添加背景图案
        /// </summary>
        private void NumStyle(ObservableCollection<MomentStyle> list)
        {
           for(int i=0;i<= list.Count-1;i++)
            {
                var style= list.ToList().ElementAt(i);
                style.imageNum = i % IMAGE_NUM;
            }
        }

        public async Task ConfigWebStyle(string[] styleNames)
        {
            IsLoadingVisibility = Visibility.Visible;
            foreach(var name in styleNames)
            {
                if (string.IsNullOrEmpty(name)) continue;
                var style = new MomentStyle(name, HttpHelper.GetUri(name, ".jpg"), HttpHelper.GetUri(name, ".png"));
                await style.CheckThumbExistAndSaveAsync();
                NewStyles.Add(style);
                NumStyle(NewStyles);
            }

            if (NewStyles.Count == 0) NoItemsVisibility = Visibility.Visible;
            else NoItemsVisibility = Visibility.Collapsed;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += ((sendert, et) =>
              {
                  IsLoadingVisibility = Visibility.Collapsed;
              });
            timer.Start();
            
        }
    }
}
