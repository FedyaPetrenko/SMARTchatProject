using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMARTchat.BLL.DTOs
{
    public class MessageDTO
    {
        public string MessageId { get; set; }
        public string Content { get; set; }
        public string SendTime { get; set; }
        public UserDTO User { get; set; }
        public ChannelDTO Channel { get; set; }
        public List<MessageDTO> Parents { get; set; }
    }
}