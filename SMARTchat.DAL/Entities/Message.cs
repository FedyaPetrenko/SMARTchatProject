using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMARTchat.DAL.Entities
{
    [Table("Messages")]
    public class Message
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessageId { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime SendTime { get; set; }

        public int ChannelId { get; set; }

        [ForeignKey("ChannelId")]
        public virtual Channel Channel { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        
        public virtual ICollection<ApplicationUser> FavouritedBy { get; set; }

        public virtual ICollection<Message> Parents { get; set; }

        public Message()
        {
            FavouritedBy = new List<ApplicationUser>();
            Parents = new List<Message>();
        }
    }
}
