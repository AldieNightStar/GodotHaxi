# RPC Interpreter

## Notes
* Allows to write and call RPC call-string
* RPC call-string is a `|` separated string with commands
* Symbols `|` can be escaped by `\\` if needed
* Don't worry about using `|`. The `Call()` function will escape it automatically

## Usage
```cs
// Create new RPC Instance
// And provide few commands. Command is (List<string>) => {}
var rpc = new RPC()
    .WithCommand("a", args => GD.Print("A: " + args[0]))
    .WithCommand("b", args => GD.Print("B: " + args[0]));

// Prepare calls for the functions
r.Call("a", ["This is a good way to call RPC"]);
r.Call("b", ["Yep"]);
r.Call("getPid", []);
r.Call("respond", ["14284 OK"])

// Send to WClient or Websocket
// returns true when ok
r.Send(client);

// Execute call-string to run on your server-client
r.Execute(src);

// Get call-string to send via your client implementation
r.GetCallString();
```

## How `call-string` looks?
```
a This is a good way to call RPC|b Yep|getPid|respond 14284 OK
```

## How to use with `WClient`?
```cs
// Create RPC instance
var rpc = new RPC().WithCommand("a", (args) => ...);

// Make new Client
var client = new WClient("wss://echo.websocket.org")
    .OnMessageText(rpc.Execute); // Here you telling to use RPC

// Connect
client.Connect();

// Call something
rpc.Call("a", ["1", "2", "3"]);

// Then send
rpc.Send(client);
```