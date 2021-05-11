import { AspNetUser } from './aspnet-user'
import { COADAPTUser } from './coadapt-user'

export interface Participant {
  id: number;
  userName: string;
  password: string;
  user?: AspNetUser;
  code: string;
  // Personal
  dateOfBirth: Date;
  region: string;
  gender: string;
  education: string;
  language: string;
  maritalStatus: string;
  // Job
  jobType: string;
  dateOfFirstJob: Date;
  dateOfCurrentJob: Date;
  // Medical
  startDate: Date;
  endDate: Date;
  // Relational
  siteId: number;
  groupId: number;
  therapistId?: string;
  therapist?: COADAPTUser;
  createdOn: Date;
}
