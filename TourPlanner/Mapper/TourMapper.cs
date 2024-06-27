using TourPlanner.Models;
using TourPlanner.Persistence.Entities;
using TourPlanner.Persistence.Repository;

namespace TourPlanner.Mapper;

public static class TourMapper
{
  public static TourEntity TourModelToEntity(TourModel tour, ITourRepository tourRepository)
  {
    var addressMapper = new AddressMapper(tourRepository);

    var fromAddress = addressMapper.StringToAddress(tour.FromAddress);
    var toAddress = addressMapper.StringToAddress(tour.ToAddress);


    var tourEntity = new TourEntity
    {
      Id = tour.Id,
      Name = tour.Name,
      Description = tour.Description,
      FromAddressId = fromAddress.Id,
      FromAddress = fromAddress,
      ToAddressId = toAddress.Id,
      ToAddress = toAddress,
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