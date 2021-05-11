import { Study } from './study'

export interface Site {
  id: number;
  name: string;
  shortname: string;
  studyId: number;
  study?: Study;
  participants: number;
}
