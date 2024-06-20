using KolosPrzygotowanie.Models;

namespace KolosPrzygotowanie.Services;

public interface IDbService
{
    Task<BookGenresDto> GetBookGenres(int bookId);
    Task<bool> DoesBookExist(int bookId);

    Task<bool> DoesGenreExist(int id);

    Task AddBookWithGenres(AddBookDto addBookDto);
    
}