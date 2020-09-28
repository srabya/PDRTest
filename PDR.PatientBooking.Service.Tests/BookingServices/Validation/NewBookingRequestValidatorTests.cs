using System;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Validation;

namespace PDR.PatientBooking.Service.Tests.BookingServices.Validation
{
    [TestFixture]
    public class NewBookingRequestValidatorTests
    {
        private IFixture _fixture;

        private NewBookingRequestValidator _newBookingRequestValidator;
        private PatientBookingContext _context;

        [SetUp]
        public void SetUp()
        {
            // Boilerplate
            _fixture = new Fixture();

            //Prevent fixture from generating from entity circular references 
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));

            // Context setup
            _context = new PatientBookingContext(new DbContextOptionsBuilder<PatientBookingContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

            // Sut instantiation
            _newBookingRequestValidator = new NewBookingRequestValidator(_context);
        }

        [Test]
        public void ValidateRequest_BookingInThePast_ReturnsFailedValidationResult()
        {
            //arrange
            var request = GetValidRequest();
            request.StartTime = DateTime.Now.Subtract(TimeSpan.FromDays(1));

            //act
            var res = _newBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain("Bookings can not be in the past");
        }

        [Test]
        public void ValidateRequest_BookingInTheFuture_PassesValidation()
        {
            //arrange
            var request = GetValidRequest();
            request.StartTime = DateTime.Now.AddDays(1);

            //act
            var res = _newBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeTrue();
        }
        [Test]
        public void ValidateRequest_DoctorAlreadyBookedAtTheStartTime_ReturnsFailedValidationResult()
        {
            //arrange
            //book a doctor
            var existingBooking = _fixture
                .Build<Order>()
                .With(x => x.StartTime, DateTime.Now.AddMinutes(15))
                .With(x => x.EndTime, DateTime.Now.AddMinutes(30))
                .Create();

            _context.Add(existingBooking);
            _context.SaveChanges();
            //setup a booking with the same doctor at the same time
            var request = GetValidRequest();
            request.StartTime = existingBooking.StartTime;
            request.DoctorId = existingBooking.DoctorId;

            //act
            var res = _newBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain("Doctor is already booked for the requested time.");
        }



        private NewBookingRequest GetValidRequest()
        {
            return _fixture.Create<NewBookingRequest>();
        }

    }
}
