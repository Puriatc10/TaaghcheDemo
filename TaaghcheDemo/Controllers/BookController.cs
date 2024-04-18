using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaaghcheDemo.Services;

namespace TaaghcheDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService) 
        {
            _bookService = bookService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetBookById(int id)
        {
            var res = await _bookService.GetBookInfo(id);

            if (string.IsNullOrEmpty(res))
            {
                return NotFound();
            }
            return Ok(res);
        }
    }
}
