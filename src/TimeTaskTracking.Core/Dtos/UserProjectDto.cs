using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Core.Dtos
{
    public class UserProjectDto
    {
        public int? Id { get; set; }
        public string? Description { get; set; }
        public string? SvnUrl { get; set; }
        public string? LiveUrl { get; set; }
        public string? LocalUrl { get; set; }
        public string ProjectName { get; set; }
        public string? ApplicationUsersId { get; set; }
        public int? ProjectsId { get; set; }
        public string? Technology { get; set; }
        public bool? Feature { get; set; }
    }
}
