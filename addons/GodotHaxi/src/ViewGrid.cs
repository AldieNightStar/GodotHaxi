using Godot;

namespace GodotHaxi;

public class ViewGrid<VIEW> where VIEW : Node2D
{
    private Node2D node;
    private VIEW[,] viewMap;

    public int Width { get; private set; }
    public int Height { get; private set; }

    public int TileWidth { get; private set; }
    public int TileHeight { get; private set; }
    public bool TileCentered { get; private set; }

    public ViewGrid(Node2D node, int width, int height, int tileW, int tileH, bool center = false)
    {
        this.node = node;
        Width = width;
        Height = height;
        TileWidth = tileW;
        TileHeight = tileH;
        TileCentered = center;
        viewMap = new VIEW[Width, Height];
    }

    public VIEW Get(int x, int y)
    {
        if (!IsBound(x, y)) return null;
        return viewMap[x, y];
    }

    public bool Set(int x, int y, VIEW v)
    {
        // When view is null, then this is Remove
        if (v == null) return Remove(x, y);

        // Check for boundaries
        if (!IsBound(x, y)) return false;

        // Remove old
        var old = Get(x, y);
        if (ReferenceEquals(old, v)) return true;
        else delete(old);

        // Set new, also add child if not yet
        viewMap[x, y] = v;
        if (!node.IsAncestorOf(v)) node.AddChild(v);

        // Update position
        var (rx, ry) = GetRealPosition(x, y);
        v.Position = new Vector2(rx, ry);

        return true;
    }

    public bool IsBound(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

    public bool Has(int x, int y) => Get(x, y) != null;

    public bool Remove(int x, int y)
    {
        if (!IsBound(x, y)) return false;
        delete(Get(x, y));
        viewMap[x, y] = null;
        return true;
    }

    public bool Swap(int x, int y, int x2, int y2, double time = 0.3)
    {
        if (!IsBound(x, y) || !IsBound(x2, y2)) return false;

        var v1 = viewMap[x, y];
        var v2 = viewMap[x2, y2];

        viewMap[x, y] = v2;
        viewMap[x2, y2] = v1;

        tweenMove(v1, x2, y2, time);
        tweenMove(v2, x, y, time);

        return true;
    }

    public bool Move(int x, int y, int x2, int y2, double time = 0.3)
    {
        // Make sure v2 pos is accessible
        if (!IsBound(x2, y2))
        {
            delete(Get(x, y));
            return true;
        }

        // Make sure v1 is there
        // Non bound will return null as well
        var v1 = Get(x, y);
        if (v1 == null) return false;

        // Remove v2 if exist
        delete(Get(x2, y2));

        // Move animation
        tweenMove(v1, x2, y2, time);

        // Assign new position
        viewMap[x, y] = null;
        viewMap[x2, y2] = v1;

        return true;
    }

    public void ClearFreed()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                var v = viewMap[x, y];
                if (v != null && !GodotObject.IsInstanceValid(v))
                {
                    viewMap[x, y] = null;
                }
            }
        }
    }

    private void delete(Node n)
    {
        if (n == null) return;
        if (GodotObject.IsInstanceValid(n)) n.QueueFree();
    }

    private void tweenMove(Node2D n, int tileX, int tileY, double time)
    {
        if (n == null) return;
        var t = node.CreateTween();
        var (x, y) = GetRealPosition(tileX, tileY);
        t.TweenProperty(n, new NodePath(Node2D.PropertyName.Position), new Vector2(x, y), time);
    }

    public (int, int) GetRealPosition(int x, int y)
    {
        var _x = x * TileWidth;
        var _y = y * TileHeight;
        var half = TileWidth / 2;
        if (TileCentered) return (_x + half, _y + half);
        return (_x, _y);
    }

    public (int, int) GetTilePosition(int x, int y)
    {
        return (x / TileWidth, y / TileHeight);
    }
}