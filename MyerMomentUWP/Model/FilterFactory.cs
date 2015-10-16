using Lumia.Imaging;
using Lumia.Imaging.Adjustments;
using Lumia.Imaging.Artistic;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation;
using Windows.UI;

namespace MyerMomentUniversal.Model
{

    public class FilterFactory
    {
        private static List<string> FilterList;

        public static List<IFilter> CreateFilterEffects(FilterKind kind)
        {
            switch(kind)
            {
                case FilterKind.BlurFilter: return new List<IFilter>(){new BlurFilter(50)};
                case FilterKind.ColorBoostFilter: return  new List<IFilter>(){new ColorBoostFilter(0.7)};
                case FilterKind.CartoonFilter: return  new List<IFilter>(){new CartoonFilter()};
                case FilterKind.GrayscaleFilter: return  new List<IFilter>(){new GrayscaleFilter()};
                case FilterKind.ExposureFilter:return new List<IFilter>() { new ExposureFilter(ExposureMode.Gamma,0.2)};
                case FilterKind.LomoFilter: return  new List<IFilter>(){new LomoFilter()};
                case FilterKind.NoiseFilter: return  new List<IFilter>(){new NoiseFilter(NoiseLevel.Medium)};
                case FilterKind.SunFilter: return new List<IFilter>() { new LomoFilter(), new LomoFilter(0.3, 0.2, LomoVignetting.Low, LomoStyle.Yellow) };
                default: return new List<IFilter>(){new AutoEnhanceFilter()};
            }
        }

        public static List<string> GetFilterList()
        {
            FilterList = new List<string>();

            FilterList.Add("Original");
            FilterList.Add("Cartoon");
            FilterList.Add("Gray");
            FilterList.Add("Exposure");
            FilterList.Add("Lomo");
            FilterList.Add("Sun");
            FilterList.Add("Noise");
            FilterList.Add("Blur");
            FilterList.Add("Boost");

            return FilterList;
        }

    }
}
