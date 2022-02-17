var lstUserAccess = [], lstGiaiDoan = [], newRow = 1, check = true, rowEdit, editor, editorpop; newRowHM = 1;
// JQuery.Inputmask example.
var checkInsert = true;
var now = $("#now").val();
$(document).ready(function () {
    $('#datetimepicker1').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        startDate: new Date(),
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
    var loaiCongViec = $("#loaiCongViecList").kendoMultiSelect({
        dataTextField: "TenLoaiCongViec",
        dataValueField: "LoaiCongViecId",
        dataSource: loaiCongViecData       
    }).data("kendoMultiSelect");  
    var dataKhachHang = getDataKhachHang()
    $("#cboKhachHang").kendoDropDownList({
        dataTextField: "TenKhachHang",
        dataValueField: "KhachHangId",
        filter: "contains",
        dataSource: dataKhachHang
     
    });
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

    var dataLoaiNganSach = getDataLoaiNganSach()
    $("#cboLoaiNganSach").kendoDropDownList({
        dataTextField: "TenLoaiNganSach",
        dataValueField: "LoaiNganSachId",
        filter: "contains",
        dataSource: dataLoaiNganSach
    });
    getDataLoaiGiaiDoan();
    $(":input").inputmask();
    $("#txtFromTimeDuAn").val(now);
});
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
                    $("#cboLoaiGiaiDoan").append('<option value=' + value.LoaiGiaiDoanDuAnId + ' id="' + value.MaMau + '">' + value.TenLoaiGiaiDoan + '</option>')
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
        } else {
            $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Người dùng này đã có trong phân quyền!");
           
        }
    } else {
        $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Bạn hãy chọn người dùng để phân quyền!");
       
    }
})
function AddTdToTableAccess(userId, fullName,avatar, checkLaQuanLy) {
    var str = '<tr id="trTableAccess-' + userId + '"><td><img src="/Assets/images/Avatar/'+avatar+'" width="20" height="20" class="m-r-5" />' + fullName + '</td><td> <div class="checkbox checkbox-primary"><input  class="ChkNguoiDung" id="chkPQNDA-'+userId+'" type="checkbox"'
    if (checkLaQuanLy) str += " checked";
    str += '><label for= "chkPQNDA-' + userId + '">Quyền quản trị</label></div></td><td><a class="btn-link deleteAccess" id="' + userId + '"><i class="fal fa-trash-alt"></i></a></td></tr>'
    $("#tblAccess").append(str);
}
$(document).on("click", ".deleteAccess", function () {
    var userId = this.id;
    $('#trTableAccess-' + userId).remove();
    for (var i = 0; i < lstUserAccess.length; i++) {
        var obj = lstUserAccess[i];
        if (obj.NguoiDungId == userId) {
            lstUserAccess.splice(i, 1);
            i = lstUserAccess.length;
        }
    }
})
$(document).on("click", ".deleteQDDA", function () {
    var id = this.id;
    $('#newTrTableGDDA-' + id).remove();
})
$(document).on("click", "#btnInsertDuAn", function () {
    if (checkInsert == true) {
        checkInsert = false;
        var checkAll = false;
        var DuAn = {}
        DuAn.KhachHangId = $("#cboKhachHang").data("kendoDropDownList").value();
        DuAn.TenDuAn = $("#txtTenDuAn").val();
        DuAn.MaDuAn = $("#txtMaDuAn").val();
        DuAn.StrNgayBatDau = $("#txtFromTimeDuAn").val();
        DuAn.QuanTam = $("#chkQuanTam")[0].checked;
        DuAn.MoTa = editor.value();
        DuAn.LoaiNganSachId = $("#cboLoaiNganSach").data("kendoDropDownList").value();
        DuAn.TongNganSach = $("#txtTongNhanSach").val();
        while (DuAn.TongNganSach.indexOf(",") != -1) {
            DuAn.TongNganSach = DuAn.TongNganSach.replace(",", "");
        }
        DuAn.TrangThaiDuAnId = $("#cboTrangThai").data("kendoDropDownList").value();
        //DuAn. = $("#txtGhiChu").val();
        //DuAn.MoTa = $("#txtGhiChu").val();
        DuAn.LoaiCongViecIds = $("#loaiCongViecList").data("kendoMultiSelect").value();
        DuAn.GiaiDoanDuAns = lstGiaiDoan;
        DuAn.LienKetNguoiDungDuAn = lstUserAccess;
        //DuAn.MoTa = $("#txtGhiChu").val();
        //DuAn.MoTa = $("#txtGhiChu").val();
        //DuAn.MoTa = $("#txtGhiChu").val();
        if (DuAn.LoaiCongViecIds != [] && DuAn.LoaiCongViecIds.length > 0 && $.trim(DuAn.TenDuAn) != "" && $.trim(DuAn.MaDuAn) != "" && $.trim(DuAn.MoTa) != "" && ((DuAn.LoaiNganSachId != "3" && $.trim(DuAn.TongNganSach) != "") || DuAn.LoaiNganSachId == "3") && DuAn.KhachHangId != "0") {

            $.ajax({
                url: "/Project/AddDuAn",
                data: DuAn,
                context: document.body,
                type: "POST",
                dataType: "html",
                async: false,
                success: function (data) {
                    if (data > 0) {
                        var url = "/project/index";
                        $.Notification.autoHideNotify('success', 'top right', 'Thông báo', "Thêm mới dự án thành công!");
                        checkAll = true;
                        setTimeout(
                            function () {
                                window.location.href = url;
                            }, 1000);
                    } else {
                        $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Thêm mới không thành công!");                       
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
        if (checkAll == false) checkInsert = true;
    }  
})
$(document).on("click", "#btnNotInsertDuAn", function () {
    var url = "/project/index";
    window.location.href = url;
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
$(document).on("click", "#btnAddPhase", function () {
    var maMau = $("#cboLoaiGiaiDoan")[0].selectedOptions[0].id;
    var loaiGiaiDoanId = $("#cboLoaiGiaiDoan").val();
    var tenLoaiGiaiDoan = $("#cboLoaiGiaiDoan")[0].selectedOptions[0].text
    var txtTenPhase = $("#txtTenGiaiDoanDuAn").val();
    var txtMoTaPhase = editorpop.value();
    var txtFrom = $("#txtFromDateGiaiDoanDuAn").val();
    var txtTo = $("#txtToDateGiaiDoanDuAn").val();
    var trangThaiHienTai = $("#chkDangThucHien")[0].checked;
    $("#txtTitle").text("Thêm mới giai đoạn");
    var checkDate = true;
    if (txtFrom != "" && txtTo != "") {
       var txtFrom1 = getstrDate(txtFrom)
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
                            hangMucCongViecs.push({ TenHangMuc: txtTenHangMuc, TrangThai: trangThaiHangMuc })
                        }
                    }                    
                })
            }
            var col = txtFrom.split('/');
            ///Bỏ trạng thái hiện tại của các hạng mục khác nếu trạng thái hạng mục mới là true;
            if (trangThaiHienTai) {
                var rows = $("tbody#tbdyHangMuc").find("tr");
                if (rows.length > 0) {
                    var lst = $(".clTTHT");
                    if (lst.length > 0) {
                        $.each(lst, function (index, value) {
                            value.className = "clTTHT";
                        })
                    }
                }
                if (lstGiaiDoan.length > 0)
                    $.each(lstGiaiDoan, function (index, value) {
                        value.TrangThaiHienTai = false;
                    })
            }
            if (check) {
                if (lstGiaiDoan.length == 0) trangThaiHienTai = true;
                lstGiaiDoan.push({ TrangThaiHienTai: trangThaiHienTai, GiaiDoanDuAnId: newRow, LoaiGiaiDoanId: loaiGiaiDoanId, TenGiaiDoan: txtTenPhase, MoTa: txtMoTaPhase, StrThoiGianBatDau: txtFrom, StrThoiGianKetThuc: txtTo, HangMucCongViecs: hangMucCongViecs })              
                var str = '<tr id="trPhase-' + newRow + '"><td>';
                if (trangThaiHienTai) str += '<span id="tdTTHT-' + newRow + '" class="text-warning clTTHT" "><i class="fas fa-star"></i></span>';
                else str += '<span id="tdTTHT-' + newRow + '" class="clTTHT" "><i class="fas fa-star"></i></span>';
                str += '</td><th scope="row" class="ship-able-status"><span class="label label-table ' + maMau + ' m-r-5">' + tenLoaiGiaiDoan + '</span><span id="' + newRow + '" class="editRowPhase">' + txtTenPhase + '</span></th><td><span>' + col[0] + '/' + col[1] + '-' + txtTo + '</span></td ><td><span class="font-13 font-w-500"> <i class="fas fa-check-square m-r-5" style="color:#3F51B5"></i>' + countCheck + '/' + hangMucCongViecs.length + '</span></td><td class="actions"><a href="#" id="' + newRow + '" class="on-default btn-link-grey edit-row m-r-10 editRowPhase"><i class="fal fa-edit"></i></a><a href="#" id="' + newRow + '" class="on-default btn-link-grey remove-row removeRowPhase"><i class="fal fa-trash-alt"></i></a></td></tr>';
                $("#tbdyPhase").append(str);
                newRow++;
            } else {               
                for (i = 0; i < lstGiaiDoan.length; i++) {
                    if (lstGiaiDoan[i].GiaiDoanDuAnId == parseInt(rowEdit)) {
                        lstGiaiDoan.splice(i, 1);
                    }
                }
                var checkHT = false;
                if (lstGiaiDoan.length > 0) {
                    $.each(lstGiaiDoan, function (index, value) {
                        if (value.TrangThaiHienTai == true) {
                            checkHT = true;
                        }
                    })
                }
                if (checkHT == false) trangThaiHienTai = true;
                lstGiaiDoan.push({ TrangThaiHienTai: trangThaiHienTai, GiaiDoanDuAnId: rowEdit, LoaiGiaiDoanId: loaiGiaiDoanId, TenGiaiDoan: txtTenPhase, MoTa: txtMoTaPhase, StrThoiGianBatDau: txtFrom, StrThoiGianKetThuc: txtTo, HangMucCongViecs: hangMucCongViecs })
                var str = '<td>';
                if (trangThaiHienTai) str += '<span id="tdTTHT-' + rowEdit + '" class="text-warning clTTHT" "><i class="fas fa-star"></i></span>';
                else str += '<span id="tdTTHT-' + rowEdit + '" class="clTTHT" "><i class="fas fa-star"></i></span>';
                str += '</td><th scope="row" class="ship-able-status"><span class="label label-table ' + maMau + '  m-r-5">' + tenLoaiGiaiDoan + '</span><span id="' + rowEdit + '" class="editRowPhase">' + txtTenPhase + '</span></th><td><span>' + col[0] + '/' + col[1] + '-' + txtTo + '</span></td ><td><span class="font-13 font-w-500"> <i class="fas fa-check-square m-r-5" style="color:#3F51B5"></i>' + countCheck + '/' + hangMucCongViecs.length + '</span></td><td class="actions"><a href="#" id="' + rowEdit + '" class="on-default btn-link-grey edit-row m-r-10 editRowPhase"><i class="fal fa-edit"></i></a><a href="#" id="' + rowEdit + '" class="on-default btn-link-grey remove-row removeRowPhase"><i class="fal fa-trash-alt"></i></a></td>';             
                $("#trPhase-" + rowEdit).html(str);
            }
            $("#add-phase-modal").modal('hide');

        } else {
            $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Tên giai đoạn dự án, mô tả, thời gian bắt đầu, kết thúc không được để trống!");  
        }
    }
   
})
$(document).on("click", ".removeRowPhase", function () {
    var id = this.id;
    $("#trPhase-" + id).remove();
    for (i = 0; i < lstGiaiDoan.length; i++) {
        if (lstGiaiDoan[i].GiaiDoanDuAnId == parseInt(id)) {
            lstGiaiDoan.splice(i, 1);
        }
    }

})
$(document).on("click", ".editRowPhase", function () {
    check = false;
    $("#txtTitle").text("Chỉnh sửa giai đoạn");
    $("#tbdyHangMuc").html('<tr id="LastRow"><td></td><td><input class="form-control" id="LastInput" type="text"></td><td></td></tr>');
    var id = this.id;
    rowEdit = id;
    $.each(lstGiaiDoan, function (index, value) {
        if (value.GiaiDoanDuAnId == id) {
            $("#txtTenGiaiDoanDuAn").val(value.TenGiaiDoan);
            $("#txtFromDateGiaiDoanDuAn").val(value.StrThoiGianBatDau);
            $("#txtToDateGiaiDoanDuAn").val(value.StrThoiGianKetThuc);
            $("#cboLoaiGiaiDoan").val(value.LoaiGiaiDoanId);
            editorpop.value(value.MoTa);
            $("#chkDangThucHien")[0].checked = value.TrangThaiHienTai;
            $("#add-phase-modal").modal('show');
            if (value.HangMucCongViecs.length > 0) {
                for (i = 0; i < value.HangMucCongViecs.length; i++) {
                    var item = value.HangMucCongViecs[i];
                    var str = '<tr id="trHM' + i + '"><td> <input class="m-t-15" id="chkHM' + i + '" type="checkbox"';
                    if(item.TrangThai) str+=" checked "
                    str += '></td> <td> <input class="form-control" id="txtHM' + i + '" type="text" value="' + item.TenHangMuc + '"></td><td><a id="' + i + '" class="removeHM"><i class="fa fa-trash-o"></i></a></td></tr>'

                    $(str).insertBefore("#LastRow");
                }
            }
        }
    })
   
    
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
function onSelectNguoiDung(e) {
    var dropdownlist = $("#cboNguoiDung").data("kendoDropDownList");
    var userId = e.dataItem.NguoiDungId;;
    var fullName = e.dataItem.HoTen; 
    var avatar = e.dataItem.Avatar;
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
            AddTdToTableAccess(userId, fullName,avatar, false);
            dropdownlist.value("00000000-0000-0000-0000-000000000000");
        } else {
            $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "người dùng này đã có trong phân quyền!"); 
        }
    }
}
$(document).on("change", ".ChkNguoiDung", function () {
    var id = this.id;
    var check = this.checked;
    var userId = id.replace("chkPQNDA-", "");
    $.each(lstUserAccess, function (index, value) {
        if (value.NguoiDungId == userId) {
            value.LaQuanLy = check;
        }
    })
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
        str += '></td> <td> <input class="form-control" id="txtHM' + newRowHM + '" type="text" value="'+text+'"></td><td><a id="' + newRowHM + '" class="removeHM"><i class="fa fa-trash-o"></i></a></td></tr>'
           
        $(str).insertBefore("#LastRow");
        //$("#LastCheckBox").checked = false;
        $("#LastInput").val("");
    }
    newRowHM++;
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

