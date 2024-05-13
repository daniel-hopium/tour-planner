using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TourPlanner.Persistence.Entities;

[Table("tours")]
public class TourEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("name")]
    public string Name { get; set; }
    [Column("description")]
    public string Description { get; set; }
    [Column("from_address_fk")]
    public int FromAddressId {  get; set; }
    public AddressEntity FromAddress {  get; set; }
    [Column("to_address_fk")]
    public int ToAddressId { get; set; }
    public AddressEntity ToAddress { get; set; }
    [Column("transport_type")]
    public string TransportType { get; set; }
    //public TourPlanner.Models.TransportType TransportType { get; set; }
    [Column("distance")]
    public double Distance { get; set; }
    [Column("est_time")]
    public int EstimatedTime {  get; set; }
    [Column("image")]
    public string Image {  get; set; }
    [Column("popularity")]
    public int Popularity { get; set; }
    [Column("child_friendliness")]
    public int ChildFriendliness {  get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [Column("created")]
    public DateTime Created { get; set; }
    
    public virtual ICollection<TourLogEntity> Logs { get; set; } = new List<TourLogEntity>();
}