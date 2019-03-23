using System;

public static class ParabolaMath {
    public const double EPSILON = double.Epsilon * 1E100;

    public static double EvalParabola(double focusX, double focusY, double directrix, double x) {
        return 0.5 * ((x - focusX) * (x - focusX) / (focusY - directrix) + focusY + directrix);
    }

    public static double IntersectParabola(double aX, double aY, double bX, double bY, double directrix) {
        return aY.ApproxEqual(bY)
            ? (aX + bX) / 2
            : (aX * (directrix - bY) + bX * (aY - directrix) +
            Math.Sqrt((directrix - aY) * (directrix - bY) *
            ((aX - bX) * (aX - bX) +
            (aY - bY) * (aY - bY))
            )
          ) / (aY - bY);
    }

    public static bool ApproxEqual(this double a, double b) {
        return Math.Abs(a - b) <= EPSILON;
    }

    public static bool ApproxGreaterThanOrEqualTo(this double a, double b) {
        return a > b || a.ApproxEqual(b);
    }

    public static bool ApproxLessThanOrEqualTo(this double a, double b) {
        return a < b || a.ApproxEqual(b);
    }
}