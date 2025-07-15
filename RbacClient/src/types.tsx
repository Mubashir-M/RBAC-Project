export interface Permission {
  id: number;
  name: string;
}

export interface RawRole {
  id: number;
  name: string;
}

export interface Role {
  roleId: number;
  name: string;
}

export interface User {
  id: number;
  username: string;
  firstName: string;
  lastName: string;
  email: string;
  isActive: boolean;
  roles: Role[];
}
