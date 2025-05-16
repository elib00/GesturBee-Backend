using System.Text.Json.Serialization;

public class FacebookUserInfoDTO
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("picture")]
    public FacebookPicture Picture { get; set; }

    public string PictureUrl => Picture?.Data?.Url;
}

public class FacebookPicture
{
    [JsonPropertyName("data")]
    public FacebookPictureData Data { get; set; }
}

public class FacebookPictureData
{
    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("is_silhouette")]
    public bool IsSilhouette { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }
}