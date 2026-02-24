namespace Domain.Entities.Products;

public class Phone : Product
{
    public double ScreenSize { get; set; }
    public string Processor { get; set; }
    public int StorageGB { get; set; }
    public int BatteryMah { get; set; }
    public string OS { get; set; }
}
