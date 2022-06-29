using Godot;
using System;

public static class HexUtils
{

    //hexagons are aligned in horizontal rows along X axis, and are 1m from side to side.
    //Z-axis length is as such =  2/sqrt(3.)
    public static readonly double hex_side_len = 1 / Math.Sqrt(3);
    //distance from hex center to the one next up
    public static readonly double hex_lengthwise_offset = 1.5 * hex_side_len;
    public static Vector3 hex_to_world(Vector2 cell)
    {
        cell = cell.Round();
        if ((int)cell.y % 2 != 0)
            cell.x -= 0.5f;
        cell.y *= (float)hex_lengthwise_offset;
        return new Vector3(cell.x, 0f, cell.y);
    }

    public static Vector2 world_to_hex(Vector3 coords)
    {
        double x = coords.x, y = coords.z;
        float s = Math.Sign(y);
        y = Math.Abs(y);
        y /= hex_side_len;
        y += 0.5;
        bool flip = false;
        if ((y % 3) > 1.5)
        {
            x += 0.5;
            flip = true;
        }
        if ((y % 1.5) > 1)
        {
            var yband = ((y % 1.5) - 1) / 0.5;
            var xband = (x + 0.5) % 1 * 2;
            if (xband > 1)
            {
                if ((xband - 1 + yband) > 1)
                {
                    y += 1;
                    if (!flip)
                        x += 0.5;
                }
            }
            else
            {
                if ((xband + 1 - yband) < 1)
                {
                    y += 1;
                    if (flip)
                        x -= 0.5;
                }
            }
        }
        y /= 1.5;
        x = Math.Round(x);
        y *= s;
        return new Vector2((float)x, (float)y).Round();

    }

}
