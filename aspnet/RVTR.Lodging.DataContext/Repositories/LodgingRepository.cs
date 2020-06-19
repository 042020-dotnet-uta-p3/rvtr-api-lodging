using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RVTR.Lodging.ObjectModel.Models;

namespace RVTR.Lodging.DataContext.Repositories
{
  /// <summary>
  /// Function to be applied for results filtering.
  /// </summary>
  using FilterFuncs = List<Expression<Func<LodgingModel, bool>>>;
  /// <summary>
  /// Function to be applied for result ordering.
  /// </summary>
  using OrderByFunc = Expression<Func<LodgingModel, Object>>;

  public class LodgingRepository : Repository<LodgingModel, LodgingQueryParamsModel>
  {
    private readonly LodgingContext dbContext;

    public LodgingRepository(LodgingContext context) : base(context)
    {
      this.dbContext = context;
    }

    /// <summary>
    /// EFCore "Include" functions for including additional data in the query.
    /// </summary>
    /// <param name="queryParams"></param>
    /// <returns></returns>
    private IQueryable<LodgingModel> IncludeQuery(LodgingQueryParamsModel queryParams)
    {
      var query = dbContext.Lodgings.AsQueryable();

      if (queryParams != null && queryParams.IncludeImages) {
        query = query
          .Include(x => x.Images)
          .Include(x => x.Rentals).ThenInclude(x => x.RentalUnit).ThenInclude(x => x.Bedrooms).ThenInclude(x => x.Images);
      }

      return query
        .Include(x => x.Location).ThenInclude(x => x.Address)
        .Include(x => x.Amenities)
        .Include(x => x.Rentals).ThenInclude(x => x.RentalUnit).ThenInclude(x => x.Bathrooms)
        .Include(x => x.Rentals).ThenInclude(x => x.RentalUnit).ThenInclude(x => x.Bedrooms).ThenInclude(x => x.Amenities)
        .Include(x => x.Rentals).ThenInclude(x => x.RentalUnit).ThenInclude(x => x.Bedrooms).ThenInclude(x => x.BedType)
        .Include(x => x.Reviews);
    }

    /// <summary>
    /// Executes a database query for a specific entity ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="queryParams"></param>
    /// <returns></returns>
    public override async Task<LodgingModel> GetAsync(int id, LodgingQueryParamsModel queryParams)
    {
      return await IncludeQuery(queryParams)
        .AsNoTracking()
        .Where(e => e.Id == id)
        .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Configures and executes a database query based on query parameters.
    /// </summary>
    /// <param name="queryParams"></param>
    /// <returns></returns>
    public override async Task<IEnumerable<LodgingModel>> GetAsync(LodgingQueryParamsModel queryParams)
    {
      var filters = GenerateFilterFuncs(queryParams);
      var orderBy = GenerateOrderByFunc(queryParams);
      var query = IncludeQuery(queryParams);
      return await GetAsync(query, filters, orderBy, queryParams.SortOrder, queryParams.Offset, queryParams.Limit);
    }

    /// <summary>
    /// Generates filtering functions based on user-supplied query parameters.
    /// </summary>
    /// <param name="queryParams"></param>
    /// <returns></returns>
    public static FilterFuncs GenerateFilterFuncs(LodgingQueryParamsModel queryParams)
    {
      // The funcs created here simply return true if there is an element matching
      // the filter parameter, or false if the element does not match. We use a
      // FirstOrDefault overload that acts as a Where clause. If there is a match,
      // then the item is valid for our filtering and should be included in the
      // result set. If not, then the item should not be included in the result set.

      var filters = new FilterFuncs();
      filters.Add(m => m.Reviews.Average(r => r.Rating) >= queryParams.RatingAtLeast);
      filters.Add(m => m.Rentals.FirstOrDefault(r => r.RentalUnit.Bedrooms.Count() >= queryParams.BedRoomsAtLeast) != null);
      filters.Add(m => m.Rentals.FirstOrDefault(r => r.RentalUnit.Bathrooms.Count() >= queryParams.BathsAtLeast) != null);

      filters.Add(m => m.Rentals.FirstOrDefault(
                    r => r.RentalUnit.Bedrooms.FirstOrDefault(b => b.BedCount >= queryParams.BedsAtLeast) != null) != null);

      if (!String.IsNullOrEmpty(queryParams.HasBedType))
      {
        filters.Add(m => m.Rentals.FirstOrDefault(
                      r => r.RentalUnit.Bedrooms.FirstOrDefault(
                        b => b.BedType.BedType == queryParams.HasBedType) != null) != null);
      }

      if (!String.IsNullOrEmpty(queryParams.HasAmenity))
      {
        filters.Add(m => m.Amenities.FirstOrDefault(a => a.Amenity == queryParams.HasAmenity) != null);
      }

      if (!String.IsNullOrEmpty(queryParams.City))
      {
        filters.Add(m => m.Location.Address.City.ToLower().Contains(queryParams.City.ToLower()));
      }

      return filters;
    }


    /// <summary>
    /// Generates ordering functions based on user-supplied data.
    /// </summary>
    /// <param name="queryParams"></param>
    /// <returns></returns>
    public static OrderByFunc GenerateOrderByFunc(LodgingQueryParamsModel queryParams)
    {
      if (!String.IsNullOrEmpty(queryParams.SortKey))
      {
        switch (queryParams.SortKey)
        {
          case "Id": return (e => e.Id);
          case "Name": return (e => e.Name);
          case "Description": return (e => e.Description);

          case "Location.Id": return (e => e.Location.Id);
          case "Location.Latitude": return (e => e.Location.Latitude);
          case "Location.Longitude": return (e => e.Location.Longitude);
          case "Location.Locale": return (e => e.Location.Locale);

          case "Location.Address.Id": return (e => e.Location.Address.Id);
          case "Location.Address.City": return (e => e.Location.Address.City);
          case "Location.Address.Country": return (e => e.Location.Address.Country);
          case "Location.Address.PostalCode": return (e => e.Location.Address.PostalCode);
          case "Location.Address.StateProvince": return (e => e.Location.Address.StateProvince);
          case "Location.Address.Street": return (e => e.Location.Address.Street);

          case "Rentals": return (e => e.Rentals.Count());
          case "Beds": return (e => e.Rentals.Sum(u => u.RentalUnit.Bedrooms.Sum(b => b.BedCount)));
          case "Bedrooms": return (e => e.Rentals.Sum(u => u.RentalUnit.Bedrooms.Count()));
          case "Bathrooms": return (e => e.Rentals.Sum(u => u.RentalUnit.Bathrooms.Count()));
          case "Occupancy": return (e => e.Rentals.Sum(u => u.RentalUnit.Occupancy));

          case "ReviewCount": return (e => e.Reviews.Count());
          case "ReviewAverageRating": return (e => e.Reviews.Average(r => r.Rating));
        }
      }
      return null;
    }
  }
}
