using System.Collections.Generic;
using Game;
using Godot;
using GodotHaxi;
using GodotHaxi.Net;

public partial class Basic : Node2D
{
    private WClient _client;

    public override void _Ready()
    {
        _client = new WClient("wss://echo-websocket.fly.dev");
        if (!_client.Connect()) GD.PushError("Can't connect");

        _client.OnMessageText(message => GD.Print("Message: " + message));
        _client.OnDisconnect((status, reason) => GD.Print($"Disconnect: {status} {reason}"));

        // Testing
        for (int i = 0; i < 32; i++) {
            _client.SendString("Hello from WClient: " + i);
        }
    }

    public override void _Process(double delta)
    {
        _client.Process();
    }

}
