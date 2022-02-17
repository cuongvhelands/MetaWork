PhongChatService = {}
PhongChatService.insertThread = function (threadId, isAsync, successCallback) {
    GLOBALA.callAjaxGET2(`${OauthService.config.baseUriResource}/MetaWork/PartialViewThread?phongchat=${threadId}`, isAsync, successCallback)
}
PhongChatService.ShowAllThread = function (phongChat, isAsync, successCallback) {
    GLOBALA.callAjaxGET2(`${OauthService.config.baseUriResource}/MetaWork/PartialViewPhongChat?phongchat=${phongChat}`, isAsync, successCallback)
}