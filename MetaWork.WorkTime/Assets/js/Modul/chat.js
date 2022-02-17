// Declare a proxy to reference the hub.
var itemSelected = "";
var phongChatId = "";
var chat = $.connection.chatHub;
// Create a function that the hub can call to broadcast messages.

chat.client.broadcastMessage = function (type,id,phongchatid,name,avatar,date, message) {
    // Html encode display name and message.
    //var encodedName = $('<div />').text(name).html();
    //var encodedMsg = $('<div />').text(message).html();
    // Add the message to the page.
    //$('#discussion').append('<li><strong>' + encodedName
    //    + '</strong>:&nbsp;&nbsp;' + encodedMsg + '</li>');
    if (type == 1 && phongchatid == phongChatId) {
        var strhtml = ' <li class="person messageChat" data-chat="person4" id="' + id + '"><img src="/Assets/images/Avatar/' + avatar + '" alt="' + name + '"><span class="name">' + name + '</span><span class="status online"></span><span class="time">' + date + '</span><span class="preview"> ' + message+'</span></li>';
        $(".chat-room").append(strhtml);
    }   

};
// Get the user name and store it to prepend to messages.
//$('#displayname').val(prompt('Enter your name:', ''));
//// Set initial focus to message input box.
//$('#message').focus();
// Start the connection.
var datausser = getAllUser();
$.connection.hub.start().done(function () {
    /*  if (!chat) chat = $.connection.chatHub;*/
    $('#sendmessage').click(function () {
        // Call the Send method on the hub.
        var text = $('#messageContent').val();
        var ps = "";
        $('#messageContent').val(" ");
        chat.server.sendChatMessage(phongChatId, text);
        // Clear text box and reset focus for next comment.
      
    });
});
$(document).ready(function () {
    loadUser();
    loadChannel();
    //$('textarea.mention1').mentiony({
    //    onDataRequest: function (mode, keyword, onDataRequestCompleteCallback) {

    //        //var data = [
    //        //    { id: 1, name: 'Nguyen Luat', 'avatar': 'https://goo.gl/WXAP1U', 'info': 'Vietnam', href: 'http://a.co/id' },
    //        //    { id: 2, name: 'Dinh Luat', 'avatar': 'https://goo.gl/WXAP1U', 'info': 'Vietnam', href: 'http://a.co/id' },
    //        //    { id: 3, name: 'Max Luat', 'avatar': 'https://goo.gl/WXAP1U', 'info': 'Vietnam', href: 'http://a.co/id' },
    //        //    { id: 4, name: 'John Neo', 'avatar': 'https://goo.gl/WXAP1U', 'info': 'Vietnam', href: 'http://a.co/id' },
    //        //    { id: 5, name: 'John Dinh', 'avatar': 'https://goo.gl/WXAP1U', 'info': 'Vietnam', href: 'http://a.co/id' },
    //        //    { id: 6, name: 'Test User', 'avatar': 'https://goo.gl/WXAP1U', 'info': 'Vietnam', href: 'http://a.co/id' },
    //        //    { id: 7, name: 'Test User 2', 'avatar': 'https://goo.gl/WXAP1U', 'info': 'Vietnam', href: 'http://a.co/id' },
    //        //    { id: 8, name: 'No Test', 'avatar': 'https://goo.gl/WXAP1U', 'info': 'Vietnam', href: 'http://a.co/id' },
    //        //    { id: 9, name: 'The User Foo', 'avatar': 'https://goo.gl/WXAP1U', 'info': 'Vietnam', href: 'http://a.co/id' },
    //        //    { id: 10, name: 'Foo Bar', 'avatar': 'https://goo.gl/WXAP1U', 'info': 'Vietnam', href: 'http://a.co/id' },
    //        //];

    //        data = jQuery.grep(datausser, function (item) {
    //            return item.name.toLowerCase().indexOf(keyword.toLowerCase()) > -1;
    //        });

    //        // Call this to populate mention.
    //        onDataRequestCompleteCallback.call(this, datausser);
    //    },
    //    timeOut: 0,
    //    debug: 1,
    //});
    $('textarea.mentions').mentionsInput({
        source: datausser,
        showAtCaret: true
    });
})
// Load user 
function loadUser() {
    ChatService.getListUser(false, function (data) {
        $(".users").html(data);
    })
}
function loadChannel() {
    ChatService.getListChannel(false, function (data) {
        $(".channels").html(data);
    })
}
$(document).on("click", ".channelChat", function () {
     itemSelected= this.id;
    ChatService.getChatChannel(itemSelected, false, function (data) {
        var duAn = JSON.parse(data);
        $(".selected-user").html('<span>To: <span class="name">' + duAn.TenDuAn + '</span></span>')
    })
    openChat();
    phongChatId = $("#phongchatid").val();
})
$(document).on("click", ".userChat", function () {
     itemSelected = this.id;
    ChatService.getChatUser(itemSelected, false, function (data) {
        var user = JSON.parse(data);
        $(".selected-user").html('<span>To: <span class="name">' + user.HoTen + '</span></span>')
    })
    openChat();
    phongChatId = $("#phongchatid").val();
})
function openChat() {
    ChatService.showRoom(itemSelected, false, function (data) {
        $("#chatContent").html(data);
    });
}

function getAllUser() {
   
    
  

    var result = [];
    $.ajax({
        url: "/Work/GetAllUser" ,
        context: document.body,
        type: "GET",
        dataType: "html",
        async: false,
        success: function (data) {
            var users = JSON.parse(data);
            //$.each(users, function (index, value) {
            //    var user = { id: value.NguoiDungId, name: value.HoTen, 'avatar': '/Assets/images/Avatar/' + value.Avatar, 'info': 'tecotec', href: '#' }
            //    result.push(user);
            //})
            $.each(users, function (index, value) {           
                var user = { value: removeAccents(value.HoTen), uid: value.NguoiDungId}
                result.push(user);
            })
        },
        error: function (xhr, status) {

        },
        complete: function (xhr, status) {
            //$('#showresults').slideDown('slow')
        }
    });
    return result;
}

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

