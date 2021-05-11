import { COADAPTUser } from './coadapt-user';
import { Organization } from './organization';

export interface Study {
  id: number;
  name: string;
  shortname: string;
  organizationId: number;
  organization?: Organization;
  sites?: null;
  groups?: null;
  supervisorId: number;
  supervisor?: COADAPTUser;
}
