# Websocket client

# Notes
* Allows to use Websocket in more easy way
* Supports binary and text formats

# Usage
```cs
// Create client
var client = new WClient("wss://echo-websocket.fly.dev", binary:false);

// Connect the client. Will return false if failed
client.Connect();

// Callbacks
client.OnMessageText(messageString => ...);
client.OnMessageBin(messageBytes => ...);
client.OnDisconnect((status, reason) => ...);

// Send messages
client.SendBin([49, 50, 51]);
client.SendString("Hello!");

// Check if closed
client.IsClosed;

// Check if connected
client.IsConnected;

// Process each move
// Need to be in _Process() method
client.Process();
```