using System.Linq;
using TourPlanner.Models;
using TourPlanner.Persistence.Entities;
using TourPlanner.Persistence.Repository;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TourPlanner.Mapper;

public class TourMapper
{
    private readonly ITourRepository _tourRepository;

    public TourMapper()
    {
        _tourRepository = TourRepository.Instance;
    }

    public TourEntity TourModelToEntity(TourModel tour)
    {
        string pattern = @"\d{1}";
        
        var fromAddress = tour.FromAddress;
        string[] fromTeile = fromAddress.Split(',');

        string[] fromCityteile = fromTeile[0].Trim().Split(" ");
        int fromZip = 0;
        int.TryParse(fromCityteile[0], out fromZip);
        string fromCity = string.Empty;
        if (fromZip > 0) 
        {
            fromCity = string.Join(" ", fromCityteile.Skip(1));
        } 
        else
        {
            fromCity = string.Join(" ", fromCityteile);
        }
        string[] fromStrasseteile = fromTeile.Length > 1 ? fromTeile[1].Trim().Split(" ") : new string[] { string.Empty };
        string fromHousenumber = (fromStrasseteile.Length > 1 && Regex.IsMatch(fromStrasseteile.Last(), pattern) ) ? fromStrasseteile.Last() : string.Empty;
        if (fromStrasseteile.Length > 1 && Regex.IsMatch(fromStrasseteile.Last(), pattern)) Array.Resize(ref fromStrasseteile, fromStrasseteile.Length - 1);
        string fromStreet = string.Join(" ", fromStrasseteile);             
        string fromCountry = fromTeile.Length > 2 ? fromTeile[2].Trim() : string.Empty;

        var fromAddressEntity = _tourRepository.GetAddressByAttributes(fromStreet, fromHousenumber, fromZip, fromCity);
        if (fromAddressEntity == null)
        {
            int fromAddressId = _tourRepository.AddAddress(new AddressEntity { Id = 0, Street = fromStreet, Housenumber = fromHousenumber, Zip = fromZip, City = fromCity, Country = fromCountry });
            fromAddressEntity = _tourRepository.GetAddressById(fromAddressId);
        }

        var toAddress = tour.ToAddress;
        string[] toTeile = toAddress.Split(',');

        string[] toCityteile = toTeile[0].Trim().Split(" ");
        int toZip = 0;
        int.TryParse(toCityteile[0], out toZip);
        string toCity = string.Empty;
        if (toZip > 0)
        {
            toCity = string.Join(" ", toCityteile.Skip(1));
        }
        else
        {
            toCity = string.Join(" ", toCityteile);
        }
        string[] toStrasseteile = toTeile.Length > 1 ? toTeile[1].Trim().Split(" ") : new string[] { string.Empty };
        string toHousenumber = (toStrasseteile.Length > 1 && Regex.IsMatch(toStrasseteile.Last(), pattern)) ? toStrasseteile.Last() : string.Empty;
        if (toStrasseteile.Length > 1 && Regex.IsMatch(toStrasseteile.Last(), pattern)) Array.Resize(ref toStrasseteile, toStrasseteile.Length - 1);
        string toStreet = string.Join(" ", toStrasseteile);
        string toCountry = toTeile.Length > 2 ? toTeile[2].Trim() : string.Empty;

        var toAddressEntity = _tourRepository.GetAddressByAttributes(toStreet, toHousenumber, toZip, toCity);
        if (toAddressEntity == null)
        {
            int toAddressId = _tourRepository.AddAddress(new AddressEntity { Street = toStreet, Housenumber = toHousenumber, Zip = toZip, City = toCity, Country = toCountry });
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