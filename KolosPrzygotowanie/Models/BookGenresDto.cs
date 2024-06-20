namespace KolosPrzygotowanie.Models;

public class BookGenresDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<string> Genres { get; set; }
}