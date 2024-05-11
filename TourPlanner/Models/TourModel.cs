using System.Net;
using System.Xml.Linq;
using TourPlanner.Persistence.Entities;

namespace TourPlanner.Models;

public enum TransportType
{
    bike, hike, running, vacation
}

public class TourModel
{
    public int Id { get; set; }
    public string Name { get; set; } 
    public string Description {  get; set; }
    public string FromAddress {  get; set; }
    public string ToAddress {  get; set; }
    public string TransportType { get; set; } //TransportType
    public double Distance { get; set; }
    public int EstimatedTime {  get; set; }
    public string Image {  get; set; }
    public int Popularity {  get; set; }
    public int ChildFriendliness { get; set; }

    //public TourModel(int id, string name, string description, string fromAddress, string toAddress, string transportType, double distance, int estimatedTime, string image, int popularity, int childFriendliness)
    public TourModel(TourEntity tourEntity)
    {
        Id = tourEntity.Id;
        Name = tourEntity.Name;
        Description = tourEntity.Description;
        FromAddress = $"{tourEntity.FromAddress.Street} {tourEntity.FromAddress.Housenumber}, {tourEntity.FromAddress.Zip} {tourEntity.FromAddress.City}";
        ToAddress = $"{tourEntity.ToAddress.Street} {tourEntity.ToAddress.Housenumber}, {tourEntity.ToAddress.Zip} {tourEntity.ToAddress.City}";
        TransportType = tourEntity.TransportType;
        Distance = tourEntity.Distance;
        EstimatedTime = tourEntity.EstimatedTime;
        Image = tourEntity.Image;
        Popularity = tourEntity.Popularity;
        ChildFriendliness = tourEntity.ChildFriendliness;
    }
}