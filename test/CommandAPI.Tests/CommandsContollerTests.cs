using System;
using System.Collections.Generic;
using Moq;
using AutoMapper;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using CommandAPI.Controllers;
using CommandAPI.Models;
using CommandAPI.Data;
using CommandAPI.Dtos;
using CommandAPI.Profiles;

namespace CommandAPI.Tests
{
    public class CommandsControllerTests : IDisposable
    {
        private Mock<ICommandAPIRepo> mockRepo;
        private CommandsProfile realProfile;
        private MapperConfiguration configuration;
        private IMapper mapper;

        public CommandsControllerTests()
        {
            mockRepo = new Mock<ICommandAPIRepo>();
            mockRepo.Setup(r => r.GetAllCommands()).Returns(GetCommands(0));

            realProfile = new CommandsProfile();
            configuration = new MapperConfiguration(cfg => cfg.AddProfile(realProfile));
            mapper = new Mapper(configuration);

                
        }

        public void Dispose()
        {
            mockRepo = null;
            mapper = null;
            configuration = null;
            realProfile = null;
        }


        [Fact]
        public void GetCommandItems_Returns200OK_WhenDBIsEmpty()
        {
            // Arrange
            mockRepo.Setup(r => r.GetAllCommands()).Returns(GetCommands(0));
            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.GetAllCommands();

            //Assert
            Assert.IsType<OkObjectResult>(result.Result);

            
        }

        [Fact]
        public void GetCommandItems_ReturnsZeroItems_WhenDBIsEmpty()
        {
            // Arrange
            mockRepo.Setup(r => r.GetAllCommands()).Returns(GetCommands(0));
            var controller = new CommandsController(mockRepo.Object, mapper);

            //Act
            var result = controller.GetAllCommands();
            var commands = result.Value as List<CommandReadDto>;

            // Assert
            Assert.Null(commands);
        }

        [Fact]
        public void GetAllCommands_ReturnsOneItem_WhenDBHasOneResource()
        {
            // Arrange 
            mockRepo.Setup(r => r.GetAllCommands()).Returns(GetCommands(1));
            var controller = new CommandsController(mockRepo.Object,mapper);

            // Act
            var result = controller.GetAllCommands();

            // Assert
            var okResult = result.Result as OkObjectResult;
            var commands = okResult.Value as List<CommandReadDto>;
            Assert.Single(commands);
        }

        [Fact]
        public void GetCommandByID_Returns404NotFound_WhenNonExistantIDIsProvided()
        {
            // Arrange
            mockRepo.Setup(r => r.GetCommandById(0)).Returns(() => null);
            var controller = new CommandsController(mockRepo.Object, mapper);


            // Act
            var results = controller.GetCommandById(1);

            // Assert
            Assert.IsType<NotFoundResult>(results.Result);
        }

        [Fact]
        public void GetCommandById_Returns200OK_WhenValidIdProvided()
        {
            // Arrange
            mockRepo.Setup(r => r.GetCommandById(1)).Returns(new Command{Id = 1, HowTo="Mock", Platform="Mock", CommandLine="Mock"});
            var controller = new CommandsController(mockRepo.Object, mapper);  

            // Act
            var results = controller.GetCommandById(1);

            // Assert
            Assert.IsType<OkObjectResult>(results.Result);
        }

        [Fact]
        public void GetCommandById_ReturnCommandReadDto_WhenValidIdIsProvided()
        {
            // Arrange
            mockRepo.Setup(r => r.GetCommandById(1)).Returns(new Command{Id = 1, HowTo="Mock", Platform="Mock", CommandLine="Mock"});
            var controller = new CommandsController(mockRepo.Object, mapper);  

            // Act
            var result = controller.GetCommandById(1);

            // Assert
            Assert.IsType<ActionResult<CommandReadDto>>(result);
        }

        [Fact]
        public void CreateCommand_ReturnsCorrectResourceType_WhenValidObjectSubmitted()
        {
            // Arrange
            mockRepo.Setup(r => r.GetCommandById(1)).Returns(new Command{Id = 1, HowTo="Mock", Platform="Mock", CommandLine="Mock"});
            var controller = new CommandsController(mockRepo.Object, mapper);  

            // Act
            var result = controller.CreateCommand(new CommandCreateDto{});

            // Assert
            Assert.IsType<ActionResult<CommandReadDto>>(result);
        }

        [Fact]
        public void CreateCommand_Returns201Created_WhenValidObjectSubmitted()
        {
            // Arrange
            mockRepo.Setup(r => r.GetCommandById(1)).Returns(new Command{Id = 1, HowTo="Mock", Platform="Mock", CommandLine="Mock"});
            var controller = new CommandsController(mockRepo.Object, mapper);  

            // Act
            var result = controller.CreateCommand(new CommandCreateDto{});

            // Assert
            Assert.IsType<CreatedAtRouteResult>(result.Result);
        }

        [Fact]
        public void UpdateCommand_Returns204NoContent_WhenValidObjectIsSubmitted()
        {
            // Arrange
            mockRepo.Setup(r => r.GetCommandById(1)).Returns(new Command{Id = 1, HowTo="Mock", Platform="Mock", CommandLine="Mock"});
            var controller = new CommandsController(mockRepo.Object, mapper);  

            // Act
            var result = controller.UpdateCommand(1,new CommandUpdateDto{});

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void UpdateCommand_Returns404NotFound_WhenNonExistantResourceIsSubmitted()
        {
            // Arrange
            mockRepo.Setup(r => r.GetCommandById(0)).Returns(() => null);
            var controller = new CommandsController(mockRepo.Object, mapper);  

            // Act
            var result = controller.UpdateCommand(0,new CommandUpdateDto{});

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void PartialUpdateCommand_Returns404NotFound_WhenNonExistantResourceIsSubmitted()
        {
            // Arrange
            mockRepo.Setup(r => r.GetCommandById(0)).Returns(() => null);
            var controller = new CommandsController(mockRepo.Object, mapper);  

            // Act
            var result = controller.PartialCommandUpdate(0,new Microsoft.AspNetCore.JsonPatch.JsonPatchDocument<CommandUpdateDto>{});

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteCommand_Returns204NoContent_WhenValidResourceIdSubmitted()
        {
            // Arrange
            mockRepo.Setup(r => r.GetCommandById(1)).Returns(new Command{Id = 1, HowTo="Mock", Platform="Mock", CommandLine="Mock"});
            var controller = new CommandsController(mockRepo.Object, mapper);  

            // Act
            var result = controller.DeleteCommand(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void DeleteCommand_Returns404NotFound_WhenNonExistantResourceIsSubmitted()
        {
            // Arrange
            mockRepo.Setup(r => r.GetCommandById(0)).Returns(() => null);
            var controller = new CommandsController(mockRepo.Object, mapper);  

            // Act
            var result = controller.DeleteCommand(0);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        private List<Command> GetCommands(int num)
        {
            var commands = new List<Command>();

            if (num > 0)
            {
                commands.Add(new Command
                {
                    Id = 0,
                    HowTo = "How to generate a migration",
                    Platform = ".Net Core EF",
                    CommandLine = "dotnet ef migrations add <Name of Migration>"
                });
            }

            return commands;
        }
    }

}