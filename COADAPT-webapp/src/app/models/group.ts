import { Study } from './study'

export interface Group {
  id: number;
  name: string;
  shortname: string;
  studyId: number;
  study?: Study;
  participants: number;
}
