using Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.Repository.ModelRepository {
    public interface IPersonalInformationRepository {
        Task<IEnumerable<PersonalInformation>> GetAllPersonalInformationAsync();
        Task<PersonalInformation> GetPersonalInformationByIdAsync(int id);
        Task<IEnumerable<PersonalInformation>> GetAllPersonalInformationByParticipantIdAsync(int participantId);
        Task<IEnumerable<PersonalInformation>> GetAllPersonalInformationAfterDateAsync(DateTime date);
        Task<IEnumerable<PersonalInformation>> GetAllPersonalInformationAfterDateByParticipantIdAsync(DateTime date, int participantId);
        Task<PersonalInformation> GetLatestPersonalInformationByParticipantIdAndPropertyAsync(int participantId, string property);
        Task<IEnumerable<PersonalInformation>> GetAllPersonalInformationByParticipantIdAndPropertyAsync(int participantId, string property);
        Task<IEnumerable<PersonalInformation>> GetAllPersonalInformationAfterDateByParticipantIdAndPropertyAsync(DateTime date, int participantId, string property);
        void CreatePersonalInformation(PersonalInformation personalInformation);
        void DeletePersonalInformation(PersonalInformation personalInformation);

    }
}
