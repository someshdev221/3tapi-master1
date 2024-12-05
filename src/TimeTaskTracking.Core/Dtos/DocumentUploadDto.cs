using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Core.Dtos
{
    public class DocumentUploadDto
    {
        public int ProjectId { get; set; }
        public IFormFile? File { get; set; }
    }
}
