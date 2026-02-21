using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GodotHaxi.UnitTest;

public class UnitTest
{
    public string TestName { get; }
    private List<object[]> _log = new List<object[]>();

    public UnitTest(string testName)
    {
        TestName = testName;
    }

    public static void LogOk(string testName, string message)
    {
        GD.Print($"✅{testName} :: ", message);
    }

    public static void LogErr(string testName, string message)
    {
        GD.Print($"⛔{testName} :: ", message);
    }

    public static void LogLine()
    {
        GD.Print("--------------------------------------------------");
    }

    public void Eq<T>(T a, T b, string message = null)
    {
        if (!a.Equals(b)) throw new TestFailException(message ?? "Test failed");
    }

    public void NotEq<T>(T a, T b, string message = null)
    {
        if (a.Equals(b)) throw new TestFailException(message ?? "Test failed");
    }

    public void True(bool a, string message = null)
    {
        if (!a) throw new TestFailException(message ?? "Test failed");
    }

    public void False(bool a, string message = null)
    {
        if (a) throw new TestFailException(message ?? "Test failed");
    }

    public void Null(object a, string message = null)
    {
        if (a != null) throw new TestFailException(message ?? "Test failed");
    }

    public void NotNull(object a, string message = null)
    {
        if (a == null) throw new TestFailException(message ?? "Test failed");
    }

    public void Fail(string message = null)
    {
        throw new TestFailException(message ?? "Test failed");
    }

    public T Throws<T>(Action action, string message = null) where T : Exception
    {
        T foundEx = null;
        try
        {
            action();
        }
        catch (T ex)
        {
            foundEx = ex;
        }
        if (foundEx == null) throw new TestFailException(message ?? "Test failed");
        return foundEx;
    }

    public void Log(params object[] values)
    {
        _log.Add(values);
    }

    public static (int ok, int err) RunTests(object value, string exact = null, string group = null)
    {
        // Get type
        var type = value.GetType();

        // Counts
        int ok = 0;
        int err = 0;
        int all = 0;

        // Check that group is used to run the tests
        var tests = GetTests(value);
        if (group != null)
        {
            tests = tests.Where(x => x.Test.Group == group);
        }

        // Get exact test if exact is specified
        // Makes run only single test
        if (exact != null)
        {
            tests = tests.Where(x => x.Method.Name == exact);
        }

        // Add vertical space
        GD.Print("");
        GD.Print("");
        GD.Print("");

        // Run tests
        foreach (var (test, method) in tests)
        {
            // Run Init methods first
            _runInits(value);

            // Increment total test count
            all++;

            // Create Unit Test object
            var unitTest = new UnitTest(method.Name);

            try
            {
                method.Invoke(value, [unitTest]);
                _showResult(unitTest, null, test.ShowTrace);
                ok++;
            }
            catch (Exception ex)
            {
                _showResult(unitTest, ex, test.ShowTrace);
                err++;
            }
        }

        // Summary
        LogLine();
        if (all < 1)
        {
            LogOk("O.O", $"No tests found here");
        }
        else if (err == 0)
        {
            LogOk("ALL", $"All tests are passed💖💖💖: {ok} tests.");
        }
        else
        {
            if (ok < 1)
            {
                LogErr("ERR", $"All tests (⛔{err}) failed⛔");
            }
            else
            {
                LogErr("ERR", $"Failed tests ⛔{err}/{all} tests. ✅{ok}/{all}");
            }

        }
        LogLine();

        // Return summary
        return (ok, err);
    }

    public static IEnumerable<(TestAttribute Test, MethodInfo Method)> GetTests(object value)
    {
        var type = value.GetType();
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
        foreach (var method in methods)
        {
            var attr = method.GetCustomAttribute<TestAttribute>();
            if (attr == null) continue;
            if (method.IsStatic) continue;

            // Validate parameters
            var parameters = method.GetParameters();
            if (parameters.Length != 1 || parameters[0].ParameterType != typeof(UnitTest)) continue;

            yield return (attr, method);
        }
    }

    private static object[] _concat(object[] a, object[] b)
    {
        return a.Concat(b).ToArray();
    }

    private static void _showLogs(string testName, List<object[]> logs)
    {
        // Show logged info if test failed
        if (logs != null && logs.Count > 0)
        {
            // Draw line
            LogLine();
            GD.Print($">>>> ⛔⛔⛔ {testName} ⛔⛔⛔");

            foreach (var log in logs)
            {
                GD.Print(_concat(["ℹ️"], log));
            }
        }
    }

    private static void _showResult(UnitTest t, Exception ex, bool showTrace)
    {
        if (ex != null)
        {
            // Show logs
            _showLogs(t.TestName, t._log);

            // Show exception 
            if (ex is TargetInvocationException)
            {
                LogErr(t.TestName, $"failed: {ex.InnerException?.Message ?? "No message"}");
                if (showTrace) GD.Print(ex.InnerException?.StackTrace.Replace("\r", ""));
            }
            else
            {
                LogErr(t.TestName, $"failed: {ex.Message}");
                if (showTrace) GD.Print(ex.StackTrace.Replace("\r", ""));
            }
        }
        else
        {
            LogOk(t.TestName, "passed.");
        }
    }

    private static void _runInits(object value)
    {
        // Get type
        var type = value.GetType();

        // Get all methods in the class
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

        // Get all methods with the Test attribute
        foreach (var method in methods)
        {
            if (method.GetCustomAttribute<InitAttribute>() != null)
            {
                try
                {
                    method.Invoke(value, null);
                }
                catch (Exception ex)
                {
                    LogErr(method.Name, $"failed: {ex.Message}");
                }
            }
        }
    }
}

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class TestAttribute : Attribute
{
    public bool ShowTrace { get; }
    public string Group { get; set; } = "";

    public TestAttribute(bool trace = false, string group = "")
    {
        ShowTrace = trace;
        Group = group;
    }
}

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class InitAttribute : Attribute
{
}

public class TestFailException : Exception
{
    public TestFailException(string message) : base(message)
    {
    }
}

public partial class TestNode : Node
{
    [Export] Container Container;
    [Export] bool UseGroups = true;

    public override void _Ready()
    {
        if (Container != null)
        {
            _activateUi();
        }
        else
        {
            UnitTest.RunTests(this);
        }
    }

    private void _activateUi()
    {
        // Get all tests
        var tests = UnitTest.GetTests(this).ToList();

        // Groups (If allowed)
        if (UseGroups)
        {
            // Get groups
            var groups = _getGroups(tests);

            // Group buttons
            foreach (var group in groups)
            {
                _testButton("🗂️", group, () => UnitTest.RunTests(this, group: group));
            }
        }

        // Exact test Buttons
        foreach (var (test, method) in tests)
        {
            _testButton("🛠️", method.Name, () => UnitTest.RunTests(this, exact: method.Name));
        }
    }

    private void _testButton(string prefix, string testName, Func<(int ok, int err)> testRunner)
    {
        var b = _button(prefix + testName);
        b.Pressed += () =>
        {
            // Set waiting text
            b.Text = "⏳" + prefix + testName;

            // Run test
            var (ok, err) = testRunner();

            // Show result
            if (err > 0)
            {
                b.Text = "⛔" + prefix + testName;
            }
            else if (err == 0 && ok > err)
            {
                b.Text = "✅" + prefix + testName;
            }
            else
            {
                b.Text = "❓" + prefix + testName;
            }
        };
    }

    private Button _button(string text)
    {
        var b = new Button();
        b.Text = text;
        Container.AddChild(b);
        return b;
    }

    private IList<string> _getGroups(IEnumerable<(TestAttribute Test, MethodInfo Method)> tests)
    {
        return tests.Where(x => x.Test.Group != "" && x.Test.Group != null)
            .Select(x => x.Test.Group)
            .Distinct()
            .ToList();
    }
}