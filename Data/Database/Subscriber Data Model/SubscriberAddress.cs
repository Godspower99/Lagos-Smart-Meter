using System.ComponentModel.DataAnnotations;

public class SubscriberAddress
{   
    [Key]
    public string SubscriberID { get; set; }
    public string city { get; set; }
    public string Phone { get; set; }
    public string Street { get; set; }
    public string BuildingNumber { get; set; }
}