using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Persistence.Entities
{
    [Table("addresses")]
    public class AddressEntity
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("street")]
        public string Street {  get; set; }
        [Column("housenumber")]
        public string Housenumber {  get; set; }
        [Column("zip")]
        public int Zip {  get; set; }
        [Column("city")]
        public string City {  get; set; }
        [Column("created")]
        public DateTime Created {  get; set; }
    }
}
