// For an introduction to the Page Control template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232511
(function () {
    "use strict";

    WinJS.UI.Pages.define("/Pages/test.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {

            // TODO:编译运行之前需要开放平台参数.
            WeiboSDKForWinRT.SdkData.appKey = "";
            WeiboSDKForWinRT.SdkData.appSecret = "";
            WeiboSDKForWinRT.SdkData.redirectUri = "";

            hookEvent();

            CopyFile('images/weibo.png', 'weibo');

            oauth();
        },

        unload: function () {
            // TODO: Respond to navigations away from this page.
        },

        updateLayout: function (element, viewState, lastViewState) {
            /// <param name="element" domElement="true" />

            // TODO: Respond to changes in viewState.
        }
    });

    var statusP;
    var resultP;
    function hookEvent() {
        var timeLineBtn = document.getElementById('timelineBtn');
        var postMsgBtn = document.getElementById('postMsgBtn');
        var picPostMsgBtn = document.getElementById('picPostMsgBtn');
        statusP = document.getElementById('statusP');
        resultP = document.getElementById('resultP');

        if (timeLineBtn) {
            timeLineBtn.onclick = function test() {
                // step1: 实例化微博请求处理类.
                var netEngine = new WeiboSDKForWinRT.SdkNetEngine();
                var requestType = WeiboSDKForWinRT.SdkRequestType.friends_TIMELINE;
                // step2: 使用给参数类传递参数.
                var timelineCMD = new WeiboSDKForWinRT.CmdTimelines();
                timelineCMD.page = '1';
                timelineCMD.count = '20';
                // step3: 发送请求.
                var result = netEngine.requestCmd(requestType, timelineCMD).then(function (response) {
                    // step4: 处理返回数据.
                    var tmp = JSON.parse(response.content);
                    statusP.innerText = "Fetch friend's Timeline successed！";
                    resultP.innerText = response.content;
                }, function (err) {
                    // step5: 异常处理.
                    var tmp = err;
                    statusP.innerText = "Fetch friend's Timeline failed！";
                });
            }
        }
        if (postMsgBtn) {
            postMsgBtn.onclick = function postStatus() {
                var netEngine = new WeiboSDKForWinRT.SdkNetEngine();
                var requestType = WeiboSDKForWinRT.SdkRequestType.post_MESSAGE;
                var timelineCMD = new WeiboSDKForWinRT.CmdPostMessage();
                timelineCMD.status = 'from WinJS call WinRT';

                var result = netEngine.requestCmd(requestType, timelineCMD).then(function (response) {
                    var tmp = JSON.parse(response.content);
                    statusP.innerText = "Post a message without picture successed！";
                    resultP.innerText = response.content;
                }, function (err) {
                    var tmp = err;
                    statusP.innerText = "Post a message without picture failed！";
                });
            }
        }
        if (picPostMsgBtn) {
            picPostMsgBtn.onclick = function postPicStatus() {
                var netEngine = new WeiboSDKForWinRT.SdkNetEngine();
                var requestType = WeiboSDKForWinRT.SdkRequestType.post_MESSAGE_PIC;
                var timelineCMD = new WeiboSDKForWinRT.CmdPostMsgWithPic();
                timelineCMD.status = encodeURIComponent('status with picture from WinJS call WinRT');
                timelineCMD.picPath = picPath;

                var result = netEngine.requestCmd(requestType, timelineCMD).then(function (response) {
                    var tmp = JSON.parse(response.content);
                    statusP.innerText = "Post a message with picture successed！";
                    resultP.innerText = response.content;
                }, function (err) {
                    var tmp = err;
                    statusP.innerText = "Post a message with picture failed！";
                });
            }
        }
    }

    function oauth() {
        var oauthClient = new WeiboSDKForWinRT.ClientOAuth();
        // 判断是否已经授权或者授权是否过期.
        if (oauthClient.isAuthorized == false) {
            oauthClient.addEventListener("logincallback", function (result) {
                if (result.target) {
                    statusP.innerText = "OAuth successed";
                }
                else {
                    statusP.innerText = "OAuth failed, msg:" + result.detail[0].errMessage;
                }
            });
            oauthClient.beginOAuth();
        }
    }

   
    var picPath;
    function CopyFile(source, dest) {
        var uri = new Windows.Foundation.Uri('ms-appx:///' + source);

        Windows.Storage.StorageFile.getFileFromApplicationUriAsync(uri).then(function (file) {
            Windows.Storage.FileIO.readBufferAsync(file).then(function (srcBuffer) {
                var folder = Windows.Storage.ApplicationData.current.localFolder;
                folder.createFolderAsync(dest, Windows.Storage.CreationCollisionOption.openIfExists).then(function (destFolder) {
                    if (destFolder) {
                        destFolder.createFileAsync(file.name, Windows.Storage.CreationCollisionOption.replaceExisting).then(function (destFile) {
                            Windows.Storage.FileIO.writeBufferAsync(destFile, srcBuffer).then(function () {
                                // Note: Completed.
                                picPath = destFile.path;
                            });
                        });
                    } else {

                    }
                },
                function (err) {

                });
            });
        }, function (err) {

        });
    }
})();
