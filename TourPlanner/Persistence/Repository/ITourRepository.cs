using System.Collections.Generic;
using System.Threading.Tasks;
using TourPlanner.Models;
using TourPlanner.Persistence.Entities;

namespace TourPlanner.Persistence.Repository;

public interface ITourRepository
{
  public List<TourEntity> GetTours();
  public Task<TourEntity?> GetTourByIdAsync(int tourId);
  public Task CreateTourAsync(TourEntity newTour);
  public Task<bool> UpdateTourAsync(TourEntity updatedTour);
  public Task<bool> DeleteTourByIdAsync(int tourId);
  public AddressEntity? GetAddressById(int tourId);

  public AddressEntity?
    GetAddressByAttributes(string street, string housnumber, int zip, string city, string country);

  public int AddAddress(AddressEntity address);
  public Task<List<TourLogEntity>> GetLogsByTourIdAsync(int tourId);
  public Task CreateTourLogAsync(TourLogEntity tourLog);
  public Task<bool> DeleteTourLogByIdAsync(int tourLogId);
  public Task<bool> UpdateTourLogAsync(TourLogEntity updatedTourLog);
  public Task<bool> GenerateTourReport(int tourId);
  public Task GenerateSummarizeReportAsync();
  public Task<bool> GenerateTourExportAsync(int tourId);
  public Task<(TransportType, double[], double[], int)> ImportTourAsync(string csvDatei);
}