using System.Collections.Generic;
using TourPlanner.Persistence.Entities;
using TourPlanner.Persistence.Utils;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TourPlanner.Models;

namespace TourPlanner.Persistence.Repository;

public class TourRepository : ITourRepository
{
    private readonly TourPlannerDbContext _dbContext;

    public TourRepository(TourPlannerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<TourEntity> GetTours()
    {
        return _dbContext.Tours.Include(t => t.FromAddress).Include(t => t.ToAddress).ToList();
    }

    public async Task<TourEntity> GetTourByIdAsync(int tourId)
    {
        return await _dbContext.Tours.FindAsync(tourId);
    }

    public async Task CreateTourAsync(TourEntity newTour)
    {
        _dbContext.Tours.Add(newTour);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateTourAsync(TourEntity updatedTour)
    {
        var existingTourEntity = _dbContext.Tours.Find(updatedTour.Id);
        if (existingTourEntity != null)
        {
            _dbContext.Entry(existingTourEntity).CurrentValues.SetValues(updatedTour);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteTourByIdAsync(int tourId)
    {
        var tourEntity = _dbContext.Tours.Find(tourId);
        _dbContext.Tours.Remove(tourEntity);
        await _dbContext.SaveChangesAsync();
    }

    public AddressEntity GetAddressById(int tourId) 
    {  
        return _dbContext.Addresses.Find(tourId); 
    }

    public AddressEntity GetAddressByAttributes(string street, string housnumber, int zip, string city)
    {
        // Führe eine Abfrage aus, um die Adresse anhand ihrer Attribute zu suchen
        return _dbContext.Addresses.FirstOrDefault(a => a.Street == street && a.Housenumber == housnumber && a.Zip == zip && a.City == city);
    }

    public int AddAddress(AddressEntity address)
    {
        _dbContext.Addresses.Add(address);
        _dbContext.SaveChanges();
        return address.Id;
    }

    public async Task<List<TourLogEntity>> GetLogsByTourIdAsync(int tourId)
    {
        // Use the Where method to filter logs based on the tourId and ToListAsync to execute the query asynchronously
        var logs = await _dbContext.TourLogs
            .AsNoTracking()
            .Where(log => log.TourId == tourId)
            .ToListAsync();

        return logs;
    }
    public async Task CreateTourLogAsync(TourLogEntity tourLog)
    {
        _dbContext.TourLogs.Add(tourLog);
        await _dbContext.SaveChangesAsync();
    }
    public async Task DeleteTourLogByIdAsync(int tourLogId)
    {
        var tourLog = await _dbContext.TourLogs.FindAsync(tourLogId);
        if (tourLog != null)
        {
            _dbContext.TourLogs.Remove(tourLog);
            await _dbContext.SaveChangesAsync();
        }
    }
    public async Task UpdateTourLogAsync(TourLogEntity updatedTourLog)
    {
        var tourLog = await _dbContext.TourLogs.FindAsync(updatedTourLog.Id);
        if (tourLog != null)
        {
            // Assuming your context is tracking changes, you only need to copy the updated values
            _dbContext.Entry(tourLog).CurrentValues.SetValues(updatedTourLog);
            await _dbContext.SaveChangesAsync();
        }
    }


}