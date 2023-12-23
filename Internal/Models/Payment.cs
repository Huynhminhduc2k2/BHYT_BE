namespace BHYT_BE.Internal.Models
{
    public class Payment
    {
        public ulong ID { get; set; }
        public string PricingPlan { get; set; }
        public float Price { get; set; }
    }
}
