using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Ej15.Api.Dtos
{
    public class CreateDoctorDto
    {
        public string Name { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public Guid SpecialityId { get; set; }
    }
}
