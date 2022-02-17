$(document).ready(function () {
    $('#txtDeadLine').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy'
    })
 
    var duAnData = getDuAns();
    var duAn= $("#cboDuAn").kendoDropDownList({
        dataTextField: "TenDuAn",
        dataValueField: "DuAnId",
        filter: "contains",
        dataSource: duAnData,
        select: onSelectDuAn,
    }).data("kendoDropDownList");
    var duAnId = duAn.value();
    var giaiDoanDuAnData = getDataGiaiDoanDuAn(duAnId);
    $("#cboGiaiDoanDuAn").kendoDropDownList({
        dataTextField: "TenGiaiDoan",
        dataValueField: "GiaiDoanDuAnId",
        filter: "contains",
        dataSource: giaiDoanDuAnData
    });
    var thuTuUuTienData = getDataDoUuTien();
    $("#cboDoUuTien").kendoDropDownList({
        dataTextField: "TenThuTuUuTien",
        dataValueField: "ThuTuUuTien",
        filter: "contains",
        dataSource: thuTuUuTienData   
    })
    var weekData = getDataWeek();
    $("#cboWeek").kendoDropDownList({
        dataTextField: "TenWeek",
        dataValueField: "Week",
        filter: "contains",
        dataSource: weekData
    })
    var trangThaiCongViecData = getDataTrangThaiCongViec();
    $("#cboTrangThai").kendoDropDownList({
        dataTextField: "TenTrangThai",
        dataValueField: "TrangThaiCongViecId",
        filter: "contains",
        dataSource: trangThaiCongViecData
    })
    var nguoiDungData = getDataNguoiDung();
    $("#cboNguoiDung").kendoDropDownList({
        dataTextField: "HoTen",
        dataValueField: "NguoiDungId",
        filter: "contains",
        dataSource: nguoiDungData
    })
});
function getDuAns() {
    var result;
    $.ajax({
        url: "/Shipable/GetDuAns",
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
function onSelectDuAn(e) {
    var duAnId = e.dataItem.DuAnId;
    var giaiDoanDuAn = $("#cboGiaiDoanDuAn").data("kendoDropDownList");
    var dataSource = getDataGiaiDoanDuAn(duAnId);
    giaiDoanDuAn.setDataSource(dataSource)
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
function getDataWeek() {
    var result;
    $.ajax({
        url: "/Shipable/GetWeeks",
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
$(document).on("click", "#btnSubmitShipable", function () {
    var ship = {}
    ship.TenCongViec = $("#txtTenShipable").val();
    ship.DuAnId = $("#cboDuAn").data("kendoDropDownList").value();
    ship.GiaiDoanDuAnId = $("#cboGiaiDoanDuAn").data("kendoDropDownList").value(); 
    ship.MoTa = $("#txtGhiChu").val();
    ship.ThuTuUuTien = $("#cboDoUuTien").data("kendoDropDownList").value();
    ship.StrNgayDuKienHoanThanh = $("#txtDeadLine").val(); 
    ship.TrangThaiCongViecId = $("#cboTrangThai").data("kendoDropDownList").value();
    ship.Tuan = $("#cboWeek").data("kendoDropDownList").value();
    ship.NguoiXuLyId = $("#cboNguoiDung").data("kendoDropDownList").value();
    ship.MaCongViec = $("#txtMaShipable").val();
    $.ajax({
        url: "/Shipable/AddShipable",
        data: ship,
        context: document.body,
        type: "POST",
        dataType: "html",
        success: function (data) {
            if (data > 0) {
                var url = "/shipable/index";
                alert("Thêm mới shipable thành công");
                window.location.href = url;
            }

        },
        error: function (xhr, status) {
        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
})
