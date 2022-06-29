using Godot;
using System;

public class Goose : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private int x = 0;
    private int y = 0;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Transform = Transform.Translated(HexUtils.hex_to_world(new Vector2(0, -16)));
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Spatial marker = (Spatial)GetNode("OccupiedHexMarker");
        Vector2 hex = HexUtils.world_to_hex(GlobalTransform.origin);
        if (hex.x != x || hex.y != y)
        {
            GD.Print("new coord: {0}:{1}", x, y);
            x = (int)hex.x;
            y = (int)hex.y;
        }
        var t = marker.GlobalTransform;
        t.origin = HexUtils.hex_to_world(hex);
        marker.GlobalTransform = t;
    }

}
