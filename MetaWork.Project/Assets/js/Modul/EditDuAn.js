var categoryId, editor;
var duAnId;
$(document).ready(function () {
    $('#datetimepicker1').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        todayHighlight: true
    })
    var loaiCongViecData = getLoaiCongViecs();
    // create MultiSelect from select HTML element
    $("#loaiCongViecList").kendoMultiSelect({
        dataTextField: "TenLoaiCongViec",
        dataValueField: "LoaiCongViecId",
        dataSource: loaiCongViecData,
        select: onSelectLoaiCongViec,
        deselect: onDeselectLoaiCongViec
    });
    var strLoaiCongViecIds = $("#loaiCongViecIdsM").val();
    if (strLoaiCongViecIds != "") {
        var loaiCongViecIds = JSON.parse(strLoaiCongViecIds);
        $("#loaiCongViecList").data("kendoMultiSelect").value(loaiCongViecIds);
    }
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
    categoryId = $("#duAnIdM").val();
    loadDuAnThanhPhan();
    var phongBanData = getPhongBan();
    var phongBan = $("#cboNhomThanhVien").kendoMultiSelect({
        dataTextField: "TenPhongBan",
        dataValueField: "PhongBanId",
        filter: "contains",
        dataSource: phongBanData,
        select: onSelectPhongBan,
        deselect: onDeselectPhongBan
    }).data("kendoMultiSelect");
    var strPhongBan = $("#strPhongBanIds").val();
    if (strPhongBan != "") {       
        var phongBanIds = JSON.parse(strPhongBan);
        $("#cboNhomThanhVien").data("kendoMultiSelect").value(phongBanIds);
    }
})
function onSelectPhongBan(e) {
    var phongBanId = e.dataItem.PhongBanId;
    var data = { DuAnId: categoryId, PhongBanId: phongBanId }
    $.ajax({
        url: "/Project/InsertLienKetDuAnPhongBan",
        data: data,
        context: document.body,
        type: "POST",
        dataType: "html",
        success: function (data) {

        },
        error: function (xhr, status) {
        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
}
function onDeselectPhongBan(e) {
    var phongBanId = e.dataItem.PhongBanId;
    var data = { DuAnId: categoryId, PhongBanId: phongBanId }
    $.ajax({
        url: "/Project/DeleteLienKetDuAnPhongBan",
        data: data,
        context: document.body,
        type: "DELETE",
        dataType: "html",
        success: function (data) {

        },
        error: function (xhr, status) {
        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
}
function getPhongBan() {
    var result;
    $.ajax({
        url: "/Project/GetPhongBans",
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
function getLoaiCongViecs() {
    var result;
    $.ajax({
        url: "/Project/GetLoaiCongViecs",
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
function loadDuAnThanhPhan() {
    $.ajax({
        url: "/Project/PartialViewDuAnThanhPhan?duAnId=" + categoryId,
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            $("#divDuAnThanhPhan").html(data);
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
}
function onSelectLoaiCongViec(e) {
    var loaiCongViecId = e.dataItem.LoaiCongViecId;
    var data = { DuAnId: categoryId, LoaiCongViecId: loaiCongViecId }
    $.ajax({
        url: "/Project/InsertLienKetLoaiCongViecDuAn",
        data: data,
        context: document.body,
        type: "POST",
        dataType: "html",
        success: function (data) {

        },
        error: function (xhr, status) {
        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
}
function onDeselectLoaiCongViec(e) {
    var loaiCongViecId = e.dataItem.LoaiCongViecId;
    data = { DuAnId: categoryId, LoaiCongViecId: loaiCongViecId }
    $.ajax({
        url: "/Project/DeleteLienKetLoaiCongViecDuAn",
        data: data,
        context: document.body,
        type: "DELETE",
        dataType: "html",
        success: function (data) {

        },
        error: function (xhr, status) {
        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
}
function openAddLoaiCongViecModal() {
    $("#txtTenLoaiCongViec").val("");  
    $("#addLoaiCongViecModal").modal('show');
}
$(document).on("click", "#btnInsetLoaiCongViec", function () {
    var tenLoaiCongViec = $("#txtTenLoaiCongViec").val();  
    if ($.trim(tenLoaiCongViec) == "") {
        $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Tên loại công việc không được để trống!");
    } else {
        data = { TenLoaiCongViec: tenLoaiCongViec }
        $.ajax({
            url: "/Project/InsertLoaiCongViec",
            data: data,
            context: document.body,
            type: "POST",
            dataType: "html",
            success: function (data) {
                if (data > 0) {
                    var loaiCongViec = $("#loaiCongViecList").data("kendoMultiSelect");
                    var listId = loaiCongViec.value();
                    if (listId.length > 0) listId.push(data);
                    else listId = [data];
                    var dataLoaiCongViec = getLoaiCongViecs();
                    loaiCongViec.setDataSource(dataLoaiCongViec);
                    loaiCongViec.value(listId);
                    $("#addLoaiCongViecModal").modal('hide');
                } else {
                    $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Error!");
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
$(document).on("click", ".clsDuAnTP", function () {
    duAnId = this.id;
    $.ajax({
        url: "/Project/GetProjectInfoById?duAnId=" + duAnId,
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            var duAn = JSON.parse(data);
            if (duAn && duAn.DuAnId > 0) {
                $("#txtDBDATP").val(duAn.LinkCoda);
                $("#txtTenDuAnCha").val(duAn.TenKhoaCha);
                $("#txtTenDuAnThanhPhan").val(duAn.TenDuAn);
                $("#txtCost").val(duAn.StrCost);      
                $("#txtCostHTp").val(duAn.StrCostH);  
                $("#txtCostTienTp").val(duAn.StrCostTien);  

                $("#txtMoTaDuAnTP").html(duAn.MoTa);
                $("#txtGhiChuDuAnTP").html(duAn.GhiChu);
                $("#txtTongNganSach").val(duAn.StrTongNganSach);
                var str = "";
                if (duAn.CongViecs && duAn.CongViecs.length > 0) {
                    $.each(duAn.CongViecs, function (index, value) {
                        str += '<tr><td>' + value.TenCongViec + '</td><td> <div class="label label-table font-13 p-r-20 label-' + value.MaMauTrangThaiCongViec + '" width="100 % ">' + value.TenTrangThai + '</div></td></tr>';
                    })
                } else {
                    str ='<tr><td colspan="2">Dự án hiện chưa có đầu mục công việc nào</td></tr>'
                }
                $("#tbdyShipables").html(str);
            }
            $("#add-phase-modal").modal("show");
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
})
function reloadTableDATP() {
    $.ajax({
        url: "/Project/GetProjectInfoById?duAnId=" + duAnId,
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            var duAn = JSON.parse(data);
            if (duAn && duAn.DuAnId > 0) {
                $("#txtDBDATP").val(duAn.LinkCoda);
                $("#txtTenDuAnCha").val(duAn.TenKhoaCha);
                $("#txtTenDuAnThanhPhan").val(duAn.TenDuAn);
                $("#txtCostH").val(duAn.CostH);
                $("#txtCostTien").val(duAn.CostTien);
                $("#txtMoTaDuAnTP").html(duAn.MoTa);
                $("#txtGhiChuDuAnTP").html(duAn.GhiChu);
                $("#txtTongNganSach").val(duAn.TongNganSach);
                var str = "";
                if (duAn.CongViecs && duAn.CongViecs.length > 0) {
                    $.each(duAn.CongViecs, function (index, value) {
                        str += '<tr><td>' + value.TenCongViec + '</td><td> <div class="label label-table font-13 p-r-20 label-' + value.MaMauTrangThaiCongViec + '" width="100 % ">' + value.TenTrangThai + '</div></td></tr>';
                    })
                } else {
                    str = '<tr><td colspan="2">Dự án hiện chưa có đầu mục công việc nào</td></tr>'
                }
                $("#tbdyShipables").html(str);
            }          
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
}
$(document).on("click", "#btnCodaDBDATP", function () {
    var link = $("#txtDBDATP").val();
    $("#add-phase-modal").modal("hide");
    $('#loadding').modal({ backdrop: 'static', keyboard: false });
    $.ajax({
        url: "/Project/GetShipablesFromDuAnThanhPhan?duAnId=" + duAnId + "&&linkCoda=" + link,
        context: document.body,
        type: "GET",
        dataType: "html",      
        success: function (data) {
            $("#txtLinkDBDMCVDATP").val(link);
            var str = "";
            var cvs = JSON.parse(data);
            if (cvs && cvs.length > 0) {

                $.each(cvs, function (index, value) {
                    if (value.IsAddNew || value.IsUpdate) {
                        str += '<tr><td> <input class="chkInsertDMCV"  checked type="checkbox"></td><td>';
                        if (value.IsAddNew)
                            str += 'Thêm mới';
                        else str+=' Cập nhật'
                        str += '</td><td>' + value.TenCongViec + '</td><td>' + value.HoTen + '</td></tr>';     
                    } 
                                      
                })
            } else {
                str = '<tr><td colspan="2">Dự án hiện chưa có thêm đầu mục công việc mới nào</td></tr>'
            }
            $("#tbyDBDMCVDATP").html(str);
            $("#DBDMDATPModal").modal("show");
            $('#loadding').modal("hide");
            
        },
        error: function (xhr, status) {
            $('#loadding').modal("hide");
            $("#add-phase-modal").modal("show");
        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
   
})
$(document).on("click", ".btnSubmitDBDMDATP", function () {
    $("#DBDMDATPModal").modal("hide");    
    if ($("#tbyDBDMCVDATP").length > 0) {
        var rows = $("tbody#tbyDBDMCVDATP").find("tr");
        var lstCV = []
        if (rows && rows.length > 0) {
            $.each(rows, function (index, value) {               
                var chks = $(".chkInsertDMCV");
                if (chks.length > 0) {
                    $.each(chks, function (index, value) {
                        if (value.checked == true) {
                            var cv = {}
                            cv.TenCongViec = value.parentElement.parentElement.cells[2].textContent;
                            cv.HoTen = value.parentElement.parentElement.cells[3].textContent;
                            cv.DuAnId = duAnId;
                            lstCV.push(cv);
                        }
                    });
                }
              

            })
        }
        if (lstCV.length > 0) {
            $.ajax({
                url: "/Project/AddListShipableFromCoda",
                data: { CongViecs: lstCV, DuAnId: duAnId},
                context: document.body,
                type: "POST",
                dataType: "html",
                async: false,
                success: function (data) {             
                        $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Thêm mới shipable thành công!");
                        reloadTableDATP();
                        $("#add-phase-modal").modal("show");
                },
                error: function (xhr, status) {

                },
                complete: function (xhr, status) {
                    //$('#showresults').slideDown('slow')                 
                }
            });
        } else {
            $("#add-phase-modal").modal("show");
        }
    } 
})

$('#DBDMDATPModal').on('hidden.bs.modal', function () {
    $("#add-phase-modal").modal("show");
});
$("#btnNotUpdateDuAn").click(function () {
    location.href="/Project/Index";
})
// đồng bộ đầu mục công việc dự án
$("#btnCodaDBALLDATP").click(function () {
    $("#DBAllDMDATPModal").modal("show");   
    $("#tbyDBAllDMCVDATP").html("");

  
})
$("#btnCodaDBAllDMCVDATP").click(function () {
    var link = $("#txtLinkDBAllDMCVDATP").val();
    $("#DBAllDMDATPModal").modal("hide");
    $('#loadding').modal({ backdrop: 'static', keyboard: false });
    $.ajax({
        url: "/Project/GetShipablesFromDuAn?duAnId=" + categoryId + "&&linkCoda=" + link,
        context: document.body,
        type: "GET",
        dataType: "html",
        success: function (data) {
            var str = "";
            if (data == "") {

            } else {
                var duAns = JSON.parse(data);
                if (duAns && duAns.length > 0) {
                    $.each(duAns, function (index, duAn) {
                        if (duAn.CongViecs && duAn.CongViecs.length > 0) {
                            var rowspan = duAn.Rowspan;
                            var i = 0;
                            $.each(duAn.CongViecs, function (index2, value) {
                                if (value.IsAddNew) {
                                    str += '<tr><td> <input class="chkInsertAllDMCV" id="' + duAn.DuAnId + '"  checked type="checkbox"></td><td>Thêm mới</td>';
                                    if (i == 0) {
                                        if (rowspan > 1) {
                                            str += '<td rowspan="' + rowspan + '">' + duAn.TenDuAn + '</td>';
                                        } else {
                                            str += '<td >' + duAn.TenDuAn + '</td>';
                                        }
                                        
                                    }
                                    str += '<td>' + value.TenCongViec + '</td><td>' + value.HoTen + '</td></tr>';
                                    i++;
                                }
                            })
                        }
                    })
                }
              
            }
            if (str == "") {
                str = '<tr><td colspan="3">Dự án hiện chưa có thêm đầu mục công việc mới nào</td></tr>'
            }
            $("#tbyDBAllDMCVDATP").html(str);
            $("#DBAllDMDATPModal").modal("show");
            $('#loadding').modal("hide");
            $("#chkAllADMCV")[0].checked=true;
        },
        error: function (xhr, status) {
            $('#loadding').modal("hide");
            $("#DBAllDMDATPModal").modal("show");
        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
})

$(document).on("click", ".btnSubmitADBDMDATP", function () {
    /*$("#DBAllDMDATPModal").modal("hide");*/
    if ($("#tbyDBAllDMCVDATP").length > 0) {
        var rows = $("tbody#tbyDBAllDMCVDATP").find("tr");
        var lstCV = []
        if (rows && rows.length > 0) {
         
                var chks = $(".chkInsertAllDMCV");
                if (chks.length > 0) {
                    $.each(chks, function (index, value) {
                        if (value.checked == true) {
                            var cv = {}
                            var cell = value.parentElement.parentElement.cells;
                            if (cell.length > 4) {
                                cv.TenCongViec = value.parentElement.parentElement.cells[3].textContent;
                                cv.HoTen = value.parentElement.parentElement.cells[4].textContent;
                            } else {
                                cv.TenCongViec = value.parentElement.parentElement.cells[2].textContent;
                                cv.HoTen = value.parentElement.parentElement.cells[3].textContent;
                            }
                            cv.DuAnId = value.id;
                            lstCV.push(cv);
                        }
                    });
                }


            }
        
        if (lstCV.length > 0) {
            $.ajax({
                url: "/Project/AddListShipableFromCoda",
                data: { CongViecs: lstCV, DuAnId: duAnId },
                context: document.body,
                type: "POST",
                dataType: "html",
                async: false,
                success: function (data) {
                    $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Thêm mới shipable thành công!");               
                    $("#DBAllDMDATPModal").modal("hide");
              
                },
                error: function (xhr, status) {

                },
                complete: function (xhr, status) {
                    //$('#showresults').slideDown('slow')                 
                }
            });
        } else {
            $("#DBAllDMDATPModal").modal("hide");



        }
    }
})
$("#chkAllADMCV").change(function () {
    var check = $("#chkAllADMCV")[0].checked;
   
    var lstCheck = $(".chkInsertAllDMCV");
    if (lstCheck != null && lstCheck.length > 0) {
        $.each(lstCheck, function (index, value) {
            value.checked = check;
        })
    }
})
$("#chkAllDMCV").change(function () {
    var check = $("#chkAllADMCV")[0].checked;

    var lstCheck = $(".chkInsertDMCV");
    if (lstCheck != null && lstCheck.length > 0) {
        $.each(lstCheck, function (index, value) {
            value.checked = check;
        })
    }
})