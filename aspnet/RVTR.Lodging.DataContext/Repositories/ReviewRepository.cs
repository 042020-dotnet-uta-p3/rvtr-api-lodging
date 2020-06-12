using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RVTR.Lodging.ObjectModel.Models;

namespace RVTR.Lodging.DataContext.Repositories
{

  using FilterFunc = Expression<Func<ReviewModel, bool>>;

  public class ReviewRepository : Repository<ReviewModel>
  {
    private LodgingContext dbContext;

    public ReviewRepository(LodgingContext context) : base(context)
    {
      this.dbContext = context;
    }

    public override async Task<IEnumerable<ReviewModel>> GetAsync()
    {
      return await dbContext.Reviews
        .AsNoTracking()
        .Include(x => x.Lodging).ThenInclude(x => x.Location).ThenInclude(x => x.Address)
        .Include(x => x.Lodging).ThenInclude(x => x.Rentals).ThenInclude(x => x.RentalUnit).ThenInclude(x => x.Bathrooms)
        .Include(x => x.Lodging).ThenInclude(x => x.Rentals).ThenInclude(x => x.RentalUnit).ThenInclude(x => x.Bedrooms)
        .Include(x => x.Lodging).ThenInclude(x => x.Rentals).ThenInclude(x => x.RentalUnit).ThenInclude(x => x.Images)
        .ToListAsync();
    }

    public override async Task<ReviewModel> GetAsync(int id)
    {
      return await dbContext.Reviews
        .AsNoTracking()
        .Include(x => x.Lodging).ThenInclude(x => x.Location).ThenInclude(x => x.Address)
        .Include(x => x.Lodging).ThenInclude(x => x.Rentals).ThenInclude(x => x.RentalUnit).ThenInclude(x => x.Bathrooms)
        .Include(x => x.Lodging).ThenInclude(x => x.Rentals).ThenInclude(x => x.RentalUnit).ThenInclude(x => x.Bedrooms)
        .Include(x => x.Lodging).ThenInclude(x => x.Rentals).ThenInclude(x => x.RentalUnit).ThenInclude(x => x.Images)
        .Where(e => e.Id == id)
        .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ReviewModel>> Find(FilterFunc searchFilter,
                                                     int maxResults)
    {
      var lodgings = await dbContext.Reviews
        .AsNoTracking()
        .Include(x => x.Lodging).ThenInclude(x => x.Location).ThenInclude(x => x.Address)
        .Include(x => x.Lodging).ThenInclude(x => x.Rentals).ThenInclude(x => x.RentalUnit).ThenInclude(x => x.Bathrooms)
        .Include(x => x.Lodging).ThenInclude(x => x.Rentals).ThenInclude(x => x.RentalUnit).ThenInclude(x => x.Bedrooms)
        .Include(x => x.Lodging).ThenInclude(x => x.Rentals).ThenInclude(x => x.RentalUnit).ThenInclude(x => x.Images)
        .Where(searchFilter)
        .Take(maxResults)
        .ToListAsync();

      return lodgings;
    }
  }
}
