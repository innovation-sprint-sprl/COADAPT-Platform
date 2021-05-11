export interface ParticipantList {
  id: number;
  code: string;
  createdOn: Date;
  therapist: string;
  organizations: string[];
  studies: string[];
  sites: string[];
  groups: string[];
}