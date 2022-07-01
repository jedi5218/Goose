using Godot;
using System;

public class Camera : Spatial
{
    private Godot.Camera camera;
    const float camera_speed = 5; // camera panning speed, in m/s
    const int pan_border_width = 150; //width of the panning border, in pixels
    float target_zoom_level = 0.5f; //target zoom level for easing
    float zoom_level = 0.5f;
    Vector3 zoom_min, zoom_max;
    float zoom_min_fov = 90, zoom_max_fov = 50;


    [Signal]
    public delegate void Command(Vector3 pos);

    Vector3 speed = new Vector3();
    public override void _Ready()
    {
        camera = GetNode<Godot.Camera>("Camera");
        SetProcessInput(true);
        var zoom_min_gizmo = GetNode<Spatial>("MinZoom");
        zoom_min = zoom_min_gizmo.Translation;
        zoom_min_gizmo.QueueFree();
        var zoom_max_gizmo = GetNode<Spatial>("MaxZoom");
        zoom_max = zoom_max_gizmo.Translation;
        zoom_max_gizmo.QueueFree();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton btn)
        {
            if (btn.ButtonIndex == (int)ButtonList.Left && btn.IsPressed())
            {
                var from = camera.ProjectRayOrigin(btn.Position);
                var dir = camera.ProjectRayNormal(btn.Position);
                var target = from + dir * (from.y / Math.Abs(dir.y)); // assumes target can only be at Y = 0
                EmitSignal("Command", target);
            }
            else if (btn.ButtonIndex == (int)ButtonList.WheelDown && btn.IsPressed())
            {
                target_zoom_level -= 0.1f;
                target_zoom_level = Mathf.Clamp(target_zoom_level, 0, 1);
                
            }
            else if (btn.ButtonIndex == (int)ButtonList.WheelUp && btn.IsPressed())
            {
                target_zoom_level += 0.1f;
                target_zoom_level = Mathf.Clamp(target_zoom_level, 0, 1);
            }
        }
        if (@event is InputEventMouseMotion mtn)
        {
            var size = GetViewport().Size;
            if ((mtn.ButtonMask & (int)(ButtonList.MaskMiddle | ButtonList.MaskRight)) != 0)
            {
                var r = RotationDegrees;
                r.y += mtn.Relative.x / size.y * camera.Fov;
                RotationDegrees = r;
            }
            var relative_pos = (mtn.Position / size) - new Vector2(0.5f, 0.5f);
            var border_width = new Vector2(0.5f, 0.5f) - (new Vector2(pan_border_width, pan_border_width) / size);
            var pos_abs = relative_pos.Abs();
            var movement = new Vector2(Mathf.Max(pos_abs.x - border_width.x, 0), Mathf.Max(pos_abs.y - border_width.y, 0))
                /
                (new Vector2(0.5f, 0.5f) - border_width);
            if(movement.Length()>0)
            {
                GD.Print("relative_pos: ", relative_pos.Sign());
            }
            movement.x *= movement.x;
            movement.y *= movement.y;
            movement *= relative_pos.Sign();
            movement *= camera_speed;
            speed = new Vector3(movement.x, 0, movement.y);
            speed = speed.Rotated(Vector3.Up, Rotation.y);

        }
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var t = Translation;
        t += speed * delta;
        Translation = t;
        zoom_level += (target_zoom_level - zoom_level) * delta / 0.5f;
        camera.Translation = zoom_min.LinearInterpolate(zoom_max, zoom_level);
        camera.Fov = Mathf.Lerp(zoom_min_fov, zoom_max_fov, zoom_level);
        camera.LookAt(Translation, Vector3.Up);
    }
}
