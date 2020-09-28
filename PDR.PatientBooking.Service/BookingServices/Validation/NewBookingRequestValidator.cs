using System;
using System.Collections.Generic;
using System.Linq;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.Validation;

namespace PDR.PatientBooking.Service.BookingServices.Validation
{
    public class NewBookingRequestValidator
    {

        public PdrValidationResult ValidateRequest(NewBookingRequest request)
        {
            var result = new PdrValidationResult(true);

            if (BookingInThePast(request, ref result))
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


    }
}