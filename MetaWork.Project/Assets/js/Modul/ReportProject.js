var duAnIdM = $("#duAnIdM").val();
var nguoiDungIdM = $("#nguoiDungIdM").val();
var startDateM = $("#startDateM").val();
var endDateM = $("#endDateM").val();
$(document).ready(function () {
    var duAnData = getDuAns();
    var duAn = $("#cboDuAn1").kendoDropDownList({
        dataTextField: "TenDuAn",
        dataValueField: "DuAnId",
        filter: "contains",
        dataSource: duAnData,
        select: onSelectDuAn
    }).data("kendoDropDownList");
    duAn.value(duAnIdM);   
    var nguoiDungKDD = getDataNguoiDung2(0);
    var nguoiDung = $("#cboNguoiDung1").kendoDropDownList({
        dataTextField: "HoTen",
        dataValueField: "NguoiDungId",
        filter: "contains",
        dataSource: nguoiDungKDD,
        select: onSelectNguoiDung
    }).data("kendoDropDownList");
    nguoiDung.value(nguoiDungIdM);
    $('#datetimepicker2').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        todayHighlight: true        
    })
    $('#datetimepicker3').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        todayHighlight: true
    })
    $("#txtFrom").val(startDateM);
    $("#txtTo").val(endDateM);
});
$(document).on("click", ".popDuAnTimeReport", function () {
    var id = this.id;
   
        $.ajax({
            url: "/Report/PartialViewReportDuAn?duAnId=" + id + "&&strStartDate=" + startDateM + "&&strEndDate=" + endDateM,           
            async: false,
            context: document.body,
            type: "GET",
            dataType: "html",
            success: function (data) {               
                $("#bdyReportProject").html(data);
                $("#ReportProjectModal").modal("show");
            },
            error: function (xhr, status) {

            },
            complete: function (xhr, status) {
                //$('#showresults').slideDown('slow')
            }
        });
})
$(document).on("click", ".popUserTimeReport", function () {
    var id = this.id;
    $.ajax({
        url: "/Report/PartialViewReportUser?nguoiDungId=" + id + "&&strStartDate=" + startDateM + "&&strEndDate=" + endDateM,
        async: false,
        context: document.body,
        type: "GET",
        dataType: "html",
        success: function (data) {
            $("#bdyReportUser").html(data);
            $("#ReportUserSpentModal").modal("show");
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
})
$(document).on("click", ".popUserProjectTimeReport", function () {
    var id = this.id;
    var col = id.split('-');
    var duAnId = col[0];
    var nguoiDungId = id.substring(duAnId.length + 1);
   
    $.ajax({
        url: "/Report/PartialViewReportUserProject?nguoiDungId=" + nguoiDungId + "&&duAnId=" + duAnId+ "&&strStartDate=" + startDateM + "&&strEndDate=" + endDateM,
        async: false,
        context: document.body,
        type: "GET",
        dataType: "html",
        success: function (data) {
            $("#bdyReportUserProject").html(data);
            $("#ReportUserSpentInProjectModal").modal("show");
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
})
$(document).on("click", "#btnSearch", function () {
    var startDate = $("#txtFrom").val();
    var endDate = $("#txtTo").val();
    var url = "/Report/ReportProject?duAnId=" + duAnIdM + "&&nguoiDungId=" + nguoiDungIdM + "&&strStartDate=" + startDate + "&&strEndDate=" + endDate;
    window.location.href = url;
})
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
function onSelectDuAn(e) {
    var duAnId = e.dataItem.DuAnId;
    var url = "/Report/ReportProject?duAnId=" + duAnId + "&&nguoiDungId=" + nguoiDungIdM + "&&strStartDate=" + startDateM + "&&strEndDate=" + endDateM;
    window.location.href = url;
}
function onSelectNguoiDung(e) {  
    var nguoiDungId = e.dataItem.NguoiDungId;  
    var url = "/Report/ReportProject?duAnId=" + duAnIdM + "&&nguoiDungId=" + nguoiDungId + "&&strStartDate=" + startDateM + "&&strEndDate=" + endDateM;
    window.location.href = url;
}
