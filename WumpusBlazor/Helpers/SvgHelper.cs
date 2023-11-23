namespace WumpusBlazor.Helpers
{
    public interface ISvgHelper
    {
        string DescribeArc(double x, double y, double radius, double startAngle, double endAngle);
        (double X, double Y) PolarToCartesian(double centerX, double centerY, double radius, double angleInDegrees);
    }

    public class SvgHelper : ISvgHelper
    {
        public (double X, double Y) PolarToCartesian(double centerX, double centerY, double radius, double angleInDegrees)
        {
            var angleInRadians = (angleInDegrees - 90) * Math.PI / 180.0;
            return (centerX + (radius * Math.Cos(angleInRadians)), centerY + (radius * Math.Sin(angleInRadians)));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0042:Deconstruct variable declaration", Justification = "<Pending>")]
        public string DescribeArc(double x, double y, double radius, double startAngle, double endAngle)
        {
            var start = PolarToCartesian(x, y, radius, endAngle);
            var end = PolarToCartesian(x, y, radius, startAngle);
            var largeArcFlag = endAngle - startAngle <= 180 ? "0" : "1";

            return $"M {start.X} {start.Y} A {radius} {radius} 0 {largeArcFlag} 0 {end.X} {end.Y}";
        }
    }
}
