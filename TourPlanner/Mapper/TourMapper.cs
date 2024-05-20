using System.Linq;
using TourPlanner.Models;
using TourPlanner.Persistence.Entities;
using TourPlanner.Persistence.Repository;
using System;

namespace TourPlanner.Mapper;

public class TourMapper
{
    private readonly ITourRepository _tourRepository;

    public TourMapper(ITourRepository tourRepository)
    {
        _tourRepository = tourRepository;
    }

    public TourEntity TourModelToEntity(TourModel tour)
    {
        var fromAddress = tour.FromAddress;
        string[] teile = fromAddress.Split(',');
        teile[0] = teile[0].Trim();
        teile[1] = teile[1].Trim();
        string[] strasseteile = teile[0].Split(" ");
        string fromHousenumber = strasseteile.Last();
        Array.Resize(ref strasseteile, strasseteile.Length - 1);
        string fromStreet = string.Join(" ", strasseteile);
        string[] cityteile = teile[1].Split(" ");
        int fromZip; int.TryParse(cityteile[0], out fromZip);
        string fromCity = cityteile[1];

        var fromAddressEntity = _tourRepository.GetAddressByAttributes(fromStreet, fromHousenumber, fromZip, fromCity);
        if (fromAddressEntity == null)
        {
            int fromAddressId = _tourRepository.AddAddress(new AddressEntity { Id = 0, Street = fromStreet, Housenumber = fromHousenumber, Zip = fromZip, City = fromCity });
            fromAddressEntity = _tourRepository.GetAddressById(fromAddressId);
        }

        var toAddress = tour.ToAddress;
        teile = toAddress.Split(',');
        teile[0] = teile[0].Trim();
        teile[1] = teile[1].Trim();
        strasseteile = teile[0].Split(" ");
        string toHousenumber = strasseteile.Last();
        Array.Resize(ref strasseteile, strasseteile.Length - 1);
        string toStreet = string.Join(" ", strasseteile);
        cityteile = teile[1].Split(" ");
        int toZip; int.TryParse(cityteile[0], out toZip);
        string toCity = cityteile[1];

        var toAddressEntity = _tourRepository.GetAddressByAttributes(toStreet, toHousenumber, toZip, toCity);
        if (toAddressEntity == null)
        {
            int toAddressId = _tourRepository.AddAddress(new AddressEntity { Street = toStreet, Housenumber = toHousenumber, Zip = toZip, City = toCity });
            toAddressEntity = _tourRepository.GetAddressById(toAddressId);
        }

        var tourEntity = new TourEntity
        {
            Id = tour.Id,
            Name = tour.Name,
            Description = tour.Description,
            FromAddressId = fromAddressEntity.Id,
            FromAddress = fromAddressEntity,
            ToAddressId = toAddressEntity.Id,
            ToAddress = toAddressEntity,
            TransportType = tour.TransportType.ToString(),
            Distance = tour.Distance,
            EstimatedTime = tour.EstimatedTime,
            Image = tour.Image,
            Popularity = tour.Popularity,
            ChildFriendliness = tour.ChildFriendliness
        };

        return tourEntity;
    }
}