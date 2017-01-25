using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.IO;
using System;
using System.Net;

namespace SMARTchat.DAL.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(40)]
        public string LastName { get; set; }

        public string UserImage { get; set; }
        public string ImageMimeType { get; set; }

        [InverseProperty("FavouritedBy")]
        public virtual ICollection<Message> FavouriteMessages { get; set; }

        [InverseProperty("Users")]
        public virtual ICollection<Channel> Channels { get; set; }

        public ApplicationUser()
        {
            FavouriteMessages = new List<Message>();
            Channels = new List<Channel>();
            var temp = Environment.CurrentDirectory;

            // bad practice!
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    byte[] data = webClient.DownloadData("http://062.ua/images/frontend/avatars/avatar.png");
                    UserImage = Convert.ToBase64String(data);
                }
                catch (Exception ex)
                {
                    var i = ex.Message;
                }
            }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Здесь добавьте утверждения пользователя
            return userIdentity;
        }
    }
}
