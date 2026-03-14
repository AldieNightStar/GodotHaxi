using Godot;
using GodotHaxi.Net;

public partial class Basic : Node2D
{
    private WClient _client;

    public override void _Ready()
    {
        var r = new RPC()
            .WithCommand("a", (args) => GD.Print(string.Join(',', args)));

        r.Call("a", ["1", "2", "3"]);

        _client = new WClient("wss://echo.websocket.org")
            .OnMessageText(r.Execute);
        _client.Connect();

        r.Send(_client);
    }

    public override void _Process(double delta)
    {
        _client.Process();
    }


}
