using VM.Lab.Interfaces.BlobAnalyzer;
using VM.Lab.Plugins.WebControl;

namespace VM.Lab.Plugins.WebControl.Runner;

class ConsoleListener : ISeedLabControlListener
{
    public void LoadRecipe(string recipeName) => Console.WriteLine($"[HARDWARE] LoadRecipe: {recipeName}");

    public void Start(string id, string initials, string comments, string predictionResultName, string blobCollectionName)
        => Console.WriteLine($"[HARDWARE] Start: ID='{id}', Initials='{initials}', Comments='{comments}', ResultName='{predictionResultName}', BlobName='{blobCollectionName}'");

    public void PrepareForStart(string id, string initials, string comments, string predictionResultName, string blobCollectionName)
        => Console.WriteLine($"[HARDWARE] PrepareForStart: ID='{id}'");

    public void Stop() => Console.WriteLine("[HARDWARE] Stop called");
    public void Flush() => Console.WriteLine("[HARDWARE] Flush called");
    public void Finish() => Console.WriteLine("[HARDWARE] Finish called");

    public void SetBinIds(Dictionary<string, string> mapping)
    {
        Console.WriteLine("[HARDWARE] SetBinIds called with mapping:");
        foreach (var pair in mapping) Console.WriteLine($"  - {pair.Key} -> {pair.Value}");
    }
}

class Program
{
    static void Main(string[] args)
    {
        var listener = new ConsoleListener();
        using var plugin = new WebSeedLabControl(listener, 8080);

        Console.WriteLine("====================================================");
        Console.WriteLine("   SEEDLAB WEB CONTROL RUNNER (Simulation Mode)     ");
        Console.WriteLine("====================================================");
        Console.WriteLine("1. Open your browser and go to: http://localhost:8080/");
        Console.WriteLine("2. Interact with the Web Interface.");
        Console.WriteLine("3. Check this console for 'HARDWARE' logs.");
        Console.WriteLine("");
        Console.WriteLine("Options:");
        Console.WriteLine("- Type 'error' to broadcast an error to the UI.");
        Console.WriteLine("- Type 'state <STATENAME>' to change the status display.");
        Console.WriteLine("- Type 'exit' to stop the server.");
        Console.WriteLine("");

        while (true)
        {
            var input = Console.ReadLine()?.ToLower();
            if (input == "exit") break;
            if (input == "error") {
                plugin.BroadcastError();
                Console.WriteLine("[RUNNER] Broadcasted error state.");
            }
            if (input?.StartsWith("state ") == true) {
                var stateName = input.Substring(6).ToUpper();
                if (Enum.TryParse<BlobAnalyzerState>(stateName, out var newState)) {
                    plugin.StateChanged(newState);
                    Console.WriteLine($"[RUNNER] State changed to: {newState}");
                } else {
                    Console.WriteLine($"[RUNNER] Invalid state: {stateName}");
                }
            }
        }
    }
}
