using Dsw2026Ej15.Data.Interfaces;
using Dsw2026Ej15.Domain;
using Dsw2026Ej15.Domain.Entities;
using System.Text.Json;

namespace Dsw2026Ej15.Data
{
    public class PersistenceInMemory : IPersistence
    {

        private readonly List<Doctor> _doctors = new List<Doctor>();
        private readonly List<Speciality> _specialities = new List<Speciality>();

        public PersistenceInMemory() 
        {
            LoadSpecialities();
        }

        private void LoadSpecialities()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "specialities.json");
            if (!File.Exists(path)) return;
            var json = File.ReadAllText(path);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var items = JsonSerializer.Deserialize<List<Speciality>>(json, options);
            if (items != null) _specialities.AddRange(items);

        }

        public Task AddAsync<T>(T entity) where T : BaseEntity
        {
            
            if (entity is Doctor doctor)
            {
                _doctors.Add(doctor);
            }
            else if (entity is Speciality speciality)
            {
                _specialities.Add(speciality);
            }

            return Task.CompletedTask;
        }

        public Task<List<T>> GetAllAsync<T>() where T : BaseEntity
        {
            
            if (typeof(T) == typeof(Doctor))
            {
                return Task.FromResult(_doctors.Cast<T>().ToList());
            }

            if (typeof(T) == typeof(Speciality))
            {
                return Task.FromResult(_specialities.Cast<T>().ToList());
            }
            return Task.FromResult(new List<T>());
        }

        public Task<T?> GetByIdAsync<T>(Guid id) where T : BaseEntity
        {
           
            if (typeof(T) == typeof(Doctor))
            {
                var doctor = _doctors.FirstOrDefault(d => d._Id == id);
                return Task.FromResult(doctor as T);
            }

            if (typeof(T) == typeof(Speciality))
            {
                var speciality = _specialities.FirstOrDefault(s => s._Id == id);
                return Task.FromResult(speciality as T);
            }
            return Task.FromResult<T?>(null);
        }

        public Task<bool> SaveChangesAsync()
        {
            
            return Task.FromResult(true);
        }

        public Task UpdateAsync<T>(T entity) where T : BaseEntity
        {
            
            if (entity is Doctor doctor)
            {
                var index = _doctors.FindIndex(d => d._Id == entity._Id);
                if (index != -1)
                {
                    _doctors[index] = doctor;
                }
            }
            else if (entity is Speciality speciality)
            {
                var index = _specialities.FindIndex(s => s._Id == entity._Id);
                if (index != -1)
                {
                    _specialities[index] = speciality;
                }
            }
            return Task.CompletedTask;
        }

        

    }
}
