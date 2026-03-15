using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace GodotHaxi.Net;

public class RPC
{
    private const char DIVIDER = '|';

    private RPCCharWriter _writer;

    private Dictionary<string, Action<Dictionary<string, string>>> _commands;

    public RPC()
    {
        _commands = new();
        _writer = new RPCCharWriter(DIVIDER);
    }

    public RPC WithCommand(string name, Action<Dictionary<string, string>> act)
    {
        _commands[name] = act;
        return this;
    }

    public void Execute(string src)
    {
        var divided = StringUtil.ParseDivided(DIVIDER, src);
        foreach (var chunk in divided)
        {
            if (chunk.Length < 1) continue;
            var (command, args) = _parseCommand(chunk);
            if (command.Length > 0) _emitCommand(command, args);
        }
    }

    public void Call(string name, Dictionary<string, string> args)
    {
        _writer.Call(name, args);
    }

    public string GetCallString()
    {
        var src = _writer.AsString();
        _writer.Reset();
        return src;
    }

    public bool Send(WClient client)
    {
        client.SendString(GetCallString());
        return false;
    }

    public bool Send(WebSocketPeer socket)
    {
        if (socket.GetReadyState() == WebSocketPeer.State.Open)
        {
            socket.SendText(GetCallString());
            return true;
        }
        return false;
    }

    private (string, Dictionary<string, string>) _parseCommand(string src)
    {
        if (src.Contains(' '))
        {
            var arr = src.Split(' ', count: 2);
            var args = StringUtil.ParseColonArguments(arr[1]);
            return (arr[0], args);
        }
        return (src, []);
    }

    private bool _emitCommand(string name, Dictionary<string, string> args)
    {
        if (_commands.ContainsKey(name))
        {
            var command = _commands[name];
            command(args);
            return true;
        }
        else
        {
            GD.PushError($"Can't emit command '{name}', because it's not set");
            return false;
        }
    }
}

public class RPCCharWriter
{
    private List<string> _list;
    private char _divider;

    public RPCCharWriter(char divider)
    {
        _list = new();
        _divider = divider;
    }

    public void Call(string name, Dictionary<string, string> args)
    {
        if (name.Contains(' '))
        {
            GD.PushError("Can't call name with space");
            return;
        }

        // Sanitize arguments
        var sanitizedName = _sanitize(name);
        var fullArgs = _sanitize(StringUtil.GetColonArguments(args));

        if (fullArgs.Length > 0)
            _list.Add($"{sanitizedName} {fullArgs}");
        else
            _list.Add(sanitizedName);
    }

    public string AsString()
    {
        return string.Join(_divider, _list);
    }

    public void Reset()
    {
        _list.Clear();
    }

    private string _sanitize(string src)
    {
        return StringUtil.Escape(src)
            .Replace($"{_divider}", $"\\{_divider}");
    }
}