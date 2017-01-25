using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.ComponentModel.DataAnnotations;

namespace SMARTchatWEB.Models
{
    public class ChannelViewModel
    {

        [Required, MaxLength(30), Display(Name = "Name*")]
        public string Name { get; set; }
    }
}