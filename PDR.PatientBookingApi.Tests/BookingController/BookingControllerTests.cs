using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Service.BookingServices;
using PDR.PatientBooking.Service.BookingServices.Requests;
using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace PDR.PatientBookingApi.Tests.BookingController
{
    [TestFixture]
    public class BookingControllerTests
    {
        Controllers.BookingController _bookingController;
        private Mock<IBookingService> _bookingService;
        private IFixture _fixture;
        private PatientBookingContext _context;


        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();

            //Prevent fixture from generating circular references
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));

            // Mock setup
            _context = new PatientBookingContext(new DbContextOptionsBuilder<PatientBookingContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            _bookingService = new Mock<IBookingService>();
        }

        [Test]
        public void AddBooking_Delegates_To_Service()
        {
            //arrange
            _bookingController = new Controllers.BookingController(_context, _bookingService.Object);
            var req = new NewBookingRequest() { PatientId = 1 };
            //act
            _bookingController.AddBooking(req);

            //assert
            _bookingService.Verify(s => s.AddBooking(req), Times.Once());
        }

        [Test]
        public void AddBooking_Returns_BadRequest_For_ArgumentException()
        {
            var errorMessage = _fixture.Create<string>();
            //arrange
            _bookingController = new Controllers.BookingController(_context, _bookingService.Object);
            _bookingService.Setup(s => s.AddBooking(It.IsAny<NewBookingRequest>()))
                .Throws(new ArgumentException(errorMessage));
            //act
            var response = _bookingController.AddBooking(new NewBookingRequest());

            //assert
            var result = response as BadRequestObjectResult;
            result.Should().NotBeNull();
            result.Value.Should().Be(errorMessage);
        }

        [Test]
        public void AddBooking_Returns_500Error_For_UnHandledExceptions()
        {
            var exception = new Exception();
            //arrange
            _bookingController = new Controllers.BookingController(_context, _bookingService.Object);
            _bookingService.Setup(s => s.AddBooking(It.IsAny<NewBookingRequest>()))
                .Throws(exception);
            //act
            var response = _bookingController.AddBooking(new NewBookingRequest());

            //assert
            var result = response as ObjectResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(500);
            result.Value.Should().BeEquivalentTo(exception);
        }

        [Test]
        public void CancelAppointment_Delegates_To_Service()
        {
            long bookingId = 1;
            //arrange
            _bookingController = new Controllers.BookingController(_context, _bookingService.Object);
            //act
            _bookingController.CancelAppointment(bookingId);

            //assert
            _bookingService.Verify(s => s.CancelBooking(1), Times.Once());
        }
    }
}