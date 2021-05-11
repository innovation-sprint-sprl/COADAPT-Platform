using Constants;
using Contracts.Logger;
using Contracts.Repository;
using Entities.Extensions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace UserManagement.WebAPI {

	public static class SeedData {

		public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw) {
			// Apply automatic migrations
			var hostingEnvironment = serviceProvider.GetService<IHostingEnvironment>();
			// if (hostingEnvironment.IsProduction()) {
				var dbContext = serviceProvider.GetRequiredService<COADAPTContext>();
				await dbContext.Database.MigrateAsync();
			// }

			await CreateUser(serviceProvider, testUserPw, "admin@coadapt.eu", Role.AdministratorRole);
			// University of Trento
			var subAdmin = await CreateUser(serviceProvider, "t3mpP@ss", "subadmin@unitn.it", Role.SubAdministratorRole);
			var organization = new Organization { Name = "University of Trento", Shortname = "UNITN" };
			await CreateOrganization(serviceProvider, organization, subAdmin.Id);
			var supervisor = await CreateUser(serviceProvider, "t3mpP@ss", "supervisor@unitn.it", Role.SupervisorRole);
			var study = new Study { Name = "Primo Turno", Shortname = "TURN1", OrganizationId = organization.Id };
			await CreateStudy(serviceProvider, study, supervisor.Id);
			var site = new Site { Name = "Trento", Shortname = "TN", StudyId = study.Id };
			await CreateSite(serviceProvider, site);
			var group = new Group { Name = "D", Shortname = "D", StudyId = study.Id };
			await CreateGroup(serviceProvider, group);
			var participant = await CreateUser(serviceProvider, testUserPw, "participant@gmail.com", Role.ParticipantRole, "SNJ-784");
			await ParticipantToStudy(serviceProvider, participant, study, site, group);
			/*
			var subAdmin = await CreateUser(serviceProvider, testUserPw, "subadmin@coadapt.eu", Role.SubAdministratorRole);
			var organization = new Organization { Name = "Innovation Sprint", Shortname = "iSprint" };
			await CreateOrganization(serviceProvider, organization, subAdmin.Id);
			var supervisor1 = await CreateUser(serviceProvider, testUserPw, "supervisor1@coadapt.eu", Role.SupervisorRole);
			var supervisor2 = await CreateUser(serviceProvider, testUserPw, "supervisor2@coadapt.eu", Role.SupervisorRole);
			var study1 = new Study { Name = "Size of Braintumor", Shortname = "braintumor", OrganizationId = organization.Id };
			await CreateStudy(serviceProvider, study1, supervisor1.Id);
			var study2 = new Study { Name = "Dementia", Shortname = "dementia", OrganizationId = organization.Id };
			await CreateStudy(serviceProvider, study2, supervisor2.Id);
			var therapist = await CreateUser(serviceProvider, testUserPw, "therapist@coadapt.eu", Role.TherapistRole);
			var participant1 = await CreateUser(serviceProvider, testUserPw, "participant1@coadapt.eu", Role.ParticipantRole, "ABCDEF00");
			var participant2 = await CreateUser(serviceProvider, testUserPw, "participant2@coadapt.eu", Role.ParticipantRole, "ABCDEF01");
			var participant3 = await CreateUser(serviceProvider, testUserPw, "participant3@coadapt.eu", Role.ParticipantRole, "ABCDEF02");
			var site1 = new Site { Name = "Athens", Shortname = "ATH", StudyId = study1.Id };
			await CreateSite(serviceProvider, site1);
			var site2 = new Site { Name = "Rome", Shortname = "ROM", StudyId = study1.Id };
			await CreateSite(serviceProvider, site2);
			var site3 = new Site { Name = "London", Shortname = "LON", StudyId = study2.Id };
			await CreateSite(serviceProvider, site3);
			var group1 = new Group { Name = "Actual", Shortname = "ACT", StudyId = study1.Id };
			await CreateGroup(serviceProvider, group1);
			var group2 = new Group { Name = "Placebo", Shortname = "PLA", StudyId = study1.Id };
			await CreateGroup(serviceProvider, group2);
			var group3 = new Group { Name = "Actual", Shortname = "ACT", StudyId = study2.Id };
			await CreateGroup(serviceProvider, group3);
			await TherapistToParticipant(serviceProvider, therapist, participant1);
			await TherapistToParticipant(serviceProvider, therapist, participant3);
			await ParticipantToStudy(serviceProvider, participant1, study1, site1, group1);
			await ParticipantToStudy(serviceProvider, participant1, study2, site3, group3);
			await ParticipantToStudy(serviceProvider, participant2, study1, site1, group2);
			await ParticipantToStudy(serviceProvider, participant3, study1, site2, group1);
			*/
		}

		private static async Task<IdentityUser> CreateUser(IServiceProvider serviceProvider,
			string testUserPw, string userName, string role, string code = "") {
			var logger = serviceProvider.GetRequiredService<ILoggerManager>();
			var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();
			if (userManager == null) {
				throw new Exception("User manager is null");
			}
			var user = await userManager.FindByNameAsync(userName);
			if (user == null) {
				user = new IdentityUser { UserName = userName };
				await userManager.CreateAsync(user, testUserPw);
				logger.LogInfo($"User {user.UserName} created");
				var coadaptService = serviceProvider.GetRequiredService<IRepositoryWrapper>();
				if (role.Equals(Role.AdministratorRole)) {
					var adminUser = new Administrator { UserId = user.Id };
					coadaptService.Administrator.CreateAdministrator(adminUser);
					await coadaptService.SaveAsync();
					logger.LogInfo($"Administrator entry with ID {adminUser.Id} created on {adminUser.CreatedOn}");
				} else if (role.Equals(Role.SubAdministratorRole)) {
					var subAdminUser = new SubAdministrator { UserId = user.Id };
					coadaptService.SubAdministrator.CreateSubAdministrator(subAdminUser);
					await coadaptService.SaveAsync();
					logger.LogInfo($"Sub-administrator entry with ID {subAdminUser.Id} created on {subAdminUser.CreatedOn}");
				} else if (role.Equals(Role.SupervisorRole)) {
					var supervisorUser = new Supervisor { UserId = user.Id };
					coadaptService.Supervisor.CreateSupervisor(supervisorUser);
					await coadaptService.SaveAsync();
					logger.LogInfo($"Supervisor entry with ID {supervisorUser.Id} created on {supervisorUser.CreatedOn}");
				} else if (role.Equals(Role.TherapistRole)) {
					var therapistUser = new Therapist { UserId = user.Id };
					coadaptService.Therapist.CreateTherapist(therapistUser);
					await coadaptService.SaveAsync();
					logger.LogInfo($"Therapist entry with ID {therapistUser.Id} created on {therapistUser.CreatedOn}");
				} else if (role.Equals(Role.ParticipantRole)) {
					var participantUser = new Participant { UserId = user.Id, Code = code };
					coadaptService.Participant.CreateParticipant(participantUser);
					await coadaptService.SaveAsync();
					coadaptService.Participant.DetachParticipant(participantUser);
					logger.LogInfo($"Participant entry with ID {participantUser.Id} created on {participantUser.CreatedOn}");
				}
				var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
				if (roleManager == null) {
					throw new Exception("Role manager is null");
				}
				if (!await roleManager.RoleExistsAsync(role)) {
					await roleManager.CreateAsync(new IdentityRole(role));
					logger.LogInfo($"{role} role created");
				} else {
					logger.LogWarn($"{role} role already exists!");
				}
				await userManager.AddToRoleAsync(user, role);
			} else {
				logger.LogWarn($"User {userName} already exists!");
			}
			return user;
		}

		private static async Task CreateOrganization(IServiceProvider serviceProvider, Organization organization, string subAdministratorUserID) {
			var coadaptService = serviceProvider.GetRequiredService<IRepositoryWrapper>();
			var logger = serviceProvider.GetRequiredService<ILoggerManager>();
			var organizationOfSameShortname = await coadaptService.Organization.GetOrganizationByShortnameAsync(organization.Shortname);
			if (!organizationOfSameShortname.IsEmptyObject()) {
				logger.LogError($"Organization with short name {organization.Shortname} already exists!");
				return;
			}

			var subAdministrator = await coadaptService.SubAdministrator.GetSubAdministratorByUserIdAsync(subAdministratorUserID);
			if (subAdministrator.IsEmptyObject()) {
				logger.LogError($"Sub-administrator with ID {subAdministratorUserID} does not exist!");
				return;
			}

			var dbOrganization = await coadaptService.Organization.GetOrganizationBySubAdministratorIdAsync(subAdministrator.Id);
			if (!dbOrganization.IsEmptyObject()) {
				logger.LogWarn($"Organization {dbOrganization.Shortname} of sub-administrator with ID {subAdministrator.UserId} already exists!");
				organization.Id = dbOrganization.Id;
				organization.SubAdministratorId = dbOrganization.SubAdministratorId;
				return;
			}

			organization.SubAdministratorId = subAdministrator.Id;
			coadaptService.Organization.CreateOrganization(organization);
			await coadaptService.SaveAsync();
			logger.LogInfo($"Organization {organization.Shortname} with sub-administrator with ID {subAdministrator.UserId} created");
		}

		private static async Task CreateStudy(IServiceProvider serviceProvider, Study study, string supervisorUserID) {
			var coadaptService = serviceProvider.GetRequiredService<IRepositoryWrapper>();
			var logger = serviceProvider.GetRequiredService<ILoggerManager>();
			var supervisor = await coadaptService.Supervisor.GetSupervisorByUserIdAsync(supervisorUserID);
			if (supervisor.IsEmptyObject()) {
				logger.LogError($"Supervisor with ID {supervisorUserID} does not exist!");
				return;
			}

			var dbStudy = await coadaptService.Study.GetStudyOfOrganizationByShortnameAsync(study.Shortname, study.OrganizationId);
			if (!dbStudy.IsEmptyObject()) {
				logger.LogWarn($"Study {dbStudy.Shortname} already exists!");
				study.Id = dbStudy.Id;
				study.SupervisorId = dbStudy.SupervisorId;
				return;
			}

			study.SupervisorId = supervisor.Id;
			coadaptService.Study.CreateStudy(study);
			await coadaptService.SaveAsync();
			logger.LogInfo($"Study {study.Shortname} with supervisor with ID {supervisor.UserId} created");
		}

		private static async Task CreateSite(IServiceProvider serviceProvider, Site site) {
			var coadaptService = serviceProvider.GetRequiredService<IRepositoryWrapper>();
			var logger = serviceProvider.GetRequiredService<ILoggerManager>();
			var dbSite = await coadaptService.Site.GetSiteOfStudyByShortnameAsync(site.Shortname, site.StudyId);
			if (!dbSite.IsEmptyObject()) {
				logger.LogWarn($"Site {dbSite.Shortname} already exists!");
				site.Id = dbSite.Id;
				return;
			}

			coadaptService.Site.CreateSite(site);
			await coadaptService.SaveAsync();
			logger.LogInfo($"Site {site.Shortname} of study with ID {site.StudyId} created");
		}

		private static async Task CreateGroup(IServiceProvider serviceProvider, Group group) {
			var coadaptService = serviceProvider.GetRequiredService<IRepositoryWrapper>();
			var logger = serviceProvider.GetRequiredService<ILoggerManager>();
			var dbGroup = await coadaptService.Group.GetGroupOfStudyByShortnameAsync(group.Shortname, group.StudyId);
			if (!dbGroup.IsEmptyObject()) {
				logger.LogWarn($"Group {dbGroup.Shortname} already exists!");
				group.Id = dbGroup.Id;
				return;
			}

			coadaptService.Group.CreateGroup(group);
			await coadaptService.SaveAsync();
			logger.LogInfo($"Group {group.Shortname} of study with ID {group.StudyId} created");
		}

		private static async Task TherapistToParticipant(IServiceProvider serviceProvider, IdentityUser therapist, IdentityUser participant) {
			var coadaptService = serviceProvider.GetRequiredService<IRepositoryWrapper>();
			var logger = serviceProvider.GetRequiredService<ILoggerManager>();
			var dbParticipant = await coadaptService.Participant.GetParticipantByUserIdAsync(participant.Id);
			if (dbParticipant.IsEmptyObject()) {
				logger.LogError($"Participant with ID {participant.Id} does not exist!");
				return;
			}

			var dbTherapist = await coadaptService.Therapist.GetTherapistByUserIdAsync(therapist.Id);
			if (dbTherapist.IsEmptyObject()) {
				logger.LogError($"Therapist with ID {therapist.Id} does not exist!");
				return;
			}

			if (dbParticipant.TherapistId == dbTherapist.Id) {
				logger.LogWarn($"Participant with ID {dbParticipant.Id} already assigned to therapist with ID {dbTherapist.Id}!");
				return;
			}

			coadaptService.Participant.UpdateParticipant(dbParticipant, new Participant { UserId = dbParticipant.UserId, Code = dbParticipant.Code, TherapistId = dbTherapist.Id });
			await coadaptService.SaveAsync();
			logger.LogInfo($"Participant with ID {dbParticipant.Id} has been assigned therapist with ID {dbTherapist.Id}");
		}

		private static async Task ParticipantToStudy(IServiceProvider serviceProvider, IdentityUser participant, Study study, Site site, Group group) {
			var coadaptService = serviceProvider.GetRequiredService<IRepositoryWrapper>();
			var logger = serviceProvider.GetRequiredService<ILoggerManager>();
			var dbParticipant = await coadaptService.Participant.GetParticipantByUserIdAsync(participant.Id);
			if (dbParticipant.IsEmptyObject()) {
				logger.LogError($"Participant with ID {participant.Id} does not exist!");
				return;
			}
			var dbStudy = await coadaptService.Study.GetStudyByIdAsync(study.Id);
			if (dbStudy.IsEmptyObject()) {
				logger.LogError($"Study {study.Shortname} does not exist!");
				return;
			}
			var dbSite = await coadaptService.Site.GetSiteOfStudyByShortnameAsync(site.Shortname, site.StudyId);
			if (dbSite.IsEmptyObject()) {
				logger.LogError($"Site {site.Shortname} does not exist!");
				return;
			}
			var dbGroup = await coadaptService.Group.GetGroupOfStudyByShortnameAsync(group.Shortname, group.StudyId);
			if (dbGroup.IsEmptyObject()) {
				logger.LogError($"Group {group.Shortname} does not exist!");
				return;
			}
			if (dbGroup.StudyId != dbStudy.Id || dbSite.StudyId != dbStudy.Id) {
				logger.LogError($"Group and site must be both of study {study.Shortname}!");
				return;
			}
			if ((await coadaptService.StudyParticipant.StudyParticipantsByParticipant(dbParticipant.Id))
				.Any(sp => sp.StudyId == study.Id)) {
				logger.LogWarn($"Participant {dbParticipant.Id} already a member of the study!");
				return;
			}
			coadaptService.StudyParticipant.CreateStudyParticipant(new StudyParticipant {
				StudyId = dbStudy.Id,
				ParticipantId = dbParticipant.Id,
				SiteId = dbSite.Id,
				GroupId = dbGroup.Id,
				StartDate = DateTime.Now,
				EndDate = null
			});
			await coadaptService.SaveAsync();
			logger.LogInfo($"Participant with ID {dbParticipant.Id} has been assigned to site {dbSite.Shortname}");
		}

	}

}
