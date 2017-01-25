using System;
using System.Collections.Generic;
using System.Linq;
using SMARTchat.BLL.DTOs;
using SMARTchat.BLL.Interfaces;
using SMARTchat.DAL.Entities;
using SMARTchat.DAL.Interfaces;

namespace SMARTchat.BLL.Services
{
    public class ChannelService : IChannelService
    {
        private IUnitOfWork Database { get; set; }

        public ChannelService(IUnitOfWork unitOfWork)
        {
            Database = unitOfWork;
        }

        public bool IsConnected(string id, UserDTO user)
        {
            Channel chl = Database.Channels.Get(id);
            return chl.Users.Any(us => us.Id == user.Id || us.Email == user.Email);
        }

        public bool IsExistById(string id)
        {
            return Database.Channels.Find(
                ch => ch.ChannelId.ToString() == id).Count() > 0;
        }

        public bool IsExistByName(string name)
        {
            return Database.Channels.Find(
                ch => ch.Name == name).Count() > 0;
        }

        public void AddChannel(ChannelDTO channel)
        {
            var chl = new Channel
            {
                Name = channel.Name
            };

            Database.Channels.Create(chl);
            Database.Save();

            //populate new id back to caller
            channel.ChannelId = chl.ChannelId.ToString();
        }

        public void AddMessage(string id, MessageDTO message)
        {
            var newMsg = new Message
            {
                ChannelId = Int32.Parse(id),
                Content = message.Content,
                SendTime = DateTime.Parse(message.SendTime),
                UserId = message.User.Id
            };

            Database.Messages.Create(newMsg);
            Database.Save();

            //populate new id back to caller
            message.MessageId = newMsg.MessageId.ToString();

            //populate answered messages back to caller 
            // and make appropriate links in DB
            for (int i = 0; i < message.Parents.Count; ++i)
            {
                var parentDTO = message.Parents[i];
                Message parent = Database.Messages.Find(
                    ms => ms.MessageId.ToString() == parentDTO.MessageId).First();
                message.Parents[i] = FormMessageDTO(parent);
                newMsg.Parents.Add(parent);
                Database.Messages.Update(newMsg);
                Database.Save();
            }

            Channel chl = Database.Channels.Find(ch => ch.ChannelId.ToString() == id).First();

            chl.Messages.Add(newMsg);

            Database.Channels.Update(chl);
            Database.Save();
        }

        public void AddUser(string id, UserDTO user)
        {
            ApplicationUser usr = Database.Users.Find(
                x => x.Id == user.Id || x.UserName == user.Name).FirstOrDefault();
            Channel chl = Database.Channels.Find(
                x => x.ChannelId.ToString() == id).FirstOrDefault();

            if (usr != null)
            {
                usr.Channels.Add(chl);
                Database.Users.Update(usr);
                Database.Save();
                //if (chl != null)
                //{
                //    chl.Users.Add(usr);
                //    Database.Channels.Update(chl);
                //    Database.Save();
                //}
            }
        }

        public void EditMessage(string id, MessageDTO message)
        {
            Channel chl = Database.Channels.Find(
                ch => ch.ChannelId.ToString() == id).FirstOrDefault();
            if (chl != null)
            {
                Message msg = chl.Messages.First(
                    ms => ms.MessageId.ToString() == message.MessageId);
                msg.Content = message.Content;

                Database.Messages.Update(msg);
                Database.Save();
            }

            Database.Channels.Update(chl);
            Database.Save();
        }

        private MessageDTO FormMessageDTO(Message msg)
        {
            var msgDTO = new MessageDTO
            {
                Channel = new ChannelDTO
                {
                    ChannelId = msg.Channel.ChannelId.ToString(),
                    Name = msg.Channel.Name
                },
                Content = msg.Content,
                MessageId = msg.MessageId.ToString(),
                SendTime = msg.SendTime.ToString("HH:mm:ss"),
                User = new UserDTO
                {
                    Id = msg.User.Id,
                    Email = msg.User.Email,
                    Name = msg.User.UserName,
                    ImageMimeType = msg.User.ImageMimeType,
                    Image = msg.User.UserImage
                },
            };
            if (msg.Parents != null)
            {
                msgDTO.Parents = new List<MessageDTO>();
                foreach (var p in msg.Parents)
                {
                    msgDTO.Parents.Add(FormMessageDTO(p));
                }
            }
            return msgDTO;
        }

        public IEnumerable<MessageDTO> GetAllMessages(string id)
        {
            List<MessageDTO> messages = new List<MessageDTO>();
            string messageId = String.Empty;
                
            foreach (var msg in Database.Channels.Find(
                ch => ch.ChannelId.ToString() == id).First().Messages)
            {
                MessageDTO msgDTO = FormMessageDTO(msg);
                if (msgDTO.MessageId == messageId) continue;
                messageId = msgDTO.MessageId;
                messages.Add(msgDTO);
            }
            return messages;
        }

        public IEnumerable<UserDTO> GetAllUsers(string id)
        {
            return Database.Channels.Find(ch => ch.ChannelId.ToString() == id)
                .First()
                .Users.Select(usr =>
                new UserDTO
                {
                    Name = usr.UserName,
                    Email = usr.Email,
                    Id = usr.Id,
                    ImageMimeType = usr.ImageMimeType,
                    Image = usr.UserImage
                }).ToList();
        }

        public ChannelDTO GetChannelById(string id)
        {
            Channel chl = Database.Channels.Find(
                ch => ch.ChannelId.ToString() == id).First();

            var chlDTO = new ChannelDTO
            {
                ChannelId = chl.ChannelId.ToString(),
                Name = chl.Name
            };

            return chlDTO;
        }

        public ChannelDTO GetChannelByName(string name)
        {
            Channel chl = Database.Channels.Find(
                ch => ch.Name == name).First();

            var chlDTO = new ChannelDTO
            {
                ChannelId = chl.ChannelId.ToString(),
                Name = chl.Name
            };

            return chlDTO;
        }

        public void RemoveChannel(string id)
        {
            Database.Channels.Delete(id);
            Database.Save();
        }

        public void RemoveMessage(string id, MessageDTO message)
        {
            Channel chl = Database.Channels.Find(
               ch => ch.ChannelId.ToString() == id).First();
            var msg = chl.Messages.First(
                ms => ms.MessageId.ToString() == message.MessageId);

            Database.Messages.Delete(msg.MessageId.ToString());
            Database.Save();

            chl.Messages.Remove(msg);
            Database.Channels.Update(chl);
            Database.Save();
        }

        public void RemoveUser(string id, UserDTO user)
        {
            Channel chl = Database.Channels.Find(
               ch => ch.ChannelId.ToString() == id).First();
            var usr = chl.Users.First(
                us => us.Id == user.Id);

            chl.Users.Remove(usr);

            Database.Channels.Update(chl);
            Database.Save();
        }

        public void RenameChannel(string id, ChannelDTO channel)
        {
            Channel chl = Database.Channels.Find(
                ch => ch.ChannelId.ToString() == id).First();
            chl.Name = channel.Name;

            Database.Channels.Update(chl);
            Database.Save();
        }
    }
}
