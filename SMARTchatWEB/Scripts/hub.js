$(function () {
    var chat = $.connection.chatHub;
    chat.client.addMessage = AddMessage;

    chat.client.onConnected = function (user, allUsers) {
        $('#userId').val(user.Id);
        $('#userName').val(user.Name);
        $('#userImage').val(user.Image);
        for (i = 0; i < allUsers.length; i++) {
            AddUser(allUsers[i]);
            if (allUsers[i].IsOnline) {
                markAsOnline(allUsers[i]);
            }
            else {
                markAsOffline(allUsers[i]);
            }
        }
        markAsOnline(user);
    }

    chat.client.loadMessagesHistory = function (messages, favMessages) {
        for (var i = 0; i < messages.length; i++) {
            AddMessage(messages[i]);
            for (var j = 0; j < favMessages.length; j ++) {
                if (messages[i].MessageId === favMessages[j].MessageId) {
                    MarkMessageAsFavourite(messages[i]);
                }
            }
        }
        scrollToBottom();
    }

    chat.client.onUserConnected = function (user, isNew) {
        if (isNew) {
            AddUser(user);
        }
        markAsOnline(user);
    }

    chat.client.onUserDisconnected = function (user) {
        markAsOffline(user);
    }

    chat.client.onMessageEdit = function (message) {
        $('#' + message.MessageId)
            .find('#messageText')
            .html(message.Content);
    }

    chat.client.onMessageDelete = function (message) {
        $('#' + message.MessageId).remove();
    }

    $.connection.hub.start().done(function () {
        window.addEventListener("beforeunload", function () {
            chat.server.disconnect();
        });

        $('#sendMessage').click(function () {
            var messageDTO = formMessageDTO();
            chat.server.send(messageDTO);
            $('textarea#message').val('');
            scrollToBottom();
        });

        $('#sendEditedMessage').click(function () {
            var messageId = $('tr.editing').attr('id');
            var messageDTO = formMessageDTO();
            messageDTO.MessageId = messageId;
            $('.editable').removeClass('editable');
            $('#sendEditedMessage').hide();
            $('#sendMessage').show();
            $('textarea#message').val('');
            chat.server.edit(messageDTO);
        });

        $('.messages').on('click', '.editMessage', function () {
            $('tr.editing').each(function () {
                $(this).removeClass('editing');
            });
            $(this).closest('tr').addClass('editing');
            var messageId = $(this).closest('tr').attr('id');
            var messageDTO = formMessageDTO(messageId);
            $('textarea#message').val(messageDTO.Content);
            $('#sendEditedMessage').show();
            $('#sendMessage').hide();
        });

        $('.messages').on('click', '.deleteMessage', function () {
            var messageId = $(this).closest('tr').attr('id');
            var messageDTO = formMessageDTO(messageId);
            chat.server.delete(messageDTO);
        });

        $('.messages').on('click', '.markFavourite', function () {
            var messageId = $(this).closest('tr').attr('id');
            var messageDTO = formMessageDTO(messageId);
            MarkMessageAsFavourite(messageDTO);
            chat.server.markAsFavourite(messageDTO);
        });
        $('.messages').on('click', '.userInfo', function () {
            $.ajax({
                url: 'Chat/Pro',
                type: 'POST',
                dataType: 'json',
                data: { id: $(this).html() },
                success: function(response) { window.location = response.url }
            });
        });
        $('.messages').on('click', '.selectMessage', function () {
            $(this).closest('tr').toggleClass('selected');
        });

        var channelDTO = formChannelDTO();
        chat.server.connect(channelDTO);
    });
});

function AddUser(user) {
    $(".users").append('<li id="' + user.Id + '"class="fa fa-user"><span>'
        + user.Name + '</span></li>');
};

function AddMessage(message) {
    var usrImg = '<img src="data:image/png;base64,' + message.User.Image + '" alt="User icon" id="user_icon">';
    var msgHead = '<li id="messageInfo">' + '<a href="#" class="userInfo">'+ message.User.Name + '</a>'
        + '<span>&nbsp;&nbsp;' + message.SendTime + '</span></li>';

    var msgBody = '<li id="messageText">' + message.Content + '</li>';
    if (message.Parents && message.Parents.length > 0) {
        msgBody = '<li id="messageQuotes">' + MessageHistory(message) + '</li>' + msgBody;
    }

    var msgEdit = '<button type="button" class="editMessage" > <span class="fa fa-pencil" aria-hidden="true"></span></button>';
    var msgDelete = '<button type="button" class="deleteMessage"> <span class="fa fa-trash" aria-hidden="true"></span></button>';
    var msgFavourite = '<button type="button" class="markFavourite"> <span class="fa fa-star" aria-hidden="true"></span></button>';
    var msgSelect = '<button type="button" class="selectMessage"> <span class="fa fa-reply" aria-hidden="true"></span></button>';

    var stringBuilder = '<tr class="messageBlock" id="' + message.MessageId
        + '"><td class="avatar" valign="top">' + usrImg + '</td>'
        + '<td class="mailInfo" valign="top"><ul>' + msgHead + msgBody + '</ul></td>';
    if (message.User.Id ==  $('#userId').val()) {
        stringBuilder += '<td valign="top">' + msgSelect + msgFavourite + msgEdit + msgDelete + '</td>';
    }
    else {
        stringBuilder += '<td>' + msgSelect + msgFavourite + '</td>';
    }
    stringBuilder += '</tr>';
    $('.messages').append(stringBuilder);
};

function formUserDTO() {
    var userDTO = {
        Id: $('#userId').val(),
        Name: $('#userName').val(),
        Image: $('#userImage').val()
    };
    return userDTO;
};

function formChannelDTO() {
    var channelDTO = {
        ChannelId: $('#channelId').val(),
        Name: $('#channelName').val()
    };
    return channelDTO;
};

function formMessageDTO(id) {

    var messageDTO = {
        Channel: formChannelDTO()
    };
    if (id == undefined) {
        messageDTO.Content = $('textarea#message').val();
        messageDTO.User = formUserDTO();
    }
    else {
        messageDTO.MessageId = id;
        messageDTO.Content
            = $('.messages #' + id).find('#messageText').html();
        var userName = $('.messages #' + id).find('#messageInfo').html();
        messageDTO.User = {};
        messageDTO.User.Name
            = userName.slice(0, userName.indexOf('<span>'));
    }
    messageDTO.Parents = [];
    $('tr.selected').each(function () {
        $(this).removeClass('selected');
        messageDTO.Parents.push({
            MessageId : $(this).attr('id')
        });
    });
    return messageDTO;
};

function markAsOnline(user) {
    $('.users #' + user.Id + ' span').css('color', 'green');
}

function markAsOffline(user) {
    $('.users #' + user.Id + ' span').css('color', 'grey');
}

function scrollToBottom() {
    $('#messagebox').animate({ scrollTop: $('.messages').height()},20);
}

function MessageHistory(message) {
    var result = '';
    for (var i = 0; i < message.Parents.length; ++i)
        result += BuildMessageTree(message.Parents[i], '', 1);
    return result;
}

function BuildMessageTree(message, result, deep) {
    var openingTags = '<blockquote>'.repeat(deep) + '<table>';
    var closingTags = '</table>' + '</blockquote>'.repeat(deep);

    var usrImg = '<img src="data:image/png;base64,' + message.User.Image + '" alt="User icon" id="user_icon">';
    var msgHead = '<li id="messageInfo">' + '<a href="#" class="userInfo">' + message.User.Name + '</a>'
        + '<span>&nbsp;&nbsp;' + message.SendTime + '</span></li>';
    var msgBody = '<li id="messageText">' + message.Content + '</li>';
    var stringBuilder = '<td class="avatar" valign="top">' + usrImg + '</td>'
        + '<td class="mailInfo" valign="top"><ul>' + msgHead + msgBody + '</ul></td>';

    result = openingTags + stringBuilder + closingTags;

    if (!message.Parents || message.Parents.length == 0)
        return openingTags + stringBuilder + closingTags;
    for (var i = 0; i < message.Parents.length; ++i) {
        var parent = message.Parents[i];
        result += BuildMessageTree(parent, result, deep + 1);
    }
    return result;
}

function MarkMessageAsFavourite(message) {
    $('.messages #' + message.MessageId + ' .markFavourite').css('color', 'red');
}