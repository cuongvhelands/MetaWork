var congViecId = 0;
$(document).on("click", ".shipable", function () {
    var id = this.id;
    $("#mdCongViec").modal();
    var nguoiDungData = getDataNguoiDung(id);
    $("#cboShipable").data("kendoDropDownList").value(id);
    var mota = getMota(id);
    $("#txtMoTa").val(mota);
    var nguoiDung = $("#cboNguoiDung").data("kendoDropDownList");
    nguoiDung.setDataSource(nguoiDungData)
    congViecId = 0;
})
function getMota(shipableId) {
    var result;
    $.ajax({
        url: "/Shipable/GetMota?congViecId=" + shipableId,
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            result = data;
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
    return result;
}
$(document).ready(function () {
    $('#txtDeadline').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy'
    })
  
    var thuTuUuTienData = getDataDoUuTien();
    $("#cboDoUuTien").kendoDropDownList({
        dataTextField: "TenThuTuUuTien",
        dataValueField: "ThuTuUuTien",
        filter: "contains",
        dataSource: thuTuUuTienData
    })   
    var DoPhucTapData = getDataDoPhucTap();
    $("#cboDoPhucTap").kendoDropDownList({
        dataTextField: "TenDoPhucTap",
        dataValueField: "DoPhucTap",
        filter: "contains",
        dataSource: DoPhucTapData
    })   
    var trangThaiCongViecData = getDataTrangThaiCongViec();
    $("#cboTrangThai").kendoDropDownList({
        dataTextField: "TenTrangThai",
        dataValueField: "TrangThaiCongViecId",
        filter: "contains",
        dataSource: trangThaiCongViecData
    })
    //var nguoiDungData = getDataNguoiDung();
    $("#cboNguoiDung").kendoDropDownList({
        dataTextField: "HoTen",
        dataValueField: "NguoiDungId",
        filter: "contains",
        //dataSource: nguoiDungData
    })
    var shipableData = getDataShipable();
    $("#cboShipable").kendoDropDownList({
        dataTextField: "TenCongViec",
        dataValueField: "CongViecId",
        filter: "contains",
        dataSource: shipableData,
        select: onSelectShipable
    })
});
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
function getDataDoPhucTap() {
    var result;
    $.ajax({
        url: "/Shipable/GetDoPhucTaps",
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
        url: "/Shipable/GetTrangThaiCongViecs",
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
function getDataNguoiDung(id) {
    var result;
    $.ajax({
        url: "/Shipable/GetNguoiDungsShipable?congViecId="+id,
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
function getDataShipable() {
    var tuan = $("#tuanM").val();
    var result;
    $.ajax({
        url: "/Todo/GetShipablesBy?tuan="+tuan,
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
function onSelectShipable(e) {
    var id = e.dataItem.CongViecId;
    var mota = getMota(id);
    $("#txtMoTa").val(mota);
    var nguoiDungData = getDataNguoiDung(id);
    var nguoiDung = $("#cboNguoiDung").data("kendoDropDownList");
    nguoiDung.setDataSource(nguoiDungData)

}


$(document).on("click", "#btnSubmit", function () {
    
    var congViec = {}
    congViec.CongViecId = congViecId;
    congViec.TenCongViec = $("#txtTenCongViec").val();
    congViec.KhoaChaId = $("#cboShipable").data("kendoDropDownList").value();
    congViec.NguoiXuLyId = $("#cboNguoiDung").data("kendoDropDownList").value();
    congViec.ThuTuUuTien = $("#cboDoUuTien").data("kendoDropDownList").value();
    congViec.DoPhucTap = $("#cboDoPhucTap").data("kendoDropDownList").value();  
    congViec.StrNgayDuKienHoanThanh = $("#txtDeadline").val();
    congViec.TrangThaiCongViecId = $("#cboTrangThai").data("kendoDropDownList").value();   
    $.ajax({
        url: "/ToDo/AddToDo",
        data: ship,
        context: document.body,
        type: "POST",
        dataType: "html",
        success: function (data) {
            if (data > 0) {
                var url = "/project/index";
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