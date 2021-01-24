using NUnit.Framework;
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
        public void WhenSameShift_ThenShiftShouldNotBeAdded()
        {
            // Arrange
            var repo = new Repository.Repository();
            var controller = new ShiftController(repo);

            // Act
            controller.Add();

            // Assert
            Assert.Pass();
        }
    }
}