using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMARTchat.DAL.Entities
{
    [Table("Channels")]
    public class Channel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChannelId { get; set; }

        [Required, MaxLength(30)]
        public string Name { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }

        public Channel()
        {
            Messages = new List<Message>();
            Users = new List<ApplicationUser>();
        }
    }
}
