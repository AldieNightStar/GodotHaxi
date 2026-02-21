using System.Collections.Generic;
using Godot;

namespace GodotHaxi;

public partial class NodeSwitcher
{
    private Dictionary<string, Node2D> nodeDict;
    private Node2D parent;
    public string CurrentNodeName { get; private set; }
    public bool IsLocked { get; set; } = false;

    public NodeSwitcher(Node2D parent, params string[] nodeNames)
    {
        this.parent = parent;
        nodeDict = new Dictionary<string, Node2D>(nodeNames.Length);

        // Assign nodes into dict
        foreach (var nodeName in nodeNames)
        {
            nodeDict[nodeName] = parent.GetNode<Node2D>(nodeName);
        }
    }

    public NodeSwitcher WithButtons(params (string path, string name)[] buttons)
    {
        foreach ((var path, var name) in buttons)
        {
            var b = parent.GetNode<Button>(path);
            b.Pressed += () => onButtonPressed(name);
        }
        return this;
    }

    public bool Switch(string name)
    {
        // Locked will do nothing
        if (IsLocked) return false;

        // Exit when incorrect data is provided
        if (!nodeDict.ContainsKey(name)) return false;
        var node = nodeDict[name];

        // Update name of the current node
        CurrentNodeName = name;

        // Remove all nodes before
        foreach (var subNode in nodeDict.Values)
        {
            if (NodeUtil.IsChild(parent, subNode))
            {
                parent.RemoveChild(subNode);
            }
        }

        // Add only correct node
        if (!NodeUtil.IsChild(parent, node))
        {
            parent.AddChild(node);
            node.Visible = true;
        }

        return true;
    }

    private void onButtonPressed(string name)
    {
        if (IsLocked) return;
        Switch(name);
    }
}