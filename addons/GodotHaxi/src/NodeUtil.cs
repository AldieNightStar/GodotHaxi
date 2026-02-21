using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace GodotHaxi;

public class NodeUtil
{
    // Path setting
    private static string PATH_TEMPLATE = "res://Objects/$$/$$.tscn";

    public static T Instance<T>(string templateName) where T : Node
    {
        var scene = GD.Load<PackedScene>(PATH_TEMPLATE.Replace("$$", templateName));
        if (scene == null) return null;

        var node = scene.Instantiate<T>();
        return node;
    }

    public static T Spawn<T>(Node into, string templateName) where T : Node
    {
        var node = Instance<T>(templateName);
        into.AddChild(node);
        return node;
    }

    public static T SpawnAt<T>(Node into, string templateName, Vector2 gPos) where T : Node2D
    {
        var node = Spawn<T>(into, templateName);
        if (node != null)
        {
            node.GlobalPosition = gPos;
        }
        return node;
    }

    public static bool IsChild(Node owner, Node child)
    {
        if (owner == null || child == null) return false;
        if (!GodotObject.IsInstanceValid(owner)) return false;
        if (!GodotObject.IsInstanceValid(child)) return false;
        return child.GetParent() == owner;
    }

    public static T TryGet<T>(Node owner, string name) where T : Node
    {
        if (!owner.HasNode(name)) return null;
        return owner.GetNode(name) as T;
    }

    public static async Task DoTimer(Node node, double sec)
    {
        await node.ToSignal(node.GetTree().CreateTimer(sec), Timer.SignalName.Timeout);
    }

    public static void ClearNodes(Node node, string[] ignores = null)
    {
        foreach (var child in node.GetChildren())
        {
            var name = child.Name.ToString();
            if (ignores != null && ignores.Contains(name)) continue;
            child.QueueFree();
        }
    }

    private static SceneTree _SCENE_TREE;

    private static SceneTree getSceneTree()
    {
        if (_SCENE_TREE == null) _SCENE_TREE = Engine.GetMainLoop() as SceneTree;
        return _SCENE_TREE;
    }

    public static T GetRootNode<T>(string name)
    {
        var node = getSceneTree().Root.GetNode("/root/" + name);
        if (node is T typedNode)
        {
            return typedNode;
        }
        else
        {
            return default;
        }
    }

    public static void VisibleOnly(Node node, string[] names)
    {
        var items = node.GetChildren()
            .Where(ch => ch is CanvasItem)
            .Select(ch => ch as CanvasItem);
        foreach (var item in items)
        {
            var flag = names.Contains<string>(item.Name);
            item.Visible = flag;
        }
    }

    public static void SetProcessing(CanvasItem item, bool flag)
    {
        var PROC = Node.ProcessModeEnum.Inherit;
        var IGNORE = Node.ProcessModeEnum.Disabled;

        item.SetProcess(flag);
        item.SetProcessInput(flag);
        item.SetProcessInternal(flag);
        item.SetProcessUnhandledInput(flag);
        item.SetProcessUnhandledKeyInput(flag);
        item.SetPhysicsProcess(flag);
        item.SetPhysicsProcessInternal(flag);
        item.ProcessMode = flag ? PROC : IGNORE;
    }

    public static void ProcessOnly(Node node, string[] names)
    {
        var items = node.GetChildren()
            .Where(ch => ch is CanvasItem)
            .Select(ch => ch as CanvasItem);

        foreach (var item in items)
        {
            var flag = names.Contains<string>(item.Name);
            item.Visible = flag;
            SetProcessing(item, flag);
        }
    }

    public static IEnumerable<T> GetOfType<T>(Node node) where T : Node
    {
        return node.GetChildren()
            .Where(c => c is T)
            .Select(c => c as T);
    }

    public static int ForEachType<T>(Node node, Action<T> act) where T : Node
    {
        var count = 0;
        var nodes = GetOfType<T>(node);
        foreach (var child in nodes)
        {
            act(child);
            count += 1;
        }
        return count;
    }

    public static IEnumerable<T> GetOfTypeFromGroup<T>(string group) where T : Node
    {
        var tree = getSceneTree();
        var list = tree.GetNodesInGroup(group);
        return list
            .Where(e => e is T)
            .Select(e => e as T);
    }

    public static Control WrapInControl(CanvasItem c, int width, int height)
    {
        Control control = new Control();
        control.CustomMinimumSize = new Vector2(width, height);
        control.AddChild(c);
        return control;
    }

    public static NodeController<NODE, DAT> Controller<NODE, DAT>(Node root)
        where NODE : Node
    {
        return new NodeController<NODE, DAT>(root);
    }
}

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

        var dict = collection
            .GroupBy(dat => _dataIdGetter(dat))
            .ToDictionary(g => g.Key, g => g.FirstOrDefault());

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