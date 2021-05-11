import { COADAPTUser } from './coadapt-user';
import { Study } from './study';

export interface Organization {
  id: number;
  name: string;
  shortname: string;
  studies?: Study;
  subAdministratorId: number;
  subAdministrator?: COADAPTUser;
}
