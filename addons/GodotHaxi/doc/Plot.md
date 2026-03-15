# Plot API

## Usage
```cs
var plot = new Plot().Build(p => {
    // Build labels
});

// Call current Step (Or start the plot)
plot.Step();

// Deferred call for step at idle frame
plot.StepDeferred();

// Call next step
plot.Next();

// Changes label and pos. Doesn't do a step by itself
plot.Goto("Start", 0);
plot.Goto("Start");

// Checks for label
plot.HasLabel("Start");

// Get current label and pos
// Can be saved and then restoted by `Goto()` function
// Returns: (string label, int pos)
plot.GetLabelPos();
```

## Building Labels
```cs
var plot = new Plot().Build(p => {
    // Set label that we are working on
    b.Label("Start");
    b.Act(myAction("Aldie"));
    b.Jump("Label2");

    b.Label("Label2");
    b.Act(waitForClick());
    b.Jump("Start");
});
```
* Custom actions can be created like that:
```cs
// Just a method that returns Action<Plot>

public Action<Plot> myAction(string name) => p =>
{
    GD.Print("Hello, " + name);
    p.Next();
}

public Action<Plot> waitForClick() => p =>
{
    // Do other stuff that calls p.Next()
}
```
