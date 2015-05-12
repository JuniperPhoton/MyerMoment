#pragma once

using namespace concurrency;
using namespace Platform;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;

namespace ImageHandleLib
{
	public ref class ImageHandleHelper sealed
	{
	public:
		ImageHandleHelper();
		static BitmapImage^ OpenImageFile(StorageFile^ file);
	};
}
