using Godot;
using System;

namespace Game;

public partial class TestNode : Sprite2D
{
    public uint Id { get; set; } = 0;

    public TestNode WithId(uint id)
    {
        Id = id;
        return this;
    }

    public void UpdateTestNode(uint x, uint y)
    {
        Position = new Vector2(x, y) * 100;
    }

    public void DeleteNode()
    {
        GD.Print($"I am deleting. ID: {Id}");
        QueueFree();
    }
}
