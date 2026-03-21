export type UserRole = 'user' | 'admin' | 'driver';

/** Профиль пассажира (PassengerProfileDto из бэкенда) */
export type PassengerProfile = {
  id: number;
  name?: string | null;
  avatarUrl?: string | null;
  isBanned: boolean;
  totalRides: number;
  totalReviews: number;
  rating: number;
};

/** Профиль водителя (DriverProfileDto из бэкенда) */
export type DriverProfile = {
  id: number;
  name?: string | null;
  avatarUrl?: string | null;
  isBanned: boolean;
  totalRides: number;
  totalReviews: number;
  rating: number;
  licenseNumber?: string | null;
  licenseExpiryDate?: string;
  currentCarId?: number;
};

/** Данные пользователя с email и ролью (из токена или auth) */
export type UserWithAuth = {
  email: string;
  role: UserRole;
  phone?: string | null;
} & (PassengerProfile | DriverProfile);

/** Объединенный тип пользователя */
export type User = PassengerProfile | DriverProfile;

export type LoginCredentials = {
  email: string;
  password: string;
};

export type RegisterCredentials = {
  email: string;
  password: string;
  name?: string;
  role?: string;
};

export type UpdatePassengerProfileRequest = {
  name?: string | null;
  avatarUrl?: string | null;
};

export type UpdateDriverProfileRequest = {
  name?: string | null;
  avatarUrl?: string | null;
  licenseNumber?: string | null;
  licenseExpiryDate?: string;
  currentCarId?: number;
};
