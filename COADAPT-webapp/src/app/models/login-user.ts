export interface LoginUser {
  id: number;
  userName: string;
  roles?: string[];
  token?: string;
  refreshToken?: string;
}
