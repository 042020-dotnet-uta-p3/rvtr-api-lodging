using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RVTR.Lodging.DataContext.Repositories;
using RVTR.Lodging.ObjectModel.Models;

namespace RVTR.Lodging.WebApi.Controllers
{
  [ApiController]
  [EnableCors("public")]
  [Route("api/[controller]")]
  public class ReviewController : ControllerBase
  {
    private readonly ILogger<ReviewController> _logger;
    private readonly UnitOfWork _unitOfWork;

    public ReviewController(ILogger<ReviewController> logger, UnitOfWork unitOfWork)
    {
      _logger = logger;
      _unitOfWork = unitOfWork;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      try
      {
        await _unitOfWork.Review.DeleteAsync(id);
        await _unitOfWork.CommitAsync();

        return Ok();
      }
      catch
      {
        return NotFound(id);
      }
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      return Ok(await _unitOfWork.Review.SelectAsync());
    }

    [HttpGet("{id")]
    public async Task<IActionResult> Get(int id)
    {
      try
      {
        return Ok(await _unitOfWork.Review.SelectAsync(id));
      }
      catch
      {
        return NotFound(id);
      }
    }

    [HttpPost]
    public async Task<IActionResult> Post(ReviewModel review)
    {
      await _unitOfWork.Review.InsertAsync(review);
      await _unitOfWork.CommitAsync();

      return Accepted(review);
    }

    [HttpPut]
    public async Task<IActionResult> Put(ReviewModel review)
    {
      _unitOfWork.Review.Update(review);
      await _unitOfWork.CommitAsync();

      return Accepted(review);
    }
  }
}
