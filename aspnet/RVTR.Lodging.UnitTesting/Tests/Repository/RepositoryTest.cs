using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RVTR.Lodging.DataContext;
using RVTR.Lodging.DataContext.Repositories;
using RVTR.Lodging.ObjectModel.Models;
using Xunit;

namespace RVTR.Lodging.UnitTesting.Tests
{
    public class RepositoryTest
  {
    private static readonly SqliteConnection _connection = new SqliteConnection("Data Source=:memory:");
    private static readonly DbContextOptions<LodgingContext> _options = new DbContextOptionsBuilder<LodgingContext>().UseSqlite(_connection).Options;

    public static readonly IEnumerable<object[]> _records = new List<object[]>()
    {
      new object[]
      {
        new LodgingModel() { Id = 1 },
        new RentalModel() { Id = 1 },
        new ReviewModel() { Id = 1 }
      }
    };

    [Theory]
    [MemberData(nameof(_records))]
    public async void Test_Repository_DeleteAsync(LodgingModel lodging, RentalModel rental, ReviewModel review)
    {
      await _connection.OpenAsync();

      try
      {
        using (var ctx = new LodgingContext(_options))
        {
          await ctx.Database.EnsureCreatedAsync();
          await ctx.Lodgings.AddAsync(lodging);
          await ctx.Rentals.AddAsync(rental);
          await ctx.Reviews.AddAsync(review);
          await ctx.SaveChangesAsync();
        }

        using (var ctx = new LodgingContext(_options))
        {
          var lodgings = new LodgingRepository(ctx);

          await lodgings.DeleteAsync(1, new LodgingSearchFilterModel());
          await ctx.SaveChangesAsync();

          Assert.Empty(await ctx.Lodgings.ToListAsync());
        }

        using (var ctx = new LodgingContext(_options))
        {
          var rentals = new RentalRepository(ctx);

          await rentals.DeleteAsync(1, new RentalSearchFilterModel());
          await ctx.SaveChangesAsync();

          Assert.Empty(await ctx.Rentals.ToListAsync());
        }

        using (var ctx = new LodgingContext(_options))
        {
          var reviews = new ReviewRepository(ctx);

          await reviews.DeleteAsync(1, new ReviewSearchFilterModel());
          await ctx.SaveChangesAsync();

          Assert.Empty(await ctx.Reviews.ToListAsync());
        }
      }
      finally
      {
        await _connection.CloseAsync();
      }
    }

    [Theory]
    [MemberData(nameof(_records))]
    public async void Test_Repository_InsertAsync(LodgingModel lodging, RentalModel rental, ReviewModel review)
    {
      await _connection.OpenAsync();

      try
      {
        using (var ctx = new LodgingContext(_options))
        {
          await ctx.Database.EnsureCreatedAsync();
        }

        using (var ctx = new LodgingContext(_options))
        {
          var lodgings = new LodgingRepository(ctx);

          await lodgings.InsertAsync(lodging);
          await ctx.SaveChangesAsync();

          Assert.NotEmpty(await ctx.Lodgings.ToListAsync());
        }

        using (var ctx = new LodgingContext(_options))
        {
          var rentals = new RentalRepository(ctx);

          await rentals.InsertAsync(rental);
          await ctx.SaveChangesAsync();

          Assert.NotEmpty(await ctx.Rentals.ToListAsync());
        }

        using (var ctx = new LodgingContext(_options))
        {
          var reviews = new ReviewRepository(ctx);

          await reviews.InsertAsync(review);
          await ctx.SaveChangesAsync();

          Assert.NotEmpty(await ctx.Reviews.ToListAsync());
        }
      }
      finally
      {
        await _connection.CloseAsync();
      }
    }

    [Fact]
    public async void Test_Repository_GetAsync()
    {
      await _connection.OpenAsync();

      try
      {
        using (var ctx = new LodgingContext(_options))
        {
          await ctx.Database.EnsureCreatedAsync();
        }

        using (var ctx = new LodgingContext(_options))
        {
          var lodgings = new LodgingRepository(ctx);

          var actual = await lodgings.GetAsync(new LodgingSearchFilterModel());

          Assert.Empty(actual);
        }

        using (var ctx = new LodgingContext(_options))
        {
          var rentals = new RentalRepository(ctx);

          var actual = await rentals.GetAsync(new RentalSearchFilterModel());

          Assert.Empty(actual);
        }

        using (var ctx = new LodgingContext(_options))
        {
          var reviews = new ReviewRepository(ctx);

          var actual = await reviews.GetAsync(new ReviewSearchFilterModel());

          Assert.Empty(actual);
        }
      }
      finally
      {
        await _connection.CloseAsync();
      }
    }

    [Fact]
    public async void Test_Repository_GetAsync_ById()
    {
      await _connection.OpenAsync();

      try
      {
        using (var ctx = new LodgingContext(_options))
        {
          await ctx.Database.EnsureCreatedAsync();
        }

        using (var ctx = new LodgingContext(_options))
        {
          var lodgings = new LodgingRepository(ctx);

          var actual = await lodgings.GetAsync(1, new LodgingSearchFilterModel());

          Assert.Null(actual);
        }

        using (var ctx = new LodgingContext(_options))
        {
          var rentals = new RentalRepository(ctx);

          var actual = await rentals.GetAsync(1, new RentalSearchFilterModel());

          Assert.Null(actual);
        }

        using (var ctx = new LodgingContext(_options))
        {
          var reviews = new ReviewRepository(ctx);

          var actual = await reviews.GetAsync(1, new ReviewSearchFilterModel());

          Assert.Null(actual);
        }
      }
      finally
      {
        await _connection.CloseAsync();
      }
    }

    [Theory]
    [MemberData(nameof(_records))]
    public async void Test_Repository_Update(LodgingModel lodging, RentalModel rental, ReviewModel review)
    {
      await _connection.OpenAsync();

      try
      {
        using (var ctx = new LodgingContext(_options))
        {
          await ctx.Database.EnsureCreatedAsync();
          await ctx.Lodgings.AddAsync(lodging);
          await ctx.Rentals.AddAsync(rental);
          await ctx.Reviews.AddAsync(review);
          await ctx.SaveChangesAsync();
        }

        using (var ctx = new LodgingContext(_options))
        {
          var lodgings = new LodgingRepository(ctx);
          var expected = await ctx.Lodgings.FirstAsync();

          expected.Name = "name";
          lodgings.Update(expected);
          await ctx.SaveChangesAsync();

          var actual = await ctx.Lodgings.FirstAsync();

          Assert.Equal(expected, actual);
        }

        using (var ctx = new LodgingContext(_options))
        {
          var rentals = new RentalRepository(ctx);
          var expected = await ctx.Rentals.FirstAsync();

          expected.Name = "name";
          rentals.Update(expected);
          await ctx.SaveChangesAsync();

          var actual = await ctx.Rentals.FirstAsync();

          Assert.Equal(expected, actual);
        }

        using (var ctx = new LodgingContext(_options))
        {
          var reviews = new ReviewRepository(ctx);
          var expected = await ctx.Reviews.FirstAsync();

          expected.Comment = "comment";
          reviews.Update(expected);
          await ctx.SaveChangesAsync();

          var actual = await ctx.Reviews.FirstAsync();

          Assert.Equal(expected, actual);
        }
      }
      finally
      {
        await _connection.CloseAsync();
      }
    }
  }
}
