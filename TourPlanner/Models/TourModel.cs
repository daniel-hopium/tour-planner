using System;
using System.Collections.Generic;
using System.Linq;
using TourPlanner.Persistence.Entities;

namespace TourPlanner.Models;

public enum TransportType
{
  bike,
  hike,
  running,
  car
}

public class TourModel
{
  public bool? IsNew;

  public TourModel(TourEntity tourEntity)
  {
    Id = tourEntity.Id;
    Name = tourEntity.Name;
    Description = tourEntity.Description;
    FromAddress = tourEntity.FromAddress.ToString();
    ToAddress = tourEntity.ToAddress.ToString();
    TransportType = (TransportType)Enum.Parse(typeof(TransportType), tourEntity.TransportType);
    Distance = tourEntity.Distance;
    EstimatedTime = tourEntity.EstimatedTime;
    Image = tourEntity.Image;
    Popularity = tourEntity.Popularity;
    ChildFriendliness = tourEntity.ChildFriendliness;

    Logs = tourEntity.Logs.Select(log => new TourLogModel(log)).ToList();
  }


  public TourModel()
  {
    Id = 0;
    Name = string.Empty;
    Description = string.Empty;
    FromAddress = string.Empty;
    ToAddress = string.Empty;
    Image = "/Persistence/Images/image1.png";
    Distance = 0.0;
    EstimatedTime = 0;
    Popularity = 0;
    IsNew = true;

    Logs = new List<TourLogModel>();
  }

  public int Id { get; set; }
  public string Name { get; set; }
  public string Description { get; set; }
  public string FromAddress { get; set; }
  public string ToAddress { get; set; }
  public TransportType TransportType { get; set; }
  public double Distance { get; set; }
  public int EstimatedTime { get; set; }
  public string Image { get; set; }
  public int Popularity { get; set; }
  public int? ChildFriendliness { get; set; }

  public List<TourLogModel> Logs { get; set; }
}