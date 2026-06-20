using Dsw2026Ej15.Data.Interfaces;
using Dsw2026Ej15.Domain;
using Dsw2026Ej15.Domain.Entities;
using Dsw2026Ej15.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2026Ej15.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IPersistence _persistence;

        public DoctorsController(IPersistence persistence)
        {
            _persistence = persistence;
        }

        // Clase DTO (Data Transfer Object) para recibir el body del POST
        public class DoctorCreateDto
        {
            public string Name { get; set; } = string.Empty;
            public string LicenseNumber { get; set; } = string.Empty;
            public Guid SpecialityId { get; set; }
        }

        // i. Primer endpoint: POST api/doctors
        [HttpPost]
        public async Task<IActionResult> CreateDoctor([FromBody] DoctorCreateDto dto)
        {
            // Validaciones (lanzan ValidationException que atrapará el Middleware)
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ValidationException("Name es requerido.");

            if (string.IsNullOrWhiteSpace(dto.LicenseNumber))
                throw new ValidationException("LicenseNumber es requerido.");

            var speciality = await _persistence.GetByIdAsync<Speciality>(dto.SpecialityId);
            if (speciality == null)
                throw new ValidationException("SpecialityId debe existir.");

            // Comportamiento: se crea activo
            var newDoctor = new Doctor
            {
                _Id = Guid.NewGuid(), // Usando _Id como tienes en tu clase BaseEntity
                _name = dto.Name,
                _licenseNumber = dto.LicenseNumber,
                _speciality = speciality,
                _isActive = true
            };

            await _persistence.AddAsync(newDoctor);

            // Respuesta 201 Created
            return Created(string.Empty, newDoctor);
        }

        // ii. Segundo endpoint: GET api/doctors
        [HttpGet]
        public async Task<IActionResult> GetActiveDoctors()
        {
            var allDoctors = await _persistence.GetAllAsync<Doctor>();

            // Descripción: obtener todos los médicos activos
            var activeDoctors = allDoctors.Where(d => d._isActive).ToList();

            return Ok(activeDoctors);
        }

        // iii. Tercer endpoint: GET api/doctors/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDoctorById(Guid id)
        {
            var doctor = await _persistence.GetByIdAsync<Doctor>(id);

            // Validación: debe existir y estar activo
            if (doctor == null || !doctor._isActive)
                return NotFound(); // Retorna 404

            // Devolvemos solo las propiedades pedidas
            var response = new
            {
                Name = doctor._name,
                LicenseNumber = doctor._licenseNumber,
                SpecialityName = doctor._speciality?._name
            };

            return Ok(response);
        }

        // iv. Cuarto endpoint: DELETE api/doctors/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteDoctor(Guid id)
        {
            var doctor = await _persistence.GetByIdAsync<Doctor>(id);

            // Validación: debe existir y estar activo
            if (doctor == null || !doctor._isActive)
                return NotFound(); // Retorna 404

            // Establecer como inactivo
            doctor._isActive = false;
            await _persistence.UpdateAsync(doctor);

            return NoContent(); // Retorna 204
        }
    }
}