using GalaSoft.MvvmLight;
using MyerMomentUniversal.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace MyerMomentUniversal.ViewModel
{
    public class StylesViewModel:ViewModelBase
    {
        private ObservableCollection<MomentStyle> _styles;
        public ObservableCollection<MomentStyle> Styles
        {
            get
            {
                return _styles ?? (_styles = new ObservableCollection<MomentStyle>());
            }
            set
            {
                if(_styles!=value)
                {
                    _styles = value;
                    RaisePropertyChanged(() => Styles);
                }
            }
        }

        public StylesViewModel()
        {
            Styles = new ObservableCollection<MomentStyle>();
            //ConfigStyleListAsync();
        }

        public void ConfigStyleListAsync()
        {
            Styles.Add(new MomentStyle("Alone"));
            Styles.Add(new MomentStyle("Amazing"));
            Styles.Add(new MomentStyle("Brave"));
            Styles.Add(new MomentStyle("Couple"));
            Styles.Add(new MomentStyle("Coffee"));
            Styles.Add(new MomentStyle("Dinner"));
            Styles.Add(new MomentStyle("Food"));
            Styles.Add(new MomentStyle("GTA5"));
            Styles.Add(new MomentStyle("Lumia"));
            Styles.Add(new MomentStyle("Love"));
            Styles.Add(new MomentStyle("Memory"));
            Styles.Add(new MomentStyle("Music"));
            Styles.Add(new MomentStyle("Night"));
            Styles.Add(new MomentStyle("Place"));
            Styles.Add(new MomentStyle("Sad"));
            Styles.Add(new MomentStyle("Scene"));
            Styles.Add(new MomentStyle("Thanks"));
            Styles.Add(new MomentStyle("Time"));

            //var style = new MomentStyle("Test", new Uri("http://121.41.21.21/MyerMoment/Style/Test.jpg"), new Uri("http://121.41.21.21/MyerMoment/Style/Test.png"));
            //await style.CheckStyleExist();
            //Styles.Add(style);
        }
    }
}
