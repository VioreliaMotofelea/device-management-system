export interface GenerateDeviceDescriptionRequest {
  name: string;
  manufacturer: string;
  type: string;
  operatingSystem: string;
  osVersion: string;
  processor: string;
  ramAmount: string;
}

export interface GenerateDeviceDescriptionResponse {
  description: string;
  source: 'openai' | 'template' | string;
}
