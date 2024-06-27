using TourPlanner.Models;
using TourPlanner.Persistence.Entities;

namespace TourPlanner.Mapper;

public static class TourLogMapper
{
  public static TourLogModel MapToModel(TourLogEntity entity)
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
}