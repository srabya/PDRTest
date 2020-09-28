using System;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Validation;

namespace PDR.PatientBooking.Service.Tests.BookingServices.Validation
{
    [TestFixture]
    public class NewBookingRequestValidatorTests
    {
        private IFixture _fixture;

        private NewBookingRequestValidator _newBookingRequestValidator;

        [SetUp]
        public void SetUp()
        {
            // Boilerplate
            _fixture = new Fixture();

            //Prevent fixture from generating from entity circular references 
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));

            // Sut instantiation
            _newBookingRequestValidator = new NewBookingRequestValidator();
        }

        [Test]
        public void ValidateRequest_BookingInThePast_ReturnsFailedValidationResult()
        {
            //arrange
            var request = _fixture.Create<NewBookingRequest>();
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
            var request = _fixture.Create<NewBookingRequest>();
            request.StartTime = DateTime.Now.AddDays(1);

            //act
            var res = _newBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeTrue();
        }

    }
}