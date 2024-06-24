using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Persistence.Entities
{
    [Table("addresses")]
    public class AddressEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("country")]
        public string? Country { get; set; }
        [Column("street")]
        public string? Street {  get; set; }
        [Column("housenumber")]
        public string? Housenumber {  get; set; }
        [Column("zip")]
        public int? Zip {  get; set; }
        [Column("city")]
        public string City {  get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column("created")]
        public DateTime? Created {  get; set; }

        public override string ToString()
        {
            string zip = Zip > 0 ? Zip.ToString() + " " : string.Empty;
            string street = Street != string.Empty ? ", " + Street : string.Empty;
            string country = Country != string.Empty ? ", " + Country : string.Empty;

            return $"{zip}{City}{street} {Housenumber}{country}";
        }
    }
}
