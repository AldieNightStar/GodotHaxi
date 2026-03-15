# RPC Interpreter

## Notes
* Allows to write and call RPC call-string
* RPC call-string is a `|` separated string with commands

## Usage
```cs
// Create new RPC Instance
// And provide few commands. Command is (List<string>) => {}
var rpc = new RPC()
    .WithCommand("print", a => GD.Print("Text: " + a["text"]))
    .WithCommand("other", a => GD.Print("Respond: " + a["id"]));

// Prepare calls for the functions
rpc.Call("print", new() {
    { "text", "Hello World" },
    { "id",   "abc123"      },
});
rpc.Call("print", new() {
    { "text", "Say Hello to me" },
    { "id",   "def987"          },
});

// Send to WClient or Websocket
// returns true when ok
rpc.Send(client);

// Execute call-string on current machine
rpc.Execute(src);

// Get call-string to Execute(...) later
// Sample:
//   print text:Hello World;id:abc123|print text:Say Hello to me;id:def987
rpc.GetCallString();
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
rpc.Call("a", new() { {"text", "Hello"} });

// Then send to the client
rpc.Send(client);
```