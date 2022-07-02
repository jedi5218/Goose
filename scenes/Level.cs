using Godot;
using System;

public class Level : Spatial
{
    SpatialMaterial m = new SpatialMaterial();
    bool show_path = false;
    Camera camera;
    PackedScene goose_scene = GD.Load<PackedScene>("res://scenes/units/goose/Goose.tscn");

    public override void _Ready()
    {
        camera = GetNode<Camera>("Camera");
        camera.Connect("Command", this, "Move");
        m.FlagsUnshaded = true;
        m.FlagsUsePointSize = true;
        m.AlbedoColor = Color.ColorN("white");
        GetNode("Container/HBoxContainer/SpinBox").Connect("value_changed", this, "ChangeGooseCount");
        
    }
    public void ChangeGooseCount(int new_count)
    {
        GD.Print("new count:", new_count);
        var geese = GetTree().GetNodesInGroup("Geese");
        var diff = new_count - geese.Count;
        if(diff < 0)
        {
            for (int i = geese.Count-1; i >= new_count; i--)
                ((Node)geese[i]).QueueFree();
        }
        else
        {
            for (int i = geese.Count; i < new_count; i++)
            {
                var goose = (Goose)goose_scene.Instance();
                goose.Translation = (HexUtils.hex_to_world(new Vector2(0, -16)));
                goose.AddToGroup("Geese");
                AddChild(goose);
            }
        }
    }

    
    
    public void Move(Vector3 to)
    {
        GetNode<OccupiedHexMarker>("OccupiedHexMarker").PlaceMarker(to);
        GetTree().CallGroup("Geese", "Move", to);
        if (show_path)
        {
            var path = GetNode<Goose>("Goose").path;
            DrawPath(path);
        }
    }

    void DrawPath(Godot.Collections.Array<Vector3> path_array)
    {
        var im = GetNode<ImmediateGeometry>("Draw");
        im.MaterialOverride = m;
        im.Clear();
        im.Begin(Mesh.PrimitiveType.Points, null);
        im.AddVertex(path_array[0]);
        im.AddVertex(path_array[path_array.Count - 1]);
        im.End();
        im.Begin(Mesh.PrimitiveType.LineStrip, null);
        foreach (var vert in path_array)
            im.AddVertex(vert);
        im.End();
    }
}
