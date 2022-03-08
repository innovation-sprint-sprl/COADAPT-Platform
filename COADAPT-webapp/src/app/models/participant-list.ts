export interface ParticipantList {
  id: number;
  code: string;
  createdOn: Date;
  therapist: string;
  psychologicalReports: number,
  phychologicalMetrics: number,
  organizations: string[];
  studies: string[];
  sites: string[];
  groups: string[];
}