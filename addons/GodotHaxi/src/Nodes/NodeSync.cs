using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace GodotHaxi;

public class NodeSync<NODE, DAT> where NODE : Node
{
    private Node _root;
    private Func<DAT, NODE> _nodeSpawner;
    private Func<NODE, uint> _nodeIdGetter;
    private Func<DAT, uint> _dataIdGetter;
    private Action<NODE, DAT> _nodeUpdater;
    private Action<NODE> _nodeDespawner;

    public NodeSync(Node rootNode)
    {
        _root = rootNode;
    }

    public NodeSync<NODE, DAT> WithSpawner(Func<DAT, NODE> spawner)
    {
        _nodeSpawner = spawner;
        return this;
    }
    
    public NodeSync<NODE, DAT> WithSpawner(string templateName, Action <NODE, DAT> spawner)
    {
        _nodeSpawner = (dat) =>
        {
            var node = NodeUtil.Spawn<NODE>(_root, templateName);
            spawner(node, dat);
            return node;
        };
        return this;
    }

    public NodeSync<NODE, DAT> WithNodeId(Func<NODE, uint> getter)
    {
        _nodeIdGetter = getter;
        return this;
    }

    public NodeSync<NODE, DAT> WithDataId(Func<DAT, uint> idGetter)
    {
        _dataIdGetter = idGetter;
        return this;
    }

    public NodeSync<NODE, DAT> WithNodeUpdater(Action<NODE, DAT> updater)
    {
        _nodeUpdater = updater;
        return this;
    }

    public NodeSync<NODE, DAT> WithDespawner(Action<NODE> despawner)
    {
        _nodeDespawner = despawner;
        return this;
    }

    public void UpdateExisting(IEnumerable<DAT> collection)
    {
        if (!_isRequiredSatisfied()) return;
        var dataDict = CollectionUtil.Assoc(collection, dat => _dataIdGetter(dat));
        var nodeDict = CollectionUtil.Assoc(_root.GetChildren().OfType<NODE>(), node => _nodeIdGetter(node));

        foreach (var (id, node) in nodeDict)
        {
            if (dataDict.ContainsKey(id)) _nodeUpdater(node, dataDict[id]);
        }
    }

    public void DespawnExisting(IEnumerable<uint> ids, bool fast = true)
    {
        if (!_isRequiredSatisfied()) return;

        // Assoc nodes by their ids
        var nodeDict = CollectionUtil.Assoc(NodeUtil.GetOfType<NODE>(_root), node => _nodeIdGetter(node));

        foreach (uint id in ids)
        {
            if (nodeDict.ContainsKey(id))
            {
                if (fast) nodeDict[id].QueueFree(); else _nodeDespawner(nodeDict[id]);
            }
        }
    }

    public void UpdateAll(IEnumerable<DAT> collection)
    {
        if (!_isRequiredSatisfied()) return;
        var dataDict = CollectionUtil.Assoc(collection, dat => _dataIdGetter(dat));
        var nodeDict = CollectionUtil.Assoc(_root.GetChildren().OfType<NODE>(), node => _nodeIdGetter(node));

        // Get what to spawn
        foreach (var (id, node) in nodeDict)
        {
            if (dataDict.ContainsKey(id))
            {
                _nodeUpdater(node, dataDict[id]);
                dataDict.Remove(id);
            }
            else
            { // Has no data
                _nodeDespawner(node);
            }
        }
        
        // Spawn remaining ids
        foreach (var (id, data) in dataDict)
        {
            var node = _nodeSpawner(data);
            if (node.GetParent() == null) _root.AddChild(node);
            _callWhenReady(node, () => _nodeUpdater(node, data));
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
        if (_nodeSpawner == null)
        {
            GD.PushError("Node spawner should not be NULL");
            satisfied = false;
        }
        if (_nodeDespawner == null)
        {
            GD.PushError("Node Despawner should not be NULL");
            satisfied = false;
        }
        return satisfied;
    }
}