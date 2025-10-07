using Godot;

namespace GodotApplication;

public partial class TestSprite : Sprite2D
{
    [Export]
    private float _rotationSpeed = 1f;

    public override void _Process(double delta)
    {
        Rotate((float)(_rotationSpeed * delta));
    }
}
