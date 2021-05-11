import { Study } from './study'
import { Participant } from './participant'
import { Site } from './site'
import { Group } from './group'

export interface StudyParticipant {
  studyId: number;
  study?: Study;
  participantId: number;
  participant?: Participant;
  siteId: number;
  site?: Site;
  groupId: number;
  group?: Group;
  startDate: Date;
  endDate: Date;
  abandoned: boolean;
  dataCollectionTurn: number;
  placeOfConsent: string;
  dateOfCurrentJob: Date;
  jobType: string;
  regionOfResidence: string;
  placeOfResidence: string;
  maritalStatus: string;
  numberOfChildren: number;
  education: string;
}
