namespace MyEngine2D.Core.Graphic;

internal struct GraphicWindowDescription
{
    internal readonly int Width;
    internal readonly int Height;
    internal readonly string Header;

    internal GraphicWindowDescription(int width, int height)
        : this(width, height, DefaultWindowHeader) { }

    internal GraphicWindowDescription(int width, int height, string header)
    {
        Width = width;
        Height = height;
        Header = header;
    }

    private const int DefaultWindowRefreshRate = 60;
    private const string DefaultWindowHeader = "MyEngine2D Game";
}