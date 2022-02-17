MetaWorkService = {};
MetaWorkService.InsertThread = function (isAsync, data, successCallback) {
    GLOBALA.callAjaxPOST2(`${OauthService.config.baseUriResource}/MetaWork/InsertThread`, isAsync, data, successCallback)
}
MetaWorkService.InsertThreadUpdateFile = function (isAsync, data, successCallback) {
    GLOBALA.callAjaxPOST2(`${OauthService.config.baseUriResource}/MetaWork/InsertThread`, isAsync, data, successCallback)
}
MetaWorkService.InsertFile = function (isAsync, data, successCallback) {
    GLOBALA.callAjaxPOST2(`${OauthService.config.baseUriResource}/MetaWork/InsertFile`, isAsync, data, successCallback)
}

MetaWorkService.ShowAllThread = function (phongChat, isAsync, successCallback) {
    GLOBALA.callAjaxGET2(`${OauthService.config.baseUriResource}/MetaWork/PartialViewThread?threadId=${phongChat}`, isAsync, successCallback)
}
MetaWorkService.ShowAllOfThread = function (phongChat, isAsync, successCallback) {
    GLOBALA.callAjaxGET2(`${OauthService.config.baseUriResource}/MetaWork/PartialViewAllOfThread?threadId=${phongChat}`, isAsync, successCallback)
}
