using Autodesk.Revit.DB;

namespace BimIshou.Utils
{
    public class XYZEqualityComparer : IEqualityComparer<XYZ>
    {
        private readonly double _tolerance;

        public XYZEqualityComparer(double tolerance = 1.0e-9)
        {
            _tolerance = tolerance;
        }
        public bool Equals(XYZ x, XYZ y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return x.IsAlmostEqualTo(y, _tolerance);
        }
        public int GetHashCode(XYZ obj)
        {
            unchecked
            {
                int hash = 19;
                hash = hash * 31 + obj.X.GetHashCode();
                hash = hash * 31 + obj.Y.GetHashCode();
                hash = hash * 31 + obj.Z.GetHashCode();
                return hash;
            }
        }
    }
}
