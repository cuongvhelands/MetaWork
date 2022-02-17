var idc;
var yearM = $("#yearM").val();
$(document).ready(function () {
    var pageSize = $("#pageSizeM").val();
    $("#selectPageSize").val(pageSize);
    var weekM = $("#weekM").val();
    var weekData = getDataWeek(weekM,yearM);
    var week = $("#cboWeek1").kendoDropDownList({
        dataTextField: "TenWeek",
        dataValueField: "Week",
        filter: "contains",
        dataSource: weekData,      
    }).data("kendoDropDownList");
    week.value(weekM + "-" + yearM);
    var duAnData = getDuAns();
    var duAn = $("#cboDuAn1").kendoDropDownList({
        dataTextField: "TenDuAn",
        dataValueField: "DuAnId",
        filter: "contains",
        dataSource: duAnData,    
    }).data("kendoDropDownList");
    var duAnId = $("#duAnIdM").val();
    duAn.value(duAnId);
    var nguoiDungData = getDataNguoiDung();
    var nguoiDung = $("#cboNguoiDung").kendoDropDownList({
        dataTextField: "HoTen",
        dataValueField: "NguoiDungId",
        filter: "contains",
        dataSource: nguoiDungData,
    }).data("kendoDropDownList");
    var nguoiDungId = $("#nguoiDungIdM").val();
    nguoiDung.value(nguoiDungId);
    //var trangThaiToDoData = getTrangThaiToDo();
    //var trangThaiToDo = $("#cboTrangThaiToDo").kendoDropDownList({
    //    dataTextField: "TenXacNhan",
    //    dataValueField: "XacNhanHoanThanh",
    //    filter: "contains",
    //    dataSource: trangThaiToDoData,
    //}).data("kendoDropDownList");
    //var trangThaiToDoId = $("#xacNhanHoanThanhM").val();  
    //trangThaiToDo.value(trangThaiToDoId);

    //
    //var emails =["cuongvh.elands@gmail.com","phuongnt.elands@gmail.com"]
    //var data = {Emails:emails,TimeFrom:1572566399, TimeTo: 1574829662}  
    //$.ajax({
    //    url: "/api/Time/GetTimeBy",
    //    data: data,
    //    context: document.body,
    //    type: "POST",
    //    dataType: "html",
    //    success: function (data) {
    //        if (data > 0) {
    //            var url = "/project/index";
    //            $.Notification.autoHideNotify('success', 'top right', 'Thông báo', "Thêm mới dự án thành công!");
    //            setTimeout(
    //                function () {
    //                    window.location.href = url;;
    //                }, 2000);
    //        } else {
    //            $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Thêm mới không thành công!");
    //        }

    //    },
    //    error: function (xhr, status) {
    //    },
    //    complete: function (xhr, status) {          
    //    }
    //});
})
function getDataWeek(week,year) {
    var result;
    $.ajax({
        url: "/Time/GetWeeks3?week="+week+"&&year="+year,
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            result = JSON.parse(data);
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
    return result;
}
function getDuAns() {
    var result;
    $.ajax({
        url: "/Shipable/GetDuAns2",
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            result = JSON.parse(data);
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
    return result;
}
function getDataNguoiDung() {
    var result;
    $.ajax({
        url: "/Time/GetNguoiDungs",
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            result = JSON.parse(data);
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
    return result;
}
function getTrangThaiToDo() {
    var result;
    $.ajax({
        url: "/Time/GetTrangThaiToDos",
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            result = JSON.parse(data);
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
    return result;
}
$(document).on('click', "#btnSearch", function () {
    var duAnId = $("#cboDuAn1").val();
    var nguoiDungId = $("#cboNguoiDung").val();
    var strweek = $("#cboWeek1").val();
    var col = strweek.split('-');
    var week = col[0];
    var year = col[1];
    var xacNhanHoanThanh = $("#cboTrangThaiToDo").val();
    var str = '/Time/ToDoList?duAnId=' + duAnId + "&&nguoiDungId=" + nguoiDungId + "&&week=" + week+"&&year="+year;
    var page = $("#pageNumM").val();
    var pageSize = $("#pageSizeM").val();

    if (xacNhanHoanThanh != "0" && xacNhanHoanThanh != 0) {
        str += "&&xacNhanHoanThanh=";
        if (xacNhanHoanThanh == "1" || xacNhanHoanThanh == 1) str += false;
        else str += true;
    }  
    if (page != "1") str += "&&page=" + page;
    str += "&&pageSize=" + pageSize;
    window.location.href = str;
})
$(document).on("change", ".chkMakeAsDone", function () {
    var id = this.id;
    var toDoId = id.replace("chkMSD-", "");
    $.ajax({
        url: "/Time/XacNhanHoanThanh?toDoId=" + toDoId,
        type: "GET",
        contentType: 'application/json; charset=utf-8',      
        async: false,
        success: function (data) {
            data = JSON.parse(data);
            if (data.Status == false || data.Status == "false") {
                $("#" + id)[0].checked = false;
                $.Notification.autoHideNotify('error', 'top right', 'Thông báo', data.Message);
            } else {
                $("#divCheck-" + toDoId).html('<i class="fas fa-check-square text-primary"></i>');
                $.Notification.autoHideNotify('success', 'top right', 'Thông báo', data.Message);
            }
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
})
$(document).on("change", ".chkMakeAsDoneTask", function () {
    idc = this.id;
    var taskId = idc.replace("chkMSD-", "");
    //if (!swal({
    //    title: "Xác nhận hoàn thành?",
    //    text: "Bạn có chắc chắn muốn xác nhận hoàn thành không?",
    //    type: "warning",
    //    showCancelButton: true,
    //    cancelButtonText: 'Không',
    //    cancelButtonClass: 'btn-white',
    //    confirmButtonClass: 'btn-warning',
    //    confirmButtonText: "Có, hoàn thành nó!",
    //    closeOnConfirm: true
    //}, function () {
    //    $.ajax({
    //        url: "/Time/XacNhanHoanThanhTask?taskId=" + taskId,
    //        type: "GET",
    //        contentType: 'application/json; charset=utf-8',
    //        async: false,
    //        success: function (data) {
    //            data = JSON.parse(data);
    //            if (data.Status == false || data.Status == "false") {
    //                $("#" + id)[0].checked = false;
    //                $.Notification.autoHideNotify('error', 'top right', 'Thông báo', data.Message);
    //            } else {
    //                $("#divCheck-" + taskId).html('<i class="fas fa-check-square text-primary"></i>');
    //                $.Notification.autoHideNotify('success', 'top right', 'Thông báo', data.Message);
    //                var lstId = JSON.parse(data.ItemId);
    //                if (lstId.length > 0) {
    //                    $.each(lstId, function (index, value) {
    //                        $("#divCheck-" + value).html('<i class="fas fa-check-square text-primary"></i>');
    //                    })
    //                }
    //            }
    //        },
    //        error: function (xhr, status) {
    //        },
    //        complete: function (xhr, status) {
    //            //$('#showresults').slideDown('slow')
    //        }
    //    });
    //    })
    const config = {      
        title: 'Xác nhận hoàn thành?',
        text: 'Bạn có chắc chắn muốn xác nhận hoàn thành không?',
        type: 'warning',
        showCancelButton: true,
        cancelButtonText: 'Không',
        cancelButtonClass: 'btn-white',
        confirmButtonClass: 'btn-warning',
        confirmButtonText: "Có, hoàn thành nó!",
        closeOnConfirm: true,
    };

    // first variant
    sweetAlert.fire(config).then(callback);

    function callback(result) {
        if (result.value) {
            $.ajax({
            url: "/Time/XacNhanHoanThanhTask?taskId=" + taskId,
            type: "GET",
            contentType: 'application/json; charset=utf-8',
            async: false,
            success: function (data) {
                data = JSON.parse(data);
                if (data.Status == false || data.Status == "false") {
                    $("#" + idc)[0].checked = false;
                    $.Notification.autoHideNotify('error', 'top right', 'Thông báo', data.Message);
                } else {
                    $("#divCheck-" + taskId).html('<i class="fas fa-check-square text-primary"></i>');
                    $.Notification.autoHideNotify('success', 'top right', 'Thông báo', data.Message);
                    var lstId = JSON.parse(data.ItemId);
                    if (lstId.length > 0) {
                        $.each(lstId, function (index, value) {
                            $("#divCheck-" + value).html('<i class="fas fa-check-square text-primary"></i>');
                        })
                    }
                }
            },
            error: function (xhr, status) {
            },
            complete: function (xhr, status) {
                //$('#showresults').slideDown('slow')
            }
        });
          
        } else {
            $("#" + idc)[0].checked = false;
        }
    }
})
$(document).on("change", "#selectPageSize", function () {
    var size = $("#selectPageSize").val();
    var url = "/Time/ToDoList?pageSize="+size;
    var page = $("#pageNumM").val();
    if (page != "1") url += "&&page=" + page;
    var nguoiDungId = $("#nguoiDungIdM").val();
    if (nguoiDungId != "00000000-0000-0000-0000-000000000000") url += "&&nguoiDungId=" + nguoiDungId;   
    var tuan = $("#weekM").val();
    if (tuan != "") url += "&&week=" + tuan;
    var nam = $("#yearM").val();
    url += "&&year=" + nam;
    var duAnId = $("#duAnIdM").val();
    if (duAnId != "0") url += "&&duAnId=" + duAnId;
    var keyWord = $("#keyWordM").val();
    if ($.trim(keyWord) != "") url += "&&keyWord=" + keyWord;
    location.href = url;
})
