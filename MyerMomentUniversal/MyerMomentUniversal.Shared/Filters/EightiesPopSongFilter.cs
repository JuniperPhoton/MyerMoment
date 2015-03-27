﻿/**
 * Copyright (c) 2013-2014 Nokia Corporation.
 * See the license file delivered with this project for more information.
 */

using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

using Lumia.Imaging;
using Lumia.InteropServices;
using Lumia.Imaging.Artistic;

namespace FilterEffects.Filters
{
    public class EightiesPopSongFilter : AbstractFilter
    {
        private const string SketchModeGroup = "SketchModeGroup";
        private const SketchMode DefaultSketchMode = SketchMode.Gray;
        private readonly SketchFilter _sketchFilter;

        public EightiesPopSongFilter()
        {
            Name = "80's Pop Song";
            ShortDescription = "Sketch";

            _sketchFilter = new SketchFilter {SketchMode = DefaultSketchMode};

            CreateControl();
        }

        protected override void SetFilters(FilterEffect effect)
        {
            effect.Filters = new List<IFilter> { _sketchFilter };
        }

        private void CreateControl()
        {
            var grid = new Grid();
            int rowIndex = 0;

            var margin = new Thickness {Top = 24};

            grid.Margin = margin;

            var sketchModeText = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = FilterControlTitleFontSize,
                Text = LocalizedStrings.GetText("SketchMode")
            };

            Grid.SetRow(sketchModeText, rowIndex++);

            var padding = new Thickness { Left = 12, Right = 12 };

            var grayRadioButton = new RadioButton { Padding = padding, GroupName = SketchModeGroup };
            
            var textBlock = new TextBlock { Text = LocalizedStrings.GetText("Gray") };

            grayRadioButton.Content = textBlock;
            grayRadioButton.Checked += grayRadioButton_Checked;
            Grid.SetRow(grayRadioButton, rowIndex++);

            var colorRadioButton = new RadioButton { Padding = padding, GroupName = SketchModeGroup };
            
            textBlock = new TextBlock { Text = LocalizedStrings.GetText("Color") };

            colorRadioButton.Content = textBlock;
            colorRadioButton.Checked += colorRadioButton_Checked;
            Grid.SetRow(colorRadioButton, rowIndex++);

            if (_sketchFilter.SketchMode == SketchMode.Gray)
            {
                grayRadioButton.IsChecked = true;
            }
            else
            {
                colorRadioButton.IsChecked = true;
            }

            for (int i = 0; i < rowIndex; ++i)
            {
                var rowDefinition = new RowDefinition();

                if (i == 0)
                {
                    rowDefinition.MinHeight = FilterControlTitleFontSize;
                    rowDefinition.MaxHeight = FilterControlTitleFontSize;
                }
                else if (i < rowIndex - 1)
                {
                    rowDefinition.MinHeight = GridRowMinimumHeight;
                    rowDefinition.MaxHeight = GridRowMaxHeight;
                }
                else
                {
                    rowDefinition.Height = GridLength.Auto;
                }

                grid.RowDefinitions.Add(rowDefinition);
            }

            grid.Children.Add(sketchModeText);
            grid.Children.Add(grayRadioButton);
            grid.Children.Add(colorRadioButton);

            Control = grid;
        }

        void grayRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Changes.Add(() => { _sketchFilter.SketchMode = SketchMode.Gray; });
            Apply();
            NotifyManipulated();
        }

        void colorRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Changes.Add(() => { _sketchFilter.SketchMode = SketchMode.Color; });
            Apply();
            NotifyManipulated();
        }
    }
}
