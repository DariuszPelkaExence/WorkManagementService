using System;
using System.Collections.Generic;
using System.Web.Http;
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


        [Test]
        [TestCase(2020, 1, 2, ShiftType.ShiftFrom0To8, 1)]
        [TestCase(2020, 1, 3, ShiftType.ShiftFrom16To24, 1)]
        [TestCase(2020, 1, 1, ShiftType.ShiftFrom0To8, 1)]
        public void Add_WhenShiftSeparatedFromExisting_ThenShiftShouldBeAdded(int year, int month, int day, ShiftType type, int workerId)
        {
            // Arrange
            var shift = new Shift() { Day = new DateTime(year, month, day), Type = type, WorkerId = workerId };
            var list = new List<Shift>();
            list.Add(shift);
            var mockedRepository = new Mock<IRepository>();
            mockedRepository.Setup(m => m.AddShift(It.IsAny<AddShift>())).Returns(1);
            mockedRepository.Setup(m => m.GetWorker(It.IsAny<int>())).Returns(new Worker() { Id = 1, FirstName = "Jan", LastName = "Hello" });
            mockedRepository.Setup(m => m.GetShiftsPerWorker(It.IsAny<int>())).Returns(new List<Shift>());

            var controller = new ShiftController(mockedRepository.Object);
            var newShift = new AddShift() { Day = new DateTime(2020, 1, 2), Type = ShiftType.ShiftFrom16To24, WorkerId = 1 };
            // Act

            var result = controller.Add(newShift);
            var okResult = result as OkObjectResult;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(1, okResult.Value);
        }

        // Test checking if shift can be added having same time as one we try to add or if we have a shift which is before or after one we try to add
        [Test]
        [TestCase(2020, 1, 2, ShiftType.ShiftFrom0To8, 1)]
        [TestCase(2020, 1, 2, ShiftType.ShiftFrom16To24, 1)]
        [TestCase(2020, 1, 1, ShiftType.ShiftFrom16To24, 1)]
        public void Add_WhenNewShiftWithSameTime_ThenShiftShouldNotBeAdded(int year, int month, int day, ShiftType type, int workerId)
        {
            // Arrange
            var mockedRepository = new Mock<IRepository>();
            var shift = new Shift() { Day = new DateTime(year, month, day), Type = type, WorkerId = workerId };
            var list = new List<Shift>();
            list.Add(shift);
            mockedRepository.Setup(m => m.AddShift(It.IsAny<AddShift>())).Returns(1);
            mockedRepository.Setup(m => m.GetWorker(It.IsAny<int>())).Returns(new Worker() { Id = 1, FirstName = "Jan", LastName = "Hello" });
            mockedRepository.Setup(m => m.GetShiftsPerWorker(It.IsAny<int>())).Returns(list);

            var controller = new ShiftController(mockedRepository.Object);
            var newShift = new AddShift() { Day = new DateTime(2020, 1, 1), Type = ShiftType.ShiftFrom0To8, WorkerId = 1 };

            // Act
            try
            {
                controller.Add(newShift);
            }
            catch (HttpResponseException exception)
            {
                // Assert
                Assert.IsTrue(true);
                var errorMessage = exception.Response.Content.ReadAsStringAsync().Result;
                Assert.AreEqual("Shift same, previous ot next exists", errorMessage);
            }
        }
    }
}