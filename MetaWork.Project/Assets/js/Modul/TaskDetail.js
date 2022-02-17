var todoId = 0; var timeEntryId = 0;
var quyen = $("#quyenM").val();
var taskId = $("#taskIdM").val();
var duAnId = $("#duAnIdM").val();
var now = $("#nowM").val();
var currentStartId = $("#currentStartId").val();
var CheckInsert = true;
$('#txtDate').datepicker({
    autoclose: true,
    format: 'dd/mm/yyyy', 
    todayHighlight: true
})
$(document).on('click', "#btnAddToDo", function () {
    $("#txtTenToDo").val("");
    $("#cboLoaiCongViec").val(0);
    $("#txtGhiChu").val("");
    $("#titleToDoModal").text("Thêm mới ToDo");
    todoId = 0;
    $("#addToDoModal").modal("show");
})
$(document).on('click', ".editToDo", function () {
    var id = this.id;
    $.ajax({
        url: "/Time/GetToDoById?todoId="+id,
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            if (data != "") {
                todoId = id;
                data = JSON.parse(data);
                $("#txtTenToDo").val(data.TenCongViec);
                $("#txtGhiChu").val(data.MoTa);
                $("#cboLoaiCongViec").val(data.LoaiCongViecIds[0]);
                $("#titleToDoModal").text("Chỉnh sửa ToDo")
                $("#addToDoModal").modal("show");
            }
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
})
$(document).on('click', "#btnSubmitToDo", function () {
    if (CheckInsert == true) {
        CheckInsert = false;
        var checkall = true;
        var toDo = {}
        toDo.KhoaChaId = taskId
        toDo.CongViecId = todoId;
        toDo.TenCongViec = $("#txtTenToDo").val();
        toDo.MoTa = $("#txtGhiChu").val();
        toDo.LoaiCongViecIds = [$("#cboLoaiCongViec").val()];
        toDo.DuAnId = duAnId
        toDo.NguoiXuLyId = $("#cboNguoiDung").val();
        if ($.trim(toDo.TenCongViec) != "" && toDo.LoaiCongViecIds[0] != "0" && toDo.LoaiCongViecIds != 0) {
            $.ajax({
                url: "/Time/InsertOrUpdateToDo",
                data: toDo,
                context: document.body,
                type: "POST",
                dataType: "html",
                success: function (data) {
                    CheckInsert = true;
                    if (data == "0" || data == 0) {
                      
                        if (todoId == 0 || todoId == "0") {
                            $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Thêm mới todo không thành công!");
                        } else {
                            $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Chỉnh sửa todo không thành công!");
                        }

                    } else {
                        if (todoId == 0 || todoId == "0") {
                            $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Thêm mới todo thành công!");
                        } else {
                            $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Chỉnh sửa todo thành công!");
                        }
                       
                        setTimeout(
                            function () {
                                location.reload();
                            }, 1000);

                    }

                },
                error: function (xhr, status) {
                    CheckInsert = true;
                },
                complete: function (xhr, status) {
                    //$('#showresults').slideDown('slow')
                }
            });
        }
        else {
            $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Tên todo và loại công việc không được để trống.");
            var checkall = false;
        }
        if (checkall == false)
            CheckInsert = true;
    }
});
$(document).on('click', "#btnAddTimeEntry", function () {
    $.ajax({
        url: "/Time/CheckToken",
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            if (data == "True" || data == true || data == "true") {
                $("#titleTimeEntryModal").text("Thêm mới TimeEntry");
                timeEntryId = 0;
                $("#txtTenTimeEntry").val("");
                $("#cboLoaiCongViec2").val(0);
                $("#txtGhiChu2").val("");
                $("#txtDate").val(now);
                $("#txtTimeFrom").val("08:00");
                $("#txtTimeTo").val("17:00")   
                $("#addTimeEntryModal").modal('show');
            } else {
                $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Bạn chưa lấy token ngày hôm nay!");
            }
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });


   
})
$(document).on('click', ".editTimeEntry", function () {
    var id = this.id;
    $.ajax({
        url: "/Time/GetTimeEntryById?timeEntryId=" + id,
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            if (data != "") {
                timeEntryId = id;
                data = JSON.parse(data);
                $("#txtDate").val(data.StrNgayLamViec);
                $("#txtTenTimeEntry").val(data.TenCongViec);
                $("#txtGhiChu2").val(data.MoTa);
                $("#cboLoaiCongViec2").val(data.LoaiCongViecIds[0]);
                $("#titleTimeEntryModal").text("Chỉnh sửa TimeEntry");
                if (quyen == 3 || quyen == "3") {
                    $("#cboTime").val(data.TimeValue);
                    if (data.XacNhanHoanThanh == true) {
                        $("#chkMaskeAsDoneModal")[0].checked = true;
                    } else {
                        $("#chkMaskeAsDoneModal")[0].checked = false;
                    }
                }
               
                $("#addTimeEntryModal").modal("show");
            }
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
})
$(document).on('click', "#btnSubmitTimeEntry", function () {
    if (CheckInsert == true) {
        CheckInsert = false;
        var checkall = true;
        timeEntry = {}
        timeEntry.CongViecId = timeEntryId;
        timeEntry.KhoaChaId = taskId;
        timeEntry.DuAnId = duAnId;
        timeEntry.TenCongViec = $("#txtTenTimeEntry").val();
        timeEntry.LoaiCongViecIds = [$("#cboLoaiCongViec2").val()]
        timeEntry.MoTa = $("#txtGhiChu2").val();
        timeEntry.StrThoiGianBatDau = $("#txtTimeFrom").val();
        timeEntry.StrThoiGianKetThuc = $("#txtTimeTo").val();
        if (quyen == 3 || quyen == "3") {
            //timeEntry.XacNhanHoanThanh = $("#chkMaskeAsDoneModal")[0].checked;
            timeEntry.StrNgayLamViec = $("#txtDate").val();
        }
        if (timeEntry.TenCongViec == "" || timeEntry.StrThoiGianBatDau == "" || timeEntry.StrThoiGianKetThuc == "") {
            $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Các trường không phải option không được để trống.!");
            checkall = false;
        } else {

            $.ajax({
                url: "/Time/InsertOrUpdateTimeEntry",
                data: timeEntry,
                context: document.body,
                type: "POST",
                dataType: "html",
                success: function (data) {
                    CheckInsert = true;
                    data = JSON.parse(data);
                    if (data.Status == false) {
                        $.Notification.autoHideNotify('error', 'top right', 'Thông báo', data.Message);                     
                    } else {
                        $.Notification.autoHideNotify('success', 'top right', 'Thành công', data.Message);
                        setTimeout(
                            function () {
                                location.reload();                              
                            }, 1000);
                    }
                },
                error: function (xhr, status) {
                    $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Error");       
                    CheckInsert = true;
                },
                complete: function (xhr, status) {
                }
            });
        }
        if (checkall == false)
        CheckInsert = true;
    }

})  
$(document).on('change', ".chkMakeAsDone", function () {
    var id = this.id;
    var check = this.checked;
    var data = { id: id.replace("chkMSD-",""),check :check}
    $.ajax({
        url: "/Time/PheDuyetToDoOrTimeEntry",
        data: data,
        context: document.body,
        type: "POST",
        dataType: "html",
        success: function (data) {
            if (data == true || data == "True" || data == "true") {
                refreshTotalTime();
            } else {
                $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Lỗi rồi!");
                setTimeout(
                    function () {
                        location.reload();
                    }, 2000);
            }
        },
        error: function (xhr, status) {
        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
})
function refreshTotalTime() {
    $.ajax({
        url: "/Time/GetTotalTime?congViecId=" + taskId,
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            if (data != "") {
                var col = data.split('/');
                $("#totalTimeDone").text("Done :" + col[0]);
                $("#totalTime").text("Total: " + col[1]);
            }
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
}
$(document).on("click", ".deleteRowToDo", function () {
    var id = this.id;
    swal({
        title: "Xóa công việc này?",
        text: "Bạn có chắc chắn muốn xóa không?",
        type: "warning",
        showCancelButton: true,
        cancelButtonText: 'Không',
        cancelButtonClass: 'btn-white',
        confirmButtonClass: 'btn-warning',
        confirmButtonText: "Có, xóa nó!",
        closeOnConfirm: true
    }, function () {          
            var data = { id: id }
            $.ajax({
                url: "/Time/DeleteToDo",
                data: data,
                context: document.body,
                type: "POST",
                dataType: "html",
                success: function (data) {
                    if (data == true || data == "True" || data == "true") {
                        $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Xóa todo thành công!");
                        setTimeout(
                            function () {
                                location.reload();
                            }, 1000);
                    } else {
                        $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Xóa toDo không thành công!");
                    }
                },
                error: function (xhr, status) {
                },
                complete: function (xhr, status) {
                    //$('#showresults').slideDown('slow')
                }
            });
    })
  
})
$(document).on("click", ".deleteRowTimer", function () {
    var id = this.id;
    swal({
        title: "Xóa time entry này?",
        text: "Bạn có chắc chắn muốn xóa không?",
        type: "warning",
        showCancelButton: true,
        cancelButtonText: 'Không',
        cancelButtonClass: 'btn-white',
        confirmButtonClass: 'btn-warning',
        confirmButtonText: "Có, xóa nó!",
        closeOnConfirm: true
    }, function () {        
            var data = { timeEntryId: id }
            $.ajax({
                url: "/Time/DeleteTimeEntry",
                data: data,
                context: document.body,
                type: "POST",
                dataType: "html",
                success: function (data) {
                    if (data == true || data == "True" || data == "true") {
                        $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Xóa timeEntry thành công!");
                        setTimeout(
                            function () {
                                location.reload();
                            }, 1000);
                    } else {
                        $.Notification.autoHideNotify('error', 'top right', 'Thông báo', "Xóa timeEntry không thành công!");
                    }
                },
                error: function (xhr, status) {
                },
                complete: function (xhr, status) {
                    //$('#showresults').slideDown('slow')
                }
            });
        })
   
})
$(document).on('click', ".iconStartTime", function () {
    var id = this.parentElement.className.replace("tdStart-","");
    data = {toDoId:id}
    $.ajax({
        url: "/Time/StartTimer2",
        data: data,
        context: document.body,
        type: "POST",
        dataType: "html",
        success: function (data) {
            var data = JSON.parse(data);
            if (data.Status == true || data.Status == "True" || data.Status == "true") {               
             
                if (currentStartId != "" && currentStartId != 0 && currentStartId != "0") {
                    $.Notification.autoHideNotify('success', 'top right', 'Thành công', data.Message +"<br/> Hệ thống sẽ load lại trang trong giây lát.");
                    setTimeout(
                        function () {
                            location.reload();
                        }, 1000);
                }
                else {
                    $.Notification.autoHideNotify('success', 'top right', 'Thành công', data.Message);
                    var lst = $(".tdStart-" + id);
                    if (lst.length > 0) {
                        $.each(lst, function (index, value) {
                            value.innerHTML = ' <i style="font-size:x-large" class="far fa-pause-circle iconPauseTime"></i>';
                        })
                    }
                    if (parseInt(data.ItemId) > 0) {
                        $.ajax({
                            url: '/Time/GetTimeLogedOfTask?todoId=' + id,
                            context: document.body,
                            type: "GET",
                            dataType: "html",
                            async: false,
                            success: function (countTime) {
                                console.log('timelog: ' + countTime)
                                var hours = Math.floor(countTime / 3600);
                                var minutes = Math.floor((countTime % 3600) / 60);
                                var seconds = Math.floor(countTime % 3600 % 60);
                                var html = '<div class="timer" onclick="openLogTime()"><span><span class="timer-run"><i class="fas fa-circle"></i></span><label class="font-13 font-w-500"><span id="hours"></span><span id="minutes"></span><span id="seconds"></span></label></span></div>';
                                $("#timer").html(html);

                                timer.start({ precision: 'seconds', startValues: { hours: hours, minutes: minutes, seconds: seconds } });
                                timer.addEventListener('secondsUpdated', function (e) {
                                    $('#hours').html(timer.getTotalTimeValues().hours.toString() + 'h ');
                                    $('#minutes').html(timer.getTimeValues().minutes.toString() + 'm ');
                                    $('#seconds').html(timer.getTimeValues().seconds.toString() + 's');
                                    $('#hours2').html(timer.getTotalTimeValues().hours.toString() + 'h ');
                                    $('#minutes2').html(timer.getTimeValues().minutes.toString() + 'm ');
                                    $('#seconds2').html(timer.getTimeValues().seconds.toString() + 's');
                                });


                            },
                            error: function (xhr, status) {
                                $.Notification.autoHideNotify('error', 'top right', 'Lỗi', "Có lỗi xảy ra khi bắt đầu công việc!");
                            },
                            complete: function (xhr, status) {
                                //$('#showresults').slideDown('slow')
                            }
                        });
                        renderListTodo('');
                    }
                    currentStartId = id;                 
                }

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
  

})
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
                $.Notification.autoHideNotify('success', 'top right', 'Thành công', 'Stop time thành công. <br/> Hệ thống sẽ load lại trang trong giây lát.');
                setTimeout(
                    function () {
                        location.reload();
                    }, 1000);
            } else {
                $.Notification.autoHideNotify('error', 'top right', 'Thông báo',result.Message);
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
$(document).on('click', "#btnMakeAsDoneTask", function () {
    swal({
        title: "Xác nhận hoàn thành?",
        text: "Bạn có chắc chắn muốn xác nhận hoàn thành không?",
        type: "warning",
        showCancelButton: true,
        cancelButtonText: 'Không',
        cancelButtonClass: 'btn-white',
        confirmButtonClass: 'btn-warning',
        confirmButtonText: "Có, hoàn thành nó!",
        closeOnConfirm: true
    }, function () {           
            $.ajax({
                url: "/Time/XacNhanHoanThanhTask?taskId=" + taskId,
                type: "GET",
                contentType: 'application/json; charset=utf-8',
                async: false,
                success: function (data) {
                    data = JSON.parse(data);
                    if (data.Status == false || data.Status == "false") {
                        $.Notification.autoHideNotify('error', 'top right', 'Thông báo', data.Message);
                    } else {                      
                        $.Notification.autoHideNotify('success', 'top right', 'Thông báo', data.Message);
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
        })
})
$(document).on('click', ".ddlTTCV", function () {
    var id = this.id;
    $.ajax({
        url: "/Time/UpdateTrangThaiTask?taskId=" + taskId +"&&trangThaiCongViecId="+id,
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            if ($.trim(data) != "") {
                $.Notification.autoHideNotify('success', 'top right', 'Thành công', "Thay đổi trạng thái công việc thành công");
                var col = data.split("-");
                document.getElementById("btnShowTT").className = "btn-" + col[0];
                $("#btnShowTT").html('<span class="m-r-15 l-l-15">' + col[1] + '</span>')
            } else {
                $.Notification.autoHideNotify('error', 'top right', 'Thành công', "Thay đổi trạng thái công việc không thành công");
            }
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
})