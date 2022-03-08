using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiModels;
using Contracts.Repository.ModelRepository;
using Entities;
using Entities.Extensions;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository.ModelRepository {
	public class ParticipantRepository : RepositoryBase<Participant>, IParticipantRepository {

		private readonly COADAPTContext _coadaptContext;

		public ParticipantRepository(COADAPTContext coadaptContext) : base(coadaptContext) {
			_coadaptContext = coadaptContext;
		}

		public void CreateParticipant(Participant participant) {
			Create(participant);
		}

		public void DeleteParticipant(Participant participant) {
			Delete(participant);
		}

		public async Task<IEnumerable<Participant>> GetAllParticipantsAsync()
		{
			return await FindAll()
				.Include(x => x.User)
				.Include(p => p.Therapist)
				.ThenInclude(t => t.User)
				.ToListAsync();
		}

		public async Task<IEnumerable<ParticipantListResponse>> GetAllParticipantsWithReportsAsync() {
			return await FindByCondition(p => p.PsychologicalReports.Any())
				.Include(participant => participant.User)
				.Include(participant => participant.PsychologicalReports)
				.Select(p => new ParticipantListResponse {
					Id = p.Id,
					Code = p.Code,
					CreatedOn = p.CreatedOn,
					Therapist = _coadaptContext.Therapists.Where(t => t.Id == p.TherapistId).Select(y => y.User.UserName).FirstOrDefault(),
					PsychologicalReports = p.PsychologicalReports.Count,
					Organizations = _coadaptContext.Organizations.Where(o => o.Studies.Any(s => s.StudyParticipants.Any(sp => sp.StudyId == s.Id && sp.ParticipantId == p.Id && sp.Abandoned == false))).Select(y => y.Name).ToList(),
					Studies = _coadaptContext.Studies.Where(s => s.StudyParticipants.Any(sp => sp.StudyId == s.Id && sp.ParticipantId == p.Id && sp.Abandoned == false)).Select(y => y.Name).ToList(),
					Sites = _coadaptContext.Sites.Where(st => st.Study.StudyParticipants.Any(sp => sp.SiteId == st.Id && sp.ParticipantId == p.Id && sp.Abandoned == false)).Select(y => y.Name).ToList(),
					Groups = _coadaptContext.Groups.Where(g => g.Study.StudyParticipants.Any(sp => sp.GroupId == g.Id && sp.ParticipantId == p.Id && sp.Abandoned == false)).Select(y => y.Name).ToList()
				})
				.ToListAsync();
		}

		public async Task<IEnumerable<ParticipantListResponse>> GetAllParticipantsWithMetricsAsync() {
			return await FindByCondition(p => p.OuraActivities.Any() || p.PhysiologicalSignals.Any())
				.Include(participant => participant.User)
				.Include(participant => participant.OuraActivities)
				.Include(participant => participant.OuraReadinesses)
				.Include(participant => participant.OuraSleeps)
				.Include(participant => participant.PhysiologicalSignals)
				.Select(p => new ParticipantListResponse {
					Id = p.Id,
					Code = p.Code,
					CreatedOn = p.CreatedOn,
					Therapist = _coadaptContext.Therapists.Where(t => t.Id == p.TherapistId).Select(y => y.User.UserName).FirstOrDefault(),
					PhychologicalMetrics = p.OuraActivities.Count + p.OuraReadinesses.Count + p.OuraSleeps.Count + p.PhysiologicalSignals.Count,
					Organizations = _coadaptContext.Organizations.Where(o => o.Studies.Any(s => s.StudyParticipants.Any(sp => sp.StudyId == s.Id && sp.ParticipantId == p.Id && sp.Abandoned == false))).Select(y => y.Name).ToList(),
					Studies = _coadaptContext.Studies.Where(s => s.StudyParticipants.Any(sp => sp.StudyId == s.Id && sp.ParticipantId == p.Id && sp.Abandoned == false)).Select(y => y.Name).ToList(),
					Sites = _coadaptContext.Sites.Where(st => st.Study.StudyParticipants.Any(sp => sp.SiteId == st.Id && sp.ParticipantId == p.Id && sp.Abandoned == false)).Select(y => y.Name).ToList(),
					Groups = _coadaptContext.Groups.Where(g => g.Study.StudyParticipants.Any(sp => sp.GroupId == g.Id && sp.ParticipantId == p.Id && sp.Abandoned == false)).Select(y => y.Name).ToList()
				})
				.ToListAsync();
		}

		public async Task<IEnumerable<ParticipantListResponse>> GetParticipantListAsync() {
			return await FindAll()
				.Include(participant => participant.User)
				.Select(p => new ParticipantListResponse {
					Id = p.Id,
					Code = p.Code,
					CreatedOn = p.CreatedOn,
					Therapist = _coadaptContext.Therapists.Where(t => t.Id == p.TherapistId).Select(y => y.User.UserName).FirstOrDefault(),
					Organizations = _coadaptContext.Organizations.Where(o => o.Studies.Any(s => s.StudyParticipants.Any(sp => sp.StudyId == s.Id && sp.ParticipantId == p.Id && sp.Abandoned == false))).Select(y => y.Name).ToList(),
					Studies = _coadaptContext.Studies.Where(s => s.StudyParticipants.Any(sp => sp.StudyId == s.Id && sp.ParticipantId == p.Id && sp.Abandoned == false)).Select(y => y.Name).ToList(),
					Sites = _coadaptContext.Sites.Where(st => st.Study.StudyParticipants.Any(sp => sp.SiteId == st.Id && sp.ParticipantId == p.Id && sp.Abandoned == false)).Select(y => y.Name).ToList(),
					Groups = _coadaptContext.Groups.Where(g => g.Study.StudyParticipants.Any(sp => sp.GroupId == g.Id && sp.ParticipantId == p.Id && sp.Abandoned == false)).Select(y => y.Name).ToList()
				})
				.ToListAsync();
		}

		public async Task<Participant> GetParticipantByIdAsync(int participantId) {
			return await FindByCondition(p => p.Id.Equals(participantId))
				.Include(p => p.Therapist)
				.ThenInclude(t => t.User)
				.DefaultIfEmpty(new Participant())
				.SingleAsync();
		}

		public async Task<ParticipantListResponse> GetParticipantListItemByIdAsync(int participantId) {
			return await FindByCondition(p => p.Id.Equals(participantId))
				.Include(p => p.User)
				.Include(p => p.PsychologicalReports)
				.Include(p => p.OuraActivities)
				.Include(p => p.OuraReadinesses)
				.Include(p => p.OuraSleeps)
				.Include(p => p.PhysiologicalSignals)
				.Select(p => new ParticipantListResponse {
					Id = p.Id,
					Code = p.Code,
					CreatedOn = p.CreatedOn,
					Therapist = _coadaptContext.Therapists.Where(t => t.Id == p.TherapistId).Select(y => y.User.UserName).FirstOrDefault(),
					PsychologicalReports = p.PsychologicalReports.Count,
					PhychologicalMetrics = p.OuraActivities.Count +p.OuraReadinesses.Count + p.OuraSleeps.Count + p.PhysiologicalSignals.Count,
					Organizations = _coadaptContext.Organizations.Where(o => o.Studies.Any(s => s.StudyParticipants.Any(sp => sp.StudyId == s.Id && sp.ParticipantId == p.Id && sp.Abandoned == false))).Select(y => y.Name).ToList(),
					Studies = _coadaptContext.Studies.Where(s => s.StudyParticipants.Any(sp => sp.StudyId == s.Id && sp.ParticipantId == p.Id && sp.Abandoned == false)).Select(y => y.Name).ToList(),
					Sites = _coadaptContext.Sites.Where(st => st.Study.StudyParticipants.Any(sp => sp.SiteId == st.Id && sp.ParticipantId == p.Id && sp.Abandoned == false)).Select(y => y.Name).ToList(),
					Groups = _coadaptContext.Groups.Where(g => g.Study.StudyParticipants.Any(sp => sp.GroupId == g.Id && sp.ParticipantId == p.Id && sp.Abandoned == false)).Select(y => y.Name).ToList()
				})
				.DefaultIfEmpty(new ParticipantListResponse())
				.SingleAsync();
		}

		public async Task<Participant> GetParticipantByCodeAsync(string code) {
			return await FindByCondition(p => p.Code.Equals(code))
				.DefaultIfEmpty(new Participant())
				.SingleAsync();
		}

		public async Task<IEnumerable<Participant>> GetParticipantsByTherapistIdAsync(int therapistId) {
			return await FindByCondition(p => p.TherapistId.Equals(therapistId))
				.Include(p => p.Therapist)
				.ThenInclude(t => t.User)
				.ToListAsync();
		}

		public async Task<IEnumerable<ParticipantListResponse>> GetParticipantsListByTherapistIdAsync(int therapistId) {
			return await FindByCondition(p => p.TherapistId.Equals(therapistId))
				.Include(p => p.Therapist)
				.ThenInclude(t => t.User)
				.Select(p => new ParticipantListResponse {
					Id = p.Id,
					Code = p.Code,
					CreatedOn = p.CreatedOn,
					Therapist = _coadaptContext.Therapists.Where(t => t.Id == p.TherapistId).Select(y => y.User.UserName).FirstOrDefault(),
					Organizations = _coadaptContext.Organizations.Where(o => o.Studies.Any(s => s.StudyParticipants.Any(sp => sp.StudyId == s.Id && sp.ParticipantId == p.Id && sp.Abandoned == false))).Select(y => y.Name).ToList(),
					Studies = _coadaptContext.Studies.Where(s => s.StudyParticipants.Any(sp => sp.StudyId == s.Id && sp.ParticipantId == p.Id && sp.Abandoned == false)).Select(y => y.Name).ToList(),
					Sites = _coadaptContext.Sites.Where(st => st.Study.StudyParticipants.Any(sp => sp.SiteId == st.Id && sp.ParticipantId == p.Id && sp.Abandoned == false)).Select(y => y.Name).ToList(),
					Groups = _coadaptContext.Groups.Where(g => g.Study.StudyParticipants.Any(sp => sp.GroupId == g.Id && sp.ParticipantId == p.Id && sp.Abandoned == false)).Select(y => y.Name).ToList()
				})
				.ToListAsync();
		}

		public async Task<IEnumerable<ParticipantListResponse>> GetParticipantsWithReportsByTherapistIdAsync(int therapistId) {
			return await FindByCondition(p => p.TherapistId.Equals(therapistId) && p.PsychologicalReports.Any())
				.Include(p => p.Therapist)
				.ThenInclude(t => t.User)
				.Include(p => p.PsychologicalReports)
				.Select(p => new ParticipantListResponse {
					Id = p.Id,
					Code = p.Code,
					CreatedOn = p.CreatedOn,
					Therapist = _coadaptContext.Therapists.Where(t => t.Id == p.TherapistId).Select(y => y.User.UserName).FirstOrDefault(),
					PsychologicalReports = p.PsychologicalReports.Count,
					Organizations = _coadaptContext.Organizations.Where(o => o.Studies.Any(s => s.StudyParticipants.Any(sp => sp.StudyId == s.Id && sp.ParticipantId == p.Id && sp.Abandoned == false))).Select(y => y.Name).ToList(),
					Studies = _coadaptContext.Studies.Where(s => s.StudyParticipants.Any(sp => sp.StudyId == s.Id && sp.ParticipantId == p.Id && sp.Abandoned == false)).Select(y => y.Name).ToList(),
					Sites = _coadaptContext.Sites.Where(st => st.Study.StudyParticipants.Any(sp => sp.SiteId == st.Id && sp.ParticipantId == p.Id && sp.Abandoned == false)).Select(y => y.Name).ToList(),
					Groups = _coadaptContext.Groups.Where(g => g.Study.StudyParticipants.Any(sp => sp.GroupId == g.Id && sp.ParticipantId == p.Id && sp.Abandoned == false)).Select(y => y.Name).ToList()
				})
				.ToListAsync();
		}

		public async Task<IEnumerable<ParticipantListResponse>> GetParticipantsWithMetricsByTherapistIdAsync(int therapistId) {
			return await FindByCondition(p => p.TherapistId.Equals(therapistId) && (p.OuraActivities.Any() || p.PhysiologicalSignals.Any()))
				.Include(p => p.Therapist)
				.ThenInclude(t => t.User)
				.Include(p => p.OuraActivities)
				.Include(p => p.OuraReadinesses)
				.Include(p => p.OuraSleeps)
				.Include(p => p.PhysiologicalSignals)
				.Select(p => new ParticipantListResponse {
					Id = p.Id,
					Code = p.Code,
					CreatedOn = p.CreatedOn,
					Therapist = _coadaptContext.Therapists.Where(t => t.Id == p.TherapistId).Select(y => y.User.UserName).FirstOrDefault(),
					PhychologicalMetrics = p.OuraActivities.Count + p.OuraReadinesses.Count + p.OuraSleeps.Count + p.PhysiologicalSignals.Count,
					Organizations = _coadaptContext.Organizations.Where(o => o.Studies.Any(s => s.StudyParticipants.Any(sp => sp.StudyId == s.Id && sp.ParticipantId == p.Id && sp.Abandoned == false))).Select(y => y.Name).ToList(),
					Studies = _coadaptContext.Studies.Where(s => s.StudyParticipants.Any(sp => sp.StudyId == s.Id && sp.ParticipantId == p.Id && sp.Abandoned == false)).Select(y => y.Name).ToList(),
					Sites = _coadaptContext.Sites.Where(st => st.Study.StudyParticipants.Any(sp => sp.SiteId == st.Id && sp.ParticipantId == p.Id && sp.Abandoned == false)).Select(y => y.Name).ToList(),
					Groups = _coadaptContext.Groups.Where(g => g.Study.StudyParticipants.Any(sp => sp.GroupId == g.Id && sp.ParticipantId == p.Id && sp.Abandoned == false)).Select(y => y.Name).ToList()
				})
				.ToListAsync();
		}

		public async Task<int> CountParticipantsByTherapistIdAsync(int therapistId) {
			return await CountByCondition(p => p.TherapistId.Equals(therapistId));
		}

		public async Task<Participant> GetParticipantByUserIdAsync(string participantUserId) {
			return await FindByCondition(p => p.UserId.Equals(participantUserId))
				.DefaultIfEmpty(new Participant())
				.SingleAsync();
		}

		public void UpdateParticipant(Participant dbParticipant, Participant participant) {
			dbParticipant.Map(participant);
			Update(dbParticipant);
		}

		public void DetachParticipant(Participant participant)
		{
			Detach(participant);
		}
	}
}
