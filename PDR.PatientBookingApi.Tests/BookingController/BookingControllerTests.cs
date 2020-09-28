using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Service.BookingServices;
using PDR.PatientBooking.Service.BookingServices.Requests;
using System;

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
    }
}