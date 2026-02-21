using Godot;

namespace GodotHaxi;

public partial class Wrap<T> : RefCounted
{
    private T _data;

    public Wrap(T data)
    {
        _data = data;
    }

    public T Unwrap() => _data;
}

public static class WrapExtension
{
    public static Wrap<T> ToGodot<T>(this T self)
    {
        return new Wrap<T>(self);
    }
}