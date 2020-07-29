using System.Threading.Tasks;
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
        IPersonalInformationRepository PersonalInformation { get; }
        IAppUsageLogRepository AppUsageLog { get; }
        Task SaveAsync();
	}

}
