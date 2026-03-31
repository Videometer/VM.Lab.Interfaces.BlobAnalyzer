using System.Net;
using System.Text;
using System.Text.Json;
using VM.Lab.Interfaces.BlobAnalyzer;

namespace VM.Lab.Plugins.WebControl;

public class WebSeedLabControl : SeedLabControl
{
    private readonly SimpleWebServer _server;
    private BlobAnalyzerState _currentState = BlobAnalyzerState.None;
    private bool _hasError = false;

    public WebSeedLabControl(ISeedLabControlListener listener) : base(listener)
    {
        _server = new SimpleWebServer(listener, 8080, GetCurrentStatus);
        _server.Start();
    }

    public WebSeedLabControl(ISeedLabControlListener listener, int port) : base(listener)
    {
        _server = new SimpleWebServer(listener, port, GetCurrentStatus);
        _server.Start();
    }

    private (BlobAnalyzerState State, bool HasError) GetCurrentStatus()
    {
        return (_currentState, _hasError);
    }

    public override void StateChanged(BlobAnalyzerState newState)
    {
        _currentState = newState;
        _hasError = false;
    }

    public override void BroadcastError()
    {
        _hasError = true;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _server?.Stop();
        }
        base.Dispose(disposing);
    }
}

internal class SimpleWebServer
{
    private readonly HttpListener _listener;
    private readonly ISeedLabControlListener _controlListener;
    private readonly Func<(BlobAnalyzerState State, bool HasError)> _statusProvider;
    private bool _isRunning;

    public SimpleWebServer(ISeedLabControlListener controlListener, int port, Func<(BlobAnalyzerState State, bool HasError)> statusProvider)
    {
        _controlListener = controlListener;
        _statusProvider = statusProvider;
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://localhost:{port}/");
    }

    public void Start()
    {
        _isRunning = true;
        _listener.Start();
        Task.Run(ListenLoop);
    }

    public void Stop()
    {
        _isRunning = false;
        _listener.Stop();
    }

    private async Task ListenLoop()
    {
        while (_isRunning)
        {
            try
            {
                var context = await _listener.GetContextAsync();
                _ = ProcessRequest(context);
            }
            catch (HttpListenerException) when (!_isRunning) { }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in web server: {ex.Message}");
            }
        }
    }

    private async Task ProcessRequest(HttpListenerContext context)
    {
        try
        {
            var request = context.Request;
            var response = context.Response;

            if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/api/status")
            {
                var status = _statusProvider();
                var json = JsonSerializer.Serialize(new { state = status.State.ToString(), hasError = status.HasError });
                await SendResponse(response, json, "application/json");
            }
            else if (request.HttpMethod == "POST")
            {
                await HandlePost(request, response);
            }
            else if (request.HttpMethod == "GET")
            {
                await SendResponse(response, GetHtml(), "text/html");
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Close();
            }
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.Close();
            Console.WriteLine($"Request processing error: {ex.Message}");
        }
    }

    private async Task HandlePost(HttpListenerRequest request, HttpListenerResponse response)
    {
        using var reader = new StreamReader(request.InputStream);
        var body = await reader.ReadToEndAsync();
        var path = request.Url.AbsolutePath;

        try
        {
            switch (path)
            {
                case "/api/load-recipe":
                    var recipe = JsonDocument.Parse(body).RootElement.GetProperty("recipe").GetString();
                    _controlListener.LoadRecipe(recipe);
                    break;
                case "/api/start":
                    var startData = JsonDocument.Parse(body).RootElement;
                    _controlListener.Start(
                        startData.GetProperty("id").GetString(),
                        startData.GetProperty("initials").GetString(),
                        startData.GetProperty("comments").GetString(),
                        startData.GetProperty("predictionResultName").GetString(),
                        startData.GetProperty("blobCollectionName").GetString()
                    );
                    break;
                case "/api/stop":
                    _controlListener.Stop();
                    break;
                case "/api/flush":
                    _controlListener.Flush();
                    break;
                case "/api/finish":
                    _controlListener.Finish();
                    break;
                case "/api/set-bin-ids":
                    var mapping = JsonSerializer.Deserialize<Dictionary<string, string>>(body);
                    _controlListener.SetBinIds(mapping);
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
            }
        }
        catch (Exception)
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
        }

        response.Close();
    }

    private async Task SendResponse(HttpListenerResponse response, string content, string contentType)
    {
        var buffer = Encoding.UTF8.GetBytes(content);
        response.ContentType = contentType;
        response.ContentLength64 = buffer.Length;
        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        response.Close();
    }

    private string GetHtml()
    {
        return @"<!DOCTYPE html>
<html>
<head>
    <title>SeedLab Control</title>
    <style>
        body { font-family: sans-serif; padding: 20px; max-width: 800px; margin: auto; background: #f0f2f5; }
        .card { background: white; padding: 20px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); margin-bottom: 20px; }
        .status-badge { display: inline-block; padding: 5px 15px; border-radius: 20px; font-weight: bold; }
        .error { background: #fee2e2; color: #dc2626; }
        .normal { background: #dcfce7; color: #166534; }
        button { padding: 10px 20px; border: none; border-radius: 4px; cursor: pointer; font-weight: bold; margin-right: 10px; }
        .btn-start { background: #22c55e; color: white; }
        .btn-stop { background: #ef4444; color: white; }
        .btn-action { background: #3b82f6; color: white; }
        input, select { padding: 8px; margin-bottom: 10px; width: 100%; box-sizing: border-box; }
        .grid { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; }
    </style>
</head>
<body>
    <h1>SeedLab Web Control</h1>
    
    <div class='card'>
        <h2>System Status</h2>
        <div id='status' class='status-badge normal'>Checking...</div>
        <div id='error-banner' style='display:none; color: red; margin-top: 10px; font-weight: bold;'>⚠️ SYSTEM ERROR BROADCASTED</div>
    </div>

    <div class='grid'>
        <div class='card'>
            <h2>Measurement Control</h2>
            <input id='recipe' placeholder='Recipe Name' value='DefaultRecipe'>
            <button class='btn-action' onclick='post(""/api/load-recipe"", {recipe: document.getElementById(""recipe"").value})'>Load Recipe</button>
            <hr>
            <input id='sampleId' placeholder='Sample ID'>
            <input id='initials' placeholder='Initials'>
            <input id='comments' placeholder='Comments'>
            <input id='resultName' placeholder='Result Name'>
            <input id='blobName' placeholder='Blob Collection Name'>
            <button class='btn-start' onclick='start()'>START</button>
            <button class='btn-stop' onclick='post(""/api/stop"")'>STOP</button>
            <button class='btn-action' onclick='post(""/api/flush"")'>FLUSH</button>
            <button class='btn-action' onclick='post(""/api/finish"")'>FINISH</button>
        </div>

        <div class='card'>
            <h2>Bin Mappings</h2>
            <div id='bins'>
                <div style='display:flex; gap:5px; margin-bottom:5px;'>
                    <input class='bin-tag' placeholder='Tag (e.g. M96_1)' style='margin-bottom:0'>
                    <input class='bin-id' placeholder='ID' style='margin-bottom:0'>
                </div>
            </div>
            <button onclick='addBinRow()'>+ Add Row</button>
            <button class='btn-action' onclick='updateBins()'>UPDATE BINS</button>
        </div>
    </div>

    <script>
        async function post(url, data = {}) {
            await fetch(url, { method: 'POST', body: JSON.stringify(data) });
        }

        function start() {
            post('/api/start', {
                id: document.getElementById('sampleId').value,
                initials: document.getElementById('initials').value,
                comments: document.getElementById('comments').value,
                predictionResultName: document.getElementById('resultName').value,
                blobCollectionName: document.getElementById('blobName').value
            });
        }

        function addBinRow() {
            const div = document.createElement('div');
            div.style = 'display:flex; gap:5px; margin-bottom:5px;';
            div.innerHTML = `<input class='bin-tag' placeholder='Tag' style='margin-bottom:0'><input class='bin-id' placeholder='ID' style='margin-bottom:0'>`;
            document.getElementById('bins').appendChild(div);
        }

        function updateBins() {
            const mapping = {};
            const tags = document.querySelectorAll('.bin-tag');
            const ids = document.querySelectorAll('.bin-id');
            tags.forEach((t, i) => { if(t.value) mapping[t.value] = ids[i].value; });
            post('/api/set-bin-ids', mapping);
        }

        async function pollStatus() {
            try {
                const res = await fetch('/api/status');
                const data = await res.json();
                const badge = document.getElementById('status');
                badge.innerText = data.state;
                badge.className = 'status-badge ' + (data.hasError ? 'error' : 'normal');
                document.getElementById('error-banner').style.display = data.hasError ? 'block' : 'none';
            } catch {}
        }
        setInterval(pollStatus, 1000);
    </script>
</body>
</html>";
    }
}
