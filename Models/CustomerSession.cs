namespace ShopAdmin.Models
{
    public class CustomerSession
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime? Birthday { get; set; }
        public string Sex { get; set; }
        public int IsActive { get; set; }
        public int? RPoint { get; set; }
        public CustomerSession()
        {

        }

        public CustomerSession(Customer customer)
        {
            Id = customer.Id;
            Name = customer.Name;
            Email = customer.Email;
            Phone = customer.Phone;
            Address = customer.Address;
            Birthday = customer.Birthday;
            Sex = customer.Sex;
            IsActive = customer.IsActive;
            RPoint = customer.RPoint;
        }
    }
}
