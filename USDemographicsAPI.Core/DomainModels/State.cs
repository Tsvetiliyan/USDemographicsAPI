using System.ComponentModel.DataAnnotations;

namespace USDemographicsAPI.Core.DomainModels;

public class State
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public string StateName { get; set; }

    [Required]
    [StringLength(2, MinimumLength = 2)]
    public string StateAbbreviation { get; set; }

    [Required]
    [StringLength(2)]
    public string StateFips { get; set; }


    /// <summary>
    /// Last updated date of the amount of population, which was taken from External API
    /// </summary>
    [Required]
    public DateTime LastUpdated { get; set; }
}
