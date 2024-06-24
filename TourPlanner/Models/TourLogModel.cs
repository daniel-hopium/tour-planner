using System;

namespace TourPlanner.Models;

public class TourLogModel
{
    public int Id { get; set; }
    public DateOnly TourDate { get; set; }
    public string Comment { get; set; }
    public int TourId { get; set; }
    public int Difficulty { get; set; }
    public double Distance { get; set; }
    public int TotalTime { get; set; }
    public int Rating { get; set; }
    public DateTime Created { get; set; }

    // Constructor to initialize the model directly from an entity
    public TourLogModel(TourPlanner.Persistence.Entities.TourLogEntity entity)
    {
        if (entity != null)
        {
            Id = entity.Id;
            TourDate = entity.TourDate; // Assuming the date is stored in a correct format
            Comment = entity.Comment;
            TourId = entity.TourId;
            Difficulty = entity.Difficulty;
            Distance = entity.Distance;
            TotalTime = entity.TotalTime;
            Rating = entity.Rating;
            Created = entity.Created;
        }
    }

    // Parameterless constructor for initializations without data
    public TourLogModel()
    {
        Id = 0;
        TourDate = DateOnly.FromDateTime(DateTime.Now); // Initialize with today's date
        Comment = string.Empty;
        TourId = 0;
        Difficulty = 0;
        Distance = 0;
        TotalTime = 0;
        Rating = 0;
        Created = DateTime.Now;
    }
}