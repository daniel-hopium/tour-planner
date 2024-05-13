using System;
using System.Collections.Generic;
using System.Linq;
using TourPlanner.Models;

namespace TourPlanner.Mapper;

public static class TourLogMapper
{
    public static TourLogModel MapToModel(TourPlanner.Persistence.Entities.TourLogEntity entity)
    {
        var model = new TourLogModel
        {
            Id = entity.Id,
            TourDate = entity.TourDate,
            Comment = entity.Comment,
            TourId = entity.TourId,
            Difficulty = entity.Difficulty,
            Distance = entity.Distance,
            TotalTime = entity.TotalTime,
            Rating = entity.Rating,
            Created = entity.Created
        };
        return model;
    }
    
    /*public static List<TourLogModel> MapToModel(List<TourPlanner.Persistence.Entities.TourLogEntity> entities)
    {
        return entities.Select(entity => new TourLogModel
        {
            Id = entity.Id,
            TourDate = DateTime.Parse(entity.TourDate), // Ensure date format is correct
            Comment = entity.Comment,
            TourId = entity.TourId,
            Difficulty = entity.Difficulty,
            Distance = entity.Distance,
            TotalTime = entity.TotalTime,
            Rating = entity.Rating,
            Created = entity.Created
        }).ToList();
    }*/
}