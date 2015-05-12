#include "pch.h"
#include "ImageHandleHelper.h"
#include <ppltasks.h>

using namespace concurrency;
using namespace Platform;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;


ImageHandleLib::ImageHandleHelper::ImageHandleHelper()
{

}

BitmapImage^ ImageHandleLib::ImageHandleHelper::OpenImageFile(StorageFile^ file)
{
	auto opentask=create_task(file->OpenAsync(FileAccessMode::Read))
		.then([](IRandomAccessStream^ fileStream)
	{
		BitmapImage^ bitmapImage = ref new BitmapImage();
		create_task(bitmapImage->SetSourceAsync(fileStream))
			.then([bitmapImage](Task^ previousTask))
		{
			var bitmap
			return bitmapImage;
		};
		
	});
				
	auto bitmap = opentask.get();

	return bitmap;
}

