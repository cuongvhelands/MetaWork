$("#btnDongBo").click(function () {
    $("#tableDBAD").html("");
    $("#DBDAModal").modal("show");
})
$("#btnCodaDBDA").click(function () {
    $("#DBDAModal").modal("hide");
    $('#loadding').modal({ backdrop: 'static', keyboard: false });
    var docId = $("#txtDABA").val();
    $.ajax({
        url: "/Project/PartialViewSynchrony?link=" + docId,
        context: document.body,
        type: "GET",
        dataType: "html",       
        success: function (data) {
            $("#tableDBAD").html(data);
            $('#loadding').modal("hide");
            $("#DBDAModal").modal("show");
        },
        error: function (xhr, status) {
            $("#DBDAModal").modal("show");
        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
})
$(".btnSubmit").click(function () {
    if ($("#tbdyTableCoda").length > 0) {
        var rows = $("tbody#tbdyTableCoda").find("tr");
        var lstProject = []
        if (rows && rows.length > 0) {
            $.each(rows, function (index, value) {
                var project = {}
                var id = value.id;
                var ma = value.id.replace("tr-","");
                if ($("#chk-" + ma).length > 0 && $("#chk-" + ma)[0].checked) {
                    project.Ma = ma;
                    var cels = $("#" + id).find("td");
                    project.Parent = cels[2].textContent.trim();
                    project.ProjectName = cels[3].textContent.trim();
                    project.Slack_Name = cels[4].textContent.trim();
                    project.Type = cels[5].textContent.trim();
                    project.TeamLead = cels[6].textContent.trim();
                    project.Status = cels[7].textContent.trim();
                    project.CostH = cels[8].textContent.trim();
                    project.CostTien = cels[9].textContent.trim();
                    project.Budget = cels[10].textContent.trim();
                    project.Risk = cels[11].textContent.trim();
                    project.Document = cels[12].innerHTML.trim();
                    project.Note = cels[13].textContent.trim();
                    lstProject.push(project);
                }
                
            })
        }
        if (lstProject.length > 0) {
            $('#loadding').modal({ backdrop: 'static', keyboard: false });
            $.ajax({
                url: "/Project/AddProjectFromCoda",
                data: { lst: lstProject },
                context: document.body,
                type: "POST",
                dataType: "html",
                async: false,
                success: function (data) {
                    $('#loadding').modal("hide");
                    var result = JSON.parse(data);
                    if (result.Status > 0) {
                        $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Đồng bộ dự án thành công!");
                        setTimeout(
                            function () {
                                location.reload();
                            }, 100);
                    } else {
                        $.Notification.autoHideNotify('error', 'top right', 'Lỗi', result.Message);
                    }
                },
                error: function (xhr, status) {
                    $('#loadding').modal("hide");
                    $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Đồng bộ dự án không thành công!");
                },
                complete: function (xhr, status) {                  
                    $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Đồng bộ dự án không thành công!");         
                }
            });
        } else {
            $.Notification.autoHideNotify('success', 'top right', 'Thông báo', "Không có dự án nào cần đồng bộ về!");
            $("#DBDAModal").modal("hide");
        }
    }
})
$(document).ready(function () {
    var dataUser = getNguoiDungAlls2();
    var multiSelectNguoiDung = $("#SelectNguoiDung").kendoMultiSelect({
        dataTextField: "HoTen",
        dataValueField: "NguoiDungId",
        dataSource: dataUser
    })
    var str = $("#strNguoiDungIdM").val();
    if (str != "") {
        var nguoiDungIds = str.split(",");
        $("#SelectNguoiDung").data("kendoMultiSelect").value(nguoiDungIds);
    }
    var dataTT = getTrangThaiDuAns();
    var multiSelectTrangThai = $("#SelectTrangThai").kendoMultiSelect({
        dataTextField: "TenTrangThaiDuAn",
        dataValueField: "TrangThaiDuAnId",
        dataSource: dataTT
    })
    var str = $("#strStatusM").val();
    if (str != "") {
        var nguoiDungIds = str.split(",");
        $("#SelectTrangThai").data("kendoMultiSelect").value(nguoiDungIds);
    }
})
$("#btnFilter").click(function(){
    var tt = $("#SelectTrangThai").data("kendoMultiSelect").value();
    var nguoiDungIds = $("#SelectNguoiDung").data("kendoMultiSelect").value();
    var group = $("#SelectGroup").val();
    var loai = $("#SelectLoaiDuAn").val();
    var str = "/project/Index?typeGroup=" + group
    if (nguoiDungIds.length > 0) {
        str += "&&strNguoiDungId=" + nguoiDungIds;
    }
    if (tt.length > 0) {
        str += "&&strStatus=" + tt;
    }
    if (loai > 0) {
        str += "&&type=" + loai;
    }
    location.href=str;
});
function getNguoiDungAlls() {
    var result;
    $.ajax({
        url: "/ToDo/GetNguoiDungAll",
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
function getNguoiDungAlls2() {
    var result;
    $.ajax({
        url: "/ToDo/GetAllNguoiDung",
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
function getTrangThaiDuAns() {
    var result;
    $.ajax({
        url: "/Project/GetTrangThaiDuAns",
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

