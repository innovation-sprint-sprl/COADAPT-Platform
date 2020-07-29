using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models {
    public class PersonalInformation : IEntity {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ParticipantId { get; set; }
        public virtual Participant Participant { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "DATETIME")]
        public DateTime DateOfReport { get; set; }

        public string MaritalStatus { get; set; }
        public int? StressLevelInMonth { get; set; }
        public int? IntrinsicFactorsOfWork { get; set; }
        public int? ManagerialRole { get; set; }
        public int? RelationshipWithOtherPeople { get; set; }
        public int? CareerAndSuccess { get; set; }
        public int? ClimateAndOrganizationalStructure { get; set; }
        public int? HomeWorkInterface { get; set; }
        public int? AttitudeTowardsLife { get; set; }
        public int? StyleOfBehavior { get; set; }
        public int? Ambition { get; set; }
        public int? TypeASynthetic { get; set; }
        public int? TypeATotal { get; set; }
        public int? OrganizationalForces { get; set; }
        public int? ManagementProcesses { get; set; }
        public int? IndividualInfluences { get; set; }
        public int? LocusOfControlSynthetic { get; set; }
        public int? LocusOfControlTotal { get; set; }
        public int? SocialSupport { get; set; }
        public int? TaskOrientation { get; set; }
        public int? Logical { get; set; }
        public int? HomeWorkRelationship { get; set; }
        public int? Time { get; set; }
        public int? Involvement { get; set; }
        public int? CareerSatisfaction { get; set; }
        public int? JobSatisfaction { get; set; }
        public int? SettingAndOrganizationalStructureSatisfaction { get; set; }
        public int? OrganizationalProcessesSatisfaction { get; set; }
        public int? InterpersonalRelationshipsSatisfaction { get; set; }
        public int? SyntheticJobSatisfaction { get; set; }
        public int? TotalJobSatisfaction { get; set; }
        public int? PsychologicalHealth { get; set; }
        public int? PhysicalHealth { get; set; }
        public int? GlobalSeverityIndex { get; set; }
        public int? PositiveSymptomTotal { get; set; }
        public int? PositiveSymptomDistressIndex { get; set; }
        public int? Somatization { get; set; }
        public int? ObsessivenessCompulsiveness { get; set; }
        public int? InterpersonalHypersensitivity { get; set; }
        public int? Depression { get; set; }
        public int? Anxiety { get; set; }
        public int? Hostility { get; set; }
        public int? PhobicAnxiety { get; set; }
        public int? ParanoidIdeation { get; set; }
        public int? Psychoticism { get; set; }
        public int? GeneralReportSCL { get; set; }
        public string ResearchGroup { get; set; }

    }
}
