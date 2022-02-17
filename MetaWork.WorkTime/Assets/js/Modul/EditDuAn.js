var duAnId = $("#duAnIdM").val();
var lstUserAccess = [], check, rowEdit, editor, editorpop, newRowHM = -1;
var now = $("#now").val();
$(document).ready(function () {
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
    $('#datetimepicker3').datepicker({
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
    var dataKhachHang = getDataKhachHang()
    $("#cboKhachHang").kendoDropDownList({
        dataTextField: "TenKhachHang",
        dataValueField: "KhachHangId",
        filter: "contains",
        dataSource: dataKhachHang
    });
    var khachHangId = $("#khachHangIdM").val();
    if (khachHangId != "") {
        $("#cboKhachHang").data("kendoDropDownList").value(khachHangId);
    }
    var dataNguoiDung = getDataNguoiDung();
    $("#cboNguoiDung").kendoDropDownList({
        dataTextField: "HoTen",
        dataValueField: "NguoiDungId",
        filter: "contains",
        dataSource: dataNguoiDung,
        select: onSelectNguoiDung
    });
    var trangThaiDuAnData = getDataTrangThaiDuAn();
    $("#cboTrangThai").kendoDropDownList({
        dataTextField: "TenTrangThaiDuAn",
        dataValueField: "TrangThaiDuAnId",
        filter: "contains",
        dataSource: trangThaiDuAnData
    })
    var trangThaiDuAnId = $("#trangThaiDuAnIdM").val();
    $("#cboTrangThai").data("kendoDropDownList").value(trangThaiDuAnId);
    var dataLoaiNganSach = getDataLoaiNganSach()
    $("#cboLoaiNganSach").kendoDropDownList({
        dataTextField: "TenLoaiNganSach",
        dataValueField: "LoaiNganSachId",
        filter: "contains",
        dataSource: dataLoaiNganSach
    });
    var loaiNganSachId = $("#loaiNganSachIdM").val();
    $("#cboLoaiNganSach").data("kendoDropDownList").value(loaiNganSachId);
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
    $("#txtMoTaGiaiDoanDuAn").kendoEditor({
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
    editorpop = $("#txtMoTaGiaiDoanDuAn").data("kendoEditor");
    getDataLoaiGiaiDoan();
    var date123 = $("#txtFromTimeDuAn").val();
    if ($.trim(date123) == "") {
        $("#txtFromTimeDuAn").val(now);
    }
    var nganSach = $("#tongNganSachM").val();
    $("#txtTongNhanSach").val(nganSach);
    $(":input").inputmask();
});
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
function getDataKhachHang() {
    var result;
    $.ajax({
        url: "/Project/GetKhachHangs",
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
function onSelectLoaiCongViec(e) {
    var loaiCongViecId = e.dataItem.LoaiCongViecId;
    var data = { DuAnId:duAnId, LoaiCongViecId:loaiCongViecId }
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
function getDataLoaiNganSach() {
    var result;
    $.ajax({
        url: "/Project/GetLoaiNganSachs",
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
function onDeselectLoaiCongViec(e) {
    var loaiCongViecId = e.dataItem.LoaiCongViecId;
    data = { DuAnId:duAnId, LoaiCongViecId:loaiCongViecId }
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
function getDataTrangThaiDuAn() {
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
function getDataLoaiGiaiDoan() {
    $.ajax({
        url: "/Project/GetLoaiGiaiDoans",
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            result = JSON.parse(data);
            if (result.length > 0) {
                $.each(result, function (index, value) {
                    $("#cboLoaiGiaiDoan").append('<option value=' + value.LoaiGiaiDoanDuAnId + ' id="'+value.MaMau+'">' + value.TenLoaiGiaiDoan + '</option>')
                })
            }
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });

}
$(document).on("click", "#btnUpdateDuAn", function () {
    var DuAn = {}
    DuAn.QuanTam = $("#chkQuanTam")[0].checked;
    DuAn.DuAnId = duAnId;
    DuAn.KhachHangId = $("#cboKhachHang").data("kendoDropDownList").value();
    DuAn.TenDuAn = $("#txtTenDuAn").val();
    DuAn.MaDuAn = $("#txtMaDuAn").val();
    DuAn.StrNgayBatDau = $("#txtFromTimeDuAn").val();
    DuAn.MoTa = editor.value();
    DuAn.TongNganSach = $("#txtTongNhanSach").val();
    while (DuAn.TongNganSach.indexOf(",") != -1) {
        DuAn.TongNganSach = DuAn.TongNganSach.replace(",", "");
    }    
    DuAn.LoaiNganSachId = $("#cboLoaiNganSach").data("kendoDropDownList").value();  
    DuAn.TrangThaiDuAnId = $("#cboTrangThai").data("kendoDropDownList").value();
    if ($.trim(DuAn.TenDuAn) != "" && $.trim(DuAn.MaDuAn) != "" && $.trim(DuAn.MoTa) != "" && ((DuAn.LoaiNganSachId != "3" && $.trim(DuAn.TongNganSach) != "") || DuAn.LoaiNganSachId == "3") && DuAn.KhachHangId != "0") {
        $.ajax({
            url: "/Project/UpdateDuAn",
            data: DuAn,
            context: document.body,
            type: "POST",
            dataType: "html",
            success: function (data) {
                if (data == "True" || data == "true") {
                    var url = "/project/index";
                    $.Notification.autoHideNotify('success', 'top right', 'Thông báo', "Chỉnh sửa dự án thành công!");
                    setTimeout(
                        function () {
                            window.location.href = url;;
                        }, 2000);

                }

            },
            error: function (xhr, status) {
            },
            complete: function (xhr, status) {
                //$('#showresults').slideDown('slow')
            }
        });
    } else {
        $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Các trường không phải option không được để trống!");
    }
})
$(document).on("click", ".deleteAccess", function () {
    if (confirm("Bạn có muốn xóa liên kết người dùng này với dự án!") == true) {
        var userId = this.id;
        $('#trTableAccess-' + userId).remove();      
        data = { DuAnId: duAnId, NguoiDungId: userId }
        $.ajax({
            url: "/Project/DeleteLienKetNguoiDungDuAn",
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
  
})
$(document).on("click", "#btnAddPermission", function () {
    var dropdownlist = $("#cboNguoiDung").data("kendoDropDownList");
    var userId = dropdownlist.value();
    if (userId != "00000000-0000-0000-0000-000000000000") {
        var fullName = dropdownlist.text();
        var checkLaQuanLy = $("#chkPQNDA")[0].checked;
        var check = true;
        for (var i = 0; i < lstUserAccess.length; i++) {
            var obj = lstUserAccess[i];
            if (obj.NguoiDungId == userId) {
                check = false;
            }
        }
        if (check) {
            lstUserAccess.push({ NguoiDungId: userId, HoTen: fullName, LaQuanLy: checkLaQuanLy })
            AddTdToTableAccess(userId, fullName, checkLaQuanLy);
            dropdownlist.value("");         
            var data = { NguoiDungId: userId, LaQuanLy: checkLaQuanLy, DuAnId: duAnId }
            $.ajax({
                url: "/Project/InsertLienKetNguoiDungDuAn",
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
        } else {
            $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Người dùng này đã có trong phân quyền!"); 
        }
    } else {
        $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Bạn hãy chọn người dùng để phân quyền!"); 
    }
})
function AddTdToTableAccess(userId, fullName,avatar, ngayThamGia, checkLaQuanLy) {
    var str = '<tr id="trTableAccess-' + userId + '"><td><img src="/Assets/images/Avatar/' + avatar +'" width="20" height="20" class="m-r-5" />' + fullName + '</td><td> <div class="checkbox checkbox-primary"><input class="chkPermission" id="chkPQNDA-' + userId + '" type="checkbox"'
    if (checkLaQuanLy) str += " checked";
    str += '><label for= "chkPQNDA-' + userId + '">Quyền quản trị</label></div></td><td>' + ngayThamGia + '</td><td><a class="btn-link deleteAccess" id="' + userId + '"><i class="fal fa-trash-alt"></i></a></td></tr>'
    $("#tblAccess").append(str);
}
$(document).on("change", ".chkPermission", function () {
    var userId = this.id.replace("chkPQNDA-", "");
    var laQuanLy = $("#" + this.id)[0].checked;
    var data = { NguoiDungId: userId, LaQuanLy: laQuanLy, DuAnId: duAnId }
    $.ajax({
        url: "/Project/UpdateLienKetNguoiDungDuAn",
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
})
$(document).on("click", "#btnNotUpdateDuAn", function () {
    var url = "/project/index";
    window.location.href = url;
})
$(document).on("click", "#btnAddPhase", function () {
    var loaiGiaiDoanId = $("#cboLoaiGiaiDoan").val();
    var tenLoaiGiaiDoan = $("#cboLoaiGiaiDoan")[0].selectedOptions[0].text;
    var maMau = $("#cboLoaiGiaiDoan")[0].selectedOptions[0].id;
    var txtTenPhase = $("#txtTenGiaiDoanDuAn").val();
    var txtMoTaPhase = editorpop.value();
    var txtFrom = $("#txtFromDateGiaiDoanDuAn").val();
    var txtTo = $("#txtToDateGiaiDoanDuAn").val();
    var trangThaiHienTai = $("#chkDangThucHien")[0].checked;
    $("#txtTitle").text("Thêm mới giai đoạn");
    var checkDate = true;
    if (txtFrom != "" && txtTo != "") {
      var  txtFrom1 = getstrDate(txtFrom);

        var date1 = new Date(txtFrom1);
        var  txtTo1 = getstrDate(txtTo);
        var date2 = new Date(txtTo1);
        if (date1 > date2) {
            $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Ngày bắt đầu không được lớn hơn ngày hiện tại!"); 
            checkDate = false;
        }
    }
    if (loaiGiaiDoanId == 0 || loaiGiaiDoanId == "0") {
        checkDate = false;
        $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Bạn hãy chọn loại giai đoạn dự án!"); 
    }
    if (checkDate) {
        if ($.trim(txtTenPhase) != "" && $.trim(txtMoTaPhase) != "" && $.trim(txtFrom) != "" && $.trim(txtTo) != "") {
            var hangMucCongViecs = []
            var rows = $("tbody#tbdyHangMuc").find("tr");
            var countCheck = 0;
            if (rows.length > 0) {
                $.each(rows, function (index, value) {
                    var rowId = value.id;
                    if (rowId != "LastRow") {
                        rowId = rowId.replace("trHM", "");
                        var txtTenHangMuc = $("#txtHM" + rowId).val();
                        var trangThaiHangMuc = $("#chkHM" + rowId)[0].checked;
                        if (trangThaiHangMuc) countCheck++;
                        if ($.trim(txtTenHangMuc != "")) {
                            hangMucCongViecs.push({ TenHangMuc: txtTenHangMuc, TrangThai: trangThaiHangMuc, HangMucCongViecId:rowId })
                        }
                    }
                })
            }         
            if (check) {
                var txtTo = $("#txtToDateGiaiDoanDuAn").val();
                var data = { TrangThaiHienTai: trangThaiHienTai, LoaiGiaiDoanId: loaiGiaiDoanId, HangMucCongViecs: hangMucCongViecs, TenGiaiDoan: txtTenPhase, StrThoiGianBatDau: txtFrom, StrThoiGianKetThuc: txtTo, MoTa: txtMoTaPhase, DuAnId: duAnId }
                $.ajax({
                    url: "/Project/InsertGiaiDoanDuAn",
                    data: data,
                    context: document.body,
                    type: "POST",
                    dataType: "html",
                    success: function (data) {
                        if (data > 0) {
                            var check = false;
                            /// hiện mầu highlight phần giai đoạn đang chạy.
                            var rows = $("tbody#tbdyPhase").find("tr");
                            if (trangThaiHienTai == true) {
                                check = true;                               
                                if (rows.length > 0) {
                                    var lst = $(".clTTHT");
                                    if (lst.length > 0) {
                                        $.each(lst, function (index, value) {
                                            value.className = "clTTHT";
                                        })
                                    }
                                }
                            } else {
                                if (rows.length > 0) {
                                    var lst = $(".clTTHT");
                                    if (lst.length > 0) {
                                        $.each(lst, function (index, value) {
                                            if (value.className == "text-warning clTTHT") {
                                                check = true;
                                            }
                                        })
                                    }
                                }
                            }
                            if (check == false) trangThaiHienTai = true;
                            var col = txtFrom.split('/');
                            var str = '<tr id="trPhase-' + data + '"><td>';
                            if (trangThaiHienTai==true) str += '<span id="tdTTHT-' + data + '" class="text-warning clTTHT" "><i class="fas fa-star"></i></span>';
                            else str += '<span id="tdTTHT-' + data + '" class="clTTHT" "><i class="fas fa-star"></i></span>';
                            str += '</td><th scope="row" class="ship-able-status"><span class="label label-table ' + maMau + ' m-r-5">' + tenLoaiGiaiDoan + '</span><span id="' + data + '" class="editRowPhase">' + txtTenPhase + '</span></th><td><span>' + col[0] + '/' + col[1] + '-' + txtTo + '</span></td ><td><span class="font-13 font-w-500"> <i class="fas fa-check-square m-r-5" style="color:#3F51B5"></i>' + countCheck + '/' + hangMucCongViecs.length + '</span></td><td class="actions"><a href="#" id="' + data + '" class="on-default btn-link-grey edit-row m-r-10 editRowPhase"><i class="fal fa-edit"></i></a><a href="#" id="' + data + '" class="on-default btn-link-grey remove-row removeRowPhase"><i class="fal fa-trash-alt"></i></a></td></tr>';
                            $("#tbdyPhase").append(str);
                        }
                    },
                    error: function (xhr, status) {
                    },
                    complete: function (xhr, status) {
                        //$('#showresults').slideDown('slow')
                    }
                });

            } else {
                var data = { TrangThaiHienTai: trangThaiHienTai, LoaiGiaiDoanId: loaiGiaiDoanId, HangMucCongViecs: hangMucCongViecs, TenGiaiDoan: txtTenPhase, StrThoiGianBatDau: txtFrom, StrThoiGianKetThuc: txtTo, MoTa: txtMoTaPhase, DuAnId: duAnId, GiaiDoanDuAnId: rowEdit }
                $.ajax({
                    url: "/Project/UpdateGiaiDoanDuAn",
                    data: data,
                    context: document.body,
                    type: "POST",
                    dataType: "html",
                    success: function (data) {
                        if (data == "True" || data == "true") {
                            var rows = $("tbody#tbdyPhase").find("tr");
                            var check = false;
                            if (trangThaiHienTai) {                              
                                if (rows.length > 0) {
                                    var lst = $(".clTTHT");
                                    if (lst.length > 0) {
                                        $.each(lst, function (index, value) {
                                            value.className = "clTTHT";
                                        })
                                    }
                                }
                            }
                            else {
                                if (rows.length > 0) {
                                    var lst = $(".clTTHT");
                                    if (lst.length > 0) {
                                        $.each(lst, function (index, value) {
                                            if (value.className == "text-warning clTTHT" && value.id != ("tdTTHT-" + rowEdit)) {
                                                check = true;
                                            }
                                        })
                                    }
                                }
                            }
                            if (check == false) trangThaiHienTai = true;
                            var col = txtFrom.split('/');
                            var str = '<td>';
                            if (trangThaiHienTai) str += '<span id="tdTTHT-' + rowEdit + '" class="text-warning clTTHT" "><i class="fas fa-star"></i></span>';
                            else str += '<span id="tdTTHT-' + rowEdit + '" class="clTTHT" "><i class="fas fa-star"></i></span>';
                            str += '</td><th scope="row" class="ship-able-status"><span class="label label-table ' + maMau + '  m-r-5">' + tenLoaiGiaiDoan + '</span><span id="' + rowEdit + '" class="editRowPhase">' + txtTenPhase + '</span></th><td><span>' + col[0] + '/' + col[1] + '-' + txtTo + '</span></td ><td><span class="font-13 font-w-500"> <i class="fas fa-check-square m-r-5" style="color:#3F51B5"></i>' + countCheck + '/' + hangMucCongViecs.length + '</span></td><td class="actions"><a href="#" id="' + rowEdit + '" class="on-default btn-link-grey edit-row m-r-10 editRowPhase"><i class="fal fa-edit"></i></a><a href="#" id="' + rowEdit + '" class="on-default btn-link-grey remove-row removeRowPhase"><i class="fal fa-trash-alt"></i></a></td>';             
                            $("#trPhase-" + rowEdit).html(str);
                        }
                    },
                    error: function (xhr, status) {
                    },
                    complete: function (xhr, status) {
                        //$('#showresults').slideDown('slow')
                    }
                });

            }
            $("#add-phase-modal").modal('hide');

        } else {
            $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Tên giai đoạn dự án và mô tả , thời gian bắt đâu, kết thúc không được để trống!");            
        }
    }
   
})
$(document).on("click", "#btnInsetLoaiCongViec", function () {
    var tenLoaiCongViec = $("#txtTenLoaiCongViec").val();
    var maLoaiCongViec = $("#txtMaLoaiCongViec").val();
    if ($.trim(tenLoaiCongViec) == "") {
        $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Tên loại công việc không được để trống!");         
    } else {
        data = { TenLoaiCongViec: tenLoaiCongViec, MaLoaiCongViec: maLoaiCongViec }
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
function openAddLoaiCongViecModal() {
    $("#txtTenLoaiCongViec").val("");
    $("#txtMaLoaiCongViec").val("");
    $("#addLoaiCongViecModal").modal('show');
}
function openAddNewPhase() {
    check = true;
    $("#txtTenGiaiDoanDuAn").val("");
    $("#txtFromDateGiaiDoanDuAn").val(now);
    $("#txtToDateGiaiDoanDuAn").val(now);
    editorpop.value("");
    $("#cboLoaiGiaiDoan").val(0);
    $("#tbdyHangMuc").html('<tr id="LastRow"><td></td><td><input class="form-control" id="LastInput" type="text"></td><td></td></tr>');
    rowEdit = 0;
    $("#chkDangThucHien")[0].checked = false;
    $("#add-phase-modal").modal('show');
}
function onSelectNguoiDung(e) {
    var dropdownlist = $("#cboNguoiDung").data("kendoDropDownList");
    var userId = e.dataItem.NguoiDungId;;
    var fullName = e.dataItem.HoTen;
    var check = true;
    if (userId != "00000000-0000-0000-0000-000000000000") {
        for (var i = 0; i < lstUserAccess.length; i++) {
            var obj = lstUserAccess[i];
            if (obj.NguoiDungId == userId) {
                check = false;
            }
        }
        if (check) {
            lstUserAccess.push({ NguoiDungId: userId, HoTen: fullName, LaQuanLy: false })
            AddTdToTableAccess(userId, fullName,e.dataItem.Avatar,now, false);
            dropdownlist.value("00000000-0000-0000-0000-000000000000");
            var data = { NguoiDungId: userId, LaQuanLy: false, DuAnId: duAnId }
            $.ajax({
                url: "/Project/InsertLienKetNguoiDungDuAn",
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
        } else {
            $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Người dùng này đã có trong phân quyền!");
        }
    }
}
$(document).on("click", ".editRowPhase", function () {
    check = false;
    $("#txtTitle").text("Chỉnh sửa giai đoạn");
    $("#tbdyHangMuc").html('<tr id="LastRow"><td></td><td><input class="form-control" id="LastInput" type="text"></td><td></td></tr>');
    var id = this.id;
    rowEdit = id;
    $.ajax({
        url: "/Project/GetGiaiDoanDuAnById?id="+id,
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            result = JSON.parse(data);
            $("#txtTenGiaiDoanDuAn").val(result.TenGiaiDoan);
            $("#txtFromDateGiaiDoanDuAn").val(result.StrThoiGianBatDau);
            $("#txtToDateGiaiDoanDuAn").val(result.StrThoiGianKetThuc);
            $("#cboLoaiGiaiDoan").val(result.LoaiGiaiDoanId);
            $("#chkDangThucHien")[0].checked = result.TrangThaiHienTai;
            editorpop.value(result.MoTa);
            $("#add-phase-modal").modal('show');
            if (result.HangMucCongViecs.length > 0) {
                for (i = 0; i < result.HangMucCongViecs.length; i++) {
                    var item = result.HangMucCongViecs[i];
                    var str = '<tr id="trHM' + item.HangMucCongViecId + '"><td> <input class="m-t-15" id="chkHM' + item.HangMucCongViecId + '" type="checkbox"';
                    if (item.TrangThai) str += " checked "
                    str += '></td> <td> <input class="form-control" id="txtHM' + item.HangMucCongViecId + '" type="text" value="' + item.TenHangMuc + '"></td><td><a id="' + item.HangMucCongViecId + '" class="removeHM"><i class="fa fa-trash-o"></i></a></td></tr>'

                    $(str).insertBefore("#LastRow");
                }
            }
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
  
   
})
$(document).on("click", ".removeRowPhase", function () {
    var id = this.id;
    if (confirm("Bạn có muốn xóa giai đoạn dự án này!") == true) {
        $.ajax({
            url: "/Project/DeleteGiaiDoanDuAn?giaiDoanDuAnId=" + id,
            context: document.body,
            type: "DELETE",
            dataType: "html",
            success: function (data) {
                if (data == "True" || data == "true") {
                    $("#trPhase-" + id).remove();
                } else {
                    $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Bạn hãy xóa hết liên kết đến giai doạn dự án này trước đã!");                 
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
$(document).on("change", ".ChkNguoiDung", function () {
    var id = this.id;
    var check = this.checked;
    var userId = id.replace("chkPQNDA-", "");
    var data = { NguoiDungId: userId, LaQuanLy: check, DuAnId: duAnId }
    $.ajax({
        url: "/TramDo/UpdateLienKetNguoiDungDuAn",
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
})
function getstrDate(strDate) {
    var col = strDate.split("/");
    return col[1] + "/" + col[0] + "/" + col[2];
}
$(document).on("change", "#LastInput", function () {
    var text = this.value;
    //var check = $("#LastCheckBox").checked;
    if ($.trim(text) != "") {
        var str = '<tr id="trHM' + newRowHM + '"><td> <input class="m-t-15" id="chkHM' + newRowHM + '" type="checkbox"';
        //if (check) str += 'checked'
        str += '></td> <td> <input class="form-control" id="txtHM' + newRowHM + '" type="text" value="' + text + '"></td><td><a id="' + newRowHM + '" class="removeHM"><i class="fa fa-trash-o"></i></a></td></tr>'

        $(str).insertBefore("#LastRow");
        //$("#LastCheckBox").checked = false;
        $("#LastInput").val("");
    }
    newRowHM--;
})
$(document).on("click", ".removeHM", function () {
    var id = this.id;
    $("#trHM" + id).remove();
})
$(document).on("change", "#cboLoaiNganSach", function () {
    var id = $("#cboLoaiNganSach").val();
    $("#txtTongNhanSach").val(0);
    if (id == 3 || id == "3") {
        $("#txtTongNhanSach").prop("disabled", true);
    } else {
        $("#txtTongNhanSach").prop("disabled", false);
    }
})