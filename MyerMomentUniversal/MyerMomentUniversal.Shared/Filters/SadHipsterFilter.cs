﻿/**
 * Copyright (c) 2013-2014 Nokia Corporation.
 * See the license file delivered with this project for more information.
 */

using System.Collections.Generic;

using Lumia.Imaging;
using Lumia.InteropServices;
using Lumia.Imaging.Artistic;

namespace FilterEffects.Filters
{
    public class SadHipsterFilter : SixthGearFilter
    {
        // Constants
        private const double DefaultBrightness = 0.5;
        private const double DefaultSaturation = 0.3;
        private const LomoVignetting DefaultLomoVignetting = LomoVignetting.Medium;
        private const LomoStyle DefaultLomoStyle = LomoStyle.Yellow;

        public SadHipsterFilter()
        {
            Name = "Sad Hipster";
            ShortDescription = "Lomo + Antique";

            Filter.Brightness = DefaultBrightness;
            Filter.Saturation = DefaultSaturation;
            Filter.LomoVignetting = DefaultLomoVignetting;
            Filter.LomoStyle = DefaultLomoStyle;

            LomoVignettingGroup = "SadHipsterLomoVignetting";
        }

        protected override void SetFilters(FilterEffect effect)
        {
            var antiqueFilter = new AntiqueFilter();
            effect.Filters = new List<IFilter> { antiqueFilter, Filter };
        }
    }
}
