$(document).ready(function () {
    if ($("#SelectNguoiDung").length > 0) {
        var phongBanId = $("#SelectPhongBan").val();
        var dataNguoiDung = getDataNguoiDung(phongBanId);
        $("#SelectNguoiDung").kendoMultiSelect({
            dataTextField: "HoTen",
            dataValueField: "NguoiDungId",
            dataSource: dataNguoiDung
        })
        var str = $("#strNguoiDungIdM").val();
        if (str != "") {
            var nguoiDungIds = str.split(",");
            $("#SelectNguoiDung").data("kendoMultiSelect").value(nguoiDungIds);
        }
    }
    var type = $("#typeM").val();
    if (type != 21) {
        $("#divTu").hide();
        $("#divDen").hide();
    }
})
var html = "";
$("#ddlTime").change(function () {
    var value = $('#ddlTime').val();
    if (value == 21) {
        $("#divTu").show();
        $("#divDen").show();
    } else {
        $("#divTu").hide();
        $("#divDen").hide();
    }
})
function getDataNguoiDung(id) {
    var result;
    $.ajax({
        url: "/Project/GetNguoiDungByPhongBanId?phongBanId=" + id,
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
$('#datetimepicker1').datepicker({
    autoclose: true,
    format: 'dd/mm/yyyy',
    todayHighlight: true
})
$('#datetimepicker2').datepicker({
    autoclose: true,
    format: 'dd/mm/yyyy',
    todayHighlight: true
})
$("#btnSearch").click(function () {
    load();
})
$("#SelectPhongBan").change(function () {
    $("#SelectNguoiDung").data("kendoMultiSelect").value([])
    load();
})
function load() {
    var phongBanId = $("#SelectPhongBan").val();
    var nguoiDungIds = $("#SelectNguoiDung").data("kendoMultiSelect").value();
    var type = $("#ddlTime").val();
    var tu = $("#txtFromDate").val();
    var den = $("#txtToDate").val();
    var str = "/Add/Index";
    var check = true;   
        if (check) {
            check = false;
            str += "?";
        } else str += "&&";
        str += "phongBanId=" + phongBanId;
        if (nguoiDungIds.length > 0) {
        if (check) {
            check = false;
            str += "?";
        } else str += "&&";
        str += "strNguoiDungId=" + nguoiDungIds;
    }
    if (check) {
        check = false;
        str += "?";
    } else str += "&&";
    str += "type=" + type;
    if (type == 21) {
        if (tu != "") {
            if (check) {
                check = false;
                str += "?";
            } else str += "&&";
            str += "strStartDate=" + tu;
        }
        if (den != "") {
            if (check) {
                check = false;
                str += "?";
            } else str += "&&";
            str += "strEndDate=" + den;
        }
    }

   
    location.href = str;
}
$(document).on("click", ".tdBtnAddToDo", function () {
    var id = this.id;
    $("#"+id).hide();
    id = id.replace("tdBtnAddToDo-", "");
    var rowspan = $("#tdrowSpan-" + id).attr('rowspan');
    var d = parseInt(rowspan)+1;
    document.getElementById("tdrowSpan-" + id).rowSpan = d + "";  
    $("#txtTenToDo-" + id).text("");
    $("#ddlDuAn-" + id).val(0);
    $("#trAddnewToDo-" + id).show();
})
$(document).on("click", ".btnHuyTD", function () {
    var id = this.id;
    var rowspan = $("#tdrowSpan-" + id).attr('rowspan');   
    var d = parseInt(rowspan) - 1;
    document.getElementById("tdrowSpan-" + id).rowSpan = d + "";  
    $("#tdBtnAddToDo-" + id).show();
    $("#trAddnewToDo-" + id).hide();
})

$(document).on("click", ".btnSubmitTD", function () {
    var id = this.id;
    var tenToDo = $("#txtTenToDo-" + id).val();
    var duAnId = $("#ddlDuAn-" + id).val();
    var check = true;
    var nguoiXuLyId = "";
    var quyen = $("#quyenM").val();
    if (tenToDo.trim() == "") {
        check = false;
        $("#errorTenToDo-" + id).text("Tên todo không được để trống.");
    } else {
        $("#errorTenToDo-" + id).text("");
    }
    if (duAnId == 0) {
        check = false;
        $("#errordllDuAn-" + id).text("Bạn hãy chọn dự án.");
    } else {        
        $("#errordllDuAn-" + id).text("");
    }
    if (quyen == 3) {
        nguoiXuLyId = $("#ddlNguoiXuLy-" + id).val();
        if (nguoiXuLyId != 0) {
            $("#errordllNguoiXuLy-" + id).text("")
        } else {
            check = false;
            $("#errordllNguoiXuLy-" + id).text("Bạn hãy chọn người xử lý.");
        }
    }
    if (check) {
        var data = {};
        data.TenCongViec = tenToDo;
        data.DuAnId = duAnId;
        data.StrNgayLamViec = id;
        if (quyen == 3) data.NguoiXuLyId = nguoiXuLyId;
        $.ajax({
            url: "/Add/AddToDo",
            context: document.body,
            type: "POST",
            data: data,
            dataType: "html",
            success: function (data) {
                var result = JSON.parse(data);
                if (result.Status) {
                    $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Cập nhật công việc thành công!");
                    reloadTable();
                }
                else {
                    $.Notification.autoHideNotify('error', 'top right', 'Thất bại', result.Message);
                }
            },
            error: function (xhr, status) {
                $.Notification.autoHideNotify('error', 'top right', 'Lỗi', "Vui lòng thử lại sau!");
            },
            complete: function (xhr, status) {
                $('#detailsTodoModal').modal('hide')
            }
        });
    }

})
function reloadTable(){
    var phongBanId = $("#phongBanIdM").val();
    var nguoiDungIds = $("#strNguoiDungIdM").val();
    var type = $("#typeM").val();
    var tu = $("#strStartDateM").val();
    var den = $("#strEndDateM").val();
    var str = "/Add/Index";
    var check = true;
    
        if (check) {
            check = false;
            str += "?";
        } else str += "&&";
        str += "phongBanId=" + phongBanId;
   
    if (nguoiDungIds.length > 0) {
        if (check) {
            check = false;
            str += "?";
        } else str += "&&";
        str += "strNguoiDungId=" + nguoiDungIds;
    }
    if (check) {
        check = false;
        str += "?";
    } else str += "&&";
    str += "type=" + type;
    if (type == 21) {
        if (tu != "") {
            if (check) {
                check = false;
                str += "?";
            } else str += "&&";
            str += "strStartDate=" + tu;
        }
        if (den != "") {
            if (check) {
                check = false;
                str += "?";
            } else str += "&&";
            str += "strEndDate=" + den;
        }
    }
    $.ajax({
        url: str,
        type: "GET",
        async: false,
        success: function (data) {          
            $("#tbyAddTodo").html($(data).find("#tbyAddTodo"));
        },
        error: function (xhr, status) {
            $.Notification.autoHideNotify('error', 'top right', 'Lỗi', "Lỗi kết nối đến máy chủ!");
        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
}

$(document).on("click", ".btnAddNewToDo", function () {
    var id = this.id;
    $("#" + id).hide();
    id = id.replace("btnAddNewToDo-", ""); 
    $("#txtTenToDo-" + id).text("");
    $("#ddlDuAn-" + id).val(0);
    $("#divAddNew-" + id).show();
})
$(document).on("click", ".btnHuyTD2", function () {
    var id = this.id;
    $("#btnAddNewToDo-" + id).show();   
    $("#divAddNew-" + id).hide();
})
$(document).on("change", ".ddlTTCV", function () {
    var id = this.id;
    id = id.replace("ddlTTTodo-", "");
    var value = this.value;
    $.ajax({
        url: "/Add/CheckUpdateTrangThaiCongViec?congViecId=" + id +"&&trangThaiCongViec="+value,
        context: document.body,
        type: "Get",      
        dataType: "html",
        success: function (data) {
            var result = JSON.parse(data);            
            if (result.Status == true) {              
                updateCongViec(id, value); 
                reloadTable();
                initTimer();
            }
            else {
                var col = result.ItemId.split("-");
                if (col == '1') {
                    if (confirm(result.Message)) {
                        updateCongViec();
                    } else {
                        $("#ddlTTTodo-" + id).val(col[1])
                    }
                } else {
                    $.Notification.autoHideNotify('error', 'top right', 'Thất bại', result.Message);
                    $("#ddlTTTodo-" + id).val(col[1])
                }
               
            }
        },
        error: function (xhr, status) {
            $.Notification.autoHideNotify('error', 'top right', 'Lỗi', "Vui lòng thử lại sau!");
        },
        complete: function (xhr, status) {
            $('#detailsTodoModal').modal('hide')
        }
    });
})
function updateCongViec(congViecId, trangThaiId){
    $.ajax({
        url: "/Add/UpdateTrangThaiCongViec?congViecId=" + congViecId + "&&trangThaiCongViec=" + trangThaiId,
        context: document.body,
        type: "Get",
        async:false,
        dataType: "html",
        success: function (data) {           
            if (data == true||data=="True") {
                $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Cập nhật công việc thành công!");

                reloadTable();
            }
            else {
                $.Notification.autoHideNotify('error', 'top right', 'Thất bại', "Cập nhật công việc không thành công!");
            }
        },
        error: function (xhr, status) {
            $.Notification.autoHideNotify('error', 'top right', 'Lỗi', "Vui lòng thử lại sau!");
        },
        complete: function (xhr, status) {
            $('#detailsTodoModal').modal('hide')
        }
    });
}
var editId = 0;
var ngayLamViec = "";
$(document).on("click", ".linkDetailToDo", function () {
    var id = this.id;
    var col = id.split('-');
    var todoid = col[1];
    var newngayLamViec = id.replace("linkDetailToDo-" + todoid + "-", "");
    var tr = $("#"+id).parents('tr').first();
    editId = todoid;
    $.ajax({
        url: "/Add/_PartialViewDetailToDo?congViecId=" + todoid ,
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            if (ngayLamViec != "") {             
                var rowspan = $("#tdrowSpan-" + ngayLamViec).attr('rowspan');
                var d = parseInt(rowspan) - 1;
                document.getElementById("tdrowSpan-" + ngayLamViec).rowSpan = d + "";  
                $("#trDetail").remove();
            }
            var rowspan1 = $("#tdrowSpan-" + newngayLamViec).attr('rowspan');
            var d1 = parseInt(rowspan1) + 1;
            document.getElementById("tdrowSpan-" + newngayLamViec).rowSpan = d1 + "";
            ngayLamViec = newngayLamViec;

            var str = '<tr id="trDetail"><td id="tdDetails" colspan="4"></tr>'
            $(str).insertAfter(tr);
            $("#tdDetails").html(data);    
            var duAnId = $("#duAnDetailId").val();
            var shipID = $("#shipDetailId").val();
            var taskId = $("#taskDetailId").val();
            reloadcboPop(duAnId, shipID, taskId);
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
    
})
$(document).on("change", "#ddlDuAnPop", function () {
    var duAnId = $("#ddlDuAnPop").val(); 
    var ship = $("#cboShipPop").data("kendoDropDownList");
    var dataSource = getShipable(duAnId);
    ship.setDataSource(dataSource)
    ship.value(0);   
    var task = $("#cboTaskPop").data("kendoDropDownList");
    var taskdata = getTask(0);
    task.setDataSource(taskdata);
    task.value(0); 
})
$(document).on("change", "#ddlShipPop", function () {
    var id = $("#ddlShipPop").val();
    $.ajax({
        url: "/Add/GetTenTaskByshiableId?shipableId=" + id,
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            result = JSON.parse(data);
            var str = ' <option value="0">Chọn Task</option>';
            $.each(result, function (index, value) {
                str += ' <option value="' + value.CongViecId + '">' + value.TenCongViec + '</option>';
            })
            $("#ddlTaskPop").html(str);
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
})
$(document).on('click', "#subMitPopCV", function () {
    var taskId = $("#cboTaskPop").data("kendoDropDownList").value();
    if (taskId == 0) {
        alert("bạn hãy chọn task");
    } else {
        var trangThai = $("#ddlTTPop").val();
        $.ajax({
            url: "/Add/ChuyenToDo2Task?toDoId=" + editId + "&&taskId=" + taskId + "&&trangThai=" + trangThai,
            context: document.body,
            type: "Get",
            dataType: "html",
            success: function (data) {
                var result = JSON.parse(data);
                if (result.Status) {
                    $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Cập nhật công việc thành công!");                   
                    reloadTable();
                } else {
                    $.Notification.autoHideNotify('error', 'top right', 'Thất bại', result.Message);
                }      
            },
            error: function (xhr, status) {
                $.Notification.autoHideNotify('error', 'top right', 'Lỗi', "Vui lòng thử lại sau!");
            },
            complete: function (xhr, status) {
              
            }
        });
    }
})
$(document).on('click', "#subMitPopTime", function () {
    $.ajax({
        url: "/Add/Approvetime?toDoId=" + editId,
        context: document.body,
        type: "Get",
        dataType: "html",
        success: function (data) {
            var result = JSON.parse(data);
            if (result.Status) {
                $.Notification.autoHideNotify('success', 'top right', 'Thành công', result.Message);
                reloadTable();
            } else {
                $.Notification.autoHideNotify('error', 'top right', 'Thất bại', result.Message);
            }      
        },
        error: function (xhr, status) {
            $.Notification.autoHideNotify('error', 'top right', 'Lỗi', "Vui lòng thử lại sau!");
        },
        complete: function (xhr, status) {
          
        }
    });
})
$(document).on('click', ".linkdeleteTTP", function () {
    var id = this.id;
    if (confirm("Bạn chắc chắn muốn xóa khoảng thời gian này chứ")) {
        $.ajax({
            url: "/Add/DeleteTime?thoiGianId=" + id,
            context: document.body,
            type: "Get",
            dataType: "html",
            success: function (data) {
                if (data == true || data == "True") {
                    $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Xóa thành công!"); 
                    reloadTable();
                }
                else {
                    $.Notification.autoHideNotify('error', 'top right', 'Thất bại', "Xóa không thành công!");
                }
            },
            error: function (xhr, status) {
                $.Notification.autoHideNotify('error', 'top right', 'Lỗi', "Vui lòng thử lại sau!");
            },
            complete: function (xhr, status) {

            }
        });
    }
})
$(document).on('click', ".linkDeleteToDo", function () {
    var id = this.id;
    if (confirm("Bạn chắc chắn muốn xóa todo này chứ")) {
        $.ajax({
            url: "/Add/DeleteToDo?todo=" + id,
            context: document.body,
            type: "Get",
            dataType: "html",
            success: function (data) {
                if (data == true || data == "True") {
                    $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Xóa thành công!");
                    reloadTable();
                }
                else {
                    $.Notification.autoHideNotify('error', 'top right', 'Thất bại', "Xóa không thành công!");
                }
            },
            error: function (xhr, status) {
                $.Notification.autoHideNotify('error', 'top right', 'Lỗi', "Vui lòng thử lại sau!");
            },
            complete: function (xhr, status) {

            }
        });
    }
})
$(document).on("click", "#cancelpopCV", function () {
    if (ngayLamViec != "") {        
        var rowspan = $("#tdrowSpan-" + ngayLamViec).attr('rowspan');
        var d = parseInt(rowspan) - 1;
        document.getElementById("tdrowSpan-" + ngayLamViec).rowSpan = d + "";
        ngayLamViec = "";
        $("#trDetail").remove();
    }
})
function reloadcboPop(duAnId,shipid,taskid){    
    var shipdata = getShipable(duAnId);
    var ship = $("#cboShipPop").kendoDropDownList({
        dataTextField: "TenCongViec",
        dataValueField: "CongViecId",
        filter: "contains",
        dataSource: shipdata,
        select: onSelectShip,
    }).data("kendoDropDownList");   
    ship.value(shipid);   
    var taskdata = getTask(shipid);
    var task = $("#cboTaskPop").kendoDropDownList({
        dataTextField: "TenCongViec",
        dataValueField: "CongViecId",
        filter: "contains",
        dataSource: taskdata,      
    }).data("kendoDropDownList");
    task.value(taskid);
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
function getShipable(duAnId) {
    var result;
    $.ajax({
        url: "/Add/GetTenShipByDuAnId?duAnId=" + duAnId,
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
function getTask(duAnId) {
    var result;
    $.ajax({
        url: "/Add/GetTenTaskByshiableId?shipableId=" + duAnId,
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
function onSelectShip(e) {
    var shipid = e.dataItem.CongViecId;
    var task = $("#cboTaskPop").data("kendoDropDownList");
    var taskdata = getTask(shipid);
    task.setDataSource(taskdata);
    task.value(0); 
}

