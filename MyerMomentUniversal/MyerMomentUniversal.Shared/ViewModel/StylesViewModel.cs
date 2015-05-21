using GalaSoft.MvvmLight;
using MyerMomentUniversal.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using System.Linq;
using Windows.UI.Xaml;
using JP.Utils.Debug;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json.Linq;
using Windows.ApplicationModel;
using Windows.Storage.Streams;

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

        //private TaskCompletionSource<int> tcs;

        public StylesViewModel()
        {
            NewStyles = new ObservableCollection<MomentStyle>();
            PackageStyles = new ObservableCollection<MomentStyle>();

            //tcs = new TaskCompletionSource<int>();

            
            IsLoadingVisibility = Visibility.Collapsed;
        }

        public async Task ConfigLocalAsync()
        {
            var task1=ConfigPackageStyle();
            var task2= ConfigInstalledStyle();
            await task1;
            await task2;
            //Task.WaitAll(new Task[2] { task1, task2 });
        }

        /// <summary>
        /// 加载预安装的样式
        /// </summary>
        private async Task ConfigPackageStyle()
        {
            var folder = await Package.Current.InstalledLocation.GetFolderAsync("Config");
            var file = await folder.GetFileAsync("LocalStyleConfig.txt");

            var str = await FileIO.ReadTextAsync(file);
            var styles = str.Split(',');
            foreach (var stylename in styles)
            {
                PackageStyles.Add(new MomentStyle(stylename));
            }
        }

        /// <summary>
        /// 加载来自Web的已经安装的样式
        /// </summary>
        private async Task ConfigInstalledStyle()
        {
            try
            {
                //tcs = new TaskCompletionSource<int>();

                IsLoadingVisibility = Visibility.Visible;

                var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("WebStyles", CreationCollisionOption.OpenIfExists);
                var files = await folder.GetFilesAsync();
                if (files.Count == 0)
                {
                    //tcs.SetResult(0);
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

                //tcs.SetResult(0);

                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += ((st, et) =>
                  {
                      IsLoadingVisibility = Visibility.Collapsed;
                      timer.Stop();
                  });
                timer.Start();
            }
            catch(Exception e)
            {
                await ExceptionHelper.WriteRecord(e);
                //tcs.SetResult(0);
            }
           
        }


        public async Task ConfigWebStyle(JArray styles)
        {
            try
            {
                //await tcs.Task;

                IsLoadingVisibility = Visibility.Visible;
                NewStyles = new ObservableCollection<MomentStyle>();

                foreach (var style in styles)
                {
                    if (style == null) continue;
                    var nameid = (string)style["name"];
                    var thumbUri = (string)style["basicUrl"] + nameid + ".png";
                    var fullsizeUri = (string)style["basicUrl"] + nameid + "2.png";
                    if (InstalledStylesList.Contains(nameid))
                    {
                        continue;
                    }

                    var newstyle = new MomentStyle(nameid, new Uri(thumbUri), new Uri(fullsizeUri));
                    await newstyle.CheckThumbAndSaveAsync();
                    NewStyles.Insert(0, newstyle);
                }

                if (NewStyles.Count == 0) NoItemsVisibility = Visibility.Visible;
                else NoItemsVisibility = Visibility.Collapsed;

                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += ((sendert, et) =>
                {
                    IsLoadingVisibility = Visibility.Collapsed;
                    timer.Stop();
                });
                timer.Start();

            }
            catch(Exception e)
            {

            }
            
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
