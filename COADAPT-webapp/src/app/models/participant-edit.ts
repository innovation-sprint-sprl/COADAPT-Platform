import { AspNetUser } from './aspnet-user'
import { COADAPTUser } from './coadapt-user'
import { StudyParticipant } from './studyparticipant'

export interface ParticipantEdit {
  id: number;
  createdOn: Date;
  dateOfBirth: Date;
  birthPlace: string;
  language: string;
  gender: string;
  files: string;
  dateOfFirstJob: Date;
  code: string;
  userId: number;
  user?: AspNetUser;
  therapistId?: number;
  therapist?: COADAPTUser;
  studyParticipants: StudyParticipant[];
}
