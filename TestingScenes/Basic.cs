using System.Collections.Generic;
using Game;
using Godot;
using GodotHaxi;

public partial class Basic : Node2D
{
    public override void _Ready()
    {
        var s = NodeUtil.Sync<uint, TestNode, uint>(this)
            .WithDataId(i => i)
            .WithNodeId(n => n.Id)
            .WithSpawner("TestNode", (node, i) => node.Id = i)
            .WithDespawner(n => n.DeleteNode())
            .WithNodeUpdater((n, i) => n.UpdateTestNode(i, i));

        var t = CreateTween();

        _then(t, s, [1, 2, 3]);
        _then(t, s, []);
        _then(t, s, [5, 0]);
        _then(t, s, []);
    }

    private void _then(Tween t, NodeSync<uint, TestNode, uint> s, IEnumerable<uint> c)
    {
        t.TweenInterval(1);
        t.TweenCallback(Callable.From(() => s.UpdateAll(c)));
    }
}
