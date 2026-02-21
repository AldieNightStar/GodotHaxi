using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace GodotHaxi;

public class NodeController<NODE, DAT> where NODE : Node
{
    private Node _root;
    private Func<DAT, NODE> _spawner;
    private Func<NODE, uint> _nodeIdGetter;
    private Func<DAT, uint> _dataIdGetter;
    private Action<NODE, DAT> _nodeUpdater;

    public NodeController(Node rootNode)
    {
        _root = rootNode;
    }

    public NodeController<NODE, DAT> WithSpawner(Func<DAT, NODE> spawner)
    {
        _spawner = spawner;
        return this;
    }

    public NodeController<NODE, DAT> WithSpawner(string name)
    {
        _spawner = (_) => NodeUtil.Spawn<NODE>(_root, name);
        return this;
    }

    public NodeController<NODE, DAT> WithNodeId(Func<NODE, uint> idGetter)
    {
        _nodeIdGetter = idGetter;
        return this;
    }

    public NodeController<NODE, DAT> WithDataId(Func<DAT, uint> idGetter)
    {
        _dataIdGetter = idGetter;
        return this;
    }

    public NodeController<NODE, DAT> WithNodeUpdater(Action<NODE, DAT> updater)
    {
        _nodeUpdater = updater;
        return this;
    }

    public void Update(IEnumerable<DAT> collection)
    {
        if (!_isRequiredSatisfied()) return;        
        var dict = CollectionUtil.Assoc(collection, dat => _dataIdGetter(dat));

        foreach (var child in _root.GetChildren())
        {
            if (child is NODE node)
            {
                var id = _nodeIdGetter(node);
                var data = dict.GetValueOrDefault(id);
                if (data != null)
                {
                    // Has data
                    _nodeUpdater(node, data);

                    // Remove from dict for the remaining to respawn
                    dict.Remove(id);
                }
                else
                {
                    // Has no data
                    node.QueueFree();
                }
            }
        }

        // Spawn remaining
        if (_spawner != null)
        {
            foreach (var data in dict.Values)
            {
                var node = _spawner(data);
                if (node.GetParent() == null) _root.AddChild(node);
                _callWhenReady(node, () => _nodeUpdater(node, data));
            }
        }
    }

    private void _callWhenReady(Node node, Action act)
    {
        if (node.IsNodeReady()) Callable.From(act).CallDeferred();
        else node.Ready += act;
    }

    private bool _isRequiredSatisfied()
    {
        bool satisfied = true;
        if (_dataIdGetter == null || _nodeIdGetter == null)
        {
            GD.PushError("ID Getters for [node / data] should not be NULL");
            satisfied = false;
        }
        if (_nodeUpdater == null)
        {
            GD.PushError("Node updater should not be NULL");
            satisfied = false;
        }
        if (_spawner == null)
        {
            GD.PushError("Node spawner should not be NULL");
            satisfied = false;
        }
        return satisfied;
    }
}