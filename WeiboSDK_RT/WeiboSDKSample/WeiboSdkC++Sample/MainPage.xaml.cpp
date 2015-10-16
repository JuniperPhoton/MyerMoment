//
// MainPage.xaml.cpp
// Implementation of the MainPage class.
//

#include "pch.h"
#include "MainPage.xaml.h"
#include "ppltasks.h"
#include "ppltasks_extra.h"

using namespace CPPSample;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace WeiboSDKForWinRT;
using namespace concurrency;
using namespace concurrency::extras;
using namespace Windows::Storage;
using namespace Windows::Storage::FileProperties;
using namespace Windows::Storage::Streams;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

MainPage::MainPage()
{
	InitializeComponent();
}


void CPPSample::MainPage::InitData()
{
	// TODO:编译运行之前需要开放平台参数.
	SdkData::AppKey = "";
	SdkData::AppSecret = "";
	SdkData::RedirectUri = "";

	CopyToIso("weibo.png");
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void MainPage::OnNavigatedTo(NavigationEventArgs^ e)
{
	// 准备数据.
	InitData();

	ClientOAuth^ oauthClient = ref new ClientOAuth();
	// 判断是否已经授权.
	if(!oauthClient->IsAuthorized)
	{
		oauthClient->LoginCallback += ref new WeiboSDKForWinRT::OAuth2LoginBack(this, &CPPSample::MainPage::Oauthrize);

		oauthClient->BeginOAuth();
	}

	(void) e;	// Unused parameter

}

void CPPSample::MainPage::Oauthrize(bool isSucces,SdkAuthError^ err,SdkAuth2Res^ response)
{
	if (isSucces)
	{
		// TODO: deal the OAuth result.
		this->statusRun->Text = "Congratulations, Authorized successfully!";
		this->ResultRun->Text = "AccesssToken:" + response->AccessToken 
			+ ", ExpriesIn:" + response->ExpriesIn 
			+ ", Uid:" + response->Uid;
	}
	else
	{
		// Note: handle the err.
		this->ResultRun->Text = "授权失败，" + err->errMessage;
	}
}

void CPPSample::MainPage::TimelineBtn_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	SdkNetEngine^ engine = ref new SdkNetEngine();
	ISdkCmdBase^ cmdBase;
	CmdTimelines^ timeline = ref new CmdTimelines();
	timeline->Count = "20";
	timeline->Page = "1";
	cmdBase = timeline;

	IAsyncOperation<SdkResponse^>^ requestOP = engine->RequestCmd(SdkRequestType::FRIENDS_TIMELINE,cmdBase);

	auto requestTask = create_task(requestOP);

	requestTask.then([this](SdkResponse^ response){
		if (response->errCode == SdkErrCode::SUCCESS)
		{
			ResultRun->Text = response->content;
			statusRun->Text = "Fetch friend's Timeline successed！";
		}
		else
		{
			// TODO: deal the error.
			ResultRun->Text = "";
			statusRun->Text = "Fetch friend's Timeline failed！";
		}
	});
}

void CPPSample::MainPage::MsgWithPicBtn_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	SdkNetEngine^ engine = ref new SdkNetEngine();
	ISdkCmdBase^ cmdBase;
	CmdPostMsgWithPic^ cmdMsg = ref new CmdPostMsgWithPic();
	cmdMsg->Status = "test for post message with picture";
	cmdMsg->PicPath = picPath;
	cmdBase = cmdMsg;

	IAsyncOperation<SdkResponse^>^ requestOP = engine->RequestCmd(SdkRequestType::POST_MESSAGE_PIC,cmdBase);

	auto requestTask = create_task(requestOP);

	requestTask.then([this](SdkResponse^ response){
		if (response->errCode == SdkErrCode::SUCCESS)
		{
			ResultRun->Text = response->content;
			statusRun->Text = "Post a message with picture successed！";
		}
		else
		{
			// TODO: deal the error.
			ResultRun->Text = response->content;
			statusRun->Text = "Post a message with picture failed！";
		}
	});
}

void CPPSample::MainPage::NoPicPostBtn_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	SdkNetEngine^ engine = ref new SdkNetEngine();
	ISdkCmdBase^ cmdBase;
	CmdPostMessage^ cmdMsg = ref new CmdPostMessage();
	cmdMsg->Status = "test for post message without picture";
	cmdBase = cmdMsg;

	IAsyncOperation<SdkResponse^>^ requestOP = engine->RequestCmd(SdkRequestType::POST_MESSAGE,cmdBase);

	auto requestTask = create_task(requestOP);

	requestTask.then([this](SdkResponse^ response){
		if (response->errCode == SdkErrCode::SUCCESS)
		{
			ResultRun->Text = response->content;
			statusRun->Text = "Post a message without picture successed！";
		}
		else
		{
			// TODO: deal the error.
			ResultRun->Text = response->content;
			statusRun->Text = "Post a message without picture failed！";
		}
	});
}


void CPPSample::MainPage::CopyToIso(String^ source)
{
	try	
	{
		Windows::ApplicationModel::Package^ package = Windows::ApplicationModel::Package::Current;
		// 源文件.
		create_task(package->InstalledLocation->GetFileAsync(source)).then([this](StorageFile^ sourceFile)
		{
			create_task(FileIO::ReadBufferAsync(sourceFile)).then([this,sourceFile](Streams::IBuffer^ buffer)		
			{
				StorageFolder^ picFolder = KnownFolders::PicturesLibrary;
				create_task(picFolder->CreateFileAsync(sourceFile->Name, CreationCollisionOption::OpenIfExists)).then([this,buffer](StorageFile^ destFile)
				{
					auto writeTask = create_task(FileIO::WriteBufferAsync(destFile, buffer));

					writeTask.then([this,destFile](){
						picPath = destFile->Path;
						statusRun->Text = "Copy file completed. PicPath:" + picPath;
					});
				});
			});
		});
	}
	catch(Exception^ er)
	{
		statusRun->Text = "Copy file to storage err, message:" + er->Message + "; picPath:" + picPath;
	} 
}

