using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourPlanner.Persistence.Entities;

[Table("addresses")]
public class AddressEntity
{
  [Key] [Column("id")] public int Id { get; set; }

  [Column("country")] public string? Country { get; set; }

  [Column("street")] public string? Street { get; set; }

  [Column("housenumber")] public string? Housenumber { get; set; }

  [Column("zip")] public int? Zip { get; set; }

  [Column("city")] public string City { get; set; }

  [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
  [Column("created")]
  public DateTime? Created { get; set; }

  public override string ToString()
  {
    var zip = Zip > 0 ? Zip + " " : string.Empty;
    var street = Street != string.Empty ? ", " + Street : string.Empty;
    var country = Country != string.Empty ? ", " + Country : string.Empty;

    return $"{zip}{City}{street} {Housenumber}{country}";
  }
}