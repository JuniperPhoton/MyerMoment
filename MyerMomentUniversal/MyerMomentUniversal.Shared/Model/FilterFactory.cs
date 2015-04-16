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
    public enum FilterKind
    {
        Original,
        AutoEnhanceFilter,
        CartoonFilter,
        GrayscaleFilter,
        LomoFilter,
        SunFilter,
        NoiseFilter,
        OilyFilter,
        BlurFilter,
        ColorBoostFilter,
    }

    public class FilterFactory
    {
        private static List<string> FilterList;

        public static List<IFilter> CreateFilterEffects(FilterKind kind)
        {
            switch(kind)
            {
                case FilterKind.AutoEnhanceFilter: return new List<IFilter>(){new AutoEnhanceFilter()};
                case FilterKind.BlurFilter: return new List<IFilter>(){new BlurFilter(50)};
                case FilterKind.ColorBoostFilter: return  new List<IFilter>(){new ColorBoostFilter(0.7)};
                case FilterKind.CartoonFilter: return  new List<IFilter>(){new CartoonFilter()};
                case FilterKind.GrayscaleFilter: return  new List<IFilter>(){new GrayscaleFilter()};
                case FilterKind.LomoFilter: return  new List<IFilter>(){new LomoFilter()};
                case FilterKind.NoiseFilter: return  new List<IFilter>(){new NoiseFilter(NoiseLevel.Medium)};
                case FilterKind.OilyFilter: return  new List<IFilter>(){new OilyFilter(OilBrushSize.Small)};
                case FilterKind.SunFilter: return new List<IFilter>() { new LomoFilter(), new LomoFilter(0.3, 0.2, LomoVignetting.Low, LomoStyle.Yellow) };
                default: return null;
            }
        }

        public static List<string> GetFilterList()
        {
            FilterList = new List<string>();

            FilterList.Add("Original");
            FilterList.Add("Auto");
            FilterList.Add("Cartoon");
            FilterList.Add("Gray");
            FilterList.Add("Lomo");
            FilterList.Add("Sun");
            FilterList.Add("Noise");
            FilterList.Add("Oily");
            FilterList.Add("Blur");
            FilterList.Add("Boost");

            return FilterList;
        }

    }
}
