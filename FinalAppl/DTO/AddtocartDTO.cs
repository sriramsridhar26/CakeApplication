using Microsoft.AspNetCore.Mvc;

namespace CakeApplication.DTO
{
    public class AddtocartDTO 
    {
        public string emailId { get; set; }
        public int itemId { get; set; }
        public string? size { get; set; }
    }
}
