using System;
using Godot;
using GodotHaxi;

public partial class Basic : Node2D
{

    public override void _Ready()
    {
        var plot = new Plot().Build(b =>
        {
            b.Label("Start");
            b.Act(_prints("Hello!"));
            b.Act(_prints("Hi!"));
            b.Act(_prints("HoeBin!"));
            b.Act(_waits(1));
            b.Act(_prints("Fin!"));
            b.Act(_waits(1));
            b.Jump("Start");
        });

        plot.Step();
    }

    private Action<Plot> _prints(string text) => p =>
    {
        GD.Print(text);
        p.Next();
    };

    private Action<Plot> _waits(double seconds) => p =>
    {
        var t = CreateTween();
        t.TweenInterval(seconds);
        t.TweenCallback(Callable.From(p.Next));
    };

}
