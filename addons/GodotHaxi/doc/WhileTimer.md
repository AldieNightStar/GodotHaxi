# While Timer

## Notes
* Constant run some action while __while-condition__ is `true`

## Usage
```cs
// Create timer
//    node      - Node to use for timer
//    interval  - Interval in seconds
//    whileFunc - Function that returns true/false
//    action    - Action that called when whileFunc is true
var t = new WhileTimer(this, 0.5,
	() => count < 10,
	() => count += 1);

// Start the timer
t.Start();

// Stop the timer
t.Stop();

// Check the timer is running
t.IsStarted();
```