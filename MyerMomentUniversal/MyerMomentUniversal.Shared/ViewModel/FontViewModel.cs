using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;

namespace MyerMomentUniversal.ViewModel
{
    public class FontViewModel:ViewModelBase
    {
        private ObservableCollection<StorageFile> fontList;
        public ObservableCollection<StorageFile> FontList
        {
            get
            {
                return fontList;
            }
            set
            {
                fontList = value;
                RaisePropertyChanged(() => FontList);
            }
        }

        private RelayCommand importCommand;
        public RelayCommand ImportCommand
        {
            get
            {
                if (importCommand != null) return importCommand;

                return importCommand = new RelayCommand(() =>
                {
                    Import();
                });
            }
        }

        public FontViewModel()
        {
            FontList = new ObservableCollection<StorageFile>();
            ReadFontsList();
        }


        public void Import()
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.Downloads;
            picker.FileTypeFilter.Add(".ttf");
            picker.PickSingleFileAndContinue();
        }

        public async void ReadFontsList()
        {
            var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Font", CreationCollisionOption.OpenIfExists);
            var fileList = await folder.GetFilesAsync();
            foreach (var file in fileList)
            {
                FontList.Add(file);
            }
        }

        public async void SaveFont(StorageFile fontFile)
        {
            var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Font", CreationCollisionOption.OpenIfExists);

            await fontFile.CopyAsync(folder);

            ReadFontsList();
        }
    }
}
