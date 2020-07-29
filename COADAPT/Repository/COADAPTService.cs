using System.Threading.Tasks;
using Contracts.Repository;
using Contracts.Repository.ModelRepository;
using Entities;
using Repository.ModelRepository;

namespace Repository {

	public class COADAPTService : IRepositoryWrapper {

		private readonly COADAPTContext _coadaptContext;

		private GroupRepository _group;
		private SiteRepository _site;
		private StudyRepository _study;
		private OrganizationRepository _organization;
		private AdministratorRepository _administrator;
		private SubAdministratorRepository _subAdministrator;
		private SupervisorRepository _supervisor;
		private TherapistRepository _therapist;
		private ParticipantRepository _participant;
		private StudyParticipantRepository _studyParticipant;
        private PersonalInformationRepository _personalInformation;
        private AppUsageLogRepository _appUsageLog;

		public COADAPTService(COADAPTContext coadaptContext) {
			_coadaptContext = coadaptContext;
		}

		public IGroupRepository Group => _group ?? (_group = new GroupRepository(_coadaptContext));

		public ISiteRepository Site => _site ?? (_site = new SiteRepository(_coadaptContext));

		public IStudyRepository Study => _study ?? (_study = new StudyRepository(_coadaptContext));

		public IOrganizationRepository Organization =>
			_organization ?? (_organization = new OrganizationRepository(_coadaptContext));

		public IAdministratorRepository Administrator =>
			_administrator ?? (_administrator = new AdministratorRepository(_coadaptContext));

		public ISubAdministratorRepository SubAdministrator =>
			_subAdministrator ?? (_subAdministrator = new SubAdministratorRepository(_coadaptContext));

		public ISupervisorRepository Supervisor =>
			_supervisor ?? (_supervisor = new SupervisorRepository(_coadaptContext));

		public ITherapistRepository Therapist =>
			_therapist ?? (_therapist = new TherapistRepository(_coadaptContext));

		public IParticipantRepository Participant =>
			_participant ?? (_participant = new ParticipantRepository(_coadaptContext));

		public IStudyParticipantRepository StudyParticipant =>
			_studyParticipant ?? (_studyParticipant = new StudyParticipantRepository(_coadaptContext));

		public IPersonalInformationRepository PersonalInformation =>
			_personalInformation ?? (_personalInformation = new PersonalInformationRepository(_coadaptContext));

		public IAppUsageLogRepository AppUsageLog =>
			_appUsageLog ?? (_appUsageLog = new AppUsageLogRepository(_coadaptContext));

		public async Task SaveAsync() {
			await _coadaptContext.SaveChangesAsync();
		}

	}

}
