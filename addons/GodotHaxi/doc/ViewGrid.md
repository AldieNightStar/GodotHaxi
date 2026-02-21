# View Grid

## Notes
* Allows to create 2D tile grid
* Like TileMap but better because you can add animations and stuff

## Usage
```cs
// Create
//   w, h    - Size of grid
//   tx, ty  - Tile size in pixels. If sprite is scaled, please multiple the size as well
//   center  - Is Node is centered (If no, it will center manualy)
var g = new ViewGrid<Sprite2D>(w, h, tx, ty, center);

// Get sprite at specific position
// Can return null if not found anything or out of bounds
g.Get(x, y);

// Set new sprite node to the grid
// (!!) Note: Element you set, should be NEWLY CREATED
// Returns false - if out of bounds or operation is failed for some reason
g.Set(x, y, sprite);

// Test that tile is in bounds of supported area
g.IsBound(x, y);

// Tests that sprite is at x, y position
g.Has(x, y);

// If there are some sprites, it will be removed
// Returns false when something was wrong
g.Remove(x, y);

// Clears nodes that already had deallocated
g.ClearFreed();

// Move sprite from position1 to position2
// It will remove sprite at second position
// Returns true if success
g.Move(x1, y1, x2, y2);

// Swaps two sprites (Works also when other position has no sprite)
// Returns true if success
g.Swap(x1, y1, x2, y2);

// Converts tile position into real position (Godot position)
g.GetRealPosition(x, y);

// Converts real position (Godot position) back into tile position
g.GetTilePosition(x, y);
```