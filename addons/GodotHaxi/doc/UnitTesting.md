# Unit Testing

## Notes
* Allows to write Unit Tests inside Godot
* Creates new Scene and adds a buttons in there

## Usage
* Do not `override _Ready()` as it contains starter logic
* Use `[Init]` methods to prepare test data before each test
* Do not use `static` methods with `[Test]` they will not work
* `[Test]` Methods must be `public`
* Also you can assign `Container`,  then it will display test buttons
```cs
public partial class MyTest : TestNode
{
    [Init]
    public void Before()
    {
        GD.Print("I am called before each test");
    }

    [Test]
    public void Test1(UnitTest t)
    {
        t.Eq(3+3, 6);
        t.Eq(2+2, 4, "2+2 must be 4");
    }
    
    // You can also specify group for tests (UI Only)
    // Groups allows to run multiple test that are in the same group
    [Test(group:"MyGroup")]
    public void Test2(UnitTest t)
    {
        t.Eq(2 * 8, 16);
    }

    [Test(trace:true)] // trace - shows StackTrace if details required
    public void TestThrow(UnitTest t)
    {
        GD.Print("Test2");
        t.Throws<KeyNotFoundException>(() => throw new KeyNotFoundException(), "Must throw exception");
    }
}
```

## Test methods
```cs
// t - is UnitTest

// Test for equality
t.Eq(a, b, message=null);
t.NotEq(a, b, message=null);

// Test boolean
t.True(value, message=null);
t.False(value, message=null);

// Test Null
t.Null(value, message=null);
t.NotNull(value, message=null);

// Test throwing
// Returns exception that it thrown
t.Throws<Exception>(() => { ... }, message=null);

// Fail the test before it ends
t.Fail(message=null);
```