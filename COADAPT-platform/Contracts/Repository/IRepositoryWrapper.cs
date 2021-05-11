using System.Collections.Generic;
using System.Threading.Tasks;
using Constants;
using Contracts.Repository.ModelRepository;

namespace Contracts.Repository {

	public interface IRepositoryWrapper {

		IGroupRepository Group { get; }
		ISiteRepository Site { get; }
		IStudyRepository Study { get; }
		IOrganizationRepository Organization { get; }
		IAdministratorRepository Administrator { get; }
		ISubAdministratorRepository SubAdministrator { get; }
		ISupervisorRepository Supervisor { get; }
		ITherapistRepository Therapist { get; }
		IParticipantRepository Participant { get; }
		IStudyParticipantRepository StudyParticipant { get; }
        IPsychologicalReportRepository PsychologicalReport { get; }
        IPhysiologicalSignalRepository PhysiologicalSignal { get; }
        IOuraActivityRepository OuraActivity { get; }
        IOuraReadinessRepository OuraReadiness { get; }
        IOuraSleepRepository OuraSleep { get; }
        IAppUsageLogRepository AppUsageLog { get; }
        IUserAccessTokenRepository UserAccessToken { get; }
        Task<int> GetCoadaptUserIdByRole(string userId, IList<string> roles);

        Task SaveAsync();
	}

}
