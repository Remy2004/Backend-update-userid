using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoun.BLL.DTOs.Document
{
    public class UploadCharityDocumentsDto
    {
        public int CharityId { get; set; }

        public List<IFormFile> Files { get; set; } = new();

        // كل ملف نوعه ايه (نفس ترتيب الملفات)
        public List<string> DocumentTypes { get; set; } = new();
    }
}
