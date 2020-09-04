using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

public class SubscriberModel : IdentityUser
{
    public string FullName { get; set; }
    public DateTime Birthday { get; set; }
    public string Gender { get; set; }
}