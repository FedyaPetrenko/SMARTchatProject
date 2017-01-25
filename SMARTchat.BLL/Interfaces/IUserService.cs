using SMARTchat.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMARTchat.DAL.Entities;

namespace SMARTchat.BLL.Interfaces
{
    public interface IUserService
    {
        bool IsConnected(string id);
        void Login(string id);
        void Logoff(string id);
        void EditUser(string id, UserDTO user);
        void RemoveUser(string id);
        UserDTO GetUserById(string id);
        UserDTO GetUserByName(string name);
        IEnumerable<ChannelDTO> GetUserChannels(string id);
        IEnumerable<MessageDTO> GetUserFavouriteMessages(string id);
        void AddMessageToFavourites(string id, string message);
        void SaveUserPhoto(string id, UserDTO user);
    }
}
