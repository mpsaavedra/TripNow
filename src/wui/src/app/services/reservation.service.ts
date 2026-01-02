import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as signalR from '@microsoft/signalr';
import { Reservation, CreateReservationRequest, ReservationCreatedResponse, ReservationStatus } from '../models/reservation.model';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class ReservationService {
    private readonly http = inject(HttpClient);
    private connection: signalR.HubConnection | null = null;

    // Use signals for reactive state
    private _reservations = signal<Reservation[]>([]);
    public readonly reservations = this._reservations.asReadonly();

    constructor() {
        this.startSignalR();
    }

    private startSignalR() {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl('/reservationhub') // Proxy handles base URL
            .withAutomaticReconnect()
            .build();

        this.connection.on('ReservationUpdated', (id: string, status: ReservationStatus) => {
            this._reservations.update(reservations =>
                reservations.map(r => r.id === id ? { ...r, status } : r)
            );
        });

        this.connection.start().catch(err => console.error('Error while starting connection: ' + err));
    }

    getReservations(): Observable<Reservation[]> {
        return this.http.get<Reservation[]>('/api/reservations');
    }

    refreshReservations() {
        this.getReservations().subscribe(data => {
            this._reservations.set(data);
        });
    }

    createReservation(request: CreateReservationRequest): Observable<ReservationCreatedResponse> {
        return this.http.post<ReservationCreatedResponse>('/api/reservations', request);
    }
}
