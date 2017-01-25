using SMARTchat.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMARTchat.BLL.Interfaces
{
    public interface IChannelService
    {
        bool IsConnected(string id, UserDTO user);
        bool IsExistById(string id);
        bool IsExistByName(string name);
        void AddChannel(ChannelDTO channel);
        void AddUser(string id, UserDTO user);
        void AddMessage(string id, MessageDTO message);
        ChannelDTO GetChannelById(string id);
        ChannelDTO GetChannelByName(string id);
        IEnumerable<UserDTO> GetAllUsers(string id);
        IEnumerable<MessageDTO> GetAllMessages(string id);
        void EditMessage(string id, MessageDTO message);
        void RenameChannel(string id, ChannelDTO channel);
        void RemoveChannel(string id);
        void RemoveUser(string id, UserDTO user);
        void RemoveMessage(string id, MessageDTO message);
    }
}
