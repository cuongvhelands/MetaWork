LeftBarService = {};
LeftBarService.getSpaceUser = function (isAsync, successCallback) {
    GLOBALA.callAjaxGET2(`${OauthService.config.baseUriResource}/MetaWork/PartialViewSpaceUser`, isAsync, successCallback)
}