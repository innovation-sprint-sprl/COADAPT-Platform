import { AspNetUser } from './aspnet-user'
import { Study } from './study'

// Generic COADAPT User -> applies to all user types of the platform.
export interface COADAPTUser {
  id: number;
  userName: string;
  organization?: string;
  user?: AspNetUser;
  studies?: string[];
  participants?: number;
  createdOn: Date;
}
