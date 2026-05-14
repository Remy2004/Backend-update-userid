using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoun.BLL.DTOs.User
{
   

    public class UserDonationHistoryDto
    {
        public string TargetTitle { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
