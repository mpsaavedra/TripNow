export interface Reservation {
    id: string;
    customerEmail: string;
    tripCountry: string;
    amount: number;
    status: ReservationStatus;
    createdAt: string;
}

export enum ReservationStatus {
    PendingRiskCheck = 'PendingRiskCheck',
    Confirmed = 'Confirmed',
    Rejected = 'Rejected',
    Cancelled = 'Cancelled'
}

export interface CreateReservationRequest {
    customerEmail: string;
    tripCountry: string;
    amount: number;
}

export interface ReservationCreatedResponse {
    id: string;
    status: string;
}
