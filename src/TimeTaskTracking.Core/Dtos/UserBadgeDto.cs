using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Core.Dtos
{
    public class UserBadgeDto
    {
        public int Id { get; set; }
        public string? BadgeName { get; set; }
        public string? BadgeImage { get; set; }
    }

}
