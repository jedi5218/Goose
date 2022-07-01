using Godot;
using System;

public class Level : Spatial
{
    const float SPEED = 2;

    float camrot = 0.0f;
    SpatialMaterial m = new SpatialMaterial();
    public Godot.Collections.Array<Vector3> path;
    bool show_path = true;
    Goose robot;
    Camera camera;

    public override void _Ready()
    {
        robot = GetNode<Goose>("Goose");
        camera = GetNode<Camera>("Camera");
        camera.Connect("Command", this, "Move");
        m.FlagsUnshaded = true;
        m.FlagsUsePointSize = true;
        m.AlbedoColor = Color.ColorN("white");
        robot.GetNode<AnimationPlayer>("Spatial/Animation").Play("goose_walk_loop");
        
    }

    public override void _PhysicsProcess(float delta)
    {
        var direction = new Vector3();
        // We need to scale the movement speed by how much delta has passed,
        // otherwise the motion won't be smooth.
        var step_size = delta * SPEED;

        if (path != null && path.Count > 0)
        {
            robot.GetNode<AnimationPlayer>("Spatial/Animation").PlaybackSpeed = SPEED;
            // Direction is the difference between where we are now
            // and where we want to go.
            var destination = path[0];
            direction = destination - robot.Translation;
            // If the next node is closer than we intend to 'step', then
            // take a smaller step. Otherwise we would go past it and
            // potentially go through a wall or over a cliff edge!
            if (step_size > direction.Length())
            {
                step_size = direction.Length();
                // We should also remove this node since we're about to reach it.
                path.RemoveAt(0);
            }

            // Move the robot towards the path node, by how far we want to travel.
            // Note: For a KinematicBody, we would instead use move_and_slide
            //so collisions work properly.
            robot.Translation += direction.Normalized() * step_size;

            // Lastly let's make sure we're looking in the direction we're traveling.
            // Clamp y to 0 so the robot only looks left and right, not up/down.
            direction.y = 0;
            if (direction.Length() > 0)
            {
                // Direction is relative, so apply it to the robot's location to
                // get a point we can actually look at.
                var look_at_point = robot.Translation + direction.Normalized();
                // Make the robot look at the point.
                robot.LookAt(look_at_point, Vector3.Up);
            }
        }
        else
            robot.GetNode<AnimationPlayer>("Spatial/Animation").PlaybackSpeed = 0;
    }
    
    public void Move(Vector3 to)
    {
        var target_point = GetNode<Navigation>("Navigation").GetClosestPoint(to);
        GetNode<OccupiedHexMarker>("OccupiedHexMarker").PlaceMarker(target_point);

        // Set the path between the robots current location and our target.
        path = new Godot.Collections.Array<Vector3>(
            GetNode<Navigation>("Navigation").GetSimplePath(robot.Translation, target_point, true));

        if (show_path)
            DrawPath(path);
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
