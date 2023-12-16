namespace BHYT_BE.Internal.Models
{
    public class VerifyOTPRequest
    {
        public string Email { get; set; }
        public string OTP { get; set; }
    }

    public class SendOTPRequest
    {
        public string Email { get; set; }
    }
}
