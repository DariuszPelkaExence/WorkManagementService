using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Teamway.Repository;
using Teamway.Repository.Model;
using Teamway.WorkManagementService.Controllers;

namespace Teamway.WorkManagementServiceUnitTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Add_WhenNewShift_ThenShiftShouldBeAdded()
        {
            // Arrange
            var mockedRepository = new Mock<IRepository>();
            mockedRepository.Setup(m => m.AddShift(It.IsAny<AddShift>())).Returns(1);
            mockedRepository.Setup(m => m.GetWorker(It.IsAny<int>())).Returns(new Worker() { Id = 1, FirstName = "Jan", LastName = "Hello"});
            mockedRepository.Setup(m => m.GetShiftsPerWorker(It.IsAny<int>())).Returns(new List<Shift>());

            var controller = new ShiftController(mockedRepository.Object);
            var newShift = new AddShift() {Day = new DateTime(2020, 1, 1), Type = ShiftType.ShiftFrom0To8, WorkerId = 1};
            // Act
            
            var result = controller.Add(newShift);
            var okResult = result as OkObjectResult;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(1, okResult.Value);
        }
    }
}