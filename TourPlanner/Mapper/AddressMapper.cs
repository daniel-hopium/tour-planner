using System;
using System.Linq;
using System.Text.RegularExpressions;
using TourPlanner.Persistence.Entities;
using TourPlanner.Persistence.Repository;

namespace TourPlanner.Mapper;

public class AddressMapper
{
  private readonly ITourRepository _tourRepository;

  public AddressMapper(ITourRepository tourRepository)
  {
    _tourRepository = tourRepository;
  }

  public AddressEntity StringToAddress(string address)
  {
    var pattern = @"\d+[A-Za-z]?";

    var parts = address.Split(',');

    var cityteile = parts[0].Trim().Split(" ");
    string city;
    if (int.TryParse(cityteile[0], out var zip) && zip > 999)
      city = string.Join(" ", cityteile.Skip(1));
    else
      city = string.Join(" ", cityteile);
    var streetparts = parts.Length > 1 ? parts[1].Trim().Split(" ") : new[] { string.Empty };
    var housenumber = streetparts.Length > 1 && Regex.IsMatch(streetparts.Last(), pattern)
      ? streetparts.Last()
      : string.Empty;
    if (streetparts.Length > 1 && Regex.IsMatch(streetparts.Last(), pattern))
      Array.Resize(ref streetparts, streetparts.Length - 1);
    var street = string.Join(" ", streetparts);
    var country = parts.Length > 2 ? parts[2].Trim() : string.Empty;

    var addressEntity = _tourRepository.GetAddressByAttributes(street, housenumber, zip, city, country);
    if (addressEntity == null)
    {
      addressEntity = new AddressEntity
        { Street = street, Housenumber = housenumber, Zip = zip, City = city, Country = country };
      var addressId = _tourRepository.AddAddress(addressEntity);
      addressEntity.Id = addressId;
    }

    return addressEntity;
  }
}