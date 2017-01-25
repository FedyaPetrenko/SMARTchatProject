using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SMARTchat.BLL.DTOs;
using SMARTchat.BLL.Interfaces;

namespace SMARTchat.Tests.Services
{
    [TestClass]
    public class ChannelServiceTests
    {
        private Mock<IChannelService> _channelServiceMock;

        [TestInitialize]
        public void Initialize()
        {
            _channelServiceMock = new Mock<IChannelService>();
        }

        [TestMethod]
        public void CanGetAllMessagesByUserId()
        {
            //Arrange
            _channelServiceMock.Setup(ch => ch.GetAllMessages(It.IsAny<string>())).Returns(new List<MessageDTO>
            {
                new MessageDTO {MessageId = "1", Content = "aaa"},
                new MessageDTO {MessageId = "2", Content = "bbb"}
            });
            MessageDTO messageDto = new MessageDTO();

            //Act
            var result = _channelServiceMock.Object.GetAllMessages("1");
            foreach (var message in result.Where(m => m.MessageId == "1"))
            {
                messageDto = message;
            }

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<MessageDTO>));
            Assert.AreEqual("aaa", messageDto.Content);
        }

        [TestMethod]
        public void CanGetAllUsersByChannelId()
        {
            //Arrange
            _channelServiceMock.Setup(ch => ch.GetAllUsers(It.IsAny<string>())).Returns(new List<UserDTO>()
            {
                new UserDTO() { Id="1", Name = "Dima1", LastName = "Kuchuk1", Email = "kuchuk1@ukr.net" },
                new UserDTO() { Id="2", Name = "Dima2", LastName = "Kuchuk2", Email = "kuchuk2@ukr.net" },
            });
            UserDTO userDto = new UserDTO();

            //Act
            var result = _channelServiceMock.Object.GetAllUsers("1");
            foreach (var user in result.Where(u => u.Id == "2"))
            {
                userDto = user;
            }

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<UserDTO>));
            Assert.AreEqual("Dima2", userDto.Name);
        }

        [TestMethod]
        public void CanGetChannelInfoByChannelId()
        {
            //Arrange
            _channelServiceMock.Setup(ch => ch.GetChannelById(It.IsAny<string>())).Returns(
                new ChannelDTO() { ChannelId = "1", Name = "Channel1" });

            //Act
            var result = _channelServiceMock.Object.GetChannelById("1");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ChannelDTO));
            Assert.AreEqual("1", result.ChannelId);
        }
    }
}
