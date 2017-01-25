using System;
using System.Collections.Generic;
using System.Linq;
using SMARTchat.BLL.DTOs;
using SMARTchat.BLL.Interfaces;
using SMARTchat.DAL.Entities;
using SMARTchat.DAL.Interfaces;
using System.Collections.Concurrent;

namespace SMARTchat.BLL.Services
{
    public class UserService : IUserService
    {
        private IUnitOfWork Database { get; }
        private static readonly ConcurrentBag<string>
            OnlineUsersId = new ConcurrentBag<string>();

        public UserService(IUnitOfWork unitOfWork)
        {
            Database = unitOfWork;
        }

        public void Login(string id)
        {
            OnlineUsersId.Add(id);
        }

        public void Logoff(string id)
        {
            OnlineUsersId.TryTake(out id);
        }

        public bool IsConnected(string id)
        {
            return OnlineUsersId.Contains(id);
        }

        public void EditUser(string id, UserDTO user)
        {
            ApplicationUser usr = Database.Users.Find(
                us => us.Id == id).First();
            usr.UserName = user.Name;
            usr.LastName = user.LastName;
            usr.Email = user.Email;
            usr.UserImage = user.Image;
            usr.ImageMimeType = user.ImageMimeType;

            Database.Users.Update(usr);
            Database.Save();
        }

        public IEnumerable<ChannelDTO> GetUserChannels(string id)
        {
            List<ChannelDTO> channels = new List<ChannelDTO>();
            foreach (var chl in Database.Users.Find(
                us => us.Id == id).First().Channels)
            {
                ChannelDTO chlDTO = new ChannelDTO
                {
                    ChannelId = chl.ChannelId.ToString(),
                    Name = chl.Name
                };
                channels.Add(chlDTO);
            }
            return channels;
        }

        public UserDTO GetUserById(string id)
        {
            if(id==null)
                return new UserDTO { Name = "Does not exist" };
            ApplicationUser usr = Database.Users.Find(
                 us=>us.Id==id).First();
            if (usr == null)
                return new UserDTO{Name="Does not exist"};
            var usrDto = new UserDTO
            {
                Name = usr.UserName,
                LastName = usr.LastName,
                Email = usr.Email,
                Id = usr.Id,
                ImageMimeType = usr.ImageMimeType,
                Image = usr.UserImage
            };
            return usrDto;
        }

        public IEnumerable<MessageDTO> GetUserFavouriteMessages(string id)
        {
            List<MessageDTO> messages = new List<MessageDTO>();

            foreach (var msg in Database.Users.Find(
                us => us.Id == id).First().FavouriteMessages)
            {
                MessageDTO msgDTO = new MessageDTO
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
                    }
                };
                messages.Add(msgDTO);
            }
            return messages;
        }

        public void RemoveUser(string id)
        {
            Database.Users.Delete(id);
            Database.Save();
        }

        public UserDTO GetUserByName(string name)
        {
            ApplicationUser usr = Database.Users.Find(
                us => us.UserName == name).FirstOrDefault();
            UserDTO usrDTO = new UserDTO
            {
                Name = usr.UserName,
                LastName = usr.LastName,
                Email = usr.Email,
                Id = usr.Id,
                ImageMimeType = usr.ImageMimeType,
                Image = usr.UserImage
            };
            return usrDTO;
        }

        public void AddMessageToFavourites(string userId, string messageId)
        {
            ApplicationUser usr = Database.Users.Find(us => us.Id == userId).FirstOrDefault();
            Message message = Database.Messages.Find(m => m.MessageId.ToString() == messageId).FirstOrDefault();

            if (usr != null)
            {
                if (!usr.FavouriteMessages.Contains(message))
                {
                    usr.FavouriteMessages.Add(message);
                    Database.Users.Update(usr);
                    Database.Save();
                }
            }
        }

        public void SaveUserPhoto(string id, UserDTO user)
        {
            ApplicationUser usr = Database.Users.Find(
                us => us.Id == id).First();
           
            usr.UserImage = user.Image;
            usr.ImageMimeType = user.ImageMimeType;

            Database.Users.Update(usr);
            Database.Save();
        }
    }
}
