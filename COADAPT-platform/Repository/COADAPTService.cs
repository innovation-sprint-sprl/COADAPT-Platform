using System.Collections.Generic;
using System.Threading.Tasks;
using Constants;
using Contracts.Repository;
using Contracts.Repository.ModelRepository;
using Entities;
using Entities.Models;
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
        private PsychologicalReportRepository _psychologicalReport;
        private PhysiologicalSignalRepository _physiologicalSignal;
        private OuraActivityRepository _ouraActivity;
        private OuraReadinessRepository _ouraReadiness;
        private OuraSleepRepository _ouraSleep;
        private AppUsageLogRepository _appUsageLog;
        private UserAccessTokenRepository _accessTokenRepository;

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

		public IPsychologicalReportRepository PsychologicalReport =>
			_psychologicalReport ?? (_psychologicalReport = new PsychologicalReportRepository(_coadaptContext));

		public IPhysiologicalSignalRepository PhysiologicalSignal =>
			_physiologicalSignal ?? (_physiologicalSignal = new PhysiologicalSignalRepository(_coadaptContext));

		public IOuraActivityRepository OuraActivity =>
			_ouraActivity ?? (_ouraActivity = new OuraActivityRepository(_coadaptContext));

		public IOuraReadinessRepository OuraReadiness =>
			_ouraReadiness ?? (_ouraReadiness = new OuraReadinessRepository(_coadaptContext));

		public IOuraSleepRepository OuraSleep =>
			_ouraSleep ?? (_ouraSleep = new OuraSleepRepository(_coadaptContext));

		public IAppUsageLogRepository AppUsageLog =>
			_appUsageLog ?? (_appUsageLog = new AppUsageLogRepository(_coadaptContext));

		public IUserAccessTokenRepository UserAccessToken => _accessTokenRepository ?? (_accessTokenRepository = new UserAccessTokenRepository(_coadaptContext));
		
		public async Task<int> GetCoadaptUserIdByRole(string userId, IList<string> roles) {
			if (roles.Contains(Role.AdministratorRole)) {
				var admin = await Administrator.GetAdministratorByUserIdAsync(userId);
				return admin.Id;
			}

			if (roles.Contains(Role.SubAdministratorRole)) {
				var subAdmin = await SubAdministrator.GetSubAdministratorByUserIdAsync(userId);
				return subAdmin.Id;
			}

			if (roles.Contains(Role.SupervisorRole)) {
				var supervisor = await Supervisor.GetSupervisorByUserIdAsync(userId);
				return supervisor.Id;
			}

			if (roles.Contains(Role.TherapistRole)) {
				var therapist = await Therapist.GetTherapistByUserIdAsync(userId);
				return therapist.Id;
			}

			var participant = await Participant.GetParticipantByUserIdAsync(userId);
			return participant.Id;
		}
		
		public async Task SaveAsync() {
			await _coadaptContext.SaveChangesAsync();
		}

	}

}
