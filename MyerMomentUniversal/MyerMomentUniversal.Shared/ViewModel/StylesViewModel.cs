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
                    if (NewStyles.Count == 0)
                    {
                        NoItemsVisibility = Visibility.Visible;
                    }
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

        private TaskCompletionSource<int> tcs;

        public StylesViewModel(bool ConfigLocalStyle=true)
        {
            NewStyles = new ObservableCollection<MomentStyle>();
            PackageStyles = new ObservableCollection<MomentStyle>();

            tcs = new TaskCompletionSource<int>();

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
                tcs = new TaskCompletionSource<int>();

                var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("WebStyles", CreationCollisionOption.OpenIfExists);
                var files = await folder.GetFilesAsync();
                if (files.Count == 0)
                {
                    tcs.SetResult(0);
                    return;
                }
                foreach (var file in files)
                {
                    if (file.DisplayName.EndsWith("2")) continue;
                    var newStyle = new MomentStyle(file.DisplayName, new Uri(file.Path), new Uri(file.Path));
                    newStyle.SetUpInstalledStyle();
                    PackageStyles.Insert(0,newStyle);
                    
                    InstalledStylesList.Add(file.DisplayName);
                }

                tcs.SetResult(0);
            }
            catch(Exception e)
            {
                await ExceptionHelper.WriteRecord(e);
                tcs.SetResult(0);
            }
           
        }


        public async Task ConfigWebStyle(string[] styleNames)
        {
            await tcs.Task;

            IsLoadingVisibility = Visibility.Visible;
            NewStyles = new ObservableCollection<MomentStyle>();

            foreach(var name in styleNames)
            {
                if (string.IsNullOrEmpty(name)) continue;

                if (InstalledStylesList.Contains(name))
                {
                    continue;
                }
                
                var style = new MomentStyle(name, HttpHelper.GetUri(name+ "2.png"), HttpHelper.GetUri(name+ ".png"));
                await style.CheckThumbExistAndSaveAsync();
                NewStyles.Insert(0,style);
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

        public async Task DownloadFullsizeCommand(string name)
        {
            var style = NewStyles.ToList().Find((s) =>
            {
                if (s.NameID == name) return true;
                else return false;
            });
            if (style != null && !style.IsDownloaded)
            {
                var success=await style.GetStyle(StyleFileType.FullSize);
                if(success)
                {
                    NewStyles.Remove(style);
                    PackageStyles.Insert(0, style);

                    if(NewStyles.Count==0)
                    {
                        NoItemsVisibility = Visibility.Visible;
                    }
                }
            }
        }

        public async Task DeleteStyle(string name)
        {
            var delete=await DeleteStyleFile(name);
            if(!delete)
            {
                return;
            }
            InstalledStylesList.Remove(InstalledStylesList.ToList().Find(n =>
            {
                if (n ==name) return true;
                else return false;
            }));
            PackageStyles.Remove(PackageStyles.ToList().Find(s=>
            {
                if (s.NameID == name) return true;
                else return false;
            }));
            
        }

        private async Task<bool> DeleteStyleFile(string displayName)
        {
            return await ExceptionHelper.TryExecute<bool>(async () =>
            {
                var folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("WebStyles");
                var thumbFile = await JP.Utils.Data.StorageFileHandleHelper.TryGetFile(folder, displayName + ".png");
                if(thumbFile==null)
                {
                    return false;
                }
                var fullSizeFile = await JP.Utils.Data.StorageFileHandleHelper.TryGetFile(folder, displayName + "2.png");
                if(fullSizeFile==null)
                {
                    return false;
                }
                await thumbFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                await fullSizeFile.DeleteAsync(StorageDeleteOption.PermanentDelete);

                return true;
            });     
        }
    }
}
