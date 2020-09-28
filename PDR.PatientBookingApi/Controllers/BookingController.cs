using Microsoft.AspNetCore.Mvc;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using PDR.PatientBooking.Service.BookingServices;
using PDR.PatientBooking.Service.BookingServices.Requests;

namespace PDR.PatientBookingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly PatientBookingContext _context;
        private readonly IBookingService _bookingService;

        public BookingController(PatientBookingContext context, IBookingService bookingService)
        {
            _context = context;
            _bookingService = bookingService;
        }

        [HttpGet("patient/{identificationNumber}/next")]
        public IActionResult GetPatientNextAppointnemtn(long identificationNumber)
        {
            var bockings = _context.Order.OrderBy(x => x.StartTime).ToList();

            if (bockings.Where(x => x.Patient.Id == identificationNumber).Count() == 0)
            {
                return StatusCode(502);
            }
            else
            {
                var bookings2 = bockings.Where(x => x.PatientId == identificationNumber);
                if (bookings2.Where(x => x.StartTime > DateTime.Now).Count() == 0)
                {
                    return StatusCode(502);
                }
                else
                {
                    var bookings3 = bookings2.Where(x => x.StartTime > DateTime.Now);
                    return Ok(new
                    {
                        bookings3.First().Id,
                        bookings3.First().DoctorId,
                        bookings3.First().StartTime,
                        bookings3.First().EndTime
                    });
                }
            }
        }

        [HttpPost()]
        public IActionResult AddBooking(NewBookingRequest newBookingRequest)
        {
            //var bookingId = new Guid();
            //var bookingStartTime = newBookingRequest.StartTime;
            //var bookingEndTime = newBookingRequest.EndTime;
            //var bookingPatientId = newBookingRequest.PatientId;
            //var bookingPatient = _context.Patient.FirstOrDefault(x => x.Id == newBookingRequest.PatientId);
            //var bookingDoctorId = newBookingRequest.DoctorId;
            //var bookingDoctor = _context.Doctor.FirstOrDefault(x => x.Id == newBookingRequest.DoctorId);
            //var bookingSurgeryType = _context.Patient.FirstOrDefault(x => x.Id == bookingPatientId).Clinic.SurgeryType;

            //var myBooking = new Order
            //{
            //    Id = bookingId,
            //    StartTime = bookingStartTime,
            //    EndTime = bookingEndTime,
            //    PatientId = bookingPatientId,
            //    DoctorId = bookingDoctorId,
            //    Patient = bookingPatient,
            //    Doctor = bookingDoctor,
            //    SurgeryType = (int)bookingSurgeryType
            //};

            //_context.Order.AddRange(new List<Order> { myBooking });
            //_context.SaveChanges();

            _bookingService.AddBooking(newBookingRequest);

            return StatusCode(200);
        }
    }
}