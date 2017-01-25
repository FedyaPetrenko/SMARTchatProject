using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMARTchat.BLL.DTOs
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsOnline { get; set; }
        public string Image { get; set; }
        public string ImageMimeType { get; set; }
    }
}