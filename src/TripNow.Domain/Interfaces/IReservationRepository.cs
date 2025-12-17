using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TripNow.Domain.Entities;

namespace TripNow.Domain.Interfaces;

public interface IReservationRepository : IGenericRepository<Reservation>
{
}