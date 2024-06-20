using KolosPrzygotowanie.Models;
using Microsoft.Data.SqlClient;

namespace KolosPrzygotowanie.Services;

public class DbService : IDbService
{
    private readonly IConfiguration _configuration;

    public DbService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<BookGenresDto> GetBookGenres(int bookId)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand titleCommand = new SqlCommand();
        titleCommand.Connection = connection;
        await connection.OpenAsync();
        titleCommand.CommandText = "SELECT title from books where PK = @bookId";
        titleCommand.Parameters.AddWithValue("@bookId", bookId);
        var titleCommandResult = await titleCommand.ExecuteScalarAsync();
        string title = "error";
        if (titleCommandResult is not null)
            title = titleCommandResult.ToString();
        using SqlCommand genresCommand = new SqlCommand();
        genresCommand.Connection = connection;
        genresCommand.CommandText =
            "SELECT name from genres g join books_genres bg on g.Pk = bg.Fk_genre where bg.fk_book = @bookid ";
        genresCommand.Parameters.AddWithValue("@bookId", bookId);
        var reader = await genresCommand.ExecuteReaderAsync();
        List<string> genres = new List<string>();
        while (await reader.ReadAsync())
        {
            var genre = reader.GetString(0);
            genres.Add(genre);
        }

        var result = new BookGenresDto()
        {
            Id = bookId,
            Title = title,
            Genres = genres

        };

        return result;

    }

    public async Task<bool> DoesBookExist(int bookId)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT 1 FROM BOOKS WHERE PK = @bookId";
        command.Parameters.AddWithValue("@bookId", bookId);
        await connection.OpenAsync();
        return await command.ExecuteScalarAsync() is not null;
    }

    public async Task<bool> DoesGenreExist(int id)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT 1 FROM genres WHERE PK = @id";
        command.Parameters.AddWithValue("@id", id);
        await connection.OpenAsync();
        return await command.ExecuteScalarAsync() is not null;
    }

    public async Task AddBookWithGenres(AddBookDto addBookDto)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand insertTitle = new SqlCommand();
        insertTitle.Connection = connection;
        insertTitle.CommandText = "INSERT INTO books (title) OUTPUT INSERTED.PK VALUES (@title)";
        insertTitle.Parameters.AddWithValue("@title", addBookDto.title);
        await connection.OpenAsync();
        int result = (int) await insertTitle.ExecuteScalarAsync();

        foreach (var genre in addBookDto.genres)
        {
            using SqlCommand insertGenre = new SqlCommand();
            insertGenre.Connection = connection;
            insertGenre.CommandText = "INSERT INTO books_genres values(@bookId, @genreId)";
            insertGenre.Parameters.AddWithValue("@bookId", result);
            insertGenre.Parameters.AddWithValue("@genreId", genre);
            Console.WriteLine($"Inserting genreId {genre} for bookId {result}");
            await insertGenre.ExecuteScalarAsync();
        }

    }
}