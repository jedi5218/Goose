using Godot;
using System;

public class OccupiedHexMarker : MeshInstance
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
    public void PlaceMarker(Vector3 world_pos)
    {
        world_pos.y = 0;
        Vector2 hex = HexUtils.world_to_hex(world_pos);
        var tr = new Transform(Basis.Identity, world_pos);
        tr.origin.y = 1;
        GetNode<Spatial>("../Pin").Transform = tr;
        var t = Transform;
        t.origin = HexUtils.hex_to_world(hex);
        Transform = t;
        GetNode<Label>("Label").Text = String.Format("coords: {0}:{1}", hex.x, hex.y);
    }
}
