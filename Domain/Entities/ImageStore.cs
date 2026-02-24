namespace Domain.Entities;

public class ImageStore
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Url { get; set; }
    public Guid ProductId { get; set; }
}
