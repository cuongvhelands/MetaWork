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
        $('#txtNgayLamViec').datepicker({
            autoclose: true,
            format: 'dd/mm/yyyy',           
            todayHighlight: true
        })
    if (quyenM == "3") {
        var dataNguoiDung = getDataNguoiDung();
        $("#cboNguoiDung").kendoDropDownList({
            dataTextField: "HoTen",
            dataValueField: "NguoiDungId",
            filter: "contains",
            dataSource: dataNguoiDung,
        });
        $("#cboNguoiDung").data("kendoDropDownList").value(nguoiDungIdM);
    }
    
    $('#txtNgayNghiPhep').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',       
        todayHighlight: true
    })
    $('#txtNgayLamViecMetting').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        todayHighlight: true
    })
    $('#txtNgayLamViecCongTacFrom').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        todayHighlight: true
    })
    $('#txtNgayLamViecCongTacTo').datepicker({
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
    var id = $("#cboDuAn").val();
    var dataGiaiDoan = getDuAnThanhPhan(id);
    var str = '<option value="0"> Chọn dự án thành phần </option>';
    if (dataGiaiDoan && dataGiaiDoan.length > 0) {
      
        $.each(dataGiaiDoan, function (index, value) {
            str += '<option value="' + value.DuAnId + '">' + value.TenDuAn + '</option>'
        })
       
    } 
    $("#cboGiaiDoanDuAnDT").html(str);
    //var dataUser2 = getNguoiDungs(id);
    //if (dataUser2 && dataUser2.length > 0) {
    //    var strnguoi = '<option value="0"> Chọn người xử lý </option>';
    //    $.each(dataUser2, function (index, value) {
    //        strnguoi += '<option value="' + value.NguoiDungId + '">' + value.HoTen + '</option>'

    //    })
    //    $("#cboNguoiDung").html(strnguoi);
    //}
})
$(document).on("change", "#cboDuAnCongTac", function () {
    var id = $("#cboDuAnCongTac").val();
    var dataGiaiDoan = getDuAnThanhPhan(id);
    var str = '<option value="0"> Chọn dự án thành phần </option>';
    if (dataGiaiDoan && dataGiaiDoan.length > 0) {

        $.each(dataGiaiDoan, function (index, value) {
            str += '<option value="' + value.DuAnId + '">' + value.TenDuAn + '</option>'
        })

    }
    $("#cboGiaiDoanDuAnDTCongTac").html(str);
    //var dataUser2 = getNguoiDungs(id);
    //if (dataUser2 && dataUser2.length > 0) {
    //    var strnguoi = '<option value="0"> Chọn người xử lý </option>';
    //    $.each(dataUser2, function (index, value) {
    //        strnguoi += '<option value="' + value.NguoiDungId + '">' + value.HoTen + '</option>'

    //    })
    //    $("#cboNguoiDung").html(strnguoi);
    //}
})
$(document).on("change", "#cboGiaiDoanDuAnDT", function () {
    var duAnId = $("#cboGiaiDoanDuAnDT").val();
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
function getDuAnThanhPhan(duAnId) {
    var result;
    $.ajax({
        url: "/ToDo/GetDuAnThanhPhans?duAnId=" + duAnId,
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
var reload = false;
$(document).on("change", "#cboShip", function () {
    var duAnId = $("#cboGiaiDoanDuAnDT").val();
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
        if (radioValue == "0") {
            congviec.DuAnId = $("#cboGiaiDoanDuAnDT").val();
            congviec.CongViecId = $("#cboTask").val();          
            congviec.StrThoiGianBatDau = $("#txtTimeFrom").val();
            congviec.StrThoiGianKetThuc = $("#txtTimeTo").val();
            congviec.StrNgayLamViec = $("#txtNgayLamViec").val();
            congviec.NguoiXuLyId = nguoiDungIdM;    
            if ( $.trim(congviec.StrNgayLamViec) != "" && congviec.DuAnId != "0"  && $.trim(congviec.StrThoiGianBatDau) != "" && $.trim(congviec.StrThoiGianKetThuc != "") ) {               
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
        else if(radioValue == "2") {
            congviec.DuAnId = $("#cboGiaiDoanDuAnDTCongTac").val();          
            congviec.StrThoiGianBatDau = $("#txtNgayLamViecCongTacFrom").val();
            congviec.StrThoiGianKetThuc = $("#txtNgayLamViecCongTacTo").val();
            congviec.NguoiXuLyId = nguoiDungIdM;
            if (congviec.DuAnId != "0" && $.trim(congviec.StrThoiGianBatDau) != "" && $.trim(congviec.StrThoiGianKetThuc != "")) {
                $.ajax({
                    url: "/Time/AddCongTac",
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
        else if (radioValue == "1") {
            var rows = $("tbody#tbdyTableCodaMeeting").find("tr");
            var lstCV = []
            if (rows && rows.length > 0) {
                $.each(rows, function (index, value) {
                    var chks = $(".chkChangeMeeting");
                    if (chks.length > 0) {
                        $.each(chks, function (index, value) {
                            if (value.checked == true) {
                                var cv = {}
                                cv.MaDuAn = value.parentElement.parentElement.cells[2].textContent;
                                cv.StrNguoiThamGia = value.parentElement.parentElement.cells[6].textContent;
                                cv.StrShipAble = value.parentElement.parentElement.cells[9].textContent;
                                cv.StrStartTime = value.parentElement.parentElement.cells[7].textContent;
                                lstCV.push(cv);
                            }
                        });
                    }
                })
            }
            if (lstCV.length > 0) {
                if (lstCV.length > 0) {
                    $.ajax({
                        url: "/ToDo/AddMeetingToDB",
                        data: { MeetingCodas: lstCV},
                        context: document.body,
                        type: "POST",
                        dataType: "html",
                        async: false,
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
    } else if (radioValue == "1") {
       setMetting();
    } else {
        setCongTac();
    }
})
function setAddOffTime() {
    $("#divOffTime").show();
    $("#divMetting").hide();  
    $("#divCongTac").hide();
}
function setCongTac() {
    $("#divOffTime").hide();
    $("#divMetting").hide();
    $("#divCongTac").show();
}
function setMetting() {
    $("#divOffTime").hide();
    $("#divMetting").show();
    $("#divCongTac").hide();
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
$(document).on("click", "#btnModalNghiPhep", function () {
    $("#addNghiPhepModal").modal("show");
    $("#txtNgayLamViec2").val("");
    $("#txtGhiChuNghiPhep").val("");
})
$(document).on("click", "#btnSubmitAddNghiPhep", function () {
    if (checkInsert == true) {
        checkInsert = false;
        var congviec = {};
        congviec.MoTa = $("#txtGhiChuNghiPhep").val();
        congviec.DayType = $("#cboLoaiNgay").val();
        congviec.StrNgayLamViec = $("#txtNgayNghiPhep").val();
        if ($.trim(congviec.MoTa) != "" && congviec.DayType != "0" && $.trim(congviec.StrNgayLamViec) ) {
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
})
$(document).on("change", "#txtNgayLamViec", function () {
    var str = $("#txtNgayLamViec").val().trim();
    if (str != "") {
        $.ajax({
            url: "/Time/PartalViewNgayLamViec?nguoiDungId=" + nguoiDungIdM + "&&strNgayLamViec=" + str,
            type: "GET",
            async: false,
            success: function (data) {
                $("#tableTime").html(data);
            },
            error: function (xhr, status) {
                $.Notification.autoHideNotify('error', 'top right', 'Lỗi', "Lỗi kết nối đến máy chủ!");
            },
            complete: function (xhr, status) {
                //$('#showresults').slideDown('slow')
            }
        });
    } else {
        $("#tableTime").html("");
    }
   
})
$(document).on("click", "#btnDBCodaMeeting", function () {
    var linkCoda = $("#txtLinkDBDMCVDATP").val().trim();
    if (linkCoda != "") {
        var str = "/Todo/PartialViewMeetingCoda?linkCoda=" + linkCoda
        $.ajax({
            url: str ,
            type: "GET",
            async: false,
            success: function (data) {
                $("#tableTimeMeeting").html(data);
            },
            error: function (xhr, status) {
                $.Notification.autoHideNotify('error', 'top right', 'Lỗi', "Lỗi kết nối đến máy chủ!");
            },
            complete: function (xhr, status) {
                //$('#showresults').slideDown('slow')
            }
        });
    } else {
        $.Notification.autoHideNotify('error', 'top right', 'Lỗi', "Bạn hãy điền link coda của buổi meeting.");
    }
    
})
//$(document).on("click", ".AddRowTb", function () {

//})