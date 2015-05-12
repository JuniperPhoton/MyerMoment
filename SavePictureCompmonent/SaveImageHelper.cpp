#include "pch.h"
#include "SaveImageHelper.h"
#include <ppltasks.h>

using namespace concurrency;
using namespace SavePictureCompmonent;
using namespace Platform;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;

SaveImageHelper::SaveImageHelper()
{

}

BitmapImage OpenImageFile(StorageFile^ file)
{
	create_task(file->OpenAsync(FileAccessMode::Read))
		.then([](IRandomAccessStream^ fileStream)
		{
			BitmapImage^ bitmapImage =ref new BitmapImage();
			bitmapImage->SetSource(fileStream);

			return bitmapImage;
		});
}
