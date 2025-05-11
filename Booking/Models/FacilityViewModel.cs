namespace Booking.Models
{
    public class FacilityViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Interest { get; set; }
        public int FreeSlots { get; set; }
        public decimal Price { get; set; } 
    }
}
