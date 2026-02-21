using Godot;

namespace GodotHaxi;

public static class NodeExtensions
{
    public static HaxiNode<T> HaxiNode<T>(this T node)
        where T : Node
    {
        return new HaxiNode<T>(node);
    }
}