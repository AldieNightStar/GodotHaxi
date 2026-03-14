using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;

namespace GodotHaxi.Net;

public class RPC
{
    private RPCCharReader _reader;
    private RPCCharWriter _writer;

    private Dictionary<string, Action<string>> _commands;

    public RPC()
    {
        _commands = new();
        _writer = new RPCCharWriter();
    }

    public RPC WithCommand(string name, Action<string> act)
    {
        _commands[name] = act;
        return this;
    }

    public void Execute(string src)
    {
        if (_reader == null) _reader = new RPCCharReader(src);
        while (!_reader.IsEnd())
        {
            var chunk = _reader.ReadUntilDivider();
            if (chunk.Length > 0) {
                var (command, arg) = _parseCommand(chunk);
                if (command.Length > 0) _emitCommand(command, arg);
            }
        }
    }

    public void Call(string name, string arg)
    {
        _writer.Call(name, arg);
    }

    public string GetCallString()
    {
        var src = _writer.AsString();
        _writer.Reset();
        return src;
    }

    private (string, string) _parseCommand(string src)
    {
        if (src.Contains(' '))
        {
            var arr = src.Split(' ', count: 2);
            return (arr[0], arr[1]);
        }
        return (src, "");
    }

    private void _emitCommand(string name, string arg)
    {
        if (_commands.ContainsKey(name))
        {
            var command = _commands[name];
            command(arg);
        }
        else
        {
            GD.PushError($"Can't emit command '{name}' because it's not set");
        }
    }
}

public class RPCCharWriter
{
    public static char DIVIDER = RPCCharReader.DIVIDER;

    private List<string> _list;

    public RPCCharWriter()
    {
        _list = new();
    }

    public void Call(string name, string arg)
    {
        arg = arg.Replace("\\", "\\\\").Replace($"{DIVIDER}", $"\\{DIVIDER}");
        _list.Add($"{name} {arg}");
    }

    public string AsString()
    {
        return string.Join('|', _list);
    }

    public void Reset()
    {
        _list.Clear();
    }
}

public class RPCCharReader
{
    public static char DIVIDER = '|';

    private string _src;
    private int _pos;

    public RPCCharReader(string src)
    {
        _src = src;
        _pos = 0;
    }

    public void Reset(string src)
    {
        _src = src;
        _pos = 0;
    }

    public bool IsEnd()
    {
        return _pos >= _src.Length;
    }

    public char ReadChar()
    {
        if (IsEnd()) return '\0';
        return _src[_pos++];
    }

    public string ReadUntilDivider()
    {
        StringBuilder sb = new StringBuilder();
        bool escaped = false;

        while (!IsEnd())
        {
            var ch = ReadChar();

            if (escaped)
            {
                sb.Append(ch);
                escaped = false;
                continue;
            }

            if (ch == '\\')
            {
                escaped = true;
                continue;
            }

            if (ch == DIVIDER) break;
            sb.Append(ch);
        }
        return sb.ToString();
    }
}