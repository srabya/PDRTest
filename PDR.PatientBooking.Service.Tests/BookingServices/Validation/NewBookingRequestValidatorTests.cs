using System;
using System.Collections;
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


        [Test, TestCaseSource(typeof(NewBookingRequestValidatorTests), "DoubleBookingTestCases")]
        public void ValidateRequest_DoctorAlreadyBookedForTheTimeSlot_ReturnsFailedValidationResult(string existingBookingStartTime,
                                                                                                    string existingBookingEndTime,
                                                                                                    string newBookingStartTime,
                                                                                                    string newBookingEndTime)
        {
            //arrange
            //book a doctor
            var existingBooking = _fixture
                .Build<Order>()
                .With(x => x.StartTime, TomorrowAt(existingBookingStartTime))
                .With(x => x.EndTime, TomorrowAt(existingBookingEndTime))
                .Create();

            _context.Add(existingBooking);
            _context.SaveChanges();
            //setup a booking with the same doctor at the same time
            var request = GetValidRequest();
            request.StartTime = TomorrowAt(newBookingStartTime);
            request.EndTime = TomorrowAt(newBookingEndTime);
            request.DoctorId = existingBooking.DoctorId;

            //act
            var res = _newBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain("Doctor is already booked for the requested time.");
        }

        [Test, TestCaseSource(typeof(NewBookingRequestValidatorTests), "ValidBookingTestCases")]
        public void ValidateRequest_DoctorNotBookedForTheTimeSlot_PassesValidation(string existingBookingStartTime,
                                                                                                    string existingBookingEndTime,
                                                                                                    string newBookingStartTime,
                                                                                                    string newBookingEndTime)
        {
            //arrange
            //book a doctor
            var existingBooking = _fixture
                .Build<Order>()
                .With(x => x.StartTime, TomorrowAt(existingBookingStartTime))
                .With(x => x.EndTime, TomorrowAt(existingBookingEndTime))
                .Create();

            _context.Add(existingBooking);
            _context.SaveChanges();
            //setup a booking with the same doctor at the same time
            var request = GetValidRequest();
            request.StartTime = TomorrowAt(newBookingStartTime);
            request.EndTime = TomorrowAt(newBookingEndTime);
            request.DoctorId = existingBooking.DoctorId;

            //act
            var res = _newBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeTrue();
        }

        private NewBookingRequest GetValidRequest()
        {
            return _fixture.Create<NewBookingRequest>();
        }

        private DateTime TomorrowAt(string time)
        {
            return DateTime.Parse(time).AddDays(1);
        }
        public static IEnumerable DoubleBookingTestCases
        {
            get
            {
                yield return new TestCaseData("15:15", "15:30", "15:10", "15:25");
                yield return new TestCaseData("15:15", "15:30", "15:15", "15:30");
                yield return new TestCaseData("15:15", "15:30", "15:20", "15:35");
                yield return new TestCaseData("15:15", "15:30", "15:30", "15:45");
            }
        }

        public static IEnumerable ValidBookingTestCases
        {
            get
            {
                yield return new TestCaseData("15:15", "15:30", "15:00", "15:10");
                yield return new TestCaseData("15:15", "15:30", "15:35", "15:50");
            }
        }

    }
}
