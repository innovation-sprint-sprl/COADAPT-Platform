using Constants;
using Contracts.Logger;
using Contracts.Repository;
using Entities.Extensions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace UserManagement.WebAPI {
	public static class SeedData {

		public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw) {
			await CreateUser(serviceProvider, testUserPw, "admin@coadapt.eu", Role.AdministratorRole);
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
				.Any(sp => sp.StudyId == study.Id && sp.EndDate == null)) {
				logger.LogWarn($"Participant {dbParticipant.Id} already an active member of the study!");
				return;
			}
			coadaptService.StudyParticipant.CreateStudyParticipant(new StudyParticipant { 
				StudyId = dbStudy.Id,
				ParticipantId = dbParticipant.Id,
				SiteId = dbSite.Id,
				GroupId = dbGroup.Id,
				StartDate = DateTime.Now,
				EndDate = null });
			await coadaptService.SaveAsync();
			logger.LogInfo($"Participant with ID {dbParticipant.Id} has been assigned to site {dbSite.Shortname}");
		}

		public static void Decrypt() {
			var message =
				"r7TNI6fDDA9wqdFYD8H7nKMDxTxUf3+MN3TcYTLYOhlIYb5EkdRCyiUMLTHEVBtQW3P0rrIelBlxvHVSTHGvlGOtBOruj8xYGsgJJRVgcrLhsJ5J5sT9v7eQ+Zb3AWbD9jI03NSrL2IsUUQtlS+/e5dYUsjlVDYkQJ1AqLTfqIn09+sRJNQiTtNlf5KGYmhx+c6AlHbkUrqBEwDVXPqi2C4gAbUUSpmxhev+0B7oIonHW1ooqnOkMd+2BF+5gi1Na40OFKERUU7v7uah9dAy6JD2oqo2H82Af9Em6vF5rEUUy0OfzrOFca8jVqIAT4CFEybK5LMzQNHLxpxk5rfFuA==|ffMIDxRdTqVXry5zXyVfdwZDd+2M6Z8BuuJD1j0VN0aeuSKz5303MAUiBKholfCV54xZ4u2guCm5KNGGcPEUKr2P46AaHy15pZsCfYPmb6oPQ+GOt/ngtyKKEJFvHrWj4Nravl5WIlpiYLkvu9gzy5wWufOUD4O1UoitNAjSDJksMsyt4Mg0mWmW4ikoPHQmrPUUaSXAl0aNVhIp3Vusix0QiU4Tt4Q+87WPLD/eT/Bor/hXqZH5zxhO/C5mAA/4x+sh2NiGCghRHsCLa+7NtEJKe/NSrSOQ3cfyfLCRDR/ukfWjyAVuOZaxl9h/Hc+b4QKSomRTXi6OCtltamNPIg==|T3hPM0NQL3dvbDhHbGg3bno1bm1MUlNaSzhnU2JFc2lNZ1JIM3VWaHpWem9qUmduRzFKVnlXdk9rVW13MUUwTkUxWWNaRml4SGJIczc4MWpuVEVKQUVDSXlrUmlkWmxHZXhXVVh1WTVLT3UvSVRoRDljMTdHSzB6TU5UQWZuaEFMLzBPa0dMcDFmd0NsSjcrR3lNMXlpbTBVQjdqWURRcnVFdFhvTXFhbnF2dEY1NFJqOThVREx4eWN4YStnU3BWSDIvWGMyWXBhSWIvRGRmYXE2a3BpTWN4VmhRMGJMV0xESFFHR01ZNVJOc25oRzR0dkJmTTRkRkgyRWFCRVlJZUFBdHFVSThocGh2YWRkOHg3eFI2cmQ1d1p4WHU0dm1ucUg5VUVib1lkU0c1SUJuZDNjTnFyaG5DNUQvay9CRUxERjh3cGI4THhDbTZMYTh0RkZZVGxvM0E1VnBLemQ1eElFK1MyaFowcE1KUlVwanBPWUU1RjBSWnU3Um8wVlhZb0RjSEIyZERES2J5ZTZCRVdHdDljSWpmN3NXOHE0M2dLT2JRSXE3R0ZQL05jc3ViQjlqUEVTcXZBWGJNeVR6RExGQjZsekZqUU9wZXNETmE2cHI2OUE9PQ==";
			System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo();
			//python interprater location
			start.FileName = @"D:/venv/datacrypt/Scripts/python.exe";
			//argument with file name and input parameters
			//start.Arguments = string.Format("{0} {1}", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "decrypt.py"), message);
			start.Arguments = string.Format("{0} {1}", "D:/Code/datacrypt/decrypt.py", message);
			start.UseShellExecute = false;// Do not use OS shell
			start.CreateNoWindow = true; // We don't need new window
			start.RedirectStandardOutput = true;// Any output, generated by application will be redirected back
			start.RedirectStandardError = true; // Any error in standard output will be redirected back (for example exceptions)
			start.LoadUserProfile = true;
			using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(start))
			{
				using (StreamReader reader = process.StandardOutput)
				{
					string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script
					string result = reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")
					Console.WriteLine("Decrypted message:\n{0}", result);
				}
			}
		}

	}
}
