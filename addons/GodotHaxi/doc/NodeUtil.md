# Node Util


## Spawning
```cs
// Create new Node without spawning
var node = NodeUtil.Instance<T>("Ball");

// Spawn entity into a node with no position set
var node = NodeUtil.Spawn<T>(this, "Ball");

// Spawn "Ball" on the screen at (15, 25)
var node = NodeUtil.SpawnAt<T>(this, "Ball", new Vector2(15, 25));
```


## Singletons
```cs
// Get root node by name (Autoload or Singleton)
//   name - Name of the node
NodeUtil.GetRootNode<GlobalData>("G");
```


## Timers
```cs
// Wait 0.3 seconds
await NodeUtil.DoTimer(this, 0.3);
```


## Cleaning
```cs
// Clear all the nodes
//   node   - Node to clear
//   ignore - (Optional) Array of names to ignore
NodeUtil.ClearNode(node);
NodeUtil.ClearNode(node, ["IgnoreMe"]);
```


## Collection and Iteration
```cs
// Trying to get Child node. Returns null instead of throw
var node = NodeUtil.TryGet<T>(this, "node");

// Get children of type T
var list = NodeUtil.GetOfType<Node2D>(this);

// Get children of type T from specific group
var list = NodeUtil.GetOfTypeFromGroup<Node2D>("Group1");

// Loop through children of specific type
//   Returns count of elements looped
NodeUtil.ForEachType<PlayerView>(this, p => p.UpdatePlayerView());
```


# Checks
```cs
// Check that node is child of the owner (Not deep)
NodeUtil.IsChild(this, node);
```


## Processing Update
* Update Visibility and Processing
```cs
// Show only specific children inside node
// The rest will set Visibe=false
NodeUtil.VisibleOnly(node, ["Scene1"]);

// Set processing to ON or OFF
// Will not change visibility
NodeUtil.SetProcessing(node, true);

// Process/Show only specific children inside node
// The rest will not be processed or visible
NodeUtil.ProcessOnly(node, ["Scene1"]);
```


## Conversion
* Converts nodes from one type to another
```cs
// Wrap node into Control with minimum size
//   [!] Make sure thata `node` is not child of other node
var control = NodeUtil.WrapInControl(node, width, height);
```


## Controller
* Accepts Node value, and Data value that node listens to.
* Allows to control nodes and update them according to their data
* Good for table top games, etc
```cs
// Create
var controller = NodeUtil.Controller<Pawn, PawnData>(_pawns)
	.WithDataId(data => data.Id)
	.WithNodeId(node => node.ID)
	.WithSpawner(data => spawnPawn(data))
	.WithNodeUpdater((node, dat) => node.UpdatePawn(dat));

// Update controller according to Collection of Data
controller.Update(dataCollection)
```