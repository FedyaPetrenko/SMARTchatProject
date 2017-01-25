using SMARTchat.BLL.DTOs;
using SMARTchat.BLL.Interfaces;
using SMARTchat.BLL.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMARTchat.DAL.Repositories;
using Ninject;

namespace SMARTchatWEB.Controllers
{
    public class ChatController : Controller
    {
        // GET: Chat
        [HttpGet]
        public ActionResult Index(ChannelDTO channel)
        {
            if (User.Identity.IsAuthenticated == true && channel != null)
            {
                if (channel.ChannelId == null && channel.Name == null)
                {
                    UserDTO userDTO = UserService.GetUserByName(User.Identity.Name);
                    userDTO.Image = null;
                    return RedirectToAction("GetChannels", userDTO);
                }
                else if (channel.ChannelId == null)
                    channel = channelService.GetChannelByName(channel.Name);
                else if (channel.Name == null)
                    channel = channelService.GetChannelById(channel.ChannelId);
                return View(channel);
            }
            else
                return RedirectToAction("Login", "Account");
        }
        [Inject]
        public IUserService UserService
        {
            get;
            set;
        }

        [Inject]
        public IChannelService channelService
        {
            get;
            set;
        }

        [HttpGet]
        public ViewResult Profile(UserDTO model)
        {
            var userId = (model != null && model.Id != null ?
                model.Id :
                UserService.GetUserByName(User.Identity.Name).Id);
            UserDTO user = UserService.GetUserById(userId);
            return View(user);
        }
        [HttpGet]
        public ActionResult GetChannels(UserDTO model)
        {
            if (User.Identity.IsAuthenticated == true)
            {
                var userId = (model != null && model.Id != null ?
                model.Id :
                UserService.GetUserByName(User.Identity.Name).Id);
                IEnumerable<ChannelDTO> channels
                    = UserService.GetUserChannels(userId);
                ViewData["Channels"] = channels;
                if (TempData.ContainsKey("ExistError"))
                {
                    TempData.Remove("ExistError");
                    ModelState.AddModelError(String.Empty, "Channel with this name already exist!");
                }
                else if (TempData.ContainsKey("DoesntExistError"))
                {
                    TempData.Remove("DoesntExistError");
                    ModelState.AddModelError(String.Empty, "Channel with this name doesn't exist!");
                }
                return View(new ChannelDTO { Name = String.Empty });
            }
            else
                return RedirectToAction("Login", "Account");
        }

        public ActionResult Create(ChannelDTO channel)
        {
            if (channelService.IsExistByName(channel.Name))
            {
                TempData.Add("ExistError", true);
                return RedirectToAction("GetChannels");
            }
            else
            {
                channelService.AddChannel(channel);
                return RedirectToAction("Index", channel);
            }
        }

        public ActionResult Find(ChannelDTO channel)
        {
            if (channelService.IsExistByName(channel.Name))
            {
                return RedirectToAction("Index", channel);
            }
            else
            {
                TempData.Add("DoesntExistError", true);
                return RedirectToAction("GetChannels");
            }
        }

        public ActionResult Favorites()
        {
            var user = UserService.GetUserByName(User.Identity.Name);
            var favourites = UserService.GetUserFavouriteMessages(user.Id);
            return View(favourites);
        }

        [HttpPost]
        public ActionResult Profile(HttpPostedFileBase image)
        {
            UserDTO userDto = UserService.GetUserByName(User.Identity.Name);
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    userDto.ImageMimeType = image.ContentType;
                    var buffer = new byte[image.ContentLength];
                    image.InputStream.Read(buffer, 0, image.ContentLength);
                    userDto.Image = Convert.ToBase64String(buffer);
                }
                else
                {
                    userDto.ImageMimeType = null;
                    userDto.Image = null;
                }

                UserService.SaveUserPhoto(userDto.Id, userDto);
                TempData["message"] = $"{userDto.Name} has been saved";
                return View(userDto);
            }
            return View(userDto);
        }

        [HttpPost]
        public ActionResult Pro(string id)
        {
            UserDTO user = UserService.GetUserByName(id);
            return Json(new {url=Url.Action("Profile","Chat",user)});
        }
    }
}