export interface Device {
  id: number;
  name: string;
  manufacturer: string;
  type: string;
  operatingSystem: string;
  osVersion: string;
  processor: string;
  ramAmount: string;
  description: string | null;
  location: string;
  assignedUserId: number | null;
  assignedUserName: string | null;
}
