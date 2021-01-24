using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Teamway.WorkManagementService.API;
using Teamway.WorkManagementService.Repository;
using Teamway.WorkManagementService.Repository.Model;

namespace Teamway.WorkManagementService.UnitTest
{
    public class Tests
    {
        [SetUp]
        // Not used
        public void Setup()
        {
        }

        [Test]
        public void Add_WhenNewShift_ThenShiftShouldBeAdded()
        {
            // Arrange
            var mockedPublisher = new Mock<IMessagePublisher>();
            var mockedRepository = new Mock<IRepository>();
            mockedRepository.Setup(m => m.AddShift(It.IsAny<AddShift>())).Returns(Task.FromResult(1));
            mockedRepository.Setup(m => m.GetWorker(It.IsAny<int>()))
                .Returns(Task.FromResult(new Worker() {Id = 1, FirstName = "Jan", LastName = "Hello"}));
            mockedRepository.Setup(m => m.GetShiftsPerWorker(It.IsAny<int>()))
                .Returns(Task.FromResult((IList<Shift>) new List<Shift>()));

            var controller = new ShiftController(mockedRepository.Object, mockedPublisher.Object);
            var newShift = new AddShift()
                {Day = new DateTime(2020, 1, 1), Type = ShiftType.ShiftFrom0To8, WorkerId = 1};
            // Act

            var result = controller.Add(newShift);
            var okResult = result.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(1, okResult.Value);
            mockedPublisher.Verify(m => m.SendMessageShiftCreated(It.IsAny<Shift>()), Times.Once);
        }

        [Test]
        [TestCase(2020, 1, 2, ShiftType.ShiftFrom0To8, 1)]
        [TestCase(2020, 1, 3, ShiftType.ShiftFrom16To24, 1)]
        [TestCase(2020, 1, 1, ShiftType.ShiftFrom0To8, 1)]
        public void Add_WhenShiftSeparatedFromExisting_ThenShiftShouldBeAdded(int year, int month, int day,
            ShiftType type, int workerId)
        {
            // Arrange
            var shift = new Shift() {Day = new DateTime(year, month, day), Type = type, WorkerId = workerId};
            var list = new List<Shift>();
            list.Add(shift);
            var mockedPublisher = new Mock<IMessagePublisher>();
            var mockedRepository = new Mock<IRepository>();
            mockedRepository.Setup(m => m.AddShift(It.IsAny<AddShift>())).Returns(Task.FromResult(1));
            mockedRepository.Setup(m => m.GetWorker(It.IsAny<int>()))
                .Returns(Task.FromResult(new Worker() {Id = 1, FirstName = "Jan", LastName = "Hello"}));
            mockedRepository.Setup(m => m.GetShiftsPerWorker(It.IsAny<int>()))
                .Returns(Task.FromResult((IList<Shift>) new List<Shift>()));

            var controller = new ShiftController(mockedRepository.Object, mockedPublisher.Object);
            var newShift = new AddShift()
                {Day = new DateTime(2020, 1, 2), Type = ShiftType.ShiftFrom16To24, WorkerId = 1};
            // Act

            var result = controller.Add(newShift);
            var okResult = result.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(1, okResult.Value);
            mockedPublisher.Verify(m => m.SendMessageShiftCreated(It.IsAny<Shift>()), Times.Once);
        }

        // Test checking if shift can be added having same time as one we try to add or if we have a shift which is before or after one we try to add
        [Test]
        [TestCase(2020, 1, 2, ShiftType.ShiftFrom0To8, 1)]
        [TestCase(2020, 1, 2, ShiftType.ShiftFrom16To24, 1)]
        [TestCase(2020, 1, 1, ShiftType.ShiftFrom16To24, 1)]
        public void Add_WhenNewShiftWithSameTime_ThenShiftShouldNotBeAdded(int year, int month, int day, ShiftType type,
            int workerId)
        {
            // Arrange
            var mockedPublisher = new Mock<IMessagePublisher>();
            var mockedRepository = new Mock<IRepository>();
            var shift = new Shift() {Day = new DateTime(year, month, day), Type = type, WorkerId = workerId};
            IList<Shift> list = new List<Shift>();
            list.Add(shift);
            mockedRepository.Setup(m => m.AddShift(It.IsAny<AddShift>())).Returns(Task.FromResult(1));
            mockedRepository.Setup(m => m.GetWorker(It.IsAny<int>()))
                .Returns(Task.FromResult(new Worker() {Id = 1, FirstName = "Jan", LastName = "Hello"}));
            mockedRepository.Setup(m => m.GetShiftsPerWorker(It.IsAny<int>())).Returns(Task.FromResult(list));

            var controller = new ShiftController(mockedRepository.Object, mockedPublisher.Object);
            var newShift = new AddShift()
                {Day = new DateTime(2020, 1, 1), Type = ShiftType.ShiftFrom0To8, WorkerId = 1};

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

        [Test]
        public async Task Get_WhenShiftExists_ThenShiftShouldBeReturned()
        {
            // Arrange
            var mockedPublisher = new Mock<IMessagePublisher>();
            var mockedRepository = new Mock<IRepository>();
            var shift = new Shift()
                {Id = 1, Day = new DateTime(2020, 2, 1), Type = ShiftType.ShiftFrom0To8, WorkerId = 3};

            mockedRepository.Setup(m => m.GetShift(It.IsAny<int>())).Returns(Task.FromResult(shift));
            var controller = new ShiftController(mockedRepository.Object, mockedPublisher.Object);

            // Act
            var result = await controller.GetAsync(1);
            var okResult = result as OkObjectResult;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var record = (Shift) okResult.Value;
            Assert.AreEqual(1, record.Id);
            Assert.AreEqual(3, record.WorkerId);
            Assert.AreEqual(2020, record.Day.Year);
            Assert.AreEqual(2, record.Day.Month);
            Assert.AreEqual(1, record.Day.Day);
        }

        [Test]
        public void Get_WhenShiftDoesNotExist_Then404ShouldBeReturned()
        {
            // Arrange
            var mockedPublisher = new Mock<IMessagePublisher>();
            var mockedRepository = new Mock<IRepository>();
            mockedRepository.Setup(m => m.GetShift(It.IsAny<int>())).Returns(Task.FromResult((Shift) null));
            var controller = new ShiftController(mockedRepository.Object, mockedPublisher.Object);

            // Act
            var result = controller.GetAsync(1);
            var notFoundResult = result.Result as NotFoundResult;

            // Assert
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public void Remove_WhenShiftDoesNotExist_ThenExceptionShouldBeReturned()
        {
            // Arrange
            var mockedPublisher = new Mock<IMessagePublisher>();
            var mockedRepository = new Mock<IRepository>();
            mockedRepository.Setup(m => m.RemoveShift(It.IsAny<int>()))
                .Returns(Task.FromResult(RemoveShiftStatus.RecordDoesNotExist));
            var controller = new ShiftController(mockedRepository.Object, mockedPublisher.Object);

            // Act
            try
            {
                controller.RemoveAsync(1);
            }
            catch (HttpResponseException exception)
            {
                // Assert
                Assert.IsTrue(true);
                var errorMessage = exception.Response.Content.ReadAsStringAsync().Result;
                Assert.AreEqual("Shift record could not be removed", errorMessage);
            }
        }

        [Test]
        public void Remove_WhenShiftExists_ThenShiftShouldBeReturned()
        {
            // Arrange
            var mockedRepository = new Mock<IRepository>();
            var mockedPublisher = new Mock<IMessagePublisher>();
            mockedRepository.Setup(m => m.RemoveShift(It.IsAny<int>())).Returns(Task.FromResult(RemoveShiftStatus.Ok));
            var controller = new ShiftController(mockedRepository.Object, mockedPublisher.Object);

            // Act
            var result = controller.RemoveAsync(1);
            var okResult = result.Result as OkResult;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            mockedPublisher.Verify(m => m.SendMessageShiftRemoved(It.IsAny<Shift>()), Times.Once);
        }

        [Test]
        public void GetShiftsPerWorker_WhenShiftExists_ThenShiftsShouldBeReturned()
        {
            // Arrange
            var mockedRepository = new Mock<IRepository>();
            var mockedPublisher = new Mock<IMessagePublisher>();
            IList<Shift> list = new List<Shift>();
            var shift = new Shift()
                {Id = 1, Day = new DateTime(2020, 11, 11), Type = ShiftType.ShiftFrom0To8, WorkerId = 1};
            list.Add(shift);
            shift = new Shift()
                {Id = 2, Day = new DateTime(2021, 12, 11), Type = ShiftType.ShiftFrom8To16, WorkerId = 1};
            list.Add(shift);
            mockedRepository.Setup(m => m.GetShiftsPerWorker(It.IsAny<int>())).Returns(Task.FromResult(list));
            var controller = new ShiftController(mockedRepository.Object, mockedPublisher.Object);

            // Act
            var result = controller.GetShiftsPerWorkerAsync(1);
            var okResult = result.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var resultList = (List<Shift>) okResult.Value;
            Assert.AreEqual(2, resultList.Count);
            var firstRecord = resultList[0];
            Assert.AreEqual(1, firstRecord.Id);
            Assert.AreEqual(1, firstRecord.WorkerId);
            Assert.AreEqual(ShiftType.ShiftFrom0To8, firstRecord.Type);
            Assert.AreEqual(new DateTime(2020, 11, 11), firstRecord.Day);
        }
    }
}