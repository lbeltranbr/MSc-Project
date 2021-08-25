public class Line
{
    public Point start;
    public Point end;
    public Point direction;
    public Line(Point s, Point e, Point dir)
    {
        start = s;
        end = e;
        direction = dir;
    }
    public Line(Point s, Point dir)
    {
        start = s;
        direction = dir;
    }

}
