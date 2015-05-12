#pragma once

#include "pch.h"

namespace SavePictureCompmonent
{
	namespace ImageHelper
	{
		public ref class SaveImageHelper sealed
		{
		public:
			SaveImageHelper();
			Windows::UI::Xaml::Media::Imaging::BitmapImage OpenImageFile();
		};
	}
}
