using System;
using ApiModels;
using Entities.Models;
using Microsoft.AspNetCore.Mvc.Internal;

namespace UserManagement.WebAPI.Extensions {
	public static class PsychologicalReportMessageExtensions {
		public static PsychologicalReport ToPsychologicalReport(this PsychologicalReportMessage message, int participantId) {
			var report = new PsychologicalReport {
				ParticipantId = participantId,
				DateOfReport = message.DateOfReport,
				TrialUserComments = message.TrialUserComments,
				TherapistComments = message.TherapistComments,
				ThoughtsOnChildren = message.ThoughtsOnChildren,
				ThoughtsOnParents = message.ThoughtsOnParents,
				StressfulEvents = message.StressfulEvents,
				MedicalConditionSelfAssessment = message.MedicalConditionSelfAssessment,
				PsychologicalConditionSelfAssessment = message.PsychologicalConditionSelfAssessment
			};
			if (int.TryParse(message.IntrinsicFactorsOfWork, out var temp))
				report.IntrinsicFactorsOfWork = temp;
			if (int.TryParse(message.ManagerialRole, out temp))
				report.ManagerialRole = temp;
			if (int.TryParse(message.RelationshipWithOtherPeople, out temp))
				report.RelationshipWithOtherPeople = temp;
			if (int.TryParse(message.CareerAndSuccess, out temp))
				report.CareerAndSuccess = temp;
			if (int.TryParse(message.ClimateAndOrganizationalStructure, out temp))
				report.ClimateAndOrganizationalStructure = temp;
			if (int.TryParse(message.HomeWorkInterface, out temp))
				report.HomeWorkInterface = temp;
			if (int.TryParse(message.AttitudeTowardsLife, out temp))
				report.AttitudeTowardsLife = temp;
			if (int.TryParse(message.StyleOfBehavior, out temp))
				report.StyleOfBehavior = temp;
			if (int.TryParse(message.Ambition, out temp))
				report.Ambition = temp;
			if (int.TryParse(message.TypeASynthetic, out temp))
				report.TypeASynthetic = temp;
			if (int.TryParse(message.TypeATotal, out temp))
				report.TypeATotal = temp;
			if (int.TryParse(message.OrganizationalForces, out temp))
				report.OrganizationalForces = temp;
			if (int.TryParse(message.ManagementProcesses, out temp))
				report.ManagementProcesses = temp;
			if (int.TryParse(message.IndividualInfluences, out temp))
				report.IndividualInfluences = temp;
			if (int.TryParse(message.LocusOfControlSynthetic, out temp))
				report.LocusOfControlSynthetic = temp;
			if (int.TryParse(message.LocusOfControlTotal, out temp))
				report.LocusOfControlTotal = temp;
			if (int.TryParse(message.SocialSupport, out temp))
				report.SocialSupport = temp;
			if (int.TryParse(message.TaskOrientation, out temp))
				report.TaskOrientation = temp;
			if (int.TryParse(message.Logical, out temp))
				report.Logical = temp;
			if (int.TryParse(message.HomeWorkRelationship, out temp))
				report.HomeWorkRelationship = temp;
			if (int.TryParse(message.Time, out temp))
				report.Time = temp;
			if (int.TryParse(message.Involvement, out temp))
				report.Involvement = temp;
			if (int.TryParse(message.CareerSatisfaction, out temp))
				report.CareerSatisfaction = temp;
			if (int.TryParse(message.JobSatisfaction, out temp))
				report.JobSatisfaction = temp;
			if (int.TryParse(message.SettingAndOrganizationalStructureSatisfaction, out temp))
				report.SettingAndOrganizationalStructureSatisfaction = temp;
			if (int.TryParse(message.OrganizationalProcessesSatisfaction, out temp))
				report.OrganizationalProcessesSatisfaction = temp;
			if (int.TryParse(message.InterpersonalRelationshipsSatisfaction, out temp))
				report.InterpersonalRelationshipsSatisfaction = temp;
			if (int.TryParse(message.SyntheticJobSatisfaction, out temp))
				report.SyntheticJobSatisfaction = temp;
			if (int.TryParse(message.TotalJobSatisfaction, out temp))
				report.TotalJobSatisfaction = temp;
			if (int.TryParse(message.PsychologicalHealth, out temp))
				report.PsychologicalHealth = temp;
			if (int.TryParse(message.PhysicalHealth, out temp))
				report.PhysicalHealth = temp;
			if (int.TryParse(message.GlobalSeverityIndex, out temp))
				report.GlobalSeverityIndex = temp;
			if (int.TryParse(message.PositiveSymptomTotal, out temp))
				report.PositiveSymptomTotal = temp;
			if (int.TryParse(message.PositiveSymptomDistressIndex, out temp))
				report.PositiveSymptomDistressIndex = temp;
			if (int.TryParse(message.Somatization, out temp))
				report.Somatization = temp;
			if (int.TryParse(message.ObsessivenessCompulsiveness, out temp))
				report.ObsessivenessCompulsiveness = temp;
			if (int.TryParse(message.InterpersonalHypersensitivity, out temp))
				report.InterpersonalHypersensitivity = temp;
			if (int.TryParse(message.Depression, out temp))
				report.Depression = temp;
			if (int.TryParse(message.Anxiety, out temp))
				report.Anxiety = temp;
			if (int.TryParse(message.Hostility, out temp))
				report.Hostility = temp;
			if (int.TryParse(message.PhobicAnxiety, out temp))
				report.PhobicAnxiety = temp;
			if (int.TryParse(message.ParanoidIdeation, out temp))
				report.ParanoidIdeation = temp;
			if (int.TryParse(message.Psychoticism, out temp))
				report.Psychoticism = temp;
			if (int.TryParse(message.PerceivedStressScale, out temp))
				report.PerceivedStressScale = temp;
			if (int.TryParse(message.SleepProblem, out temp))
				report.SleepProblem = temp;
			return report;
		}
	}
}
