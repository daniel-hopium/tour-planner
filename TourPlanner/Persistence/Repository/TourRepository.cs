using System.Collections.Generic;
using TourPlanner.Persistence.Entities;
using TourPlanner.Persistence.Utils;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TourPlanner.Models;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using System.IO;
using System;
using TourPlanner.Mapper;
using TourPlanner.Exceptions;
using log4net;
using System.Reflection;

namespace TourPlanner.Persistence.Repository;

public class TourRepository : ITourRepository
{
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


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
        try
        {
            return _dbContext.Tours.Include(t => t.FromAddress).Include(t => t.ToAddress).ToList();
        }
        catch (Exception ex)
        {
            log.Error($"TourRepository.GetTours: Tours could not be loaded:{ex}");
            throw new DALException("Error in TourRepository.GetTours", ex);
        }
    }

    public async Task<TourEntity?> GetTourByIdAsync(int tourId)
    {
        try
        {
            return await _dbContext.Tours.FindAsync(tourId);
        }
        catch (Exception ex)
        {
            log.Error($"TourRepository.GetTourByIdAsync: Tour could not be loaded:{ex}");
            throw new DALException("Error in TourRepository.GetTourByIdAsync", ex);
        }
    }

    public async Task CreateTourAsync(TourEntity newTour)
    {
        try
        {
            _dbContext.Tours.Add(newTour);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            log.Error($"TourRepository.CreateTourAsync: Tour could not be created:{ex}");
            throw new DALException("Error in TourRepository.CreateTourAsync", ex);
        }
    }

    public async Task<bool> UpdateTourAsync(TourEntity updatedTour)
    {
        try
        {
            var existingTourEntity = _dbContext.Tours.Find(updatedTour.Id);
            if (existingTourEntity != null)
            {
                _dbContext.Entry(existingTourEntity).CurrentValues.SetValues(updatedTour);
                await _dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            log.Error($"TourRepository.UpdateTourAsync: Tour ({updatedTour.Id}) could not be updated:{ex}");
            throw new DALException("Error in TourRepository.CreateTourAsync", ex);
        }
    }

    public async Task<bool> DeleteTourByIdAsync(int tourId)
    {
        try
        {
            var tourEntity = _dbContext.Tours.Find(tourId);
            if (tourEntity != null)
            {
                _dbContext.Tours.Remove(tourEntity);
                await _dbContext.SaveChangesAsync();
            }
            return false;
        }
        catch (Exception ex)
        {
            log.Error($"TourRepository.DeleteTourByIdAsync: Tour ({tourId}) could not be deleted:{ex}");
            throw new DALException("Error in TourRepository.DeleteTourByIdAsync", ex);
        }
    }

    public AddressEntity? GetAddressById(int tourId) 
    {
        try
        {
            return _dbContext.Addresses.Find(tourId);
        }
        catch (Exception ex)
        {
            log.Error($"TourRepository.GetAddressById: Address could not be loaded:{ex}");
            throw new DALException("Error in TourRepository.GetAddressById", ex);
        }
    }

    public AddressEntity? GetAddressByAttributes(string street, string housnumber, int zip, string city, string country)
    {
        try
        {
            return _dbContext.Addresses.FirstOrDefault(a => a.Housenumber == housnumber && a.City == city && (a.Zip == zip || a.Country == country) && (a.Street == street || a.Street == a.Country));
        }
        catch (Exception ex)
        {
            log.Error($"TourRepository.GetAddressByAttributes: Address could not be loaded:{ex}");
            throw new DALException("Error in TourRepository.GetAddressByAttributes", ex);
        }
    }

    public int AddAddress(AddressEntity address)
    {
        try
        {
            _dbContext.Addresses.Add(address);
            _dbContext.SaveChanges();
            return address.Id;
        }
        catch (Exception ex)
        {
            log.Error($"TourRepository.AddAddress: Address could not be added:{ex}");
            throw new DALException("Error in TourRepository.AddAddress", ex);
        }
    }

    public async Task<List<TourLogEntity>> GetLogsByTourIdAsync(int tourId)
    {
        try
        {
            // Use the Where method to filter logs based on the tourId and ToListAsync to execute the query asynchronously
            var logs =  await _dbContext.TourLogs
                .AsNoTracking()
                .Where(log => log.TourId == tourId)
                .ToListAsync();

            return logs;
        }
        catch (Exception ex)
        {
            log.Error($"TourRepository.GetLogsByTourIdAsync: Tour Logs could not be loaded:{ex}");
            throw new DALException("Error in TourRepository.GetLogsByTourIdAsync", ex);
        }
    }

    public async Task CreateTourLogAsync(TourLogEntity tourLog)
    {
        try
        {
            _dbContext.TourLogs.Add(tourLog);
            await _dbContext.SaveChangesAsync();
            await CalculatePopularity(tourLog.TourId);
            await CalculateChildfriendly(tourLog.TourId);
        }
        catch (Exception ex)
        {
            log.Error($"TourRepository.CreateTourLogAsync: Tour Log for tour {tourLog.TourId} could not be created:{ex}");
            throw new DALException("Error in TourRepository.CreateTourLogAsync", ex);
        }
    }
    public async Task<bool> DeleteTourLogByIdAsync(int tourLogId)
    {
        try
        {
            var tourLog = await _dbContext.TourLogs.FindAsync(tourLogId);
            if (tourLog != null)
            {
                _dbContext.TourLogs.Remove(tourLog);
                await _dbContext.SaveChangesAsync();
                await CalculatePopularity(tourLog.TourId);
                await CalculateChildfriendly(tourLog.TourId);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            log.Error($"TourRepository.DeleteTourLogByIdAsync: Tour Log could not be deleted:{ex}");
            throw new DALException("Error in TourRepository.DeleteTourLogByIdAsync", ex);
        }
    }
    public async Task<bool> UpdateTourLogAsync(TourLogEntity updatedTourLog)
    {
        try
        {
            var tourLog = await _dbContext.TourLogs.FindAsync(updatedTourLog.Id);
            if (tourLog != null)
            {
                // Assuming your context is tracking changes, you only need to copy the updated values
                _dbContext.Entry(tourLog).CurrentValues.SetValues(updatedTourLog);
                await _dbContext.SaveChangesAsync();
                await CalculateChildfriendly(tourLog.TourId);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            log.Error($"TourRepository.UpdateTourLogAsync: Tour Log could not be updated:{ex}");
            throw new DALException("Error in TourRepository.UpdateTourLogAsync", ex);
        }
    }

    private async Task CalculatePopularity(int tourId)
    {
        try
        {
            TourEntity? tour = await _dbContext.Tours.FindAsync(tourId);

            if(tour == null)
            {
                throw new DALException("Error in TourRepository.CalculatePopularity, Tour not found");
            }

            int count = _dbContext.TourLogs.Where(log => log.TourId == tourId).Count();
            int popularity = 0;

            if (count == 0) { popularity = 0; }
            else if (count == 1 || count == 2) { popularity = 1; }
            else if (count == 3 || count == 4) { popularity = 2; }
            else if (count > 4 && count <= 7) { popularity = 3; }
            else if (count > 7 && count <= 10) { popularity = 4; }
            else if (count > 10) { popularity = 5; }

            tour.Popularity = popularity;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            log.Warn(ex);
        }
    }

    private async Task CalculateChildfriendly(int tourId)  // bis 8 -> 5; ab 8 -> 4; ab 16 -> 3; ab 20 -> 2, ab 24 -> 1, ab 28 -> 0
    {
        try
        {
            TourEntity? tour = await _dbContext.Tours.FindAsync(tourId);  //.AsNoTracking().FirstOrDefaultAsync(tour => tour.Id == tourId); 

            if(tour == null)
            {
                throw new DALException("Error in TourRepository.CalculateCildfriendly, Tour not found");
            }

            var logsOfTour = _dbContext.TourLogs.Where((log => log.TourId == tourId)).ToList();

            if (logsOfTour.Count == 0)
            {
                tour.ChildFriendliness = null;
                await _dbContext.SaveChangesAsync();
                return; 
            }

            int sum = 0;  

            foreach (var log in logsOfTour)
            {
                sum += (int)(log.Difficulty * log.Distance + (log.TotalTime / 60));
            }

            int res = sum / logsOfTour.Count;
            int friendly = 0;

            if(res < 8) { friendly = 5; } 
            else if(res >= 8 &&  res < 16) { friendly = 4; }        
            else if(res >= 16 && res < 24) { friendly = 3; }
            else if(res >= 24 && res < 28) { friendly = 2; }
            else if(res >= 28 && res < 32) { friendly = 1; }
            else if(res >= 32) { friendly = 0; }     

            tour.ChildFriendliness = friendly;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            log.Warn(ex);
        }
    }

    public async Task<bool> GenerateTourReport(int tourId)
    {
        try 
        { 
            TourEntity? tour = _dbContext.Tours.Find(tourId);
            //var tourLogs = _dbContext.TourLogs.Where(log => log.TourId == tourId);
            //var tourLogs = tour.Logs.ToList();
            if(tour != null)
            {
                await ReportGenerator.GenerateTourReportAsync(tour);
                return true;
            }

            return false;
        }
        catch (UtilsException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            log.Error($"TourRepository.GenerateTourReport: Could not load Tour for report: {ex}");
            throw new DALException("Error in TourRepository.GenerateTourReport", ex);
        }
    }

    public async Task GenerateSummarizeReportAsync()
    {
        try
        {
            var tours = _dbContext.Tours.ToList();
            List<TourSummary> tourSummaries = new ();

            foreach (var tour in tours)
            {
                var logs = tour.Logs.ToList(); // dbContext.TourLogs.Where(log => log.TourId == tour.Id).ToList();

                int sumTime = 0;
                double sumDistance = 0;
                int sumRating = 0;

                foreach (var log in logs)
                {
                    sumTime += log.TotalTime;
                    sumDistance += log.Distance;
                    sumRating += log.Rating;
                }

                if(logs.Count > 0)
                {
                    tourSummaries.Add(new TourSummary(tour.Id, tour.Name, sumTime / logs.Count, sumDistance / logs.Count, sumRating / logs.Count));
                }
                else
                {
                    tourSummaries.Add(new TourSummary(tour.Id, tour.Name, 0, 0, 0));
                }                
            }

            await ReportGenerator.GenerateSummarizeReportAsync(tourSummaries);
        }
        catch (UtilsException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            log.Error($"TourRepository.GenerateSummarizeReportAsync: Could not load data for Summarize-report: {ex}");
            throw new DALException("Error in TourRepository.GenerateSummarizeReportAsync", ex);
        }
    }

    public async Task<bool> GenerateTourExportAsync(int tourId)
    {
        try
        {
            var tour = _dbContext.Tours.Find(tourId);
            //var tourLogs = _dbContext.TourLogs.Where(log => log.TourId == tourId);
            //var tourLogs = tour.Logs.ToList();
            if(tour != null)
            {
                await ExportGenerator.ExportTourAsync(tour);
                return true;
            }
            
            return false;
        }
        catch (UtilsException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            log.Error($"TourRepository.GenerateTourExportAsync: Could not load data for Tour-report: {ex}");
            throw new DALException("Error in TourRepository.GenerateTourExportAsync", ex);
        }
    }

    public async Task<(TransportType, double[], double[], int)> ImportTourAsync(string csvDatei)
    {
        try
        {
            using var reader = new StreamReader(csvDatei);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                Encoding = System.Text.Encoding.UTF8, 
                Quote = '"',
                Escape = '"',
                BadDataFound = null,
                MissingFieldFound = null
            });
            {
                // read tour-header
                await csv.ReadAsync();
                csv.ReadHeader();
                await csv.ReadAsync();

                // read tour data
                var tour = new TourModel
                {
                    Id = 0,
                    Name = csv.GetField<string>("Name"),
                    Description = csv.GetField<string>("Description"),
                    FromAddress = csv.GetField<string>("FromAddress"),
                    ToAddress = csv.GetField<string>("ToAddress"),
                    TransportType = (TransportType)Enum.Parse(typeof(TransportType), csv.GetField<string>("TransportType")),
                    Distance = csv.GetField<double>("Distance"),
                    EstimatedTime = csv.GetField<int>("EstimatedTime"),
                    Image = csv.GetField<string>("Image"),
                    Popularity = csv.GetField<int>("Popularity"),
                    ChildFriendliness = csv.GetField<string>("ChildFriendliness") == string.Empty ? null : csv.GetField<int>("ChildFriendliness")
                };

                var tourEntity = TourMapper.TourModelToEntity(tour, _instance);
                /*_dbContext.Tours.Add(tourEntity);
                await _dbContext.SaveChangesAsync();*/
                await CreateTourAsync(tourEntity);


                // read TourLogs header
                await csv.ReadAsync();
                csv.ReadHeader();

                // read TourLog data
                while (await csv.ReadAsync())
                {
                    var tourLog = new TourLogEntity
                    {
                        TourId = tourEntity.Id,
                        TourDate = csv.GetField<DateOnly>("TourDate"),
                        Comment = csv.GetField<string>("Comment"),
                        Difficulty = csv.GetField<int>("Difficulty"),
                        Distance = csv.GetField<double>("Distance"),
                        TotalTime = csv.GetField<int>("TotalTime"),
                        Rating = csv.GetField<int>("Rating")
                    };

                    _dbContext.TourLogs.Add(tourLog);
                }

                await _dbContext.SaveChangesAsync(); // {TransportType}_{_coordinatesStart[0]}_{_coordinatesStart[1]}_{_coordinatesEnd[0]}_{_coordinatesEnd[1]}.png


                string[] imagePathParts = tour.Image.Split("_");
                Double.TryParse(imagePathParts[1], out double start0);
                Double.TryParse(imagePathParts[2], out double start1);
                double[] start = new double[] {start0, start1};
                Double.TryParse(imagePathParts[3], out double end0);
                Double.TryParse(imagePathParts[4].Split(".")[0], out double end1);
                double[] end = new double[] { end0, end1 };

                return (tour.TransportType, start, end, tourEntity.Id);
            }
        }
        catch (Exception ex)
        {
            log.Error($"TourRepository.ImportTourAsync: Could not import data from Tour-Import: {ex}");
            throw new DALException("Error in TourRepository.ImportTourAsync", ex);
        }
    }
}