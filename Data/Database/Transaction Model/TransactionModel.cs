using System;
using System.ComponentModel.DataAnnotations;

public class TransactionModel
{
    [Key]
    public string TransactionID { get; set; }
    public double Amount { get; set ;}
    public string Sender { get; set; }
    public string PaymentType { get; set; }
    public string PaymentTypeID { get; set; }
    public string MeterID { get; set ;}
    public string SubscriberID { get; set ;}
    public DateTime TransactionDate { get; set; }
    
}