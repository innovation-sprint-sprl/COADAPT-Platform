using Contracts.Repository.ModelRepository;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.ModelRepository {
    public class PersonalInformationRepository : RepositoryBase<PersonalInformation>, IPersonalInformationRepository {
        public PersonalInformationRepository(COADAPTContext coadaptContext) : base(coadaptContext) { }

        public void CreatePersonalInformation(PersonalInformation personalInformation) {
            Create(personalInformation);
        }

        public void DeletePersonalInformation(PersonalInformation personalInformation) {
            Delete(personalInformation);
        }

        public async Task<IEnumerable<PersonalInformation>> GetAllPersonalInformationAfterDateAsync(DateTime date) {
            return await FindByCondition(p => p.DateOfReport.CompareTo(date) >= 0)
                .ToListAsync();
        }

        public async Task<IEnumerable<PersonalInformation>> GetAllPersonalInformationAfterDateByParticipantIdAsync(DateTime date, int participantId) {
            return await FindByCondition(p => p.DateOfReport.CompareTo(date) >= 0 && p.ParticipantId.Equals(participantId))
                .ToListAsync();
        }

        public async Task<IEnumerable<PersonalInformation>> GetAllPersonalInformationAsync() {
            return await FindAll().ToListAsync();
        }

        public async Task<IEnumerable<PersonalInformation>> GetAllPersonalInformationByParticipantIdAsync(int participantId) {
            return await FindByCondition(p => p.ParticipantId.Equals(participantId))
                .ToListAsync();
        }

        public async Task<PersonalInformation> GetLatestPersonalInformationByParticipantIdAndPropertyAsync(int participantId, string property) {
            return await FindByCondition(p => p.ParticipantId.Equals(participantId) && typeof(PersonalInformation).GetProperty(property).GetValue(p) != null)
                .DefaultIfEmpty(new PersonalInformation())
                .LastAsync();
        }

        public async Task<IEnumerable<PersonalInformation>> GetAllPersonalInformationByParticipantIdAndPropertyAsync(int participantId, string property) {
            return await FindByCondition(p => p.ParticipantId.Equals(participantId) && typeof(PersonalInformation).GetProperty(property).GetValue(p) != null)
                .ToListAsync();
        }

        public async Task<IEnumerable<PersonalInformation>> GetAllPersonalInformationAfterDateByParticipantIdAndPropertyAsync(DateTime date, int participantId, string property) {
            return await FindByCondition(p => p.ParticipantId.Equals(participantId) &&
                    p.DateOfReport.CompareTo(date) >= 0 &&
                    typeof(PersonalInformation).GetProperty(property).GetValue(p) != null)
                .ToListAsync();
        }

        public async Task<PersonalInformation> GetPersonalInformationByIdAsync(int id) {
            return await FindByCondition(p => p.Id == id)
                .DefaultIfEmpty(new PersonalInformation())
                .SingleAsync();
        }
    }
}
