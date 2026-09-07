using System.Net.Http.Json;

namespace CommentService.Clients;

public class ProfanityClient
{
    private readonly HttpClient _httpClient;

    public ProfanityClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> ContainsProfanityAsync(string text)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/profanity/check",
            text);

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content.ReadFromJsonAsync<ProfanityResponse>();

        return result?.ContainsProfanity ?? false;
    }

    private class ProfanityResponse
    {
        public bool ContainsProfanity { get; set; }
    }
}
