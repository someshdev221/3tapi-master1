using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Core.Dtos    
{
    public class UserToolsDto
    {
        public int Id { get; set; }  
        public string Description { get; set; }     
        public string NetworkUrl { get; set; }     
        public string LocalUrl { get; set; }
        public DateTime DateTime { get; set; }
        public string ApplicationUsersId { get; set; }
        public string Technology { get; set; }
    }
}
