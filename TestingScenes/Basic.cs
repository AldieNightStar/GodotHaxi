using Godot;
using GodotHaxi.Net;

public partial class Basic : Node2D
{
    public override void _Ready()
    {
        var r = new RPC()
            .WithCommand("a", s => GD.Print("A: " + s))
            .WithCommand("b", s => GD.Print("B: " + s));
        
        r.Call("a", "The |fucking| way to sell the |price|");
        r.Call("b", "The \\\\\\Hell||| no");
        
        var src = r.GetCallString();
        
        r.Execute(src);
    }

}
