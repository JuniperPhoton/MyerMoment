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
       

        public List<string> InstalledStylesList = new List<string>();

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

        private RelayCommand<string> _downloadCommand;
        public RelayCommand<string> DownloadCommand
        {
            get
            {
                if (_downloadCommand != null) return _downloadCommand;
                else
                {
                    return _downloadCommand = new RelayCommand<string>(async (name) =>
                    {
                        await DownloadFullsizeCommand(name);
                    });
                }
            }
        }

        public StylesViewModel(bool ConfigLocalStyle=true)
        {
            NewStyles = new ObservableCollection<MomentStyle>();
            PackageStyles = new ObservableCollection<MomentStyle>();
            if (ConfigLocalStyle)
            {
                var task = Config();
            }
        }

        public async Task Config()
        {
            ConfigPackageStyle();
            await ConfigInstalledStyle();
            //NumStyle(PackageStyles);
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
        private async Task ConfigInstalledStyle()
        {
            try
            {
                var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("WebStyles", CreationCollisionOption.OpenIfExists);
                var files = await folder.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.DefaultQuery);
                if (files.Count == 0) return;
                foreach (var file in files)
                {
                    if (file.DisplayName.EndsWith("2")) continue;
                    var newStyle = new MomentStyle(file.DisplayName, new Uri(file.Path), new Uri(file.Path));
                    newStyle.SetUpInstalledStyle();
                    PackageStyles.Insert(0,newStyle);
                    InstalledStylesList.Add(file.DisplayName);
                }
            }
            catch(Exception e)
            {
                await ExceptionHelper.WriteRecord(e);
            }
           
        }


        public async Task ConfigWebStyle(string[] styleNames)
        {
            IsLoadingVisibility = Visibility.Visible;
            foreach(var name in styleNames)
            {
                if (string.IsNullOrEmpty(name)) continue;
                var style = new MomentStyle(name, HttpHelper.GetUri(name+ "2.png"), HttpHelper.GetUri(name+ ".png"));
                await style.CheckThumbExistAndSaveAsync();
                NewStyles.Insert(0,style);
                if(InstalledStylesList.Contains(style.NameID))
                {
                    style.IsDownloaded = true;
                }
            }

            //NumStyle(NewStyles);

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

        public async Task DownloadFullsizeCommand(string name)
        {
            var style = NewStyles.ToList().Find((s) =>
            {
                if (s.NameID == name) return true;
                else return false;
            });
            if (style != null && !style.IsDownloaded)
            {
                await style.GetStyle(StyleFileType.FullSize);
                PackageStyles.Clear();
                ConfigPackageStyle();
                await ConfigInstalledStyle();
                //NumStyle(PackageStyles);
            }
        }
    }
}
