using System.Text;
using System.Text.Json;
using VM.Lab.Interfaces.BlobAnalyzer;
using VM.Lab.Plugins.WebControl;
using Xunit;

namespace VM.Lab.Plugins.WebControl.Tests;

public class MockSeedLabControlListener : ISeedLabControlListener
{
    public string LastRecipe { get; private set; }
    public Dictionary<string, string> LastBinMappings { get; private set; }
    public bool StartCalled { get; private set; }

    public void LoadRecipe(string recipeName) => LastRecipe = recipeName;

    public void Start(string id, string initials, string comments, string predictionResultName, string blobCollectionName) 
        => StartCalled = true;

    public void PrepareForStart(string id, string initials, string comments, string predictionResultName, string blobCollectionName) { }
    public void Stop() { }
    public void Flush() { }
    public void Finish() { }
    public void SetBinIds(Dictionary<string, string> mapping) => LastBinMappings = mapping;
}

public class WebControlTests
{
    [Fact]
    public async Task Test_LoadRecipe_Via_Api()
    {
        var mockListener = new MockSeedLabControlListener();
        int port = 8081; // Use different port for test
        using var plugin = new WebSeedLabControl(mockListener, port);

        using var client = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(new { recipe = "TestRecipe" }), Encoding.UTF8, "application/json");
        
        var response = await client.PostAsync($"http://localhost:{port}/api/load-recipe", content);
        
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal("TestRecipe", mockListener.LastRecipe);
    }

    [Fact]
    public async Task Test_SetBinIds_Via_Api()
    {
        var mockListener = new MockSeedLabControlListener();
        int port = 8082;
        using var plugin = new WebSeedLabControl(mockListener, port);

        using var client = new HttpClient();
        var mapping = new Dictionary<string, string> { { "Tag1", "ID1" } };
        var content = new StringContent(JsonSerializer.Serialize(mapping), Encoding.UTF8, "application/json");
        
        var response = await client.PostAsync($"http://localhost:{port}/api/set-bin-ids", content);
        
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(mockListener.LastBinMappings);
        Assert.Equal("ID1", mockListener.LastBinMappings["Tag1"]);
    }

    [Fact]
    public async Task Test_Status_Polling()
    {
        var mockListener = new MockSeedLabControlListener();
        int port = 8083;
        using var plugin = new WebSeedLabControl(mockListener, port);

        plugin.StateChanged(BlobAnalyzerState.MEASURING);
        plugin.BroadcastError();

        using var client = new HttpClient();
        var response = await client.GetAsync($"http://localhost:{port}/api/status");
        var json = await response.Content.ReadAsStringAsync();
        var data = JsonDocument.Parse(json).RootElement;

        Assert.Equal("MEASURING", data.GetProperty("state").GetString());
        Assert.True(data.GetProperty("hasError").GetBoolean());
    }
}
