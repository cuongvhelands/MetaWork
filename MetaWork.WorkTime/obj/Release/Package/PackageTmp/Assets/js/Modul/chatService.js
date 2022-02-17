ChatService = {};
ChatService.getListUser = function (isAsync, successCallback) {
    GLOBALA.callAjaxGET2(`${OauthService.config.baseUriResource}/Work/PartialViewListUserChat`, isAsync,  successCallback)
}
ChatService.getListChannel = function (isAsync, successCallback) {
    GLOBALA.callAjaxGET2(`${OauthService.config.baseUriResource}/Work/PartialViewListChannelChat`, isAsync, successCallback)
}
ChatService.getChatUser = function (userId, isAsync, successCallback) {
    GLOBALA.callAjaxGET2(`${OauthService.config.baseUriResource}/Work/GetChatUser?userId=${userId}`, isAsync, successCallback)
}
ChatService.getChatChannel = function (duAnId, isAsync, successCallback) {
    GLOBALA.callAjaxGET2(`${OauthService.config.baseUriResource}/Work/GetChatChannel?duAnId=${duAnId}` , isAsync, successCallback)
}
ChatService.addToGroup = function (duAnId, isAsync, successCallback) {
    GLOBALA.callAjaxGET2(`${OauthService.config.baseUriResource}/Work/AddToGroup?duAnId=${duAnId}`, isAsync, successCallback)
}
ChatService.showRoom = function (itemId, isAsync, successCallback) {
    GLOBALA.callAjaxGET2(`${OauthService.config.baseUriResource}/Work/PartialViewRoomChat?itemId=${itemId}`, isAsync, successCallback)
}


ChatService.getUsers = function (isAsync, successCallback) {
    GLOBALA.callAjaxGET2(`${OauthService.config.baseUriResource}/Work/GetAllUser`, isAsync, successCallback)
}