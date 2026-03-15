using Godot;
using GodotHaxi;
using GodotHaxi.Net;

public partial class Basic : Node2D
{
    public override void _Ready()
    {
        var rpc = new RPC().WithCommand("print", args => GD.Print(args["text"]));
        rpc.Call("print", new() {
            { "text", "Hello World" },
            { "id",   "abc123"      },
        });
        rpc.Call("print", new() {
            { "text", "Say Hello to me" },
            { "id",   "def987"          },
        });

        var str = rpc.GetCallString();
        GD.Print(str);
        rpc.Execute(str);
    }

}
