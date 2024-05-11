using System.Collections.Generic;
using TourPlanner.Persistence.Entities;
using TourPlanner.Persistence.Utils;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TourPlanner.Persistence.Repository;

public class TourRepository
{
    private readonly TourPlannerDbContext _dbContext;

    public TourRepository(TourPlannerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<TourEntity> GetTours()
    {
        return _dbContext.Tours.Include(t => t.FromAddress).Include(t => t.ToAddress).ToList();
    }
}