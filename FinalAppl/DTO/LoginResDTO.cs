using CakeApplication.Model;
using Microsoft.AspNetCore.Mvc;

namespace CakeApplication.DTO
{
    public class LoginResDTO
    {
        public string emailId { get; set; }
        public string customerName { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }

        public LoginResDTO(User user)
        {
            this.emailId = user.emailId;
            this.customerName = user.customerName;
            this.MobileNo = user.MobileNo;
            this.Address = user.Address;
        }
    }
}
