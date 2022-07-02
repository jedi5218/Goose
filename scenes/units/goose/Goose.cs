using Godot;
using System;

public class Goose : Spatial
{
    private int x = 0;
    private int y = 0;
    public Godot.Collections.Array<Vector3> path;
    const float SPEED = 2;
    public override void _Ready()
    {
        Transform = Transform.Translated(HexUtils.hex_to_world(new Vector2(0, -16)));
        GetNode<AnimationPlayer>("Spatial/Animation").Play("goose_walk_loop");
    }
    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Spatial marker = (Spatial)GetNode("OccupiedHexMarker");
        Vector2 hex = HexUtils.world_to_hex(GlobalTransform.origin);
        if (hex.x != x || hex.y != y)
        {
            x = (int)hex.x;
            y = (int)hex.y;
        }
        var t = marker.GlobalTransform;
        t.origin = HexUtils.hex_to_world(hex);
        marker.GlobalTransform = t;
    }
    public override void _PhysicsProcess(float delta)
    {
        Vector3 direction;
        // We need to scale the movement speed by how much delta has passed,
        // otherwise the motion won't be smooth.
        var step_size = delta * SPEED;

        if (path != null && path.Count > 0)
        {
            GetNode<AnimationPlayer>("Spatial/Animation").PlaybackSpeed = SPEED;
            // Direction is the difference between where we are now
            // and where we want to go.
            var destination = path[0];
            direction = destination - Translation;
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
            Translation += direction.Normalized() * step_size;

            // Lastly let's make sure we're looking in the direction we're traveling.
            // Clamp y to 0 so the robot only looks left and right, not up/down.
            direction.y = 0;
            if (direction.Length() > 0)
            {
                // Direction is relative, so apply it to the robot's location to
                // get a point we can actually look at.
                var look_at_point = Translation + direction.Normalized();
                // Make the robot look at the point.
                LookAt(look_at_point, Vector3.Up);
            }
        }
        else
            GetNode<AnimationPlayer>("Spatial/Animation").PlaybackSpeed = 0;
    }
    public void Move(Vector3 to)
    {
        var target_point = GetNode<Navigation>("../Navigation").GetClosestPoint(to);
        path = new Godot.Collections.Array<Vector3>(
            GetNode<Navigation>("../Navigation").GetSimplePath(Translation, target_point, true));
    }
}
