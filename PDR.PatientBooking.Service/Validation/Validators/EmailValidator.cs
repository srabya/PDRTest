using System.ComponentModel.DataAnnotations;
namespace PDR.PatientBooking.Service.Validation.Validators
{
    public class Email
    {
        public static bool IsValid(string email)
        {

            return new EmailAddressAttribute().IsValid(email);

        }
    }
}
