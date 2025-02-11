namespace Donation.Web.Services
{
    public class DonationService
    {
        private List<Donation> donations = new()
    {
        new Donation { Code = "12345", Amount = 100, Date = DateTime.Now },
        new Donation { Code = "67890", Amount = 200, Date = DateTime.Now.AddDays(-1) }
    };

        public Donation GetDonationByCode(string code)
        {
            return donations.FirstOrDefault(d => d.Code == code);
        }
    }

    public class Donation
    {
        public string Code { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
