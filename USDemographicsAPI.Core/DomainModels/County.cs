using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace USDemographicsAPI.Core.DomainModels;

public class County
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string CountyName { get; set; }

    [Required]
    public int StateId { get; set; }

    [Required]
    [ForeignKey("StateId")]
    public State State { get; set; }

    [Required]
    [StringLength(3)]
    public string CountyFips { get; set; }

    [Required]
    public int Population { get; set; }

    [Required]
    public double PopulationPerSquareMile { get; set; }

    [Required]
    public double SquareMiles { get; set; }

    [Required]
    public double ShapeArea { get; set; }

    [Required]
    public double ShapeLength { get; set; }

    //This is here if there is any need to get only a few states and the data is actually new
    /// <summary>
    /// Last updated date of the amount of population, which was taken from External API
    /// </summary>
    [Required]
    public DateTime LastUpdated { get; set; }
}
