using System.ComponentModel.DataAnnotations;

public class MeterModel
{   
    #region Public Properties
    [Key]
    public string MeterID { get; set; }
    public string TarriffID { get; set; }
    public string TypeID { get; set; }
    public string MeterState { get; set; }
    public string ProvisionState { get; set;}
    #endregion
    
}