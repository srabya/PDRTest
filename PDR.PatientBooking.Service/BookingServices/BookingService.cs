using System;
using System.Collections.Generic;
using System.Linq;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Validation;

namespace PDR.PatientBooking.Service.BookingServices
{
    public class BookingService : IBookingService
    {
        private readonly PatientBookingContext _context;
        private readonly INewBookingRequestValidator _validator;

        public BookingService(PatientBookingContext context, INewBookingRequestValidator validator)
        {
            _context = context;
            _validator = validator;
        }

        public void AddBooking(NewBookingRequest newBookingRequest)
        {
            var validationResult = _validator.ValidateRequest(newBookingRequest);

            if (!validationResult.PassedValidation)
            {
                throw new ArgumentException(validationResult.Errors.First());
            }

            var bookingId = new Guid();
            var bookingStartTime = newBookingRequest.StartTime;
            var bookingEndTime = newBookingRequest.EndTime;
            var bookingPatientId = newBookingRequest.PatientId;
            var bookingPatient = _context.Patient.FirstOrDefault(x => x.Id == newBookingRequest.PatientId);
            var bookingDoctorId = newBookingRequest.DoctorId;
            var bookingDoctor = _context.Doctor.FirstOrDefault(x => x.Id == newBookingRequest.DoctorId);
            var bookingSurgeryType = _context.Patient.FirstOrDefault(x => x.Id == bookingPatientId).Clinic.SurgeryType;

            var myBooking = new Order
            {
                Id = bookingId,
                StartTime = bookingStartTime,
                EndTime = bookingEndTime,
                PatientId = bookingPatientId,
                DoctorId = bookingDoctorId,
                Patient = bookingPatient,
                Doctor = bookingDoctor,
                SurgeryType = (int)bookingSurgeryType
            };

            _context.Order.AddRange(new List<Order> { myBooking });
            _context.SaveChanges();
        }

        public void CancelBooking(Guid identificationNumber)
        {
            var booking = _context.Order.FirstOrDefault(b => b.Id == identificationNumber);

            if (booking is null)
            {
                throw new ArgumentException("Invalid booking");
            }
        }
    }
}
