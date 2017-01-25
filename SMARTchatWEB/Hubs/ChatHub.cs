using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using SMARTchat.BLL.DTOs;
using SMARTchat.BLL.Interfaces;
using System.Collections.Generic;
using SMARTchat.DAL.Entities;

namespace SMARTchatWEB.Hubs
{
    public class ChatHub : Hub
    {
        private IUserService UserService { get; }
        private IChannelService ChannelService { get; set; }

        private static readonly ConcurrentDictionary<string, string>
            ConnectionIdsUser = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, string>
            ConnectionIdsChannel = new ConcurrentDictionary<string, string>();


        public ChatHub(IUserService userService, IChannelService channelService)
        {
            this.UserService = userService;
            this.ChannelService = channelService;
        }

        public void Disconnect()
        {
            string connectionId = Context.ConnectionId;
            string channelId;
            string userId;
            ConnectionIdsChannel.TryRemove(connectionId, out channelId);
            ConnectionIdsUser.TryRemove(connectionId, out userId);
            UserDTO user = UserService.GetUserById(userId);
            UserService.Logoff(userId);
            // Exception chanelId=null
            Clients.Group(channelId).onUserDisconnected(user);
        }

        public void Send(MessageDTO message)
        {
            ChannelDTO channel = message.Channel;
            message.SendTime = DateTime.UtcNow.ToString("HH:mm:ss");
            ChannelService.AddMessage(channel.ChannelId, message);
            Clients.Group(channel.ChannelId).addMessage(message);
        }

        public void Edit(MessageDTO message)
        {
            ChannelDTO channel = message.Channel;
            ChannelService
                .EditMessage(channel.ChannelId, message);
            Clients.Group(channel.ChannelId).onMessageEdit(message);
        }

        public void Delete(MessageDTO message)
        {
            ChannelDTO channel = message.Channel;
            ChannelService
                .RemoveMessage(channel.ChannelId, message);
            Clients.Group(channel.ChannelId).onMessageDelete(message);
        }

        public void Connect(ChannelDTO channel)
        {
            string channelId = channel.ChannelId;
            string connectionId = Context.ConnectionId;
            
            UserDTO user 
                = UserService.GetUserByName(Context.User.Identity.Name);
            
            ConnectionIdsUser.TryAdd(connectionId, user.Id);
            ConnectionIdsChannel.TryAdd(connectionId, channelId);

            UserService.Login(user.Id);
            Groups.Add(connectionId, channelId);
            // new user connected, add him to users list
            if (!ChannelService.IsConnected(channelId, user))
            {
                ChannelService.AddUser(channelId, user);
                Clients.Group(channelId, connectionId)
                    .onUserConnected(user, true);
            }
            // user reconnected, mark him as online
            else
            {
                Clients.Group(channelId, connectionId)
                    .onUserConnected(user, false);
            }

            List<UserDTO> allChannelUsers
                = (List<UserDTO>)ChannelService.GetAllUsers(channelId);
            allChannelUsers.ForEach(
                x => x.IsOnline = UserService.IsConnected(x.Id));
            Clients.Caller.onConnected(user, allChannelUsers);

            Clients.Caller.loadMessagesHistory(ChannelService.GetAllMessages(channelId), UserService.GetUserFavouriteMessages(user.Id));
           
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            // user disconnected, mark him as offline
            //Disconnect();
            return base.OnDisconnected(stopCalled);
        }

        public void MarkAsFavourite(MessageDTO message)
        {
            UserDTO user
                = UserService.GetUserByName(Context.User.Identity.Name);
            UserService.AddMessageToFavourites(user.Id, message.MessageId);
        }
    }
}