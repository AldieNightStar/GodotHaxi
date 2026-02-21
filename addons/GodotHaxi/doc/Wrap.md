# Wrapper

## Usage
```cs
// Turns variable into Godot Variant
// Alternative:
//     new Wrap(value)
var w = value.ToGodot();

// Unwrap value back
var value = w.Unwrap()
```

## In functions
* Just add `Wrap<T>` into your method arguments
```cs
void UpdatePlayer(Wrap<PlayerData> dat)
{
    var playerDat = dat.Unwrap();
    // ...
}
```