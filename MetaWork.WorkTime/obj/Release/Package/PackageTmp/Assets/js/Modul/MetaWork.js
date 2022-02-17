$("#divAddContent").hide();
var ListEditor = [];
var phongChatId;
var tabchat = "";
var ListIdMessageShowed = [];
/// file xác định khi update file tab file và tab document
var fileIdG = "";
var isLinkFile = false;
var idFileInput = "";
var idDocumentInput = "";
function SetUpEditor() {
    var editors = [].slice.call(document.querySelectorAll('textarea'));
    editors.forEach(function (div) {
        var check = true;
        for (i = 0; i < ListEditor.length; i++) {
            var editor = ListEditor[i];
            if (editor) {
                var idtest = editor.id;
                if (idtest == div.id) {
                    check = false;
                }
            }
        }
        if (check) {
            var editor = new Jodit(div, {
                preset: 'inline',
                events: {
                    beforeEnter: keyborad
                }
            });
            ListEditor.push(editor);
        }
    });
}
function keyborad(e) {
    if (!e.altKey && !e.shiftKey && !e.ctrlKey) {
        var arrays = e.path;
        var edi;
        for (i = 0; i < arrays.length; i++) {
            var idtest = arrays[i].id;
            if (idtest) {
                if (idtest != "" && idtest.indexOf("pcThread-") != -1) {
                    edi = arrays[i];
                }
            }
        }
        if (edi) {
            var editorid = edi.id;
            var threadId = editorid.replace("pcThread-", "");
            var searId = "txtEditorSendThread-" + threadId;
            $.each(ListEditor, function (index, value) {
                if (value.id == searId) {
                    var text = value.getEditorValue();
                    if (text != "" && text.trim() != "") {
                        sendMessageThread(text, threadId)
                        setTimeout(function () {
                            value.setEditorValue("");
                        }, 20);

                        //$("#txtEditorSendThread-" + threadId)[0].parentElement.firstElementChild.firstElementChild.firstElementChild.innerHTML = "";
                    }
                }
            })
        } else {
            for (i = 0; i < arrays.length; i++) {
                var idtest = arrays[i].id;
                if (idtest) {
                    if (idtest != "" && idtest.indexOf("divAddContent") != -1) {
                        edi = arrays[i];
                    }
                }
            }
            if (edi) {
                var searId = "txtEditorAddThread";
                $.each(ListEditor, function (index, value) {
                    if (value.id == searId) {
                        var text = value.getEditorValue();
                        if (text != "" && text.trim() != "") {
                            var inpt = { content: text, phongChatId: phongChatId };
                            MetaWorkService.InsertThread(true, inpt, function (data) {
                                chat.server.insertThread(data, phongChatId);
                            })
                            /* sendMessageThread(text, threadId)*/
                            /*  value.setEditorValue("");*/
                            setTimeout(function () {
                                value.setEditorValue("");
                            }, 200);
                        }
                    }
                })
            }
        }

    }
}
$(document).ready(function () {
    $.ajax({
        url: "/MetaWork/PartialViewPhongChat?phongchat=" + $("#phongChatIdM").val(),
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            $("#contentChanel").html(data);
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
    SetUpEditor();
    $("#contentChanel").scrollTop($("#contentChanel").prop("scrollHeight"));
    phongChatId = $("#phongChatIdM").val();
    $("#loadingchannel").hide();
    $("#ChannelContentDetail").show();
})
// xác định folder cha cho tab file và tab ducment
var FileCha = "";

$(document).on("click", "#btnSendThread", function () {
    var value;
    var inputeditor;
    $.each(ListEditor, function (index, editor) {
        if (editor.id == "txtEditorAddThread") {
            value = editor.getEditorValue();
            inputeditor = editor;
        }
    })
    var text = { content: value, phongChatId: phongChatId };
    MetaWorkService.InsertThread(true, text, function (data) {

        $("#divAddButton").show();
        $("#divAddContent").hide();
        chat.server.insertThread(data, phongChatId);
        setTimeout(function () {
            inputeditor.setEditorValue("");
        }, 20);
    })

})
$(document).on("click", ".tabchat", function () {
    var id = this.id;
    if (id != tabchat) {
        tabchat = id;
        if (id == "profile-tab") {
            FileCha = "";
            reloadTabFile();
        } else if (id == "contact-tab") {
            FileCha = "";
            reloadTabDocument();
        } else if (id == "calendar-tab") {
            reloadCalendar();
        } else if (id = "outliner-tab") {
            reloadTaskOuliner();
        }
    }
})
function reloadCalendar() {
    $.ajax({
        url: '/MetaWork/GetInfoChannel?phongChatId=' + phongChatId,
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (result) {
            var obj = JSON.parse(result);
            if (obj.PhongChatInfos[0].NoiDung != "") {
                $("#tabcalendarContent").html('<iframe src="' + obj.PhongChatInfos[0].NoiDung + '" style="border: 0" width="800" height="600" frameborder="0" scrolling="no"></iframe>')
                $("#tabcalendarContent").show();
                $("#SettupCalendarContent").hide();
            } else {
                $("#tabcalendarContent").hide();
                $("#SettupCalendarContent").show();
            }
        },
        error: function (err) {
            alert(err.statusText);
        }
    });
}
function reloadTaskOuliner() {
    //$.ajax({
    //    url: '/MetaWork/GetTaskOutliner?phongChatId=' + phongChatId,
    //    context: document.body,
    //    type: "GET",
    //    dataType: "html",
    //    async: false,
    //    success: function (result) {
    //        var obj = JSON.parse(result);
    //        if (obj.PhongChatInfos[0].NoiDung != "") {
    //            $("#tabcalendarContent").html('<iframe src="' + obj.PhongChatInfos[0].NoiDung + '" style="border: 0" width="800" height="600" frameborder="0" scrolling="no"></iframe>')
    //            $("#tabcalendarContent").show();
    //            $("#SettupCalendarContent").hide();
    //        } else {
    //            $("#tabcalendarContent").hide();
    //            $("#SettupCalendarContent").show();
    //        }
    //    },
    //    error: function (err) {
    //        alert(err.statusText);
    //    }
    //});
}
// chat 
var chat = $.connection.chatHub;
chat.client.notification = function (id, user) {

}
chat.client.broadcastMessageThread = function (id, phongchatid, nguoiDungid, name, avatar, date, message) {
    var thread = $("#pcThreadBody-" + phongchatid);
    if (thread) {
        if (ListIdMessageShowed.indexOf(id) == -1) {
            ListIdMessageShowed.push(id);
            var lstUserOfthread = $("#lastUserpc-" + phongchatid).val();
            var insertNew = false;
            var useridM = $("#nguoiDungIdM").val();
            if (lstUserOfthread != nguoiDungid) insertNew = true;
            else {
                var lstTimeOfThread = $("#lastTimepc-" + phongchatid).val();
                if (lstTimeOfThread.indexOf(" ") != -1) insertNew = true;
                else {
                    insertNew = checkOutTime(lstTimeOfThread, date);
                }
            }
            var strHTML = "";
            if (insertNew) {
                if (useridM != nguoiDungid) {
                    strHTML = ' <li class="agent clearfix"><ul class="chat-user" id="groupUserChat-' + id + '"><li><span class="chat-img left clearfix mx-2"><img src="/Assets/images/Avatar/' + avatar + '" alt="Agent" class="img-circle" /></span><div class="mb-2 chat-info"><small class="text-muted"><span class="glyphicon glyphicon-time"></span>' + name + ', ' + date + '</small><br /></div><div class="chat-body clearfix"><p>' + message + '</p></div></li></ul></li>'
                } else {
                    strHTML = ' <li class="admin clearfix"><div class="mb-2 ms-2 chat-info"><small class="text-muted"><span class="glyphicon glyphicon-time"></span>' + date + '</small><br /></div><ul class="chat-me"  id="groupUserChat-' + id + '"><li class="hover-action" id="divMessThread-' + id + '"><div class="dropdown right clearfix ms-2"><a href="#" class="dropdown-button" type="button" id="dropdownMenuButton-' + id + '" data-bs-toggle="dropdown" aria-expanded="false"><i class="fal fa-ellipsis-v"></i></a><ul class="dropdown-menu dropdown-sm dropdown-menu-end" aria-labelledby="dropdownMenuButton-' + id + '"><li><a class="dropdown-item deleteMessThread" id="' + id + '" href="#">Delete</a></li></ul></div><div class="chat-body clearfix me"><p>' + message + '</p></div></li></ul></li>';
                }
                $("#pcThreadBody-" + phongchatid).append(strHTML);
                $("#lastUserpc-" + phongchatid).val(nguoiDungid);
                $("#lastTimepc-" + phongchatid).val(date);
                $("#lastGroupUserChat-" + phongchatid).val(id);
            } else {
                if (useridM != nguoiDungid) {
                    strHTML = '<li><span class="chat-img left clearfix mx-2">';
                    strHTML += '<img src="/Assets/images/Avatar/' + avatar + '" alt="Agent" class="img-circle" /></span>';
                    strHTML += '<div class="mb-2 chat-info"><small class="text-muted"><span class="glyphicon glyphicon-time">';
                    strHTML += '</span>' + name + ', ' + date + '</small><br /></div><div class="chat-body clearfix">';
                    strHTML += '<p>' + message + '</p></div></li>';
                } else {
                    strHTML = '<li class="hover-action" id="divMessThread-' + id + '"><div class="dropdown right clearfix ms-2">';
                    strHTML += '<a href="#" class="dropdown-button" type="button" id="dropdownMenuButton-' + id + '" data-bs-toggle="dropdown" aria-expanded="false">';
                    strHTML += '<i class="fal fa-ellipsis-v"></i></a>';
                    strHTML += '<ul class="dropdown-menu dropdown-sm dropdown-menu-end" aria-labelledby="dropdownMenuButton-' + id + '">';
                    strHTML += '<li><a class="dropdown-item deleteMessThread" id="' + id + '" href="#">Action</a></li></ul></div>'
                    strHTML += '<div class="chat-body clearfix me"><p>' + message + '</p></div></li>';
                }
                var lstGroupId = $("#lastGroupUserChat-" + phongchatid).val();
                $("#groupUserChat-" + lstGroupId).append(strHTML);
            }
            /* $("#contentChanel").scrollTop($("#contentChanel").prop("scrollHeight"));*/
        }

    }
};
function checkOutTime(time1, time2) {
    var check = false;
    var col1 = time1.split(":");
    var col2 = time2.split(":");
    var h1 = parseInt(col1[0]);
    var h2 = parseInt(col2[0]);
    var m1 = parseInt(col1[1]);
    var m2 = parseInt(col2[1]);
    if (h1 < h2) {
        if (m2 + 60 - m1 >= 5) check = true;
    } else {
        if (m2 - m1 >= 5) check = true;
    }
    return check;
}
chat.client.insertThread = function (threadId, phongchatid) {
    MetaWorkService.ShowAllThread(threadId, true, function (data) {
        $(data).insertBefore($("#divAddContent"));
        SetUpEditor();

    })
};
chat.client.deleteMessage = function (id, isOwner) {
    if (isOwner == true || isOwner == "true" || isOwner == "True") {
        $("#divMessThread-" + id).html("Tin nhắn đã thu hồi.");
    } else {
        alert("Delete Messsage Error.")
    }
}
$.connection.hub.start().done(function () {
    $(document).on("click", ".editorSendMessage", function () {
        var id = this.id;
        var threadId = id.replace("btnSendMessageThread-", "");
        var editorid = "txtEditorSendThread-" + threadId;
        $.each(ListEditor, function (index, value) {
            if (value.id == editorid) {
                var text = value.getEditorValue();
                if (text != "" && text.trim() != "") {

                    sendMessageThread(text, threadId)
                    setTimeout(function () {
                        value.setEditorValue("");
                    }, 20);
                }
            }
        })
        //// Clear text box and reset focus for next comment.
    });
});
$(document).on("click", "#btnAddThread", function () {
    $("#divAddButton").hide();
    $("#divAddContent").show();
    $("#contentChanel").scrollTop($("#contentChanel").prop("scrollHeight"));
})
function AddNewThread(newThreadId, content) {

}
function sendMessageThread(message, phongChatId) {
    chat.server.sendChatMessageThread(message, phongChatId);
}
//endChat


// Calendar
$(document).on("click", ".uploadMeeting", function () {
    $("#ModalAddEventCalendar").modal("show");
    idFileInput = this.id;
})
$(document).on("click", "#btnSubmitAddEventCalendar", function () {
    var obje = {};
    var txt = $("#txtNguoiThamGiaEventCalendar").val();
    var atten = [];
    var nguois = txt.split(';');
    $.each(nguois, function (index, value) {
        if (value && value.trim() != "") {
            atten.push({ Email: value })
        }
    });
    obje.Summary = $("#txtTieuDeEventCalendar").val();
    obje.Descrition = $("#txtNoiDungEventCalendar").val();
    obje.Start = {}
    obje.Start.DateTime = $("#txtStartTimeEventCalendar").val();
    obje.End = {}
    obje.End.DateTime = $("#txtEndTimeEventCalendar").val();
    obje.Attendees = atten;
    $.ajax({
        url: '/CalendarEvent/CreateEvent?to=' + phongChatId,
        data: obje,
        context: document.body,
        type: "POST",
        dataType: "html",
        async: false,
        success: function (result) {
            $("#ModalAddEventCalendar").modal("hide");
            if (result != "error") {
                var text = '<p><a href="' + result + '" target="_blank" title="' + result + '" >' + obje.Summary + '</a></p> <p>' + obje.Descrition + '</p>'
                if (idFileInput == "AddThread") {

                } else {
                    var threadId = idFileInput.replace("SendThread-", "");
                    sendMessageThread(text, threadId)
                }
            } else {
                alert(result);
            }

        },
        error: function (err) {
            alert(err.statusText);
        }
    });
})
//end Calendar
000
// Search
$(document).on("keyup", "#txtSearchFileTab", function () {
    var arrayFileId = [];
    var value = $("#txtSearchFileTab").val().toLowerCase();
    value = removeAccents(value);
    var col1 = value.split(" ");
    var table = document.getElementById("tableFileTab");
    var tr = table.getElementsByTagName("tr");
    for (i = 0; i < tr.length; i++) {
        var td = tr[i].getElementsByTagName("td");
        if (td && td.length > 0) {
            for (j = 0; j < td.length; j++) {
                var txtValue = td[j].textContent;
                txtValue = removeAccents(txtValue);
                $.each(col1, function (index, value2) {
                    if (txtValue.toLowerCase().indexOf(value2) > -1) {
                        var fileid = tr[i].id.replace("trFileTabId-", "")
                        if (jQuery.inArray(fileid, arrayFileId) == -1) {
                            arrayFileId.push(fileid);
                        }
                    }
                })
            }
        }
    }

    var trs = $("#tableFileTab tr");
    if (trs.length > 0) {
        $.each(trs, function (index, value) {
            if (index > 0) {
                var id = value.id;
                var fileid = id.replace("trFileTabId-", "")
                var show = false;
                if (jQuery.inArray(fileid, arrayFileId) != -1) {
                    show = true;
                }
                if (show) value.hidden = false;
                else value.hidden = true;
            }

        })
    }
})
function removeAccents(str) {
    var AccentsMap = [
        "aàảãáạăằẳẵắặâầẩẫấậ",
        "AÀẢÃÁẠĂẰẲẴẮẶÂẦẨẪẤẬ",
        "dđ", "DĐ",
        "eèẻẽéẹêềểễếệ",
        "EÈẺẼÉẸÊỀỂỄẾỆ",
        "iìỉĩíị",
        "IÌỈĨÍỊ",
        "oòỏõóọôồổỗốộơờởỡớợ",
        "OÒỎÕÓỌÔỒỔỖỐỘƠỜỞỠỚỢ",
        "uùủũúụưừửữứự",
        "UÙỦŨÚỤƯỪỬỮỨỰ",
        "yỳỷỹýỵ",
        "YỲỶỸÝỴ"
    ];
    for (var i = 0; i < AccentsMap.length; i++) {
        var re = new RegExp('[' + AccentsMap[i].substr(1) + ']', 'g');
        var char = AccentsMap[i][0];
        str = str.replace(re, char);
    }
    return str;
}

// end Search

// File
function reloadTabFile() {
    $.ajax({
        url: "/MetaWork/PartialViewTabFile?phongchat=" + $("#phongChatIdM").val() + "&&fileCha=" + FileCha,
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            $("#tabFileContent").html(data);
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
}
function reloadTabDocument() {
    $.ajax({
        url: "/MetaWork/PartialViewTabDocument?phongchat=" + $("#phongChatIdM").val() + "&&fileCha=" + FileCha,
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            $("#tabDocumentContent").html(data);
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
}
function inputFileLocal(type, FileId, FileName, fileGuild, text) {
    fileData = { phongChatId: phongChatId, fileName: FileName, filePath: fileGuild, fileType: type, FileId: FileId }
    $.ajax({
        url: '/MetaWork/UploadFile',
        data: fileData,
        context: document.body,
        type: "POST",
        dataType: "html",
        async: false,
        success: function (result) {
            if (result == "True" || result == "true") {
                if (idDocumentInput == "btnAddTabFileDocument" || idDocumentInput == "btnAddTabFileLink") {
                    reloadTabFile();
                } else {
                    reloadTabDocument();
                }
                var inpt = { content: text, phongChatId: phongChatId };
                MetaWorkService.InsertThread(true, inpt, function (data) {
                    chat.server.insertThread(data, phongChatId);
                })
            }
        },
        error: function (err) {
            alert(err.statusText);
        }
    });
}
$(document).on("click", "#btnAddTabFileDocument", function () {
    idDocumentInput = this.id;
    $("#ModalAddDocumentThread").modal("show");
    $("#titleModalAddDocument").val("Add document");
    $("#txtTieuDeDocumentThread").val("");
    $("#txtDocumentLinkThread").val("");
    isLinkFile = false;
})
$(document).on('change', '#inputFile', function () {
    var fileUpload = $("#inputFile").get(0);
    if (fileUpload != "" && fileUpload) {
        var files = fileUpload.files;

        // Create FormData object
        var fileData = new FormData();
        // Looping over all files and add it to FormData object
        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);
        }
        $.ajax({
            url: '/Home/UploadFiles',
            type: "POST",
            contentType: false, // Not to set any content header
            processData: false, // Not to process data
            data: fileData,
            async: false,
            success: function (result) {
                if (result != "") {
                    $('#FileBrowse').find("*").prop("disabled", true);
                    LoadProgressBar(result); //calling LoadProgressBar function to load the progress bar.
                }
            },
            error: function (err) {
                alert(err.statusText);
            }
        });
    }

})
function LoadProgressBar(result) {
    var progressbar = $("#progressbar");
    var progressLabel = $("#progresslabel");
    progressbar.show();
    $("#progressbar").progressbar({
        //value: false,
        change: function () {
            progressLabel.text(
                progressbar.progressbar("value") + "%");  // Showing the progress increment value in progress bar
        },
        complete: function () {
            progressLabel.text("Loading Completed!");
            progressbar.progressbar("value", 0);  //Reinitialize the progress bar value 0
            progressLabel.text("");
            progressbar.hide(); //Hiding the progress bar
            //var markup = "<tr><td>" + result + "</td><td><a href='#' onclick='DeleteFile(\"" + result + "\")'><span class='glyphicon glyphicon-remove red'></span></a></td></tr>"; // Binding the file name
            //$("#ListofFiles tbody").append(markup);
            var threadId = 0;
            var editorid;
            if (idFileInput == "AddThread") {
                editorid = "txtEditorAddThread";
                $.each(ListEditor, function (index, editor) {
                    if (editor.id == editorid) {
                        editor.setEditorValue(editor.getEditorValue() + result);
                    }
                })
            } else if (idFileInput == "btnAddTabFileLocal") {
                var obj = JSON.parse(result);
                idDocumentInput = idFileInput;
                inputFileLocal(1, fileIdG, obj.FileName, obj.FilePath, obj.TextContent);                
            } else {
                threadId = idFileInput.replace("SendThread-", "");
                editorid = "txtEditorSendThread-" + threadId;
                $.each(ListEditor, function (index, editor) {
                    if (editor.id == editorid) {
                        editor.setEditorValue(editor.getEditorValue() + result);
                    }
                })
            }


            $('#Files').val('');

        }
    });

    function progress() {
        var val = progressbar.progressbar("value") || 0;
        progressbar.progressbar("value", val + 1);
        if (val < 99) {
            setTimeout(progress, 25);
        }
    }
    setTimeout(progress, 100);
}
$(document).on('click', ".uploadfile", function () {
    fileIdG = "";
    $("#inputFile").click();
    idFileInput = this.id;
})
$(document).on('click', ".uploadDocument", function () {
    fileIdG = "";
    isLinkFile = false;
    $("#ModalAddDocumentThread").modal("show");
    $("#titleModalAddDocument").val("Add document");
    $("#txtTieuDeDocumentThread").val("");
    $("#txtDocumentLinkThread").val("");
    idDocumentInput = this.id;
})
$(document).on('click', "#btnSubmitDocumentThread", function () {
    var titleDocument = $("#txtTieuDeDocumentThread").val();
    var linkDocument = $("#txtDocumentLinkThread").val();
    if (titleDocument.trim() != "" && linkDocument.trim() != "") {
        $("#ModalAddDocumentThread").modal("hide");
        var typefile = 3;
        var strfile = "Link";
        if (!isLinkFile) { typefile = 2; strfile = "Document" }
        if (fileIdG == "") {
            if (idDocumentInput == "btnAddTabFile" + strfile || idDocumentInput == "btnAddTab" + strfile) {
                var strhtml = '<a href="' + linkDocument + '" target="_blank" name="' + titleDocument + '"  class="UploadFile' + strfile + '" id="' + linkDocument + '" title="' + linkDocument + '" > ' + titleDocument + '</a>'
                inputFileLocal(typefile, fileIdG, titleDocument, linkDocument, strhtml);
            }
            else {
                var strhtml = '<a href="' + linkDocument + '" name="' + titleDocument + '" target="_blank"  class="ImportNewFile' + strfile + '" id="' + linkDocument + '" title="' + linkDocument + '" > ' + titleDocument + '</a>'
                var threadId = 0;
                var editorid;
                if (idDocumentInput != "AddThread") {
                    threadId = idDocumentInput.replace("SendThread-", "");
                    editorid = "txtEditorSendThread-" + threadId;
                } else {
                    editorid = "txtEditorAddThread";
                }

                $.each(ListEditor, function (index, editor) {
                    if (editor.id == editorid) {
                        editor.setEditorValue(editor.getEditorValue() + strhtml);
                    }
                })
            }
        }
        else {
            var strhtml = '<a href="' + linkDocument + '" target="_blank" name="' + titleDocument + '"  class="UploadFile' + strfile + '" id="' + linkDocument + '" title="' + linkDocument + '" > ' + titleDocument + '</a>'
            inputFileLocal(typefile, fileIdG, titleDocument, linkDocument, strhtml);
        }



    }
})
$(document).on('change', '#inputTabFile', function () {
    var fileUpload = $("#inputTabFile").get(0);
    if (fileUpload != "" && fileUpload) {
        var files = fileUpload.files;

        // Create FormData object
        var fileData = new FormData();
        // Looping over all files and add it to FormData object
        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);
        }
        $.ajax({
            url: '/MetaWork/UploadTabFileLocal',
            type: "POST",
            contentType: false, // Not to set any content header
            processData: false, // Not to process data
            data: fileData,
            async: false,
            success: function (result) {
                if (result != "") {
                    $('#FileBrowse').find("*").prop("disabled", true);
                    LoadProgressBar(result); //calling LoadProgressBar function to load the progress bar.
                }
            },
            error: function (err) {
                alert(err.statusText);
            }
        });
    }

})

$(document).on("click", ".deleteMessThread", function () {
    var id = this.id;
    chat.server.DeleteMessage(id, phongChatId);
    //$.ajax({
    //    url: '/MetaWork/DeleteMessage?messageId='+id,
    //    context: document.body,
    //    type: "GET",
    //    dataType: "html",
    //    async: false,
    //    success: function (result) {
    //        if (result == "true" || result == "True") {
    //            $("#divMessThread-" + id).html("Tin nhắn đã thu hồi.")
    //        } else {
    //            alert("Delete Messsage Error.")
    //        }
    //    },
    //    error: function (err) {
    //        alert(err.statusText);
    //    }
    //});
})
$(document).on("click", ".deleteFileTab", function () {
    var id = this.id;
    if (confirm("Bạn có muốn xóa File này không?")) {
        $.ajax({
            url: '/MetaWork/DeleteFile?fileId=' + id,
            context: document.body,
            type: "GET",
            dataType: "html",
            async: false,
            success: function (result) {
                if (result == "true" || result == "True") {
                    reloadTabFile();
                } else {
                    alert("Delete File Error.")
                }
            },
            error: function (err) {
                alert(err.statusText);
            }
        });
    }

})
$(document).on("click", ".deleteDocumentTab", function () {
    var id = this.id;
    if (confirm("Bạn có muốn xóa tài liệu này không?")) {
        $.ajax({
            url: '/MetaWork/DeleteFile?fileId=' + id,
            context: document.body,
            type: "GET",
            dataType: "html",
            async: false,
            success: function (result) {
                if (result == "true" || result == "True") {
                    reloadTabDocument();
                } else {
                    alert("Delete Document Error.")
                }
            },
            error: function (err) {
                alert(err.statusText);
            }
        });
    }

})
$(document).on("click", "#btnAddTabFileLink", function () {
    idDocumentInput = this.id;
    fileIdG = "";
    isLinkFile = true;
    $("#ModalAddDocumentThread").modal("show");
    $("#titleModalAddDocument").text("Add file link");
    $("#txtTieuDeDocumentThread").val("");
    $("#txtDocumentLinkThread").val("");
})
$(document).on("click", ".uploadgoogledrive", function () {
    fileIdG = "";
    isLinkFile = true;
    $("#ModalAddDocumentThread").modal("show");
    $("#titleModalAddDocument").val("Add file link");
    $("#txtTieuDeDocumentThread").val("");
    $("#txtDocumentLinkThread").val("");
    var strhtml = '<a href="' + linkDocument + '" target="_blank" name="' + titleDocument + '"  class="UploadFile' + strfile + '" id="' + linkDocument + '" title="' + linkDocument + '" > ' + titleDocument + '</a>'
    idDocumentInput = this.id;
})
$(document).on("click", ".editFileLinkTab", function () {
    fileIdG = this.id;
    idDocumentInput = "btnAddTabFileDocument";
    isLinkFile = true;
    $.ajax({
        url: "/MetaWork/GetFileInfo?fileId=" + fileIdG,
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            if (data && data != "") {
                var obje = JSON.parse(data);
                $("#ModalAddDocumentThread").modal("show");
                $("#titleModalAddDocument").text("Edit file ");
                $("#txtTieuDeDocumentThread").val(obje.FileName);
                $("#txtDocumentLinkThread").val(obje.FilePath);
            }
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
})
$(document).on("click", ".editFileLocalTab", function () {
    fileIdG = this.id;
    idDocumentInput = "editFileLocal";
    isLinkFile = false;
    $.ajax({
        url: "/MetaWork/GetFileInfo?fileId=" + fileIdG,
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            if (data && data != "") {
                var obje = JSON.parse(data);
                $("#modalUpdateFileLocal").modal("show");
                $("#txtTieuDeFileLocal").val(obje.FileName);
            }
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
})
$(document).on("click", "#btnSubmitFileLocal", function () {
    var text = $("#txtTieuDeFileLocal").val();
    if (text != "" && text.trim() != "") {
        fileData = { phongChatId: phongChatId, fileName: text, filePath: "", fileType: 0, FileId: fileIdG }
        $.ajax({
            url: '/MetaWork/UploadFile',
            data: fileData,
            context: document.body,
            type: "POST",
            dataType: "html",
            async: false,
            success: function (result) {
                if (result == "True" || result == "true") {
                    reloadTabFile();
                    var inpt = { fileId: fileIdG, phongChatId: phongChatId };
                    MetaWorkService.InsertThreadUpdateFile(false, inpt, function (data) {
                        chat.server.insertThread(data, phongChatId);
                    })
                }
            },
            error: function (err) {
                alert(err.statusText);
            }
        });
    }

})
$(document).on("click", ".UploadFileLocal", function () {
    var id = this.id;
    var fileId = id.replace("_", ".");
    window.open(
        OauthService.config.baseUriResource + "Uploads/" + fileId,
        '_blank' // <- This is what makes it open in a new window.
    );
})
$(document).on("click", "#btnAddTabFileLocal", function () {
    $("#inputTabFile").click();
    idFileInput = this.id;
})
$(document).on("click", "#btnAddTabDocument", function () {
    idDocumentInput = this.id;
    $("#ModalAddDocumentThread").modal("show");
    $("#titleModalAddDocument").val("Add document");
    $("#txtTieuDeDocumentThread").val("");
    $("#txtDocumentLinkThread").val("");
    isLinkFile = false;
})
$(document).on("click", ".exspanAllThread", function () {
    var id = this.id;
    var thrdid = id.replace("exspanThread-", "");
    MetaWorkService.ShowAllOfThread(thrdid, false, function (data) {
        $("#pcThreadBody-" + thrdid).html(data);
    })
})

$(document).on("click", ".UploadDocumentLink", function () {
    var id = this.id;
    window.open(
        id,
        '_blank' // <- This is what makes it open in a new window.
    );
})
//End file

//Folder
$(document).on("click", "#btnAddTabFileFolder", function () {
    $("#modalFolder").modal("show");
    $("#titleFolder").text("Thêm thư mục");
    $("#txtTieuDeFolder").val("");
})
$(document).on("click", "#btnSubmitFileFolder", function () {
    var text = $("#txtTieuDeFolder").val();
    if (text != "" && text.trim() != "") {
        fileData = { phongChatId: phongChatId, fileName: text, fileId: FileCha }
        $.ajax({
            url: '/MetaWork/InsertFolder',
            data: fileData,
            context: document.body,
            type: "POST",
            dataType: "html",
            async: false,
            success: function (result) {
                if (result == "True" || result == "true") {
                    $("#modalFolder").modal("hide");
                    reloadTabFile();
                }
            },
            error: function (err) {
                alert(err.statusText);
            }
        });
    }
})
$(document).on("doubleClick", ".OpenFolderDoubleClick", function () {
    fileIdG = this.id;
    reloadTabFile();
})
$(document).on("click", "tr", function () {
    $(this).addClass("selected").siblings().removeClass("selected");
})
// Show info Channel
$(document).on("click", "#divShowDetailChannel", function () {
    $.ajax({
        url: '/MetaWork/GetInfoChannel?phongChatId=' + phongChatId,
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (result) {
            var obj = JSON.parse(result);
            $("#titleAddChannel").text("Chỉnh sửa channel");
            $("#txtTieuDeChannelAdd").val(obj.TenPhongChat);           
            nguoiThamGias = [];
            $("#selectAddChannel").val("").trigger('change');
            var strhtml = "<tr><td> Họ Tên </td><td></td></tr>";
            $.each(obj.HoTenNguoiDungs, function (index, value) {
                nguoiThamGias.push(value.NguoiDungId);
                strhtml += '<tr id="trNguoiThamGia' + value.NguoiDungId + '"><td>' + value.HoTen + ' </td><td><a class="btn btn-link btnXoaNguoiThamGia" id="' + value.NguoiDungId + '">Xóa</a></td></tr>'               
            })
            $("#tdyNguoiThamGia").html(strhtml);
            $("#tblNguoiThamGia").show();            
            $("#txtCalendarIdAddChannel").val(obj.PhongChatInfos[0].GhiChu);
            $("#txtEmbedCalendarAddChannel").val(obj.PhongChatInfos[0].NoiDung);
            $("#ModalAddChannel").modal("show");
        },
        error: function (err) {
            alert(err.statusText);
        }
    });
})
// drag and drop
$("#table1 .childgrid tr, #table2 .childgrid tr").draggable({
    helper: function () {
        var selected = $('.childgrid tr.selectedRow');
        if (selected.length === 0) {
            selected = $(this).addClass('selectedRow');
        }
        var container = $('<div/>').attr('id', 'draggingContainer');
        container.append(selected.clone().removeClass("selectedRow"));
        return container;
    }
});

$("#table1 .childgrid, #table2 .childgrid").droppable({
    drop: function (event, ui) {
        $(this).append(ui.helper.children());
        $('.selectedRow').remove();
    }
});

$(document).on("click", ".childgrid tr", function () {
    $(this).addClass("selectedRow").siblings().removeClass("selectedRow");

});

