using System.Text.Json.Serialization;

namespace USDemographicsAPI.Core.Dtos;

public class ReadCountyDto
{
    [JsonPropertyName("OBJECTID")]
    public int OBJECTID { get; set; }

    [JsonPropertyName("NAME")]
    public string NAME { get; set; } = string.Empty;

    [JsonPropertyName("STATE_NAME")]
    public string STATE_NAME { get; set; } = string.Empty;

    [JsonPropertyName("STATE_ABBR")]
    public string STATE_ABBR { get; set; } = string.Empty;

    [JsonPropertyName("STATE_FIPS")]
    public string STATE_FIPS { get; set; } = string.Empty;

    [JsonPropertyName("COUNTY_FIPS")]
    public string COUNTY_FIPS { get; set; } = string.Empty;

    [JsonPropertyName("FIPS")]
    public string FIPS { get; set; } = string.Empty;

    [JsonPropertyName("POPULATION")]
    public int? POPULATION { get; set; }

    [JsonPropertyName("POP_SQMI")]
    public double? POP_SQMI { get; set; }

    [JsonPropertyName("SQMI")]
    public double SQMI { get; set; }

    [JsonPropertyName("Shape__Area")]
    public double Shape__Area { get; set; }

    [JsonPropertyName("Shape__Length")]
    public double Shape__Length { get; set; }
}
