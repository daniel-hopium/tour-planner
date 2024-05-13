using System;

namespace TourPlanner.Models;

public class TourLogModel
{
    public int Id { get; set; }
    public DateTime TourDate { get; set; }
    public string Comment { get; set; }
    public int TourId { get; set; }
    public int Difficulty { get; set; }
    public double? Distance { get; set; }
    public int TotalTime { get; set; }
    public int Rating { get; set; }
    public DateTime Created { get; set; }

    // Constructor to initialize the model directly from an entity
    public TourLogModel(TourPlanner.Persistence.Entities.TourLogEntity entity)
    {
        if (entity != null)
        {
            Id = entity.Id;
            TourDate = DateTime.Parse(entity.TourDate); // Assuming the date is stored in a correct format
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
    }
}