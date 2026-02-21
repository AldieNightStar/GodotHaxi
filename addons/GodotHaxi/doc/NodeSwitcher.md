# Node Switcher

## Notes
* Allows to change menus in game
* Only single Menu able to be active. Others will be inactive and invisible

## Usage
```cs
// Create switcher and assign few buttons
// Accepts list of Node2D names that is in `this` Node2D
nodeSwitcher = new NodeSwitcher(this,
	"Menu1",
	"Menu2",
	"Menu3",
)

// Add Signals to Buttons
//   - Path is relative to the node it operates on
//
// Format:
//   ("Path to the button", "Menu name")
//
nodeSwitcher.WithButtons(
	("MyButtons/MenuButton1", "Menu1"),
	("MyButtons/MenuButton2", "Menu2"),
	("MyButtons/MenuButton3", "Menu3")
);

// Switch
nodeSwitcher.Switch("Menu1");

// Lock / Unlock
nodeSwitcher.IsLocked = true;

// Add signal to buttons
nodeSwitcher.WithButtons(
	("path/button", "Menu1")
)
```