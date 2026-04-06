export const DEVICE_TYPE_VALUES = ['phone', 'tablet'] as const;
export type DeviceTypeValue = (typeof DEVICE_TYPE_VALUES)[number];
