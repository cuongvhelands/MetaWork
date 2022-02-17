var nguoiDungIdM;
var quyenM;
var nguoiTaoIdM;
var checkInsert = true;
$(document).ready(function () {
    quyenM = $("#quyenM").val();
    nguoiDungIdM = $("#nguoiDungIdM").val();
    nguoiTaoIdM = $("#nguoiTaoIdM").val();
    var str = $("#hqM").val();
    var b = new Date(str);
    if (quyenM == "3") {
        var dataNguoiDung = getDataNguoiDung();
        $("#cboNguoiDung").kendoDropDownList({
            dataTextField: "HoTen",
            dataValueField: "NguoiDungId",
            filter: "contains",
            dataSource: dataNguoiDung,
        });
        $("#cboNguoiDung").data("kendoDropDownList").value(nguoiDungIdM);
        $('#txtNgayLamViec').datepicker({
            autoclose: true,
            format: 'dd/mm/yyyy',
            todayHighlight: true
        })
    } else {
        $('#txtNgayLamViec').datepicker({
            autoclose: true,
            format: 'dd/mm/yyyy',
            startDate: b,
            endDate: new Date(),
            todayHighlight: true
        })
    }
    
    $('#txtNgayLamViec2').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',       
        todayHighlight: true
    })
})
function getDataNguoiDung() {
    var result;
    $.ajax({
        url: "/Project/GetNguoiDungs",
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
$(document).on("click", "#btnModalAdd", function () {
    //$("#RegTime").checked = true;
    $("#RegTime").prop("checked", true);
    $("#txtTenTimeEntry").val("");
    $("#cboDuAn").val("0");
    $("#cboShip").val("0");
    $("#cboTask").val("0");
    $("#cboLoaiCongViec").val("0");
    $("#txtTimeFrom").val("");
    $("#txtTimeTo").val("");
    $("#txtNgayLamViec").val("");
    $("#txtGhiChu").val("");
    $("#cboLoaiNgay").val("0");
    $("#txtNgayLamViec2").val("");
    setAddOffTime();
    $("#addOffTimeModal").modal("show");
})
$(document).on("change", "#cboDuAn", function () {
    var duAnId = $("#cboDuAn").val();
    if (duAnId != "0") {
        $.ajax({
            url: "/Time/GetShipAbleBy?duAnId=" + duAnId + "&&userId=" + nguoiDungIdM,
            context: document.body,
            type: "GET",
            dataType: "html",
            async: false,
            success: function (data) {
                result = JSON.parse(data);
                var str = ' <option value="0">Chọn Ship able</option>';
                $.each(result, function (index, value) {
                    str += ' <option value="' + value.CongViecId + '">' + value.TenCongViec + '</option>';
                })
                $("#cboShip").html(str);
            },
            error: function (xhr, status) {

            },
            complete: function (xhr, status) {
                //$('#showresults').slideDown('slow')
            }
        });
    }
})
var reload = false;
$(document).on("change", "#cboShip", function () {
    var duAnId = $("#cboDuAn").val();
    var shipId = $("#cboShip").val();
    if (shipId != "0") {
        $.ajax({
            url: "/Time/GetTaskBy?duAnId=" + duAnId + "&&userId=" + nguoiDungIdM + "&&shipId=" + shipId,
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
                $("#cboTask").html(str);
            },
            error: function (xhr, status) {

            },
            complete: function (xhr, status) {
                //$('#showresults').slideDown('slow')
            }
        });
    }
})
$(document).on("change", "#cboTask", function () {
    var taskId = $("#cboTask").val();
    $.ajax({
        url: "/Time/GetLoaiCongViecByTaskId?taskId=" + taskId,
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            result = JSON.parse(data);
            var str = ' <option value="0">Chọn loại công việc</option>';
            $.each(result, function (index, value) {
                str += ' <option disabled value="' + value.LoaiCongViecId + '">' + value.TenLoaiCongViec + '</option>';
                if (value.LoaiCongViecs.length > 0) {
                    for (i = 0; i < value.LoaiCongViecs.length; i++) {
                        str += '<option value="' + value.LoaiCongViecs[i].LoaiCongViecId + '">' + value.LoaiCongViecs[i].TenLoaiCongViec + '</option>'
                    }
                }
            })
            $("#cboLoaiCongViec").html(str);
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
})
$(document).on("click", "#btnSubmitAddOffTime", function () {
    if (checkInsert == true) {
        checkInsert = false;
        var checkall = false;
        var radioValue = $("input[name='defaultExampleRadios']:checked").val();
        var congviec = {};
        congviec.MoTa = $("#txtGhiChu").val();

        if (radioValue == "0") {
            congviec.DuAnId = $("#cboDuAn").val();
            congviec.CongViecId = $("#cboTask").val();
            congviec.LoaiCongViecId = $("#cboLoaiCongViec").val();
            congviec.StrThoiGianBatDau = $("#txtTimeFrom").val();
            congviec.StrThoiGianKetThuc = $("#txtTimeTo").val();
            congviec.StrNgayLamViec = $("#txtNgayLamViec").val();
            congviec.TenCongViec = $("#txtTenTimeEntry").val();
            congviec.NguoiXuLyId = nguoiDungIdM;
            if ($.trim(congviec.TenCongViec) != "" && $.trim(congviec.StrNgayLamViec) != "" && congviec.DuAnId != "0" && $.trim(congviec.StrThoiGianBatDau) != "" && $.trim(congviec.StrThoiGianKetThuc != "") && $.trim(congviec.MoTa != "")) {
               
                    $.ajax({
                        url: "/Time/AddOffTime",
                        data: congviec,
                        context: document.body,
                        type: "POST",
                        async: false,
                        dataType: "html",
                        success: function (data) {
                            data = JSON.parse(data);
                            if (data.Status == false) {
                                $.Notification.autoHideNotify('error', 'top right', 'Thông báo', data.Message);
                                checkInsert = true;
                            } else {
                                checkall = true;
                                $.Notification.autoHideNotify('success', 'top right', 'Thành công', data.Message);
                                setTimeout(
                                    function () {
                                        location.reload();
                                    }, 1000);
                            }
                           
                        },
                        error: function (xhr, status) {                           
                        },
                        complete: function (xhr, status) {                          
                            //$('#showresults').slideDown('slow')
                        }
                    });
               

            }
            else {
                $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Hãy điền hết các trường không phải option.");
            }
        }
        else {
            congviec.DayType = $("#cboLoaiNgay").val();
            congviec.StrNgayLamViec = $("#txtNgayLamViec2").val();
            if ($.trim(congviec.MoTa) != "" && congviec.DayType != "0" && $.trim(congviec.StrNgayLamViec) != "") {
                if (nguoiDungIdM == nguoiTaoIdM) {
                  
                        $.ajax({
                            url: "/Time/AddDayType",
                            data: congviec,
                            context: document.body,
                            type: "POST",
                            async: false,
                            dataType: "html",
                            success: function (data) {
                                data = JSON.parse(data);
                                if (data.Status == false) {
                                    $.Notification.autoHideNotify('error', 'top right', 'Thông báo', data.Message);    
                                    checkInsert = true;
                                } else {
                                    checkall = true;
                                    $.Notification.autoHideNotify('success', 'top right', 'Thành công', data.Message);
                                    setTimeout(
                                        function () {
                                            location.reload();
                                        }, 1000);
                                }
                            },
                            error: function (xhr, status) {                             
                            },
                            complete: function (xhr, status) {

                                //$('#showresults').slideDown('slow')
                            }
                        });
                   

                } else {
                    $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Bạn không đăng ký ngày nghỉ hộ được.");
                }

            }
            else {
                $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Hãy điền hết các trường không phải option.");
            }
        }
        if (checkall == false) {
            checkInsert = true;
        }
    }
   
})
$(document).on("change", ".radioCt", function () {
    var radioValue = $("input[name='defaultExampleRadios']:checked").val();
    if (radioValue == "0") {
        setAddOffTime();
    } else {
        setAddDayType();
    }
})
function setAddOffTime() {
    $("#rowAddTime1").show();
    $("#rowAddTime2").show();
    $("#rowAddTime3").show();
    $("#rowDayType").hide();
}
function setAddDayType() {
    $("#rowAddTime1").hide();
    $("#rowAddTime2").hide();
    $("#rowAddTime3").hide();

    $("#rowDayType").show();
}
$(document).on("click", ".editRowTb", function () {
    var id = this.id;

    $.ajax({
        url: "/Time/ChiTietNgayCong?strNgayLamViec=" + id + "&&userId=" + nguoiDungIdM,
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            $("#tbyDetailDay").html(data);
            $("#ModalDetailDay").modal("show");
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });

})
$(document).on("click", ".deleteChiTietNgayCong", function () {
    var id = this.id;
    if (confirm("bạn có muốn xóa thời gian làm việc này không?")) {
        $.ajax({
            url: "/Time/DeleteThoiGian?id=" + id,
            context: document.body,
            type: "GET",
            dataType: "html",
            async: false,
            success: function (data) {
                if (data == true || data == "True" || data == "true") {
                    $.Notification.autoHideNotify('success', 'top right', 'Thông báo', "Bạn đã xóa thành công!");
                    $("#trCT-" + id).remove();
                    reload = true;
                } else {
                    $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Xóa không thành công!");
                }
            },
            error: function (xhr, status) {

            },
            complete: function (xhr, status) {
                //$('#showresults').slideDown('slow')
            }
        });
    }

})
$('#ModalDetailDay').on('hidden.bs.modal', function () {
    location.reload();
});
$(document).on("click", "#btnSearch", function () {
    var month = $("#dllMonth").val();
    str = "/Time/ReportUser?month=" + month;
    if (quyenM == "3") {
        var userId = $("#cboNguoiDung").val();
        str += "&&nguoiDungId=" + userId;
    }
    location.href = str;
})
//$(document).on("click", ".AddRowTb", function () {

//})