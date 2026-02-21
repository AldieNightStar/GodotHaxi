using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace GodotHaxi;

public struct HaxiNode<NODE> where NODE : Node
{
    public NODE Node { get; private set; }

    public HaxiNode(NODE node)
    {
        Node = node;
    }

    public readonly T Spawn<T>(string name) where T : Node => NodeUtil.Spawn<T>(Node, name);
    public readonly T SpawnAt<T>(string name, Vector2 gpos) where T : Node2D => NodeUtil.SpawnAt<T>(Node, name, gpos);

    // Timers
    public readonly async Task Wait(double time) => await NodeUtil.DoTimer(Node, time);

    // Controller
    public readonly NodeController<NODE, T> Controller<T>() => NodeUtil.Controller<NODE, T>(Node);

    // UI
    public readonly Control WrapInControlNode(int w, int h)
    {
        if (Node is CanvasItem ci) return NodeUtil.WrapInControl(ci, w, h);
        return null;
    }

    // Processing
    public readonly void SetProcessing(bool b) => NodeUtil.SetProcessing(Node, b);
    public readonly void ProcessOnly(string[] names) => NodeUtil.ProcessOnly(Node, names);
    public readonly void SetVisible(bool b)
    {
        if (Node is CanvasItem ci) ci.Visible = b;
    }

    // Iteration
    public readonly IEnumerable<HaxiNode<T>> EachOfType<T>() where T: Node => NodeUtil.GetOfType<T>(Node).Select(n => n.HaxiNode());
}