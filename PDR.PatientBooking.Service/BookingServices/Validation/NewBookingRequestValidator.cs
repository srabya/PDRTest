using System;
using System.Collections.Generic;
using System.Linq;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.Validation;

namespace PDR.PatientBooking.Service.BookingServices.Validation
{
    public class NewBookingRequestValidator : INewBookingRequestValidator
    {
        private readonly PatientBookingContext _context;

        public NewBookingRequestValidator(PatientBookingContext context)
        {
            _context = context;
        }
        public PdrValidationResult ValidateRequest(NewBookingRequest request)
        {
            var result = new PdrValidationResult(true);

            if (BookingInThePast(request, ref result))
                return result;

            if (DoctorAlreadyBooked(request, ref result))
                return result;

            return result;
        }

        private bool BookingInThePast(NewBookingRequest request, ref PdrValidationResult result)
        {
            var errors = new List<string>();

            if (request.StartTime < DateTime.Now)
                errors.Add("Bookings can not be in the past");

            if (errors.Any())
            {
                result.PassedValidation = false;
                result.Errors.AddRange(errors);
                return true;
            }

            return false;
        }

        private bool DoctorAlreadyBooked(NewBookingRequest request, ref PdrValidationResult result)
        {
            var errors = new List<string>();


            if (_context.Order.Any(x => x.DoctorId == request.DoctorId && x.StartTime <= request.EndTime && request.StartTime <= x.EndTime))
                errors.Add("Doctor is already booked for the requested time.");


            if (errors.Any())
            {
                result.PassedValidation = false;
                result.Errors.AddRange(errors);
                return true;
            }

            return false;
        }


    }
}
