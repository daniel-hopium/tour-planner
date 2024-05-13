using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Persistence.Entities;

namespace TourPlanner.Persistence.Repository
{
    public interface ITourRepository
    {
        public List<TourEntity> GetTours();
        public Task<TourEntity> GetTourByIdAsync(int tourId);
        public Task CreateTourAsync(TourEntity newTour);
        public Task UpdateTourAsync(TourEntity updatedTour);
        public Task DeleteTourByIdAsync(int tourId);
        public AddressEntity GetAddressById(int tourId);
        public AddressEntity GetAddressByAttributes(string street, string housnumber, int zip, string city);
        public int AddAddress(AddressEntity address);
    }
}
