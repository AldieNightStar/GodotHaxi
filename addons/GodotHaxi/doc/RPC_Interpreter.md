# RPC Interpreter

## Notes
* Allows to write and call RPC call-string
* RPC call-string is a `|` separated string with commands
* Symbols `|` can be escaped by `\\` if needed

## Usage
```cs
// Create new RPC Instance
// And provide few commands. Command is (string) => {}
var rpc = new RPC()
    .WithCommand("a", arg => GD.Print("A: " + s))
    .WithCommand("b", arg => GD.Print("B: " + s));

// Prepare calls for the functions
r.Call("a", "This is a good way to call RPC");
r.Call("b", "Yep");
r.Call("getPid", "");
r.Call("respond", "14284 OK")

// Get call-string to send via your client implementation
r.GetCallString();

// Execute call-string to run on your server-client
r.Execute(src);
```

## How `call-string` looks?
```
a This is a good way to call RPC|b Yep|getPid |respond 14284 OK
```