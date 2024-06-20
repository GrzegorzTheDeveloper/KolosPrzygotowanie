using KolosPrzygotowanie.Models;
using KolosPrzygotowanie.Services;
using Microsoft.AspNetCore.Mvc;

namespace KolosPrzygotowanie.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly IDbService _service;

    public BooksController(IDbService service)
    {
        _service = service;
    }

    [HttpGet("{id}/genres")]
    public async Task<IActionResult> GetBooksGenres(int id)
    {
        if (!await _service.DoesBookExist(id))
            return NotFound("Book with given id doesn't exist");

        return Ok(await _service.GetBookGenres(id));
    }

    [HttpPost]
    public async Task<IActionResult> AddBookWithGenres([FromBody] AddBookDto addBookDto)
    {
        foreach(var genre in addBookDto.genres)
            if (!await _service.DoesGenreExist(genre))
                return NotFound($"Genre {genre} doesn't exist");
        await _service.AddBookWithGenres(addBookDto);
        return Ok();
    }
    
}