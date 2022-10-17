using Microsoft.AspNetCore.Mvc;

namespace CakeApplication.DTO
{
        public class LoginDTO
        {
            public string emailId { get; set; }
            public string password { get; set; }


        }
        public class UserDTO : LoginDTO
        {
            public string customerName { get; set; }
            public string MobileNo { get; set; }
            public string Address { get; set; }
        }
}
