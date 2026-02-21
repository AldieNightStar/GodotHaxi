using Godot;

namespace GodotHaxi;

public class SpriteUtil
{
    public static void UpdateRegion(Sprite2D sprite, int id)
    {
        var size = sprite.RegionRect.Size;
        sprite.RegionRect = new Rect2(id * size.X, 0, size);
    }

    public static void ResizePx(Sprite2D sprite, int w, int h)
    {
        sprite.Scale = new Vector2(w, h) / sprite.GetRect().Size;
    }
}