var GLOBALA = {};
GLOBALA.utils = {};
//GLOBALA.linkRoot = _WEB_URL;
GLOBALA.linkRoot = '';

GLOBALA.callAjaxPOST = function (url, data, callback) {
    $.ajax({
        url: GLOBALA.linkRoot + url,
        type: "POST",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: data ? JSON.stringify(data) : null,
        success: function (data) {
            callback(data);
        },
        error: function (xhr, status, error) {
            GLOBALA.utils.onError(xhr, status, error);
        }
    });
};

GLOBALA.callAjaxPOST2 = function (url, isAsync, data, callback) {
    $.ajax({
        url: GLOBALA.linkRoot + url,
        type: "POST",
        async: isAsync,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: data ? JSON.stringify(data) : null,
        success: function (data) {
            callback(data);
        },
        error: function (xhr, status, error) {
            GLOBALA.utils.onError(xhr, status, error);
        }
    });
};

GLOBALA.callAjaxPOST3 = function (url, isAsync, data, auth, successCallBack) {
    $.ajax({
        url: url,
        type: "POST",
        async: isAsync,
        contentType: 'application/json; charset=utf-8',
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', auth);
        },
        dataType: 'json',
        data: data ? JSON.stringify(data) : null,
        success: function (data) {
            if (successCallBack) successCallBack(data);
        },
        error: function (xhr, status, error) {
            GLOBALA.utils.onError(xhr, status, error);
        }
    });
};

GLOBALA.callAjaxGET2 = function (url, isAsync, successCallBack) {
    $.ajax({
        url: url,
        type: "GET",
        async: isAsync,
        contentType: 'application/json; charset=utf-8',
        //beforeSend: function (xhr) {
        //    xhr.setRequestHeader('Authorization', auth);
        //},
        dataType: 'html',
        success: function (data) {
            successCallBack(data);
        },
        error: function (xhr, status, error) {
            GLOBALA.utils.onError(xhr, status, error);
        }
    });
};

GLOBALA.callAjaxGET3 = function (url, isAsync, auth, successCallBack) {
    $.ajax({
        url: url,
        type: "GET",
        async: isAsync,
        contentType: 'application/json; charset=utf-8',
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', auth);
        },
        dataType: 'json',
        success: function (data) {
            successCallBack(data);
        },
        error: function (xhr, status, error) {
            GLOBALA.utils.onError(xhr, status, error);
        }
    });
};

GLOBALA.callAjaxDELETE3 = function (url, isAsync, auth, successCallBack) {
    $.ajax({
        url: url,
        type: "DELETE",
        async: isAsync,
        contentType: 'application/json; charset=utf-8',
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', auth);
        },
        dataType: 'json',
        success: function (data) {
            successCallBack(data);
        },
        error: function (xhr, status, error) {
            GLOBALA.utils.onError(xhr, status, error);
        }
    });
};



//Xử lý ajax error
GLOBALA.utils.onError = function (xhr, errorType, exception) {
  /*  var responseText = JSON.parse(xhr.responseText);*/
    $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "error ajax");
    //GLOBALA.utils.loading(false);
    //var responseText;
    //var showText = "";
    //try {
    //    responseText = JSON.parse(xhr.responseText);
    //    showText += "<div><div><b>" + errorType + " " + exception + "</b></div>";
    //    showText += "<div><u>Exception</u>:<br /><br />" + responseText.ExceptionType + "</div>";
    //    showText += "<div><u>StackTrace</u>:<br /><br />" + responseText.StackTrace + "</div>";
    //    showText += "<div><u>Message</u>:<br /><br />" + responseText.Message + "</div></div>";
    //} catch (e) {
    //    responseText = xhr.responseText.replace(".7em", "13px").replace("<pre>", "").replace("</pre>", "");
    //    showText = responseText;
    //}
    //GLOBALA.utils.showMessage.show({ msg: showText, icon: GLOBALA.utils.showMessage.UNSUCCESS, title: xhr.statusText, ajaxErr: true });
};

