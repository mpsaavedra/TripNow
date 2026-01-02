import { Component, signal, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, NonNullableFormBuilder, Validators } from '@angular/forms';
import { ReservationService } from './services/reservation.service';
import { CreateReservationRequest } from './models/reservation.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  private readonly reservationService = inject(ReservationService);
  private readonly fb = inject(NonNullableFormBuilder);

  public readonly reservations = this.reservationService.reservations;
  public readonly title = signal('Dashboard');

  public reservationForm = this.fb.group({
    customerEmail: ['', [Validators.required, Validators.email]],
    tripCountry: ['', [Validators.required]],
    amount: [0, [Validators.required, Validators.min(1)]]
  });

  public isCreating = signal(false);

  ngOnInit() {
    this.reservationService.refreshReservations();
  }

  onSubmit() {
    if (this.reservationForm.valid) {
      this.isCreating.set(true);
      const request: CreateReservationRequest = this.reservationForm.getRawValue();
      this.reservationService.createReservation(request).subscribe({
        next: () => {
          this.reservationForm.reset();
          this.reservationService.refreshReservations();
          this.isCreating.set(false);
          // Flowbite modal close logic could be here if using flowbite JS
        },
        error: (err) => {
          console.error('Failed to create reservation', err);
          this.isCreating.set(false);
        }
      });
    }
  }
}
