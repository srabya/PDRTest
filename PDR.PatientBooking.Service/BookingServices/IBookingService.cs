using System;
using PDR.PatientBooking.Service.BookingServices.Requests;

namespace PDR.PatientBooking.Service.BookingServices
{
    public interface IBookingService
    {
        public void AddBooking(NewBookingRequest newBookingRequest);
        void CancelBooking(Guid identificationNumber);
    }
}