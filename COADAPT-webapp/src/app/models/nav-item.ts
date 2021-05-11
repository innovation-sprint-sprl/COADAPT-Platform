export interface NavItem {
  displayName: string;
  iconName: string;
  roles?: string[];
  route?: string;
  children?: NavItem[];
}
