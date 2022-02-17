var shipableId = 0, editor;
var checkInsertShip = true;
var duAnId = $("#duAnIdM").val();
var giaiDoanIdM = $("#giaiDoanM").val();
var noiDungCmId = 0;
$(document).ready(function () {
    selectDuAn(duAnId);
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
    var dataGiaiDoan = getDataGiaiDoanDuAn(duAnId);
    if (dataGiaiDoan.length > 0) {
        var str = '<option value="0">Tất cả giai đoạn</option>';
        $.each(dataGiaiDoan, function (index, value) {
            str += '<option value="' + value.GiaiDoanDuAnId + '"';
            if (value.GiaiDoanDuAnId == giaiDoanIdM) {
                str += " selected";
            }
            str+='>' + value.TenGiaiDoan + '</option>'
        })
        $("#ddlGiaiDoan").html(str);
    }
    var heith = $("#abd").height();
    $("#dba").height(heith + 100);
    var width = $("#abd").width();
    $("#dba").width(width);
});
$(document).on("change", "#ddlGiaiDoan", function () {
    var giaiDoan = $("#ddlGiaiDoan").val();
    var str = "/Project/ProjectChart?duAnId=" + duAnId + "&&giaiDoanDuAnId=" + giaiDoan;
    location.href = str;
})
$(document).on('click', ".collapeGD", function () {
    var idE = this.id;
    id = idE.replace("linksCollap-", "");
    if ($("#" + idE).hasClass("collapedShip")) {
        $("#" + idE).removeClass("collapedShip");
        $("#icon-" + id).html('<a id="linksCollap-' + id + '" class="collapeGD"><i class="fas fa-angle-double-down"></i></a>');
        if ($(".collapeByGD-" + id).length > 0) {
            $.each($(".collapeByGD-" + id), function (index, value) {
                value.classList.remove("in");
                value.classList.add("out");
            })
        }
    } else {
        $("#icon-" + id).html('<a id="linksCollap-' + id + '" class="collapeGD"><i class="fas fa-angle-double-up"></i></a>');
        $("#" + idE).addClass("collapedShip");
        if ($(".collapeByGD-" + id).length > 0) {
            $.each($(".collapeByGD-" + id), function (index, value) {
                value.classList.remove("out");
                value.classList.add("in");
            })
        }
    }
   
})
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
function setSelectTrangThaiCongViec2(trangThaiShipable, shipalbeId) {
    var trangThaiCongViecData = getDataTrangThaiCongViecByKhoaCha2(trangThaiShipable, shipalbeId);
    if (trangThaiCongViecData.length > 0) {
        var str = '';
        $.each(trangThaiCongViecData, function (index, value) {
            str += '<option value="' + value.TrangThaiCongViecId + '">' + value.TenTrangThai + '</option>';
        })
        $("#cboTrangThai2").html(str);
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
function getDataWeek() {
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
    setSelectWeek(tuanHT, yearHT);
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

function getDataTrangThaiCongViecByKhoaCha2(chaId, shipId) {
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
                setSelectWeek(result.Tuan, result.Nam);
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
                    async: false,
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
$(".progress").popover({ trigger: "hover" });
$(document).on("click", ".popUpCM", function (index, value) {
    var id = this.id;
})
$(document).on("click", ".popUpCM", function () {
    var id = this.id;
    shipableId = id;
    $.ajax({
        url: "/Project/CommentShipable?shipAbleId=" + id ,
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {

            $("#modalCMBDY").html(data);

            $("#CommentModal").modal("show");
            $("#btnCancel").hide();
            noiDungCmId = 0;
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
})
$(document).on("click", "#btnSendCM", function () {
    var text = $("#txtNoiDung").val();
    var data = {}
    data.NoiDungChiTiet = $.trim(text);
    data.ShipAbleId = shipableId;
    if (noiDungCmId != 0) {
        data.NoiDungId = noiDungCmId;
    }   
    if ($.trim(text)!="") {
        if (noiDungCmId == 0) {
            $.ajax({
                url: "/Project/InsertCommentShip",
                data: data,
                context: document.body,
                type: "POST",
                dataType: "html",
                success: function (data) {
                    insertComment(data);
                    $("#txtNoiDung").val("");
                    $("#btnCancel").hide();
                    noiDungCmId = 0;
                },
                error: function (xhr, status) {
                },
                complete: function (xhr, status) {
                    //$('#showresults').slideDown('slow')
                }
            });
        } else {
            $.ajax({
                url: "/Project/EditCommentShip",
                data: data,
                context: document.body,
                type: "POST",
                dataType: "html",
                success: function (data) {
                    if (data == true || data == "true" || data == "True") {
                        $("#NDComment-" + noiDungCmId).text($.trim(text));
                    } else {
                        $.Notification.autoHideNotify('error', 'top right', 'Thành công', "Chỉnh sửa comment không thành công!");
                    }                  
                    $("#txtNoiDung").val("");
                    $("#btnCancel").hide();
                    noiDungCmId = 0;
                },
                error: function (xhr, status) {
                },
                complete: function (xhr, status) {
                    //$('#showresults').slideDown('slow')
                }
            });
        }
    }
})
function insertComment(noiDungId) {
    $.ajax({
        url: "/Project/PartialViewComment?noiDungId=" + noiDungId,
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            $("#ulChatM").prepend(data);
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
}

$(document).on("click", ".EditComment", function () {
    var id = this.id;
    noiDungCmId = id;
    $("#btnCancel").show();
    $("#txtNoiDung").val($.trim($("#NDComment-" + id).text()));
})
$(document).on("click", "#btnCancel", function () {   
    noiDungCmId = 0;
    $("#btnCancel").hide();
    $("#txtNoiDung").val("");
})
$(document).on('click', ".deleteComment", function () {
    var id = this.id;
    if (confirm("Bạn có muốn xóa Comment này không?")) {
        $.ajax({
            url: "/Project/DeleteComment?noiDungId=" + id,
            context: document.body,
            type: "POST",
            dataType: "html",
            success: function (data) {
                if (data == "True" || data == "true" || data == true) {
                    $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Xóa comment thành công!");
                    $("#Comment-" + id).remove();
                } else {
                    $.Notification.autoHideNotify('error', 'top right', 'Thành công', "Xóa comment không thành công!");
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