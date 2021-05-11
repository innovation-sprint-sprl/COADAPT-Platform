import { LoginUser, Role } from '../models';

export class RoleValidator {
  constructor() { }

  static isAdmin(user: LoginUser) {
    return user.roles[0] === Role.Administrator;
  }

  static isSubAdmin(user: LoginUser) {
    return user.roles[0] === Role.SubAdministrator;
  }

  static isSupervisor(user: LoginUser) {
    return user.roles[0] === Role.Supervisor;
  }

  static isTherapist(user: LoginUser) {
    return user.roles[0] === Role.Therapist;
  }

  static isParticipant(user: LoginUser) {
    return user.roles[0] === Role.Participant;
  }

}
