var newRowHM = -1;
var shipableId = 0, taskIdA=0, editor, editorDT,editor2,multiSelectNguoiDung;
var checkInsertShip = true;
var checkInsertTime = true;
var dataUser;
var timer2 = new easytimer.Timer();
var LoginUserId;
var strUrl = "";
$(document).ready(function () {  
    LoginUserId = $("#NguoiDungIdM").val();
    dataUser = getNguoiDungAlls();
    var strNguoiXuLyShip = '<option value="0"> Chọn người xử lý </option>';
    if (dataUser && dataUser.length > 0) {
        $.each(dataUser, function (index, value) {
            strNguoiXuLyShip += '<option value=' + value.PhongBanId + ' disabled>' + value.TenPhongBan + '</option>';
            if (value.NguoiDungs && value.NguoiDungs.length > 0) {
                $.each(value.NguoiDungs, function (index2, value2) {
                    strNguoiXuLyShip += '<option value="' + value2.NguoiDungId + '"';
                    if (value2.NguoiDungId == LoginUserId) strNguoiXuLyShip += ' selected ';
                    strNguoiXuLyShip += ' > &nbsp;&nbsp; ' + value2.HoTen + '</option>';
                })
            }
        })
    }
    $("#cboNguoiXuLyShip").html(strNguoiXuLyShip);
    var duAnData = getDuAns();
    var duAn = $("#SelectDuAnF").kendoMultiSelect({
        dataTextField: "TenDuAn",
        dataValueField: "DuAnId",
        filter: "contains",
        dataSource: duAnData,
        change: onSelectDuAn
    }).data("kendoMultiSelect");
    var strDuAn = $("#strDuAnIds").val();
    if (strDuAn != "") {
        var duAnIds = strDuAn.split(",");
        $("#SelectDuAnF").data("kendoMultiSelect").value(duAnIds);
    }

    var trangThaiDuAnData = getTrangThaiDuAns();
    var trangThaiDuAn = $("#SelectTrangThaiDuAnF").kendoMultiSelect({
        dataTextField: "TenTrangThaiDuAn",
        dataValueField: "TrangThaiDuAnId",
        filter: "contains",
        dataSource: trangThaiDuAnData,      
    }).data("kendoMultiSelect");
    var strTrangThaiDuAn = $("#strTrangThaiDuAnIds").val();
    if (strTrangThaiDuAn != "") {
        var trangThaiDuAnIds = strTrangThaiDuAn.split(",");
        $("#SelectTrangThaiDuAnF").data("kendoMultiSelect").value(trangThaiDuAnIds);
    }

    var phongBanData = getPhongBan();
    var phongBan = $("#SelectPhongBanF").kendoMultiSelect({
        dataTextField: "TenPhongBan",
        dataValueField: "PhongBanId",
        filter: "contains",
        dataSource: phongBanData,
        change: onSelectPhongBan
    }).data("kendoMultiSelect");
    var strPhongBan = $("#strPhongBanIds").val();
    if (strPhongBan != "") {
        var phongBanIds = strPhongBan.split(",");
        $("#SelectPhongBanF").data("kendoMultiSelect").value(phongBanIds);
    }
   
    if (duAnData && duAnData.length > 0) {
        $.each(duAnData, function (index, value) {
            var str = '<option value=' + value.DuAnId + '>' + value.TenDuAn + '</option>'
            $("#cboDuAn").append(str);
            $("#cboDuAnDT").append(str);
        })
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
        $("#cboTrangThaiDT").append(str);
    }
    $('#datetimepicker11').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        startDate: new Date(),
        todayHighlight: true
    })
    $('#datetimepicker11DT').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',     
        todayHighlight: true
    })
    $('#datetimepicker12').datepicker({
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
    $("#txtGhiChuDT").kendoEditor({
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
    $("#txtGhiChuM").kendoEditor({
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
    editorDT = $("#txtGhiChuDT").data("kendoEditor");
    editor2 = $("#txtGhiChuM").data("kendoEditor");
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
    $('#datetimepickerQTA').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        todayHighlight: true
    })
    var dataUser2 = getNguoiDungs();
    multiSelectNguoiDung= $("#SelectNguoiDung").kendoMultiSelect({
        dataTextField: "HoTen",
        dataValueField: "NguoiDungId",
        dataSource: dataUser2
    })
    var str = $("#strNguoiDungIdM").val();
    if (str != "") {
        var nguoiDungIds = str.split(",");
        $("#SelectNguoiDung").data("kendoMultiSelect").value(nguoiDungIds);
    }
    var type = $("#typeM").val();
    if (type != 21) {
        $("#divTu").hide();
        $("#divDen").hide();
    }

    /// chạy time
    var timeid = $("#currentTodoIdM").val();
    if (timeid > 0) {
        var countTime = parseInt($("#TimeValueM").val());
        var hours = Math.floor(countTime / 3600);
        var minutes = Math.floor((countTime % 3600) / 60);
        var seconds = Math.floor(countTime % 3600 % 60);
        var html = '<div class="timer"><span><span class="timer-run"><i class="fas fa-circle"></i></span><label class="font-13 font-w-500"><span id="hours4"></span><span id="minutes4"></span><span id="seconds4"></span></label></span></div>';
        $("#divTimerT").html(html);
        timer2.start({ precision: 'seconds', startValues: { hours: hours, minutes: minutes, seconds: seconds } });
        timer2.addEventListener('secondsUpdated', function (e) {
            $('#hours4').html(timer2.getTotalTimeValues().hours.toString() + 'h ');
            $('#minutes4').html(timer2.getTimeValues().minutes.toString() + 'm ');
            $('#seconds4').html(timer2.getTimeValues().seconds.toString() + 's');
           
        });
    }

})
function onSelectDuAn(e) {
    var duAnIds = this.value();
    var phongBanIds = $("#SelectPhongBanF").data("kendoMultiSelect").value();   
    $.ajax({
        url: "/ToDo/GetNguoiDungsBy?strDuAnIds=" + duAnIds + "&&strPhongBanIds=" + phongBanIds,   
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            var dataNguoiDung2 = JSON.parse(data);
            var multiselect = $("#SelectNguoiDung").data("kendoMultiSelect");
            multiselect.setDataSource(new kendo.data.DataSource({ data: dataNguoiDung2 }));  
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')                 
        }
    });
}
function onSelectPhongBan(e) {
    var phongBanIds = this.value();
    var duAnIds = $("#SelectDuAnF").data("kendoMultiSelect").value();    
    $.ajax({
        url: "/ToDo/GetNguoiDungsBy?strDuAnIds=" + duAnIds + "&&strPhongBanIds=" + phongBanIds,       
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            var dataNguoiDung2 = JSON.parse(data); 
            var multiselect = $("#SelectNguoiDung").data("kendoMultiSelect");
            multiselect.setDataSource(new kendo.data.DataSource({ data: dataNguoiDung2 }));  
          

        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')                 
        }
    });
}
$(document).on("change", "#cboDuAn", function () {
    var id = $("#cboDuAn").val();
    var dataGiaiDoan = getDuAnThanhPhan(id);
    if (dataGiaiDoan && dataGiaiDoan.length > 0) {
        var str = '<option value="0"> Chọn dự án thành phần </option>';
        $.each(dataGiaiDoan, function (index, value) {
            str+= '<option value="' + value.DuAnId + '">' + value.TenDuAn + '</option>'           
        })
        $("#cboGiaiDoanDuAn").html(str);
    }
    //var dataUser2 = getNguoiDungs(id);
    //if (dataUser2 && dataUser2.length > 0) {
    //    var strnguoi = '<option value="0"> Chọn người xử lý </option>';
    //    $.each(dataUser2, function (index, value) {
    //        strnguoi += '<option value="' + value.NguoiDungId + '">' + value.HoTen + '</option>'
           
    //    })
    //    $("#cboNguoiDung").html(strnguoi);
    //}
})
$(document).on("change", "#cboDuAnDT", function () {
    var id = $("#cboDuAnDT").val();
    changeDuAnDT(id);
    //var dataUser2 = getNguoiDungs(id);
    //if (dataUser2 && dataUser2.length > 0) {
    //    var strnguoi = '<option value="0"> Chọn người xử lý </option>';
    //    $.each(dataUser2, function (index, value) {
    //        strnguoi += '<option value="' + value.NguoiDungId + '">' + value.HoTen + '</option>'

    //    })
    //    $("#cboNguoiDung").html(strnguoi);
    //}
})
function changeDuAnDT(id) {
    var dataGiaiDoan = getDuAnThanhPhan(id);
    if (dataGiaiDoan && dataGiaiDoan.length > 0) {
        var str = '<option value="0"> Chọn dự án thành phần </option>';
        $.each(dataGiaiDoan, function (index, value) {
            str += '<option value="' + value.DuAnId + '">' + value.TenDuAn + '</option>'
        })
        $("#cboGiaiDoanDuAnDT").html(str);
    }
}
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
function setSelectWeek(week, year) {
    $.ajax({
        url: "/Shipable/GetWeeks3?week=" + week + "&&year=" + year,
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
            $("#cboWeek").val(week + "-" + year);
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
}
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
function getGiaiDoanDuAn(duAnId) {
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
function getNguoiDungs() {
    var result;
    var duAnIds = $("#SelectDuAnF").data("kendoMultiSelect").value();    
    var phongBanIds = $("#SelectPhongBanF").data("kendoMultiSelect").value();   
    $.ajax({
        url: "/ToDo/GetNguoiDungsBy?strDuAnIds=" + duAnIds + "&&strPhongBanIds=" + phongBanIds,
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
function getNguoiDungAlls() {
    var result;   
    $.ajax({
        url: "/ToDo/GetNguoiDungAll",
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
        url: "/ToDo/GetDuAns",
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
function getPhongBan() {
    var result;
    $.ajax({
        url: "/ToDo/GetPhongBans",
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
function getTrangThaiDuAns() {
    var result;
    $.ajax({
        url: "/ToDo/GetTrangThaiDuAns",
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
function getDataTrangThaiCongViecByKhoaCha(chaId) {
    var result;
    $.ajax({
        url: "/Shipable/GetTrangThaiShipablesByKhoaCha?khoaChaId=" + chaId,
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
function openAddShipModal() {
    var ma = $("#maM").val();
    $("#custom-width-modalLabel").text("Thêm mới ship-able");
    $("#txtTenShipable").val("");
/*    $("#txtMaShipable").val(ma);*/
    $("#cboGiaiDoanDuAn").val(0);
    /*$("#cboNguoiDung").val("");*/
    $("#txtDeadLine").val($("#nowM").val());
    $("#addShipModal").modal("show");
    $("#tbdyHangMuc").html('<tr id="LastRow"><td></td><td> <input class="form-control" id="LastInput" type="text"></td><td></td><td></td></tr>')
    shipableId = 0;
    editor.value("");
    //$("#btnDelete").hide();
    //$("#cboTrangThai2").val(0);
    //$("#cboTrangThai2").hide();
    var tuanHT = $("#tuanHTM").val();
    var yearHT = $("#yearM").val();
    setSelectWeek(tuanHT, yearHT);
/*    $("#divXuLy").hide();*/
  /*  setSelectTrangThaiCongViec(0);*/
    $("#divSaveQuyTrinh").hide();
}
$(document).on("change", "#LastInput", function () {
    var text = this.value;
    //var check = $("#LastCheckBox").checked;
    if ($.trim(text) != "") {
        var str = '<tr id="trHM' + newRowHM + '"><td> <input class="m-t-15" id="chkHM' + newRowHM + '" type="checkbox" checked';
        //if (check) str += 'checked'
        str += '></td> <td> <input class="form-control" id="txtHM' + newRowHM + '" type="text" value="' + text + '"></td><td><select id="cboNguoiDung' + newRowHM + '" class="form-control"><option value="0"> Chọn người xử lý </option>';
        if (dataUser && dataUser.length > 0) {
            $.each(dataUser, function (index, value) {
                str += '<option value=' + value.PhongBanId + ' disabled>' + value.TenPhongBan + '</option>';
                if (value.NguoiDungs && value.NguoiDungs.length > 0) {
                    $.each(value.NguoiDungs, function (index2, value2) {
                        str += '<option value=' + value2.NguoiDungId + ' > &nbsp;&nbsp; ' + value2.HoTen + '</option>';
                    })
                }
            })
        }
        str +='</select></td><td><a id="' + newRowHM + '" class="removeHM"><i class="fa fa-trash-o"></i></a></td></tr>'

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
$(document).on("click", "#btnSubmit", function () {
    if (checkInsertShip == true) {
        checkInsertShip = false;
        var checkAll = false;
        var ship = {}
        ship.TenCongViec = $("#txtTenShipable").val();
        ship.DuAnId = $("#cboGiaiDoanDuAn").val();
        ship.GiaiDoanDuAnId = $("#cboGiaiDoanDuAn").val();
        ship.MoTa = editor.value();        
       /* ship.ThuTuUuTien = $("#cboDoUuTien").val();*/
        ship.StrNgayDuKienHoanThanh = $("#txtDeadLine").val();
        //var trangThai1 = $("#cboTrangThai").val();
        //var trangThai2 = $("#cboTrangThai2").val();
        //if (trangThai2 == 0) {
        //    ship.TrangThaiCongViecId = trangThai1;
        //} else {
        //    ship.TrangThaiCongViecId = trangThai2;
        //}
        //var value = $("#cboWeek").val();
        //var col = value.split("-");
        //ship.Tuan = col[0];
        //ship.Nam = col[1];
      /*  ship.NguoiXuLyId = $("#cboNguoiDung").val();*/
        /*ship.MaCongViec = $("#txtMaShipable").val(); */    
        //if (ship.TrangThaiCongViecId == 6 || ship.TrangThaiCongViecId == 12) {
        //    ship.XuLyVaoTuanTiepTheo = $("#chkXuLyINW")[0].checked;
        //}
        var check = true;
        if ($.trim(ship.TenCongViec) == "" || ship.DuAnId == "0") {
            check = false;
        }
        if (check) {           
                var lstTask = []
                var rows = $("tbody#tbdyHangMuc").find("tr");
                var countCheck = 0;
                if (rows.length > 0) {
                    $.each(rows, function (index, value) {
                        if (check) {
                            var rowId = value.id;
                            if (rowId != "LastRow") {
                                rowId = rowId.replace("trHM", "");
                                var txtTenHangMuc = $("#txtHM" + rowId).val();
                                var trangThaiHangMuc = $("#chkHM" + rowId)[0].checked;
                                if (trangThaiHangMuc && $.trim(txtTenHangMuc != "")) {
                                    var nguoidungId = $("#cboNguoiDung" + rowId).val();
                                    if (nguoidungId == 0 || nguoidungId == "0") {
                                        $.Notification.autoHideNotify('error', 'top right', 'Lỗi', "Bạn hãy chọn người xử lý cho các công việc cần làm!");
                                        checkInsertShip = true
                                        checkAll = true;
                                        check = false;
                                        return;
                                    }
                                    lstTask.push({ TenCongViec: txtTenHangMuc, NguoiXuLyId: nguoidungId })
                                }
                            }
                        }
                   
                       
                    })
                }
                ship.CongViecs = lstTask;
            if (check) {
                $.ajax({
                    url: "/ToDo/AddShipable",
                    data: ship,
                    context: document.body,
                    type: "POST",
                    dataType: "html",
                    async: false,
                    success: function (data) {
                        checkInsertShip = true;
                        if (data > 0) {
                            $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Thêm mới shipable thành công!");
                            checkAll = true;
                           
                            load();
                            dongBoShip(data);
                        } else {
                        }
                    },
                    error: function (xhr, status) {
                        checkInsertShip = true;
                    },
                    complete: function (xhr, status) {
                        checkInsertShip = true;
                        //$('#showresults').slideDown('slow')                 
                    }
                });
            } else {
                $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Các trường bắt buộc (*) không được để trống!");
            }
        }
        else {
            $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Các trường bắt buộc (*) không được để trống!");
        }
        if (checkAll == false) checkInsertShip = true;
    }
})
function dongBoShip(shipId) {
    $.ajax({
        url: "/ToDo/DongBoShipableToCoda?shipId=" + shipId,
        context: document.body,
        type: "GET",
        dataType: "html",        
        success: function (data) {          
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
                          
        }
    });
}
$(document).on("click", "#btnFilter", function () {
    load();
})
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
function load(checkload) {
    var duAnIds = $("#SelectDuAnF").data("kendoMultiSelect").value();
    var trangThaiDuAnIds = $("#SelectTrangThaiDuAnF").data("kendoMultiSelect").value();
    var phongBanIds = $("#SelectPhongBanF").data("kendoMultiSelect").value(); 
    var quanTam = $("#chkQuanTam")[0].checked;  
    var nguoiDungIds = $("#SelectNguoiDung").data("kendoMultiSelect").value();
    var type = $("#ddlTime").val();
    var tu = $("#txtFromDate").val();
    var den = $("#txtToDate").val();
    var str = "/ToDo/ToDoWork";
    var check = true;
    if (duAnIds.length >  0) {
        if (check) {
            check = false;
            str += "?";
        } else str += "&&";
        str += "strDuAnId=" + duAnIds;
    }
    if (trangThaiDuAnIds.length > 0) {
        if (check) {
            check = false;
            str += "?";
        } else str += "&&";
        str += "strTrangThaiDuAnId=" + trangThaiDuAnIds;
    }

    if (phongBanIds.length >  0) {
        if (check) {
            check = false;
            str += "?";
        } else str += "&&";
        str += "strPhongBanIds=" + phongBanIds;
    }   
   
    if (quanTam == true) {
        if (check) {
            check = false;
            str += "?";
        } else str += "&&";
        str += "quanTam=" + quanTam;
    }
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
            strUrl = str;
            if (checkload==true) location.href = str;
            else {               
                $("#tableToDoWork").html($(data).find("#tableToDoWork"));
            }
          
           
        },
        error: function (xhr, status) {
            $.Notification.autoHideNotify('error', 'top right', 'Lỗi', "Lỗi kết nối đến máy chủ!");
        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
}
$(document).on("click", "#btnSearch", function () {
    load(true);
})
$(document).on("change", ".ddlTTCV", function () {
    var id = this.id;
    id = id.replace("ddlTTCV-", "");
    var value = this.value;
    $.ajax({
        url: "/ToDo/UpdateStatus?CongViecId=" + id + "&&TrangThaiId=" + value,
        context: document.body,
        type: "Post",
        dataType: "html",
        success: function (data) {
            if (data == true||data=="True") {
                $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Update trạng thái thành công!");
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
$(document).on("click", ".checkQuanTamT", function () {
    var id = this.id;
    $.ajax({
        url: "/ToDo/UpdateQuanTam?CongViecId=" + id ,
        context: document.body,
        type: "Get",
        dataType: "html",
        success: function (data) {
            if (data == true || data == "True") {
                $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Update quan tâm thành công!");
                load(true);
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
$(document).on('click', ".iconStartTime", function () {
    var id = this.parentElement.className.replace("tdStart-", "");
  
    $.ajax({
        url: "/Time/CheckStartTime3?taskId=" +id,      
        context: document.body,
        type: "GET",
        dataType: "html",
        success: function (data) {
            var result = JSON.parse(data);
            if (result.Status == true) {               
                StartTime(id);            
            }
            else {
                var col = result.ItemId;
                if (col == '1') {
                    if (confirm(result.Message)) {
                        StartTime(id);                   
                    } 
                } else {
                    $.Notification.autoHideNotify('error', 'top right', 'Thất bại', result.Message);                  
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

function StartTime(taskId) {
    var data = {taskId:taskId };
    $.ajax({
        url: "/Time/StartTimer3",
        data: data,
        async: false,
        context: document.body,
        type: "POST",
        dataType: "html",
        success: function (data) {
            var data = JSON.parse(data);
            if (data.Status == true || data.Status == "True" || data.Status == "true") {
                $.Notification.autoHideNotify('success', 'top right', 'Thành công', data.Message + "<br/> Hệ thống sẽ load lại trang trong giây lát.");             
                initTimer();
                load(true);
              
            } else {
                $.Notification.autoHideNotify('error', 'top right', 'Thông báo', data.Message);
                console.log(Jdata.ItemId);
                console.log(JSON.parse(data.ItemId));
            }
        },
        error: function (xhr, status) {
        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
}
$(document).on("keyup", "#txtTenShipF", function () {
/*$("#txtTenShipF").keyup(function (event) {*/
    var arrayDuAn = [];
    var arrayShipId = [];
    var value = $("#txtTenShipF").val().toLowerCase();  
    value = removeAccents(value);
    var col1 = value.split(" ");
    var table = document.getElementById("tableToDoWork");
    var tr = table.getElementsByTagName("tr");
    for (i = 0; i < tr.length; i++) {    
        var td = tr[i].getElementsByTagName("td")[1];
        if (td) {
            var txtValue = td.textContent || td.innerText;
            txtValue = removeAccents(txtValue);
            $.each(col1, function (index, value2) {             
                if (txtValue.toLowerCase().indexOf(value2) > -1) {
                    var col = tr[i].className.split("-");;
                    if (jQuery.inArray(col[3], arrayDuAn) == -1) {
                        arrayDuAn.push(col[3]);
                    }
                    if (jQuery.inArray(col[1], arrayShipId) == -1) {
                        arrayShipId.push(col[1]);
                    }
                } 
            })
           
        }
    }    
    var trs = $("#tableToDoWork tr");
    if (trs.length > 0) {
        $.each(trs, function (index, value) {
            if (index > 0) {
                var id = value.id;
                var col = classname.split("-");
                var show = false;
                if (col[0] == "trDuAn" && jQuery.inArray(col[1], arrayDuAn) != -1) {
                    show = true;
                }
                if (col[0] == "ship" && jQuery.inArray(col[1], arrayShipId) != -1) {
                    show = true;
                }
                if (show) value.hidden=false;
                else value.hidden = true;
            }
           
        })
    }   
});
function removeAccents(str) {
    var AccentsMap = [
        "aàảãáạăằẳẵắặâầẩẫấậ",
        "AÀẢÃÁẠĂẰẲẴẮẶÂẦẨẪẤẬ",
        "dđ", "DĐ",
        "eèẻẽéẹêềểễếệ",
        "EÈẺẼÉẸÊỀỂỄẾỆ",
        "iìỉĩíị",
        "IÌỈĨÍỊ",
        "oòỏõóọôồổỗốộơờởỡớợ",
        "OÒỎÕÓỌÔỒỔỖỐỘƠỜỞỠỚỢ",
        "uùủũúụưừửữứự",
        "UÙỦŨÚỤƯỪỬỮỨỰ",
        "yỳỷỹýỵ",
        "YỲỶỸÝỴ"
    ];
    for (var i = 0; i < AccentsMap.length; i++) {
        var re = new RegExp('[' + AccentsMap[i].substr(1) + ']', 'g');
        var char = AccentsMap[i][0];
        str = str.replace(re, char);
    }
    return str;
}
$(document).on('click', ".iconPauseTime", function () {
    var id = this.parentElement.className.replace("tdStart-", "");
    var day = this.parentElement.id
    data = { toDoId: id }
    $.ajax({
        url: '/Time/StopTimer2',
        context: document.body,
        type: "POST",
        async: false,
        data: data,
        dataType: "html",
        success: function (data) {
            var result = JSON.parse(data);
            if (result.Status) {
                timer.stop();
                $.Notification.autoHideNotify('success', 'top right', 'Thành công', 'Stop time thành công.');
                setTimeout(
                    function () {
                        location.href = strUrl;                      
                    }, 1000);
            } else {
                $.Notification.autoHideNotify('error', 'top right', 'Thông báo', result.Message);
            }

            //var html = '<a href="#" onclick="openLogTime()" class="waves-effect waves-light" id="open_log"><i class="fal fa-stopwatch"></i></a>';
            //$("#timer").html(html);  
            //var lst = $(".tdStart-" + id);
            //if (lst.length > 0) {
            //    $.each(lst, function (index, value) {
            //        value.innerHTML = ' <span class="showStartTime">'+day+'</span><i style="font-size:x-large" class="far fa-play-circle m-l-10 iconStartTime"></i>';
            //    })
            //}
        },
        error: function (xhr, status) {
            $.Notification.autoHideNotify('error', 'top right', 'Lỗi', "Có lỗi xảy ra khi tải dữ liệu!");
        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
})
$(document).on("click", ".addnewtask", function () {
    var id = this.id;
    shipableId = id.split("-")[0]; 
    $("#addTaskModal").modal("show");
    $("#tbdyHangMuc2").html('<tr id="LastRow2"><td></td><td> <input class="form-control" id="LastInput2" type="text"></td><td></td><td></td></tr>');
})
$(document).on("change", "#LastInput2", function () {
    var text = this.value;
    //var check = $("#LastCheckBox").checked;
    if ($.trim(text) != "") {
        var str = '<tr id="trHM2' + newRowHM + '"><td> <input class="m-t-15" id="chkHM2' + newRowHM + '" type="checkbox" checked';
        //if (check) str += 'checked'
        str += '></td> <td> <input class="form-control" id="txtHM2' + newRowHM + '" type="text" value="' + text + '"></td><td><select id="cboNguoiDung2' + newRowHM + '" class="form-control"><option value="0"> Chọn người xử lý </option>';
        if (dataUser && dataUser.length > 0) {
            $.each(dataUser, function (index, value) {
                str += '<option value=' + value.PhongBanId + ' disabled>' + value.TenPhongBan + '</option>';
                if (value.NguoiDungs && value.NguoiDungs.length > 0) {
                    $.each(value.NguoiDungs, function (index2, value2) {
                        str += '<option value="' + value2.NguoiDungId + '"';
                        if (value2.NguoiDungId == LoginUserId) str += ' selected ';
                        str += ' > &nbsp;&nbsp; ' + value2.HoTen + '</option>';
                    })
                }
            })
        }
        str += '</select></td><td><a id="' + newRowHM + '" class="removeHM2"><i class="fa fa-trash-o"></i></a></td></tr>'

        $(str).insertBefore("#LastRow2");
        //$("#LastCheckBox").checked = false;
        $("#LastInput2").val("");
    }
    newRowHM--;
})
$(document).on("click", ".removeHM2", function () {
    var id = this.id;
   
    $("#trHM2" + id).remove();
})
$(document).on("click", "#btnSubmitTask", function () {
    if (checkInsertShip == true) {
        checkInsertShip = false;
        var check = true;
        var ship = { CongViecId: shipableId }      
           
                var lstTask = []
                var rows = $("tbody#tbdyHangMuc2").find("tr");
                var countCheck = 0;
                if (rows.length > 0) {
                    $.each(rows, function (index, value) {
                        var rowId = value.id;
                        if (rowId != "LastRow2") {
                            rowId = rowId.replace("trHM2", "");
                            var txtTenHangMuc = $("#txtHM2" + rowId).val();
                            var trangThaiHangMuc = $("#chkHM2" + rowId)[0].checked;
                            if (trangThaiHangMuc && $.trim(txtTenHangMuc != "")) {
                                var nguoidungId = $("#cboNguoiDung2" + rowId).val();
                                if (nguoidungId == 0 || nguoidungId == "0") {
                                    $.Notification.autoHideNotify('error', 'top right', 'Lỗi', "Bạn hãy chọn người xử lý cho các công việc cần làm!");
                                    checkInsertShip = true;    
                                    check = false;
                                    return;
                                }
                                lstTask.push({ TenCongViec: txtTenHangMuc, NguoiXuLyId: nguoidungId })
                            }
                        }
                    })
        }
        if (check) {
        ship.CongViecs = lstTask;
        if (lstTask.length > 0) {
           
                $.ajax({
                    url: "/ToDo/AddTask",
                    data: ship,
                    context: document.body,
                    type: "POST",
                    dataType: "html",
                    async: false,
                    success: function (data) {
                        if (data == true || data == "True") {
                            $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Thêm mới task thành công!");
                            checkAll = true;
                            $("#addTaskModal").modal("hide");
                            checkInsertShip = true;
                            //setTimeout(
                            //    function () {
                            //        location.reload();
                            //    }, 100);
                            load(true);
                        } else {
                        }
                    },
                    error: function (xhr, status) {
                        checkInsertShip = true;
                    },
                    complete: function (xhr, status) {
                        checkInsertShip = true;
                        //$('#showresults').slideDown('slow')                 
                    }
                });
            }
        else {
            $.Notification.autoHideNotify('error', 'top right', 'Lỗi', "Bạn hãy chọn task cần thêm!");
            checkInsertShip = true;
        }

          
        } 
            
          
        
        
        if (checkAll == false) checkInsertShip = true;
    }
})
$(document).on("change", "#cboTemplate", function () {
    var value = $("#cboTemplate").val();
    if (value == 0 || value == "0") {
        $("#tbdyHangMuc").html('<tr id="LastRow"><td></td><td> <input class="form-control" id="LastInput" type="text"></td><td></td><td></td></tr>')
    } else {
        $.ajax({
            url: "/ToDo/GetTeamplateById?id=" + value,
            context: document.body,
            type: "Get",
            dataType: "html",
            success: function (data) {
                var temp = JSON.parse(data);
                if (temp) {
                    $("#txtTenShipable").val(temp.TenShip);
                    editor.value(temp.NoiDung);
                    var lstTask = JSON.parse(temp.StrTask);
                    if (lstTask && lstTask.length > 0) {
                        $("#tbdyHangMuc").html('<tr id="LastRow"><td></td><td> <input class="form-control" id="LastInput" type="text"></td><td></td><td></td></tr>')
                        $.each(lstTask, function (index, value) {
                            var str = '<tr id="trHM' + newRowHM + '"><td> <input class="m-t-15" id="chkHM' + newRowHM + '" type="checkbox" checked';
                            //if (check) str += 'checked'
                            str += '></td> <td> <input class="form-control" id="txtHM' + newRowHM + '" type="text" value="' + value.TenCongViec + '"></td><td><td><select id="cboNguoiDung' + newRowHM + '" class="form-control"><option value="0"> Chọn người xử lý </option>';
                            if (dataUser && dataUser.length > 0) {
                                $.each(dataUser, function (index2, value2) {
                                    str += '<option value=' + value2.PhongBanId + ' disabled>' + value2.TenPhongBan + '</option>';
                                    if (value2.NguoiDungs && value2.NguoiDungs.length > 0) {
                                        $.each(value2.NguoiDungs, function (index3, value3) {
                                            str += '<option value="' + value3.NguoiDungId + '"'
                                            if (value.NguoiXuLyId == value3.NguoiDungId) str += ' selected';
                                            str += ' > &nbsp;&nbsp; ' + value3.HoTen + '</option>';
                                        })
                                    }
                                })




                               
                            }
                            str += '</select></td><td><a id="' + newRowHM + '" class="removeHM"><i class="fa fa-trash-o"></i></a></td></tr>'

                            $(str).insertBefore("#LastRow");
                            newRowHM--;
                        })
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
    }

   
})
$(document).on("change", "#LastInputM", function () {
    var text = this.value;
    //var check = $("#LastCheckBox").checked;
    if ($.trim(text) != "") {
        var str = '<tr id="trHMM' + newRowHM + '"><td> <input class="m-t-15" id="chkHMM' + newRowHM + '" type="checkbox"';
        //if (check) str += 'checked'
        str += '></td> <td> <input class="form-control" id="txtHMM' + newRowHM + '" type="text" value="' + text + '"></td><td><a id="' + newRowHM + '" class="removeHMM"><i class="fa fa-trash-o"></i></a></td></tr>'

        $(str).insertBefore("#LastRowM");
        //$("#LastCheckBox").checked = false;
        $("#LastInputM").val("");
    }
    newRowHM--;
})
$(document).on("click", ".removeHMM", function () {
    var id = this.id;

    $("#trHMM" + id).remove();
})
$(document).on("click", ".LuuMauShip", function ()
{
    var id = this.id;
    var shipId = id.replace("btnLuuMau-", "");
    $.ajax({
        url: "/ToDo/GetInfoShip?shipId=" + shipId,
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            result = JSON.parse(data);
            $("#txtTenShipableM").val(result.TenCongViec);
            editor2.value(result.MoTa);
            if (result.CongViecs && result.CongViecs.length > 0) {
                $.each(result.CongViecs, function (index, value) {
                    var str = '<tr id="trHMM' + newRowHM + '"><td> <input class="m-t-15" id="chkHMM' + newRowHM + '" type="checkbox" checked';
             
                    str += '></td> <td> <input class="form-control" id="txtHMM' + newRowHM + '" type="text" value="' + value.TenCongViec + '"></td><a id="' + newRowHM + '" class="removeHMM"><i class="fa fa-trash-o"></i></a></td></tr>';
                    $(str).insertBefore("#LastRowM");
                    newRowHM--;
                })      
            }
            $("#addTemplate").modal("show");
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });

    
})
$(document).on("click", "#btnSubmitIM", function () {
    var mau = {}
    mau.TenMau = $("#txtTenMauShipableM").val();
    mau.TenShip = $("#txtTenShipableM").val();
    mau.NoiDung = editor2.value();
    var lstTask = []
    var rows = $("tbody#tbdyHangMuc").find("tr");
    var countCheck = 0;
    if (rows.length > 0) {
        $.each(rows, function (index, value) {
            if (check) {
                var rowId = value.id;
                if (rowId != "LastRow") {
                    rowId = rowId.replace("trHM", "");
                    var txtTenHangMuc = $("#txtHM" + rowId).val();
                    var trangThaiHangMuc = $("#chkHM" + rowId)[0].checked;
                    if (trangThaiHangMuc && $.trim(txtTenHangMuc != "")) {
                        var nguoidungId = $("#cboNguoiDung" + rowId).val();
                        if (nguoidungId == 0 || nguoidungId == "0") {
                            $.Notification.autoHideNotify('error', 'top right', 'Lỗi', "Bạn hãy chọn người xử lý cho các công việc cần làm!");
                            checkInsertShip = true
                            checkAll = true;
                            check = false;
                            return;
                        }
                        lstTask.push({ TenCongViec: txtTenHangMuc, NguoiXuLyId: nguoidungId })
                    }
                }
            }


        })
    }
    mau.StrTask = JSON.stringify(lstTask);
    var check = true;
    var strError = "";
    if ($.trim(mau.TenMau) == "") {
        strError += "Tên mẫu ship không được để trống.";
        check = false;
    }
    if ($.trim(mau.TenShip) == "") {
        strError += "Tên ship không được để trống.";
        check = false;
    }
    if ($.trim(mau.NoiDung) == "") {
        strError += "Mô tả ship không được để trống.";
        check = false;
    }
    if (lstTask.length == 0) {
        strError += "Bạn hãy thêm task cho mẫu ship.";
        check = false;
    }
    if (check) {
        $.ajax({
            url: "/ToDo/InsertTemplate",
            data: mau,
            context: document.body,
            type: "POST",
            async: false,
            dataType: "html",
            success: function (data) {
                if (data > 0) {
                    $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Thêm mẫu shipable thành công!");
                    checkAll = true;
                    setMau();
                    $("#addTemplate").modal("hide");
                } else {
                    $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Tên mẫu shipable đã tồn tại!");
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
function setMau() {
    $.ajax({
        url: "/ToDo/GetAllTeamplate",
        context: document.body,
        type: "Get",
        dataType: "html",
        success: function (data) {
            var temp = JSON.parse(data);
            if (temp && temp.length) {
                var str = '<option value="0">Chọn Template </option>';
                $.each(temp, function (index, value) {
                    str += '<option value="' + value.MauGoiChuyenGiaoId + '">' + value.TenMau + ' </option>';                 
                })
                $("#cboTemplate").html(str);
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


// detail todo
$("#MoTaToUpdate").kendoEditor({
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
function detailTodo(id) {    
        var motaUpdate = $("#MoTaToUpdate").data("kendoEditor");
        $.ajax({
            url: "/Todo/GetCongViecById?CongViecId=" + id,
            context: document.body,
            type: "GET",
            dataType: "html",
            success: function (data) {
                var result = JSON.parse(data);
                console.log(result);
                $("#DuAnIdToUpdate").val(result.DuAnChaId);
                changeDuAnDT(result.DuAnChaId);
                $("#CongViecIdToUpdate").val(result.CongViecId);
                $("#ShipIdUpdateTodo").val(result.KhoaChaId);
                $("#TuanToUpdate").val(result.Tuan);
                $("#GiaiDoanDuAnUpdateTodo").val(result.DuAnId);
                $("#TenCongViecToUpdate").val(result.TenCongViec);
                motaUpdate.value(result.MoTa);
                $("#TenKhoaCha").val(result.TenKhoaCha);
                $("#MaCongViecToUpdate").val(result.MaCongViec);
                $("#NgayDuKienHoanThanhToUpdate").val(result.StrNgayDuKienHoanThanhFull);
                $("#ThoiGianUocLuongToUpdate").val(result.ThoiGianUocLuong);
                $("#DiemPointToUpdate").val(result.DiemPoint);
                $("#ListCongViecInShip").html('');
                $("#ThuTuUuTienToUpdate").val(result.ThuTuUuTien);
                $("#DoPhucTapToUpdate").val(result.DoPhucTap);
                $("#TenKhoaCha").text(result.TenKhoaCha);
               
                $('#TrangThaiCongViecToUpdate').selectpicker('val', result.TrangThaiCongViecId);
                getNguoiDungForTodo(result.DuAnId);
                getLoaiCongViec(result.DuAnId);
                $('#LoaiCongViecToUpdate').val(result.LoaiCongViecIds).trigger('change');
                $('#NguoiXuLyToUpdate').val(result.NguoiXuLyId).trigger('change');
                $('#NguoiHoTroToUpdate').val(result.NguoiHoTroId).trigger('change');
                $('#detailsTodoModal').modal('show');
            },
            error: function (xhr, status) {
            },
            complete: function (xhr, status) {
                //$('#showresults').slideDown('slow')
            }
        });
  

}
$(document).on("click", ".EditTaskT", function () {
    var id = this.id;
    detailTodo(id);
})
$(document).on("click", ".deleteTaskT", function () {
    var id = this.id;
    if (confirm("Bạn có muốn xóa Task này không?")) {
        $.ajax({
            url: "/ToDo/DeleteTask?taskId=" + id,
            context: document.body,
            type: "POST",
            dataType: "html",
            success: function (data) {
                if (data == "True" || data == "true" || data == true) {
                    $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Xóa task thành công!");
                    setTimeout(
                        function () {
                            location.href = strUrl; 
                          /*  location.reload();*/
                        }, 1000);
                } else {
                    $.Notification.autoHideNotify('error', 'top right', 'Thành công', "Xóa task không thành công!");
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
function updateTodo() {
    var userId = '@userLoged.NguoiDungId';
    var mota2 = $("#MoTaToUpdate").data("kendoEditor");
    var CongViecId = $("#CongViecIdToUpdate").val();
    var DuAnId = $("#GiaiDoanDuAnUpdateTodo").val();
    var KhoaChaId = $("#ShipIdUpdateTodo").val(); 
    var Tuan = $("#TuanToUpdate").val();
    var TenCongViec = $("#TenCongViecToUpdate").val();
    var MoTa = mota2.value();
    var date = $("#NgayDuKienHoanThanhToUpdate").val();
    var TrangThaiCongViecId = $("#TrangThaiCongViecToUpdate").val();
    var ThuTuUuTien = $('select[id=ThuTuUuTienToUpdate]').val();
    var DoPhucTap = $('select[id=DoPhucTapToUpdate]').val();
    var DiemPoint = $("#DiemPointToUpdate").val();
    var NguoiXuLyId = $("#NguoiXuLyToUpdate").val();
    var HoTenNguoiXuLy = $("#userList option:selected").text();
    var NguoiHoTroId = $("#NguoiHoTroToUpdate").val();
    var LoaiCongViecIds = $("#LoaiCongViecToUpdate").val();
    var ThoiGianUocLuong = $("#ThoiGianUocLuongToUpdate").val();   
        $.ajax({
            url: "/Todo/UpdateTodo",
            context: document.body,
            type: "POST",
            data: {
                CongViecId: CongViecId,
                DuAnId: DuAnId,
                KhoaChaId: KhoaChaId,         
                TenCongViec: TenCongViec,
                Tuan: Tuan,
                LaShipAble: false,
                TenCongViec: TenCongViec,
                MoTa: MoTa,
                MaCongViec: null,
                TrangThaiCongViecId: TrangThaiCongViecId,
                date: date,
                ThuTuUuTien: ThuTuUuTien,
                DoPhucTap: DoPhucTap,
                DiemPoint: DiemPoint,
                NguoiXuLyId: NguoiXuLyId,
                NguoiHoTroId: NguoiHoTroId,
                LoaiCongViecIds: LoaiCongViecIds,
                ThoiGianUocLuong: ThoiGianUocLuong,
            },
            dataType: "html",
            success: function (data) {
                var result = JSON.parse(data);
                if (result.Status) {
                    $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Cập nhật công việc thành công!");
                    setTimeout(
                        function () {
                            location.href = strUrl; 
                           /* location.reload();*/
                        }, 100);
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
function getNguoiDungForTodo(id) {
    $('#userList').html('');
    $('#userSupportList').html('');
    $.ajax({
        url: "/Todo/GetNguoiDung?DuAnId=" + id,
        type: "GET",
        async: false,
        success: function (data) {
            var result = JSON.parse(data);
            var data = $.map(result, function (obj) {
                obj.id = obj.id || obj.NguoiDungId; // replace pk with your identifier
                obj.text = obj.text || obj.HoTen
                return obj;
            });
            $('#NguoiXuLyToUpdate').select2({
                data: data
            });
            $('#NguoiHoTroToUpdate').select2({
                data: data
            });
        },
        error: function (xhr, status) {
            alert('Error load data!')
        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
}
function getLoaiCongViec(id) {
    $.ajax({
        url: "/Todo/GetLoaiCongViec?DuAnId=" + id,
        type: "GET",
        async: false,
        success: function (data) {
            var result = JSON.parse(data);
            var data = $.map(result, function (obj) {
                obj.id = obj.id || obj.LoaiCongViecId; // replace pk with your identifier
                obj.text = obj.text || obj.TenLoaiCongViec
                return obj;
            });
            $('#LoaiCongViecToUpdate').select2({
                data: data,
                tags: true,
                tokenSeparators: [',', ' ']
            });
        },
        error: function (xhr, status) {
            alert('Error load data!')
        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
}
$(document).on('click', "#btnSubmitQT", function () {
    $("#divSaveQuyTrinh").show();
    $("#txtTenQuyTrinh").val("");
})
$(document).on('click', "#btnHuyQT", function () {
    $("#divSaveQuyTrinh").hide();
})
$(document).on('click', "#btnLuuQT", function () {
    var check = true;
    var mau = {}
    mau.TenMau = $("#txtTenQuyTrinh").val();
    mau.TenShip = $("#txtTenShipable").val();
    mau.NoiDung = editor.value();
    var lstTask = []
    var rows = $("tbody#tbdyHangMuc").find("tr");
    var countCheck = 0;
    if (rows.length > 0) {
        $.each(rows, function (index, value) {
            if (check) {
                var rowId = value.id;
                if (rowId != "LastRow") {
                    rowId = rowId.replace("trHM", "");
                    var txtTenHangMuc = $("#txtHM" + rowId).val();
                    var trangThaiHangMuc = $("#chkHM" + rowId)[0].checked;
                    if (trangThaiHangMuc && $.trim(txtTenHangMuc != "")) {
                        var nguoidungId = $("#cboNguoiDung" + rowId).val();
                        if (nguoidungId == 0 || nguoidungId == "0") {
                            $.Notification.autoHideNotify('error', 'top right', 'Lỗi', "Bạn hãy chọn người xử lý cho các công việc cần làm!");                           
                            check = false;
                            return;
                        }
                        lstTask.push({ TenCongViec: txtTenHangMuc, NguoiXuLyId: nguoidungId })
                    }
                }
            }


        })
    }
    if (lstTask.length > 0) mau.StrTask = JSON.stringify(lstTask);
    var check = true;
    var strError = "";
    if ($.trim(mau.TenMau) == "") {
        strError += "Tên quy trình không được để trống.";
        check = false;
    }
    if ($.trim(mau.TenShip) == "") {
        strError += "Tên ship không được để trống.";
        check = false;
    }
    if ($.trim(mau.NoiDung) == "") {
        strError += "Mô tả ship không được để trống.";
        check = false;
    }
    if (lstTask.length == 0) {
        strError += "Bạn hãy thêm task cho quy trình.";
        check = false;
    }
    if (check) {
        $.ajax({
            url: "/ToDo/InsertTemplate",
            data: mau,
            context: document.body,
            type: "POST",
            async: false,
            dataType: "html",
            success: function (data) {
                if (data > 0) {
                    $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Lưu quy trình thành công!");                
                    setMau();
                    $("#divSaveQuyTrinh").hide();
                } else {
                    $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Tên quy trình đã tồn tại!");
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
        $.Notification.autoHideNotify('error', 'top right', 'Thông báo', strError);
    }
})
///detail ship
function getDataTrangThaiCongViecBy(id) {
    var result;
    $.ajax({
        url: "/ToDo/GetTrangThaiShipablesBy?ttId=" + id,
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
$(document).on('click', ".shipdetails", function () {
    shipableId = this.id;
    $.ajax({
        url: "/ToDo/GetDetailShipById?shipId=" + shipableId,
        context: document.body,
        type: "Get",
        dataType: "html",
        success: function (data) {
            var ship = JSON.parse(data);
            $("#txtTenShipableDT").val(ship.TenCongViec);
            $("#cboDuAnDT").val(ship.DuAnChaId);
            changeDuAnDT(ship.DuAnChaId);
            $("#cboGiaiDoanDuAnDT").val(ship.DuAnId)
            $("#TuanBatDauDT").text(ship.Tuan);
            var result = getDataTrangThaiCongViecBy(ship.TrangThaiCongViecId);        
            if (result && result.length > 0) {
                var str2 = "";
                $.each(result, function (index22, value22) {
                    str2 += '<option value="' + value22.TrangThaiCongViecId + '">' + value22.TenTrangThai + '</option>"';
                })
            }
            $("#cboTrangThaiDT").html(str2);
            $('#cboTrangThaiDT').val(ship.TrangThaiCongViecId);
            $("#txtDeadLineDT").val(ship.StrNgayDuKienHoanThanh);
            $("#cboNguoiXuLyShip").val(ship.NguoiXuLyId);
            editorDT.value(ship.MoTa);
            var str = "";
            if (ship.CongViecs != null && ship.CongViecs.length > 0) {

                $.each(ship.CongViecs, function (index, value) {
                    str += '<tr><td> ' + value.TenCongViec + '</td>'
                    str += '<td class="ship-able-status" ><div class="label font-14 label-table label-' + value.MaMauTrangThaiCongViec + '">' + value.TenTrangThai + '</div> </td>';
                    str += '<td>';
                    if (value.NguoiDungs && value.NguoiDungs.length > 0) {
                        $.each(value.NguoiDungs, function (index2, value2) {
                            str += '<img src="/Assets/images/Avatar/' + value2.Avatar + '" alt="user-img" class="img-circle m-r-5" height="25" title="' + value2.HoTen + '" width="25">'
                        })
                    }
                    str += '</td>'
                    str += '<td>' + value.StrTotalTime + '</td> </tr > '
                })
                if (ship.StrTotalTime) {
                    str += '<tr><td colspan="3">Tổng thời gian đã log: </td><td> ' + ship.StrTotalTime+'</td></tr>'
                }
            } else {
               str+=' <tr><td colspan="4"> Không có task cần làm </td></tr>'
            }
            $("#ListCongViecInShip").html(str);

            $("#shipDetail").modal("show");
        },
        error: function (xhr, status) {
            $.Notification.autoHideNotify('error', 'top right', 'Lỗi', "Vui lòng thử lại sau!");
        },
        complete: function (xhr, status) {
          
        }
    });
  
})
$("#btnUpdateShip").click(function () {
    var ship = { CongViecId: shipableId };
    ship.TenCongViec = $("#txtTenShipableDT").val();
    ship.DuAnId = $("#cboGiaiDoanDuAnDT").val();
    ship.TrangThaiCongViecId = $("#cboTrangThaiDT").val();
    ship.NguoiXuLyId = $("#cboNguoiXuLyShip").val();
    ship.MoTa = editorDT.value();
    ship.StrNgayDuKienHoanThanh = $("#txtDeadLineDT").val();
    var check = true;
    if ($.trim(ship.TenCongViec) == "" || ship.DuAnId == "0"||ship.DuAnId=="") {
        check = false;
    }
    if (check) {
        $.ajax({
            url: "/ToDo/UpdateShipable",
            data: ship,
            context: document.body,
            type: "POST",
            dataType: "html",
            async: false,
            success: function (data) {
                if (data > 0) {
                    $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Update shipable thành công!");
                    checkAll = true;
                    setTimeout(
                        function () {
                            location.href = strUrl; 
                            /*location.reload();*/
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
    } else {
        $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Các trường bắt buộc (*) không được để trống!");
    }
})
$(document).on('click', ".AddTimeWTT", function () {
    taskIdA = this.id;
    $.ajax({
        url: "/ToDo/GetInfoTask?taskId=" + taskIdA,
        context: document.body,
        type: "Get",
        dataType: "html",
        success: function (data) {         
            var task = JSON.parse(data);
            if (task) {
                $("#cboDuAnQTA").html('<option value="' + task.DuAnId + '">' + task.TenDuAn + '</option>');
                $("#cboShipQTA").html('<option value="' + task.KhoaChaId + '">' + task.TenKhoaCha + '</option>');
                $("#cboTaskQTA").html('<option value="' + task.CongViecId + '">' + task.TenCongViec + '</option>');
                //$("#txtTimeFromQTA").val("");
                //$("#txtTimeToQTA").val("");
                //$("#txtNgayLamViecQTA").val("");
                $("#addQuickTimeModal").modal("show");
            } else {
                $.Notification.autoHideNotify('error', 'top right', 'Lỗi', "Vui lòng thử lại sau!");
            }
            
        },
        error: function (xhr, status) {
            $.Notification.autoHideNotify('error', 'top right', 'Lỗi', "Vui lòng thử lại sau!");
        },
        complete: function (xhr, status) {

        }
    });

})
$("#btnSubmitQTA").click(function () {
    if (checkInsertTime) {
        checkInsertTime = false;
        var congviec = {}
        congviec.CongViecId = taskIdA;
        congviec.StrThoiGianBatDau = $("#txtTimeFromQTA").val();
        congviec.StrThoiGianKetThuc = $("#txtTimeToQTA").val();
        congviec.StrNgayLamViec = $("#txtNgayLamViecQTA").val();
        $.ajax({
            url: "/Time/AddQTA",
            data: congviec,
            context: document.body,
            type: "POST",
            async: false,
            dataType: "html",
            success: function (data) {
                checkInsertTime = true;
                data = JSON.parse(data);
                if (data.Status == false) {
                    $.Notification.autoHideNotify('error', 'top right', 'Thông báo', data.Message);                 
                } else {                    
                    $.Notification.autoHideNotify('success', 'top right', 'Thành công', data.Message);
                    $("#addQuickTimeModal").modal("hide");
                    load(false);            
                }

            },
            error: function (xhr, status) {
                checkInsertTime = true;
            },
            complete: function (xhr, status) {
                checkInsertTime = true;
            }
        });
    }
   

})

$(document).on("change", ".ddlTTShipable", function () {
    var id = this.id;
    var ttid = $("#" + id).val();
    var shipID = id.split('-')[1];
    $.ajax({
        url: "/ToDo/UpdateStatus?CongViecId=" + shipID + "&&TrangThaiId="+ttid,
        context: document.body,
        type: "POST",
        dataType: "html",
        async: false,
        success: function (data) {
            if (data == "True" || data == true || data == "true") {
                $.Notification.autoHideNotify('success', 'top right', 'Thông báo', "Cập nhập trạng thái shipable thành công!");
            } else {
                $.Notification.autoHideNotify('error', 'top right', 'Lỗi', "Vui lòng thử lại sau!");
            }
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
})
$(document).on("change", ".ddlTTTask", function () {
    var id = this.id;
    var ttid = $("#" + id).val();
    var shipID = id.split('-')[1];
    $.ajax({
        url: "/ToDo/UpdateTrangThaiTask?CongViecId=" + shipID + "&&TrangThaiId=" + ttid,
        context: document.body,
        type: "POST",
        dataType: "html",
        async: false,
        success: function (data) {
            if (data == "True" || data == true || data == "true") {
                $.Notification.autoHideNotify('success', 'top right', 'Thông báo', "Cập nhập trạng thái công việc thành công!");
            } else {
                $.Notification.autoHideNotify('error', 'top right', 'Lỗi', "Vui lòng thử lại sau!");
            }
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
})