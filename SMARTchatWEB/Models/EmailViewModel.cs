using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.ComponentModel.DataAnnotations;
using SMARTchat.BLL.DTOs;

namespace SMARTchatWEB.Models
{
    public class EmailViewModel
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Error Name")]
        public string UserName { get; set; }

        [Display(Name = "Email From")]
        [Required(ErrorMessage = "Error Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Error email")]
        public string EmailFrom { get; set; }

        [Display(Name = "Email To")]
        [Required(ErrorMessage = "Error Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Error email")]
        public string EmailTo { get; set; }

        [Display(Name = "Enter Subject")]
        [Required(ErrorMessage = "Error Name")]
        public string Subject { get; set; }

        [Display(Name = "Message")]
        [DataType(DataType.MultilineText)]
        public string Message { get; set; }
        [Display(Name = "Channel")]
        public ChannelDTO Channel { get; set; }
    }
}
