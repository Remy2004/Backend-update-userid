using Aoun.BLL.DTOs.Charity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoun.BLL.Interfaces
{
    public interface ICharityDashboardService
    {
        Task<CharityDashboardDto> GetDashboardAsync(string userId);
    }
}
