using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TourPlanner.Persistence.Entities;
using TourPlanner.Persistence.Repository;

namespace TourPlanner.Mapper
{
    public class AddressMapper
    {
        private readonly ITourRepository _tourRepository;

        public AddressMapper(ITourRepository tourRepository)
        {
            _tourRepository = tourRepository;
        }


        public AddressEntity StringToAddress(string address)
        {
            string pattern = @"\d+[A-Za-z]?";

            string[] parts = address.Split(',');

            string[] cityteile = parts[0].Trim().Split(" ");
            string city;
            if (int.TryParse(cityteile[0], out int zip) && zip > 999)
            {
                city = string.Join(" ", cityteile.Skip(1));
            }
            else
            {
                city = string.Join(" ", cityteile);
            }
            string[] streetparts = parts.Length > 1 ? parts[1].Trim().Split(" ") : new string[] { string.Empty };
            string housenumber = (streetparts.Length > 1 && Regex.IsMatch(streetparts.Last(), pattern)) ? streetparts.Last() : string.Empty;
            if (streetparts.Length > 1 && Regex.IsMatch(streetparts.Last(), pattern)) Array.Resize(ref streetparts, streetparts.Length - 1);
            string street = string.Join(" ", streetparts);
            string country = parts.Length > 2 ? parts[2].Trim() : string.Empty;

            var addressEntity = _tourRepository.GetAddressByAttributes(street, housenumber, zip, city, country);
            if (addressEntity == null)
            {
                addressEntity = new AddressEntity { Street = street, Housenumber = housenumber, Zip = zip, City = city, Country = country };
                int addressId = _tourRepository.AddAddress(addressEntity);
                addressEntity.Id = addressId;
            }

            return addressEntity;
        }
    }
}
