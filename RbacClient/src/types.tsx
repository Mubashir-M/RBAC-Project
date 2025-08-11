export interface Permission {
  id: number;
  name: string;
}

export interface RawRole {
  id: number;
  name: string;
  description: string;
}
export interface Role {
  roleId: number;
  name: string;
  description?: string;
}

export interface User {
  userId: number;
  username: string;
  firstName: string;
  lastName: string;
  email: string;
  isActive: boolean;
  roles: Role[];
}

export type EventType =
  | "UserCreated"
  | "LoginSuccess"
  | "UserUpdated"
  | "UserDeleted"
  | "RoleAssigned"
  | "RoleUpdated"
  | "SystemError";

export interface Event {
  eventId: string;
  type: EventType;
  timestamp: string;
  description: string;
  userId: number | null;
  username: string | null;
  sourceIPAddress: string | null;
  affectedEntityId: number | null;
  affectedEntityName: string;
}
