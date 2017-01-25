using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SMARTchat.BLL.DTOs;
using SMARTchat.BLL.Interfaces;

namespace SMARTchat.Tests.Services
{
    [TestClass]
    public class UserServiceTests
    {
        private Mock<IUserService> _userServiceMock;
            
        [TestInitialize]
        public void Initialize()
        {
            _userServiceMock = new Mock<IUserService>();
        }

        [TestMethod]
        public void CanGetUserByName()
        {
            //Arrange
            _userServiceMock.Setup(u => u.GetUserByName(It.IsAny<string>())).Returns(new UserDTO()
            {
                Id="1",
                Name = "Dima",
                LastName = "Kuchuk",
                Email = "kuchuk@ukr.net"
            });

            //Act
            var rezult = _userServiceMock.Object.GetUserByName("Dima");

            //Assert
            Assert.IsNotNull(rezult);
            Assert.IsInstanceOfType(rezult, typeof(UserDTO));
            Assert.AreEqual("1", rezult.Id);
         }

        [TestMethod]
        public void CanGetUserById()
        {
            //Arrange
            _userServiceMock.Setup(u => u.GetUserById(It.IsAny<string>())).Returns(new UserDTO()
            {
                Id = "1",
                Name = "Dima",
                LastName = "Kuchuk",
                Email = "kuchuk@ukr.net"
            });

            //Act
            var rezult = _userServiceMock.Object.GetUserById("1");

            //Assert
            Assert.IsNotNull(rezult);
            Assert.IsInstanceOfType(rezult, typeof(UserDTO));
            Assert.AreEqual("Dima",rezult.Name);
        }

        [TestMethod]
        public void CanGetUserChannels()
        {
            //Arrange
            _userServiceMock.Setup(u => u.GetUserChannels(It.IsAny<string>())).Returns(new List<ChannelDTO>()
            {
                new ChannelDTO() {ChannelId = "1", Name = "Channel1"},
                new ChannelDTO() {ChannelId = "2", Name = "Channel2"}
            });
            ChannelDTO channelDto = new ChannelDTO();

            //Act
            var rezult = _userServiceMock.Object.GetUserChannels("1");
            foreach (var channel in rezult.Where(channel => channel.Name == "Channel2"))
            {
                channelDto = channel;
            }

            //Assert
            Assert.IsNotNull(rezult);
            Assert.IsInstanceOfType(rezult, typeof(List<ChannelDTO>));
            Assert.AreEqual("Channel2", channelDto.Name);
        }

        [TestMethod]
        public void CanGetUserFavouriteMessages()
        {
            //Arange
            _userServiceMock.Setup(u => u.GetUserFavouriteMessages(It.IsAny<string>())).Returns(new List<MessageDTO>()
            {
                new MessageDTO() {MessageId = "1", Content = "aaa"},
                new MessageDTO() {MessageId = "2", Content = "bbb"}
            });
            MessageDTO messageDto = new MessageDTO();

            //Act
            var result = _userServiceMock.Object.GetUserFavouriteMessages("1");
            foreach (var message in result.Where(mes => mes.MessageId == "2"))
            {
                messageDto = message;
            }

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<MessageDTO>));
            Assert.AreEqual("bbb", messageDto.Content);
        }
    }
}
