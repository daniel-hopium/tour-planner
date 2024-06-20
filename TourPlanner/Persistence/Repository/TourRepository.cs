using System.Collections.Generic;
using TourPlanner.Persistence.Entities;
using TourPlanner.Persistence.Utils;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TourPlanner.Models;
using static TourPlanner.ViewModels.Utils.MapCreator;

namespace TourPlanner.Persistence.Repository;

public class TourRepository : ITourRepository
{
    // Singleton Pattern without Dependency Injection -> all ViewModels able use same repository
    private static TourRepository _instance;
    public static TourRepository Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new TourRepository(new TourPlannerDbContext());
            }
            return _instance;
        }
    }

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
        CalculatePopularity(tourLog.TourId);
        CalculateCildfriendly(tourLog.TourId);
    }
    public async Task DeleteTourLogByIdAsync(int tourLogId)
    {
        var tourLog = await _dbContext.TourLogs.FindAsync(tourLogId);
        if (tourLog != null)
        {
            _dbContext.TourLogs.Remove(tourLog);
            await _dbContext.SaveChangesAsync();
            CalculatePopularity(tourLog.TourId);
            CalculateCildfriendly(tourLog.TourId);
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
            CalculateCildfriendly(tourLog.TourId);
        }
    }

    private void CalculatePopularity(int tourId)
    {
        int count = _dbContext.TourLogs.Where(log => log.TourId == tourId).Count();
        int popularity = 0;

        if(count == 0) {  popularity = 0; }
        else if (count == 1 || count == 2) { popularity = 1; }
        else if (count == 3 || count == 4) { popularity = 2; }
        else if (count > 4 && count <= 7) { popularity = 3; }
        else if (count > 7 && count <= 10) { popularity = 4; }
        else if (count > 10) { popularity = 5; }

        TourEntity tour = _dbContext.Tours.Find(tourId);
        if (tour != null)
        {
            tour.Popularity = popularity;
            _dbContext.SaveChanges();
        }
    }

    private void CalculateCildfriendly(int tourId)  // bis 8 -> 5; ab 8 -> 4; ab 16 -> 3; ab 20 -> 2, ab 24 -> 1, ab 28 -> 0
    {
        var logsOfTour = _dbContext.TourLogs.Where((log => log.TourId == tourId)).ToList();

        int sum = 0;  

        foreach (var log in logsOfTour)
        {
            sum += (int)(log.Difficulty * log.Distance + (log.TotalTime / 60));
        }

        int res = logsOfTour.Count > 0 ? sum / logsOfTour.Count : 32;
        int friendly = 0;

        if(res < 8)
        {
            friendly = 5;
        } 
        else if(res >= 8 &&  res < 16) 
        { 
            friendly = 4;
        }
        else if(res >= 16 && res < 24)
        {
            friendly = 3;
        }
        else if(res >= 24 && res < 28) 
        { 
            friendly = 2;
        }
        else if(res >= 28 && res < 32)
        {
            friendly = 1;
        }
        else if(res >= 32)
        {
            friendly = 0;
        }

        TourEntity tour = _dbContext.Tours.Find(tourId);
        if (tour != null)
        {
            tour.ChildFriendliness = friendly;
            _dbContext.SaveChanges();
        }
    }

}