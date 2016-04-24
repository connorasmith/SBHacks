using UnityEngine;
using System.Collections;

namespace PicoGames.Utilities
{
    public class Shape
    {
        const float SQR_TWO = 1.4142135f;

        #region Standard Polygons
        public static Vector3[] GetSquare(float _centerScale = 2)
        {
            return GetPolygon(4, _centerScale);
        }
        public static Vector3[] GetPentagon(float _centerScale = 2)
        {
            return GetPolygon(5, _centerScale);
        }
        public static Vector3[] GetHexagon(float _centerScale = 2)
        {
            return GetPolygon(6, _centerScale);
        }
        public static Vector3[] GetHeptagon(float _centerScale = 2)
        {
            return GetPolygon(7, _centerScale);
        }
        public static Vector3[] GetOctagon(float _centerScale = 2)
        {
            return GetPolygon(8, _centerScale);
        }
        public static Vector3[] GetNonagon(float _centerScale = 2)
        {
            return GetPolygon(9, _centerScale);
        }
        public static Vector3[] GetDecagon(float _centerScale = 2)
        {
            return GetPolygon(10, _centerScale);
        }
        public static Vector3[] GetDodecagon(float _centerScale = 2)
        {
            return GetPolygon(12, _centerScale);
        }
        public static Vector3[] GetPolygon(int _sides, float _centerScale = 2)
        {
            return GetRoseCurve(_sides, 1, _centerScale, true);
        }
        #endregion

        #region Standard Shapes
        public static Vector3[] GetStar(float _centerScale = 2)
        {
            return GetRoseCurve(5, 2, _centerScale, true);
        }
        #endregion

        public static Vector3[] GetRoseCurve(int _points, int _detail, float _centerScale, bool _unitize)
        {
            _points = Mathf.Max(3, _points);
            _detail = Mathf.Max(1, _detail);

            Vector3[] pnts = new Vector3[_points * _detail];
            int k = _points;

            Vector3 minBounds = Vector3.one * float.MaxValue;
            Vector3 maxBounds = Vector3.one * float.MinValue;
            for (int i = 0; i < pnts.Length; i++)
            {
                float theta = i * (2 * Mathf.PI / pnts.Length);
                float r = Mathf.Cos(theta * k) + _centerScale;

                pnts[i] = new Vector3(r * Mathf.Cos(theta), r * Mathf.Sin(theta), 0);

                minBounds = Vector3.Min(minBounds, pnts[i]);
                maxBounds = Vector3.Max(maxBounds, pnts[i]);
            }

            if (_unitize)
                Unitize(ref pnts, minBounds, maxBounds);

            return pnts;
        }
        
        public static void Unitize(ref Vector3[] _points, Vector3 _min, Vector3 _max)
        {
            float unit = Vector3.Distance(_min, _max) / SQR_TWO;
            for (int i = 0; i < _points.Length; i++)
                _points[i] /= unit;
        }
    }
}
