namespace Domain.Entities.Products;

public class Book : Product
{
    public string Author { get; set; }
    public string ISBN { get; set; }
    public int Pages { get; set; }
}
