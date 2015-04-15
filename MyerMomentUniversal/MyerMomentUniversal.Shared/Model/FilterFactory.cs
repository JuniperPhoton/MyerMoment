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
        BlurFilter,
        ColorBoostFilter,
        CartoonFilter,
        GrayscaleFilter,
        LomoFilter,
        NoiseFilter,
        OilyFilter 
    }

    public class FilterFactory
    {
        private static List<string> FilterList;

        public static IFilter CreateFilterEffect(FilterKind kind)
        {
            switch(kind)
            {
                case FilterKind.AutoEnhanceFilter: return new AutoEnhanceFilter();
                case FilterKind.BlurFilter: return new BlurFilter(50);
                case FilterKind.ColorBoostFilter: return new ColorBoostFilter(0.7);
                case FilterKind.CartoonFilter: return new CartoonFilter();
                case FilterKind.GrayscaleFilter: return new GrayscaleFilter();
                case FilterKind.LomoFilter: return new LomoFilter();
                case FilterKind.NoiseFilter: return new NoiseFilter(NoiseLevel.Medium);
                case FilterKind.OilyFilter: return new OilyFilter(OilBrushSize.Small);
                default: return null;
            }
        }

        public static List<string> GetFilterList()
        {
            FilterList = new List<string>();

            FilterList.Add("Original");
            FilterList.Add("Auto");
            FilterList.Add("Blur");
            FilterList.Add("Boost");
            FilterList.Add("Cartoon");
            FilterList.Add("Gray");
            FilterList.Add("Lomo");
            FilterList.Add("Noise");
            FilterList.Add("Oily");

            return FilterList;
        }

    }
}
