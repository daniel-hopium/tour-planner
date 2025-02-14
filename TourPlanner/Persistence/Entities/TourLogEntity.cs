﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourPlanner.Persistence.Entities;

[Table("tourlogs")]
public class TourLogEntity
{
  [Key] [Column("id")] public int Id { get; set; }

  [Column("tour_date")] [Required] public DateOnly TourDate { get; set; }

  [Column("comment")] public string Comment { get; set; }

  [Column("tour_id_fk")] [Required] public int TourId { get; set; }

  [ForeignKey("TourId")] public virtual TourEntity Tour { get; set; }

  [Column("difficulty")] [Required] public int Difficulty { get; set; }

  [Column("distance")] public double Distance { get; set; }

  [Column("total_time")] [Required] public int TotalTime { get; set; }

  [Column("rating")] [Required] public int Rating { get; set; }

  [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
  [Column("created")]
  public DateTime Created { get; set; }
}