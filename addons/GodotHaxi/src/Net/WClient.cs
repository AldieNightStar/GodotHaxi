using System;
using System.Collections.Generic;
using System.Text;
using Godot;

namespace GodotHaxi.Net;

public class WClient
{
    const int RETRY_COUNT = 8;

    private string _url;
    private WebSocketPeer _socket;
    private bool _isBin;
    private bool _wasClosed;

    private Action<int, string> _onDisconnect;
    private Action<string> _onMessageText;
    private Action<byte[]> _onMessageBin;

    private Queue<string> _messageTextQueue = new();
    private Queue<byte[]> _messageBinQueue = new();

    public WClient(string url, bool binary = false)
    {
        _url = url;
        _isBin = binary;
        _wasClosed = false;
        _socket = new WebSocketPeer();
    }

    public WClient OnDisconnect(Action<int, string> act)
    {
        _onDisconnect = act;
        return this;
    }

    public WClient OnMessageText(Action<string> act)
    {
        _onMessageText = act;
        return this;
    }

    public WClient OnMessageBin(Action<byte[]> act)
    {
        _onMessageBin = act;
        return this;
    }

    public bool IsClosed => _wasClosed || _isClosed(_state());

    public bool Connect()
    {
        var err = _socket.ConnectToUrl(_url);
        if (err != Error.Ok)
        {
            GD.PushError($"Can't connect from WClient: {_url}");
            _wasClosed = true;
            return false;
        }
        _wasClosed = false;
        return true;
    }

    public void Process()
    {
        if (_wasClosed) return;

        _socket.Poll();
        var state = _state();

        if (_isConnected(state))
        {
            _sendQueuedMessage();
            _emitPackets(_receivePackets());
        }
        else if (_isClosed(state))
        {
            _emitClosed();
        }
    }

    public void SendString(string message)
    {
        if (_wasClosed) return;
        _messageTextQueue.Enqueue(message);
    }

    public void SendBin(byte[] data)
    {
        if (_wasClosed) return;
        _messageBinQueue.Enqueue(data);
    }

    public void Close()
    {
        if (_wasClosed) return;

        _wasClosed = true;
        _socket.Close();
    }

    private void _sendQueuedMessage()
    {
        if (_messageBinQueue.Count > 0) {
            var message = _messageBinQueue.Dequeue();
            _socket.Send(message);
        }

        if (_messageTextQueue.Count > 0) {
            var message = _messageTextQueue.Dequeue();
            _socket.SendText(message);
        }
    }

    private List<byte[]> _receivePackets()
    {
        List<byte[]> messages = [];
        while (_socket.GetAvailablePacketCount() > 0)
        {
            byte[] data = _socket.GetPacket();
            messages.Add(data);
        }
        return messages;
    }

    private void _emitClosed()
    {
        int code = _socket.GetCloseCode();
        string reason = _socket.GetCloseReason();
        if (_onDisconnect != null) _onDisconnect(code, reason);
        _wasClosed = true;
    }

    private void _emitPackets(List<byte[]> packets)
    {
        foreach (var packet in packets)
        {
            _emitPacket(packet);
        }
    }

    private void _emitPacket(byte[] data)
    {
        if (_isBin)
        {
            if (_onMessageBin != null) _onMessageBin(data);
        }
        else
        {
            if (_onMessageText != null) _onMessageText(_fromBin(data));
        }
    }

    private WebSocketPeer.State _state() => _socket.GetReadyState();
    private bool _isConnected(WebSocketPeer.State s) => s == WebSocketPeer.State.Open;
    private bool _isClosed(WebSocketPeer.State s) => s == WebSocketPeer.State.Closed;
    private string _fromBin(byte[] data) => Encoding.UTF8.GetString(data);
}