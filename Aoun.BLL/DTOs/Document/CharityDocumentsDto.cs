using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoun.BLL.DTOs.Document
{
    public class CharityDocumentsDto
    {
        public IFormFile RegistrationCertificate { get; set; } = null!;
        public IFormFile TaxCard { get; set; } = null!;
        public IFormFile BankAccountProof { get; set; } = null!;
        public IFormFile NationalId { get; set; } = null!;
    }
}
