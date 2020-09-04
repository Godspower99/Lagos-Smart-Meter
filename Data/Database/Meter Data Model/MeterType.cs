using System.ComponentModel.DataAnnotations;

public class MeterType
{
    [Key]
    public string ID { get; set; }

    public string Name { get; set; }

    public string Description { get; set;}
}