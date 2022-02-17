var shipableId = 0, editor;
var checkInsertShip = true;
var teamIdM = $("#teamM").val();
$(document).ready(function () {
    var weekData = getDataWeek();
    var week = $("#cboWeek1").kendoDropDownList({
        dataTextField: "TenWeek",
        dataValueField: "Week",
        filter: "contains",
        dataSource: weekData,       
        select: onSelectWeek
    }).data("kendoDropDownList");
    var tuan = $("#tuanM").val();
    var year = $("#yearM").val();
    week.value(tuan+"-"+year);
    var trangThaiCvData = getDataTrangThaiCongViec3();
    var trangThaiCv = $("#cboTrangThai11").kendoMultiSelect({
        dataTextField: "TenTrangThai",
        dataValueField: "TrangThaiCongViecId",
        filter: "contains",
        dataSource: trangThaiCvData,
        change: onChangeTrangThai,
        //select: onSelectTrangThai
    }).data("kendoMultiSelect");
    //var loaiCongViecData = getLoaiCongViecs();
    //// create MultiSelect from select HTML element
    //var loaiCongViec = $("#loaiCongViecList").kendoMultiSelect({
    //    dataTextField: "TenLoaiCongViec",
    //    dataValueField: "LoaiCongViecId",
    //    dataSource: loaiCongViecData
    //}).data("kendoMultiSelect");  
    var trangThaiCvId = $("#trangThaiCvM").val();
    if (trangThaiCvId.length > 0) {
        var trangThais = JSON.parse(trangThaiCvId);
        $("#cboTrangThai11").data("kendoMultiSelect").value(trangThais);
    }
    var duAnId = $("#duAnIdM").val();
    var duAnData = getDuAns();
    var duAn = $("#cboDuAn1").kendoDropDownList({
        dataTextField: "TenDuAn",
        dataValueField: "DuAnId",
        filter: "contains",
        dataSource: duAnData,
        select: onSelectDuAn      
    }).data("kendoDropDownList");
    
    duAn.value(duAnId);
    var teamData = getDataTeam();
    var team = $("#cboTeam").kendoDropDownList({
        dataTextField: "TenPhongBan",
        dataValueField: "PhongBanId",
        filter: "contains",
        dataSource: teamData,
        select: onSelectTeam
    }).data("kendoDropDownList");

    team.value(teamIdM);


    var nguoiDungIdMS = $("#nguoiDungMSh").val();
    var nguoiDungKDD = getDataNguoiDung2(teamIdM);
    var nguoiDung = $("#cboNguoiDung1").kendoDropDownList({
        dataTextField: "HoTen",
        dataValueField: "NguoiDungId",
        filter: "contains",
        dataSource: nguoiDungKDD,
        select: onSelectNguoiDung
    }).data("kendoDropDownList");
    nguoiDung.value(nguoiDungIdMS);


    if (duAnData.length > 0) {
        var str = "";
        $.each(duAnData, function (index, value) {
            str += '<option id="' + value.DuAnId+'" value="' + value.DuAnId + '" class="changeDA">' + value.TenDuAn + '</option>';
        })
        $("#cboDuAn").html(str);
    }
    var nguoiDungData = getDataNguoiDung();
    if (nguoiDungData.length > 0) {
        var str = '<option value="">Chọn người dùng</option>';
        $.each(nguoiDungData, function (index, value) {
            str += '<option value="' + value.NguoiDungId + '">' + value.HoTen + '</option>';
        })
        $("#cboNguoiDung").html(str);
    } 
    var thuTuUuTien = getDataDoUuTien();
    if (thuTuUuTien.length > 0) {
        var str = '';
        $.each(thuTuUuTien, function (index, value) {
            str += '<option value="' + value.ThuTuUuTien + '">' + value.TenThuTuUuTien + '</option>';
        })
        $("#cboDoUuTien").html(str);
    }  
    var trangThaiCongViecData2 = getDataTrangThaiCongViec2();
    if (trangThaiCongViecData2.length > 0) {
        var str = '';
        $.each(trangThaiCongViecData2, function (index, value) {
            str += '<option value="' + value.TrangThaiCongViecId + '">' + value.TenTrangThai + '</option>';
        })
        $("#cboTrangThai2").append(str);
    }
    $('#datetimepicker1').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        startDate: new Date(),
        todayHighlight: true
    })
    $("#txtGhiChu").kendoEditor({
        tools: [
            "bold",
            "italic",
            "underline",           
            "justifyLeft",
            "justifyCenter",
            "justifyRight",
            "justifyFull",
            "insertUnorderedList",
            "insertOrderedList",             
        ]
    });
    editor = $("#txtGhiChu").data("kendoEditor");
});
function setSelectTrangThaiCongViec(trangThaiShipable) {
    var trangThaiCongViecData = getDataTrangThaiCongViecByKhoaCha(trangThaiShipable);
    if (trangThaiCongViecData.length > 0) {
        var str = '';
        $.each(trangThaiCongViecData, function (index, value) {
            str += '<option value="' + value.TrangThaiCongViecId + '">' + value.TenTrangThai + '</option>';
        })
        $("#cboTrangThai").html(str);
    }
}
function setSelectTrangThaiCongViec2(trangThaiShipable,shipalbeId) {
    var trangThaiCongViecData = getDataTrangThaiCongViecByKhoaCha2(trangThaiShipable, shipalbeId);
    if (trangThaiCongViecData.length > 0) {
        var str = '';
        $.each(trangThaiCongViecData, function (index, value) {
            str += '<option value="' + value.TrangThaiCongViecId + '">' + value.TenTrangThai + '</option>';
        })
        $("#cboTrangThai2").html(str);
    }
}

function setSelectWeek(week,year) {    
    $.ajax({
        url: "/Shipable/GetWeeks3?week="+week+"&&year="+year,
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            result = JSON.parse(data);
            var str = "";
            $.each(result, function (index, value) {    
                str += '<option value="' + value.Week + '"';
                if (value.Week == week) str += " selected";
                str += '>' + value.TenWeek + '</option > ';                             
            })
                 $("#cboWeek").html(str);
            $("#cboWeek").val(week+"-"+year);
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
}
function getDataWeek(){
    var result;
    $.ajax({
        url: "/Shipable/GetWeeks2",
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
function getDataTeam() {
    var result;
    $.ajax({
        url: "/Shipable/GetTeam",
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
function onSelectWeek(e) {
    var value = e.dataItem.Week;  
    var col = value.split("-");
    var tuan = col[0];
    var year = col[1];
    var duAnId = $("#cboDuAn1").data("kendoDropDownList").value();    
    var nguoiDungId = $("#cboNguoiDung1").data("kendoDropDownList").value();
    var trangThaiCvId = $("#cboTrangThai11").data("kendoMultiSelect").value();
    var teamId = $("#cboTeam").data("kendoDropDownList").value();
    var url = "/Shipable/index?duAnId=" + duAnId + "&&tuan=" + tuan + "&&trangThaiIds=" + trangThaiCvId + "&&year=" + year + "&&nguoiDungId=" + nguoiDungId+"&&teamId="+teamId;
    window.location.href = url;
}
function onSelectDuAn(e) {    
    var value = $("#cboWeek1").data("kendoDropDownList").value();
    var nguoiDungId = $("#cboNguoiDung1").data("kendoDropDownList").value();
    var col = value.split("-");
    var tuan = col[0];
    var year = col[1];
    var duAnId = e.dataItem.DuAnId;
    var trangThaiCvId = $("#cboTrangThai11").data("kendoMultiSelect").value();
    var teamId = $("#cboTeam").data("kendoDropDownList").value();
    var url = "/Shipable/index?duAnId=" + duAnId + "&&tuan=" + tuan + "&&trangThaiIds=" + trangThaiCvId + "&&year=" + year + "&&nguoiDungId=" + nguoiDungId + "&&teamId=" + teamId;
    window.location.href = url;
}
function onChangeTrangThai(e) {
    var value = $("#cboWeek1").data("kendoDropDownList").value();
    var col = value.split("-");
    var tuan = col[0];
    var year = col[1];
    var duAnId = $("#cboDuAn1").data("kendoDropDownList").value();
    var trangThaiCvId = $("#cboTrangThai11").data("kendoMultiSelect").value();
    //var trangThaiCvId = e.dataItem.TrangThaiCongViecId;
    var nguoiDungId = $("#cboNguoiDung1").data("kendoDropDownList").value();  
    var teamId = $("#cboTeam").data("kendoDropDownList").value();
    var url = "/Shipable/index?duAnId=" + duAnId + "&&tuan=" + tuan + "&&trangThaiIds=" + trangThaiCvId + "&&year=" + year + "&&nguoiDungId=" + nguoiDungId + "&&teamId=" + teamId;
    window.location.href = url;
}
//$(document).on("changle", "#cboTrangThai11", function () {
//        var value = $("#cboWeek1").data("kendoDropDownList").value();
//    var col = value.split("-");
//    var tuan = col[0];
//    var year = col[1];
//    var duAnId = $("#cboDuAn1").data("kendoDropDownList").value();
//    var trangThaiCvId = $("#cboTrangThai11").data("kendoMultiSelect").value();
//    var nguoiDungId = $("#cboNguoiDung1").data("kendoDropDownList").value();  
//    var teamId = $("#cboTeam").data("kendoDropDownList").value();
//    var url = "/Shipable/index?duAnId=" + duAnId + "&&tuan=" + tuan + "&&trangThaiIds=" + trangThaiCvId + "&&year=" + year + "&&nguoiDungId=" + nguoiDungId + "&&teamId=" + teamId;
//    window.location.href = url;

//})
function onSelectNguoiDung(e) {
    var value = $("#cboWeek1").data("kendoDropDownList").value();
    var col = value.split("-");
    var tuan = col[0];
    var year = col[1];
    var duAnId = $("#cboDuAn1").data("kendoDropDownList").value();
    var trangThaiCvId = $("#cboTrangThai11").data("kendoMultiSelect").value();
    var nguoiDungId = e.dataItem.NguoiDungId;   
    var teamId = $("#cboTeam").data("kendoDropDownList").value();
    var url = "/Shipable/index?duAnId=" + duAnId + "&&tuan=" + tuan + "&&trangThaiIds=" + trangThaiCvId + "&&year=" + year + "&&nguoiDungId=" + nguoiDungId + "&&teamId=" + teamId;
    window.location.href = url;
}
function onSelectTeam(e) {
    var value = $("#cboWeek1").data("kendoDropDownList").value();
    var col = value.split("-");
    var tuan = col[0];
    var year = col[1];
    var duAnId = $("#cboDuAn1").data("kendoDropDownList").value();
    var trangThaiCvId = $("#cboTrangThai11").data("kendoMultiSelect").value();
    var nguoiDungId = $("#cboNguoiDung1").data("kendoDropDownList").value(); 
    var teamId = e.dataItem.PhongBanId;
    var url = "/Shipable/index?duAnId=" + duAnId + "&&tuan=" + tuan + "&&trangThaiIds=" + trangThaiCvId + "&&year=" + year + "&&nguoiDungId=" + nguoiDungId + "&&teamId=" + teamId;
    window.location.href = url;
}
function openAddShipModal() {
    var ma = $("#maM").val();
    $("#custom-width-modalLabel").text("Thêm mới ship-able");
    $("#txtTenShipable").val("");
    $("#txtMaShipable").val(ma);
    $("#cboGiaiDoanDuAn").val(0);
    $("#cboNguoiDung").val("");  
    $("#txtDeadLine").val($("#nowM").val());
    $("#addShipModal").modal("show");
    shipableId = 0;
    editor.value("");
    $("#btnDelete").hide();
    $("#cboTrangThai2").val(0);
    $("#cboTrangThai2").hide();
    var tuanHT = $("#tuanHTM").val();
    var yearHT = $("#yearM").val();
    setSelectWeek(tuanHT,yearHT);
    $("#divXuLy").hide();
    setSelectTrangThaiCongViec(0);
    noDisableAllCommonInPop();  
}
function selectDuAn(duAnId) {
    var data = getDataGiaiDoanDuAn(duAnId);
    var str = '<option value="0">Chọn giai đoạn </option>';
    if (data.length > 0) {      
        $.each(data, function (index, value) {
            str += '<option value="' + value.GiaiDoanDuAnId + '">' + value.TenGiaiDoan + '</option>';
        })       
    }
    $("#cboGiaiDoanDuAn").html(str);
}
$("#cboDuAn").change(function () {
    var id = $("#cboDuAn").val();
    selectDuAn(id);
})
function getDataGiaiDoanDuAn(duAnId) {
    var result;
    $.ajax({
        url: "/Shipable/GetGiaiDoanDuAns?duAnId=" + duAnId,
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
        url: "/Shipable/GetNguoiDungs",
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
function getDataNguoiDung2(teamId) {
    var result;
    $.ajax({
        url: "/Shipable/GetNguoiDungsByPhongBan?phongBanId=" + teamId,
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
function getDataDoUuTien() {
    var result;
    $.ajax({
        url: "/Shipable/GetThuTuUuTiens",
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
function getDataTrangThaiCongViec() {
    var result;
    $.ajax({
        url: "/Shipable/GetTrangThaiShipables",
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
function getDataTrangThaiCongViecByKhoaCha(chaId) {
    var result;
    $.ajax({
        url: "/Shipable/GetTrangThaiShipablesByKhoaCha?khoaChaId="+chaId,
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

function getDataTrangThaiCongViecByKhoaCha2(chaId,shipId) {
    var result;
    $.ajax({
        url: "/Shipable/GetTrangThaiShipablesByKhoaCha2?khoaChaId=" + chaId + "&&shipId=" + shipId,
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
function getDataTrangThaiCongViec2() {
    var result;
    $.ajax({
        url: "/Shipable/GetTrangThaiShipables2",
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
function getDataTrangThaiCongViec3() {
    var result;
    $.ajax({
        url: "/Shipable/GetTrangThaiShipablesAll",
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
$(document).on("click", "#btnSubmit", function (index, value) {
    if (checkInsertShip == true) {
        checkInsertShip = false;
        var checkAll = false;
        var ship = {}
        ship.TenCongViec = $("#txtTenShipable").val();
        ship.DuAnId = $("#cboDuAn").val();
        ship.GiaiDoanDuAnId = $("#cboGiaiDoanDuAn").val();
        ship.MoTa = editor.value();
        ship.ThuTuUuTien = $("#cboDoUuTien").val();
        ship.StrNgayDuKienHoanThanh = $("#txtDeadLine").val();
        var trangThai1 = $("#cboTrangThai").val();
        var trangThai2 = $("#cboTrangThai2").val();
        if (trangThai2 == 0) {
            ship.TrangThaiCongViecId = trangThai1;
        } else {
            ship.TrangThaiCongViecId = trangThai2;
        }
        var value = $("#cboWeek").val();
        var col = value.split("-");
        ship.Tuan = col[0];
        ship.Nam = col[1];
        ship.NguoiXuLyId = $("#cboNguoiDung").val();
        ship.MaCongViec = $("#txtMaShipable").val();
        if (ship.TrangThaiCongViecId == 6 || ship.TrangThaiCongViecId == 12) {
            ship.XuLyVaoTuanTiepTheo = $("#chkXuLyINW")[0].checked;
        }
        var check = true;
        if ($.trim(ship.TenCongViec) == "" || ship.DuAnId == "0" || ship.GiaiDoanDuAnId == "0" || $.trim(ship.MoTa) == "") {
            check = false;
        }
        if (check) {
            if (shipableId == 0) {
              
                    $.ajax({
                        url: "/Shipable/AddShipable",
                        data: ship,
                        context: document.body,
                        type: "POST",
                        dataType: "html",
                        async:false,
                        success: function (data) {
                            if (data > 0) {
                                $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Thêm mới shipable thành công!");
                                checkAll = true;
                                setTimeout(
                                    function () {
                                        location.reload();
                                    }, 1000);
                            } else {                               
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
                ship.CongViecId = shipableId;
                $.ajax({
                    url: "/Shipable/UpdateShipable",
                    data: ship,
                    context: document.body,
                    type: "POST",
                    async: false,
                    dataType: "html",
                    success: function (data) {
                        if (data == "True" || data == "true") {
                            $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Chỉnh sửa shipable thành công!");
                            checkAll = true;
                            setTimeout(
                                function () {
                                    location.reload();
                                }, 1000);
                        } else {
                            $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Chỉnh sửa shipable không thành công!");
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
        else {
            $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Các trường không phải option không được để trống!");
        }
        if (checkAll == false) checkInsertShip = true;
    }
})
$(document).on("click", ".EditShipable", function () {
    var id = this.id;
    showEdit(id)
})
$(document).on("click", "#btnDelete", function () {
    if (confirm("Bạn có muốn xóa shipable này không?")) {
        $.ajax({
            url: "/Shipable/DeleteShipable?congViecId=" + shipableId,           
            context: document.body,
            type: "POST",
            dataType: "html",
            success: function (data) {
                if (data == "True" || data == "true" || data == true) {
                    $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Xóa shipable thành công!");
                    setTimeout(
                        function () {
                            location.reload();
                        }, 2000);      
                } else {
                    $.Notification.autoHideNotify('error', 'top right', 'Thành công', "Xóa shipable không thành công!");
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
$(document).on("change", "#cboTrangThai", function () {
    var id = $("#cboTrangThai").val();
    if (id == 3 || id == "3") {
        setSelectTrangThaiCongViec2(3, shipableId);
        $("#cboTrangThai2").val(0);
        $("#cboTrangThai2").show();
    } else {
        $("#cboTrangThai2").val(0);
        $("#cboTrangThai2").hide();
    }
})
$(document).on("change", "#cboTrangThai2", function () {
    var id = $("#cboTrangThai2").val();
    if (id == 6 || id == "6") {
        $("#divXuLy").show();
        $("#chkXuLyINW")[0].checked = false;
    } else {
        $("#divXuLy").hide();
    }  
})
function disableAllCommonInPop() {
    $("#cboWeek").prop("disabled", true);
    $("#cboDuAn").prop("disabled", true);
    $("#cboNguoiDung").prop("disabled", true);
    $("#txtDeadLine").prop("disabled", true);
    $("#cboGiaiDoanDuAn").prop("disabled", true);
    $("#txtTenShipable").prop("disabled", true);
    $("#cboDoUuTien").prop("disabled", true);
    $("#cboTrangThai").prop("disabled", true);
    $("#cboTrangThai2").prop("disabled", true);
    $("#txtGhiChu").pop("disable", true);
}
function noDisableAllCommonInPop() {
    $("#cboWeek").prop("disabled", false);
    $("#cboDuAn").prop("disabled", false);
    $("#cboNguoiDung").prop("disabled", false);
    $("#txtDeadLine").prop("disabled", false);
    $("#cboGiaiDoanDuAn").prop("disabled", false);
    $("#txtTenShipable").prop("disabled", false);
    $("#cboDoUuTien").prop("disabled", false);
    $("#cboTrangThai").prop("disabled", false);
    $("#cboTrangThai2").prop("disabled", false);
    $("#divDelete").show();
    $("#btnSubmit").show();
    //$("#txtGhiChu").pop("disable", false);

    $("#divXuLy").hide();
}
function setzIndex(index) {
    $("#overlay").css("z-index", index);
}

$(document).on("click", ".deleteShipableT", function () {
    var id = this.id;
    if (confirm("bạn có muốn xóa shipable này không?")) {
        $.ajax({
            url: "/Shipable/DeleteShipable?congViecId=" + id,
            context: document.body,
            type: "POST",
            dataType: "html",
            success: function (data) {
                if (data == "True" || data == "true" || data == true) {
                    $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Xóa shipable thành công!");
                    setTimeout(
                        function () {
                            location.reload();
                        }, 1000);
                } else {
                    $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Xóa shipable không thành công!");
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
function showEdit(shipid) {
    noDisableAllCommonInPop();
    $.ajax({
        url: "/Shipable/GetById?congViecId=" + shipid,
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            if (data != "") {
                var result = JSON.parse(data);
                $("#custom-width-modalLabel").text("Chỉnh sửa ship-able");
                $("#txtMaShipable").val(result.MaCongViec);
                $("#cboDuAn").val(result.DuAnId);
                selectDuAn(result.DuAnId);
                if (result.HoTenNguoiXuLy != "")
                    $("#cboNguoiDung").val(result.NguoiXuLyId);
                else $("#cboNguoiDung").val("");
                $("#txtDeadLine").val(result.StrNgayDuKienHoanThanhFull);
                $("#cboGiaiDoanDuAn").val(result.GiaiDoanDuAnId);
                $("#txtTenShipable").val(result.TenCongViec);
                editor.value(result.MoTa);
                setSelectWeek(result.Tuan,result.Nam);
                //$("#cboWeek").val(result.Tuan);
                $("#cboDoUuTien").val(result.ThuTuUuTien);
                if (result.TrangThaiCongViecId == 3) {
                    setSelectTrangThaiCongViec(result.TrangThaiCongViecId);
                    $("#cboTrangThai").val(3);
                    $("#cboWeek").prop("disabled", true);
                    $("#cboTrangThai2").show();
                    setSelectTrangThaiCongViec2(result.TrangThaiCongViecId, result.CongViecId);
                    $("#cboTrangThai2").val(0);
                    $("#lblTrangThaiPlan").hide();
                } else {
                    if (result.TrangThaiCongViec.KhoaChaId == 3) {
                        setSelectTrangThaiCongViec(3);
                        setSelectTrangThaiCongViec2(3, result.CongViecId);
                        $("#divDelete").hide();
                        if (result.TrangThaiCongViecId == 12) {
                            $("#cboTrangThai2").prop("disabled", false);
                        } else {
                            $("#cboTrangThai2").prop("disabled", true);
                            $("#txtDeadLine").prop("disabled", true);
                            $("#cboDoUuTien").prop("disabled", true);
                            if (result.TrangThaiCongViecId == 4) $("#btnSubmit").hide();
                        }
                        $("#cboWeek").prop("disabled", true);
                        $("#cboDuAn").prop("disabled", true);
                        $("#cboNguoiDung").prop("disabled", true);
                        $("#cboGiaiDoanDuAn").prop("disabled", true);
                        $("#txtTenShipable").prop("disabled", true);

                        $("#cboTrangThai").prop("disabled", true);
                        $("#cboTrangThai").val(3);
                        $("#cboTrangThai2").val(result.TrangThaiCongViecId);
                        $("#cboTrangThai2").show();
                        $("#lblTrangThaiPlan").hide();

                    } else {
                        $("#cboWeek").prop("disabled", false);
                        if (result.TrangThaiCongViecId == 1) {
                            if (!(result.CongViecs.length > 0)) {
                                setSelectTrangThaiCongViec(0);
                                $("#lblTrangThaiPlan").text("Bạn chưa có công việc nào gắn với shipable này.")
                                $("#lblTrangThaiPlan").show();
                            } else {
                                setSelectTrangThaiCongViec(result.TrangThaiCongViecId);
                                $("#cboTrangThai").val(result.TrangThaiCongViecId);
                            }

                        } else {
                            setSelectTrangThaiCongViec(result.TrangThaiCongViecId);
                            $("#cboTrangThai").val(result.TrangThaiCongViecId);
                        }

                        $("#cboTrangThai2").hide();
                        $("#cboTrangThai2").val(0);
                    }
                }

                if (result.TrangThaiCongViecId == 6) {
                    $("#divXuLy").show();
                    if (result.XuLyVaoTuanTiepTheo) {
                        $("#btnSubmit").hide();
                        $("#chkXuLyINW")[0].checked = true;
                        $("#chkXuLyINW").prop("disabled", false);
                    } else {
                        $("#chkXuLyINW")[0].checked = false;
                    }
                } else {
                    $("#divXuLy").hide();
                }
                if (result.TrangThaiCongViecId == 12) {
                    $("#divXuLy").show();                    
                        $("#chkXuLyINW")[0].checked = false;
                   
                } else {
                    $("#divXuLy").hide();
                }


                $("#btnDelete").show();
                $("#addShipModal").modal("show");

                shipableId = shipid;
            }

        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
}