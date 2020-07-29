using Entities.Models;
using ApiModels;

namespace Entities.Extensions {
    public static class PersonalInformationExtensions {
        public static void FromRequest(this PersonalInformation personalInformation, PersonalInformationRequest request) {
            personalInformation.ParticipantId = request.ParticipantId;
            personalInformation.MaritalStatus = request.MaritalStatus;
            personalInformation.StressLevelInMonth = request.StressLevelInMonth;
            personalInformation.IntrinsicFactorsOfWork = request.IntrinsicFactorsOfWork;
            personalInformation.ManagerialRole = request.ManagerialRole;
            personalInformation.RelationshipWithOtherPeople = request.RelationshipWithOtherPeople;
            personalInformation.CareerAndSuccess = request.CareerAndSuccess;
            personalInformation.ClimateAndOrganizationalStructure = request.ClimateAndOrganizationalStructure;
            personalInformation.HomeWorkInterface = request.HomeWorkInterface;
            personalInformation.AttitudeTowardsLife = request.AttitudeTowardsLife;
            personalInformation.StyleOfBehavior = request.StyleOfBehavior;
            personalInformation.Ambition = request.Ambition;
            personalInformation.TypeASynthetic = request.TypeASynthetic;
            personalInformation.TypeATotal = request.TypeATotal;
            personalInformation.OrganizationalForces = request.OrganizationalForces;
            personalInformation.ManagementProcesses = request.ManagementProcesses;
            personalInformation.IndividualInfluences = request.IndividualInfluences;
            personalInformation.LocusOfControlSynthetic = request.LocusOfControlSynthetic;
            personalInformation.LocusOfControlTotal = request.LocusOfControlTotal;
            personalInformation.SocialSupport = request.SocialSupport;
            personalInformation.TaskOrientation = request.TaskOrientation;
            personalInformation.Logical = request.Logical;
            personalInformation.HomeWorkRelationship = request.HomeWorkRelationship;
            personalInformation.Time = request.Time;
            personalInformation.Involvement = request.Involvement;
            personalInformation.CareerSatisfaction = request.CareerSatisfaction;
            personalInformation.JobSatisfaction = request.JobSatisfaction;
            personalInformation.SettingAndOrganizationalStructureSatisfaction = request.SettingAndOrganizationalStructureSatisfaction;
            personalInformation.OrganizationalProcessesSatisfaction = request.OrganizationalProcessesSatisfaction;
            personalInformation.InterpersonalRelationshipsSatisfaction = request.InterpersonalRelationshipsSatisfaction;
            personalInformation.SyntheticJobSatisfaction = request.SyntheticJobSatisfaction;
            personalInformation.TotalJobSatisfaction = request.TotalJobSatisfaction;
            personalInformation.PsychologicalHealth = request.PsychologicalHealth;
            personalInformation.PhysicalHealth = request.PhysicalHealth;
            personalInformation.GlobalSeverityIndex = request.GlobalSeverityIndex;
            personalInformation.PositiveSymptomTotal = request.PositiveSymptomTotal;
            personalInformation.PositiveSymptomDistressIndex = request.PositiveSymptomDistressIndex;
            personalInformation.Somatization = request.Somatization;
            personalInformation.ObsessivenessCompulsiveness = request.ObsessivenessCompulsiveness;
            personalInformation.InterpersonalHypersensitivity = request.InterpersonalHypersensitivity;
            personalInformation.Depression = request.Depression;
            personalInformation.Anxiety = request.Anxiety;
            personalInformation.Hostility = request.Hostility;
            personalInformation.PhobicAnxiety = request.PhobicAnxiety;
            personalInformation.ParanoidIdeation = request.ParanoidIdeation;
            personalInformation.Psychoticism = request.Psychoticism;
            personalInformation.GeneralReportSCL = request.GeneralReportSCL;
            personalInformation.ResearchGroup = request.ResearchGroup;
    }
}
}
