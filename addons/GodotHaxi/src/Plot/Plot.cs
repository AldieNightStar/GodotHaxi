using System;
using System.Collections.Generic;
using Godot;

namespace GodotHaxi;

public class Plot
{
    private Dictionary<string, List<Action<Plot>>> _labels;
    private string _currentLabel;
    private int _currentLabelPos;

    public Plot()
    {
        _labels = new();
        _currentLabel = "Start";
        _currentLabelPos = 0;
    }

    public bool HasNext()
    {
        return _currentLabelPos >= 0 && _currentLabelPos < _getLabelSize(_currentLabel);
    }

    public void Next()
    {
        if (HasNext())
        {
            _currentLabelPos += 1;
            StepDeferred();
        }
    }

    public void Step()
    {
        var act = _getAt(_currentLabel, _currentLabelPos);
        if (act != null) act(this);
    }

    public void StepDeferred()
    {
        Callable.From(Step).CallDeferred();
    }

    public bool HasLabel(string label) => _labels.ContainsKey(label);

    public void Goto(string label, int pos = 0)
    {
        _currentLabel = label;
        _currentLabelPos = pos;
    }

    public Plot Build(Action<PlotBuilder> b)
    {
        b(new PlotBuilder(_labels));
        return this;
    }

    public (string, int) GetLabelPos() => (_currentLabel, _currentLabelPos);

    private Action<Plot> _getAt(string label, int pos)
    {
        if (!_labels.ContainsKey(label)) return null;
        var actions = _labels[label];
        if (pos < 0 || pos >= actions.Count) return null;
        return actions[pos];
    }

    private int _getLabelSize(string label)
    {
        if (!_labels.ContainsKey(label)) return 0;
        return _labels[label].Count;
    }
}

public class PlotBuilder
{
    private Dictionary<string, List<Action<Plot>>> _labels;
    private List<Action<Plot>> _currentList;

    public PlotBuilder(Dictionary<string, List<Action<Plot>>> labels)
    {
        _labels = labels;
    }

    public void Label(string label)
    {
        if (_labels.ContainsKey(label))
        {
            _currentList = _labels[label];
        }
        else
        {
            _currentList = new();
            _labels[label] = _currentList;
        }
    }

    public void Act(Action<Plot> act)
    {
        if (_currentList == null) Label("Start");
        _currentList.Add(act);
    }

    public void Jump(string label) => Act(p => { p.Goto(label); p.StepDeferred(); });
}