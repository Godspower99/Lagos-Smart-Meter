using System;
using System.ComponentModel.DataAnnotations;

public class SubscriberMetersModel
{   
    public string SubscriberID { get; set; }
    
    [Key]
    public string MeterID { get; set; }

    public DateTime AssignedDate { get; set; }
}