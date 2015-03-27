﻿/**
 * Copyright (c) 2013-2014 Nokia Corporation.
 * See the license file delivered with this project for more information.
 */

using Lumia.Imaging;
using Lumia.InteropServices;

namespace FilterEffects.Filters
{
    public class OriginalImageFilter : AbstractFilter
    {
        public OriginalImageFilter()
        {
            Name = "Original";
            ShortDescription = "No filters";
        }

        protected override void SetFilters(FilterEffect effect)
        {
            // No need to do anything
        }
    }
}
