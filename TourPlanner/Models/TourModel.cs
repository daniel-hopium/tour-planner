using System.Net;
using System.Xml.Linq;
using TourPlanner.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TourPlanner.Mapper;

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
    public TransportType TransportType { get; set; } //string
    public double Distance { get; set; }
    public int EstimatedTime {  get; set; }
    public string Image {  get; set; }
    public int Popularity {  get; set; }
    public int ChildFriendliness { get; set; }
    public bool? IsNew;

    //public TourModel(int id, string name, string description, string fromAddress, string toAddress, string transportType, double distance, int estimatedTime, string image, int popularity, int childFriendliness)
    public TourModel(TourEntity tourEntity)
    {
        Id = tourEntity.Id;
        Name = tourEntity.Name;
        Description = tourEntity.Description;
        FromAddress = $"{tourEntity.FromAddress.Street} {tourEntity.FromAddress.Housenumber}, {tourEntity.FromAddress.Zip} {tourEntity.FromAddress.City}";
        ToAddress = $"{tourEntity.ToAddress.Street} {tourEntity.ToAddress.Housenumber}, {tourEntity.ToAddress.Zip} {tourEntity.ToAddress.City}";
        TransportType = (TransportType)Enum.Parse(typeof(TransportType),tourEntity.TransportType); //tourEntity.TransportType;
        Distance = tourEntity.Distance;
        EstimatedTime = tourEntity.EstimatedTime;
        Image = tourEntity.Image;
        Popularity = tourEntity.Popularity;
        ChildFriendliness = tourEntity.ChildFriendliness; 
    }

    

    public TourModel() {
        Id = 0;
        Name = string.Empty;
        Description = string.Empty;
        FromAddress = string.Empty;
        ToAddress = string.Empty;
        Image = "/Persistence/Images/image1.png";
        Distance = 3.2;
        EstimatedTime = 60;
        Popularity = 0;
        ChildFriendliness = 5;
        IsNew = true;
    }
}