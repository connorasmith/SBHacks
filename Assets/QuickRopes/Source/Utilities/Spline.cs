using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PicoGames.Utilities
{
    [System.Serializable]
    public class SplinePoint
    {
        public SplinePoint(Vector3 _position, Quaternion _rotation)
        {
            position = _position;
            rotation = _rotation;
        }

        [SerializeField]
        public Vector3 position;
        [SerializeField]
        public Quaternion rotation;
    }

    [AddComponentMenu("PicoGames/Utilities/Spline")]
    [System.Serializable]
    public class Spline : MonoBehaviour
    {
        public enum ControlPointMode
        {
            Free,
            Aligned,
            Mirrored
        }
               
        [SerializeField]
        public int outputResolution = 1000;
        [SerializeField, HideInInspector]
        public bool hasChanged = false;
                
        [SerializeField, HideInInspector]
        private List<Vector3> points = new List<Vector3>(new Vector3[]
            {
                new Vector3(0, 0, 0),
                new Vector3(0, -1, 0),
                new Vector3(0, -4, 0),
                new Vector3(0, -5, 0)
            });

        [SerializeField, HideInInspector]
        private List<ControlPointMode> modes = new List<ControlPointMode>(new ControlPointMode[]
            {
                ControlPointMode.Mirrored,
                ControlPointMode.Mirrored
            });

        [SerializeField]
        private bool isLooped = false;
        [SerializeField]
        private bool evenlyDistributPoints = true;
        [SerializeField, HideInInspector]
        private float curveLength = -1;
        
        public float CurveLength 
        { 
            get 
            {
                if (curveLength < 0)
                    UpdateCurveLength();

                return curveLength; 
            } 
        }
        public int ControlCount { get { return (points.Count - 1) / 3; } }
        public Vector3[] Points { get { return points.ToArray(); } }

        public bool IsLooped
        {
            get { return isLooped; }
            set
            {
                if (value != isLooped)
                {
                    hasChanged = true;

                    isLooped = value;
                    if (value == true)
                    {
                        if (ControlCount < 2)
                        {
                            AddCurve(0);
                            Vector3 offset = (GetPoint(1) - GetPoint(0));

                            SetControlPoint(1, GetControlPoint(0) - new Vector3(offset.y, -offset.x, 0) * 1.5f);
                            SetPoint(4, GetPoint(points.Count - 2) - new Vector3(offset.y, -offset.x, 0) * 1.5f);
                        }

                        evenlyDistributPoints = true;

                        modes[0] = modes[modes.Count - 1];
                        SetPoint(points.Count - 1, points[0]);
                    }
                }
            }
        }
        public bool EvenPointDistribution { get { return evenlyDistributPoints; } set { evenlyDistributPoints = isLooped ? true : value; } }
        
        public void Reset()
        {
            points = new List<Vector3>(new Vector3[]
            {
                new Vector3(0, 0, 0),
                new Vector3(0, -1, 0),
                new Vector3(0, -4, 0),
                new Vector3(0, -5, 0)
            });

            modes = new List<ControlPointMode>(new ControlPointMode[]
            {
                ControlPointMode.Mirrored,
                ControlPointMode.Mirrored
            });

            UpdateCurveLength();
            hasChanged = true;
        }

        public void AddCurve(int _atIndex, ControlPointMode _defaultMode = ControlPointMode.Mirrored)
        {
            Vector3 p1 = points[_atIndex * 3];
            Vector3 p2 = points[(_atIndex + 1) * 3];
            Vector3 center = (p1 + p2) * 0.5f;
            Vector3 direction = (p2 - p1).normalized;

            points.InsertRange((_atIndex * 3) + 2, new Vector3[] 
            {
                center - direction,
                center,
                center + direction
            });

            modes.Insert(_atIndex, _defaultMode);
            EnforceMode(_atIndex);

            if (isLooped)
            {
                points[points.Count - 1] = points[0];
                modes[modes.Count - 1] = modes[0];
                EnforceMode(0);
            }

            hasChanged = true;
        }
        public void RemoveCurve(int _index)
        {
            if (_index == 0 || _index == ControlCount)
                return;

            points.RemoveRange((_index * 3) - 1, 3);
            modes.RemoveAt(_index);

            hasChanged = true;
        }
        
        public ControlPointMode GetMode(int _index)
        {
            return modes[(_index + 1) / 3];
        }
        public void SetMode(int _index, ControlPointMode _mode)
        {
            int modeIndex = (_index + 1) / 3;
            modes[modeIndex] = _mode;

            if (isLooped)
            {
                if(modeIndex == 0)
                {
                    modes[modes.Count - 1] = _mode;                    
                }
                else if(modeIndex == modes.Count - 1)
                {
                    modes[0] = _mode;
                }
            }

            EnforceMode(_index);
            hasChanged = true;
        }
        private void EnforceMode(int _index)
        {
            int modeIndex = (_index + 1) / 3;

            ControlPointMode mode = modes[modeIndex];
            if (mode == ControlPointMode.Free || !isLooped && (modeIndex == 0 || modeIndex == modes.Count - 1))
                return;

            int middleIndex = modeIndex * 3;
            int fixedIndex, enforcedIndex;
            if(_index <= middleIndex)
            {
                fixedIndex = middleIndex - 1;
                if (fixedIndex < 0)
                    fixedIndex = points.Count - 2;
                enforcedIndex = middleIndex + 1;
                if (enforcedIndex >= points.Count - 2)
                    enforcedIndex = 1;
            }
            else
            {
                fixedIndex = middleIndex + 1;
                if (fixedIndex >= points.Count)
                    fixedIndex = 1;
                enforcedIndex = middleIndex - 1;
                if (enforcedIndex < 0)
                    enforcedIndex = points.Count - 2;
            }

            Vector3 middle = points[middleIndex];
            Vector3 enforcedTangent = middle - points[fixedIndex];

            if (mode == ControlPointMode.Aligned)
                enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);

            points[enforcedIndex] = middle + enforcedTangent;
        }

        public Vector3 GetControlPoint(int _index, Space _space = Space.Self)
        {
            return GetPoint(_index * 3, _space);
        }
        public void SetControlPoint(int _index, Vector3 _position, Space _space = Space.Self)
        {
            SetPoint(_index * 3, _position, _space);
        }

        //public void SetPoin
        public void SetPoint(int _index, Vector3 _position, Space _space = Space.Self)
        {
            if (_space == Space.World)
                _position = transform.InverseTransformPoint(_position);
            
            if (_index % 3 == 0)
            {
                Vector3 delta = _position - points[_index];
                if (isLooped)
                {
                    if(_index == 0)
                    {
                        points[1] += delta;
                        points[points.Count - 2] += delta;
                        points[points.Count - 1] = _position;
                    }
                    else if(_index == points.Count - 1)
                    {
                        points[0] = _position;
                        points[1] += delta;
                        points[_index - 1] += delta;
                    }
                    else
                    {
                        points[_index - 1] += delta;
                        points[_index + 1] += delta;
                    }
                }
                else
                {
                    if (_index > 0)
                    {
                        points[_index - 1] += delta;
                    }

                    if (_index + 1 < points.Count)
                    {
                        points[_index + 1] += delta;
                    }
                }
            }

            points[_index] = _position;
            EnforceMode(_index);

            UpdateCurveLength();
            hasChanged = true;
        }
        public Vector3 GetPoint(int _index, Space _space = Space.Self)
        {
            return (_space == Space.World) ? transform.TransformPoint(points[_index]) : points[_index];
        }

        public Vector3 GetPointOnCurve(float _t)
        {
            int i;
            if(_t >= 1f)
            {
                _t = 1f;
                i = points.Count - 4;
            }
            else
            {
                _t = Mathf.Clamp01(_t) * ((points.Count - 1) / 3);
                i = (int)_t;
                _t -= i;
                i *= 3;
            }

            return GetBezierPoint(points[i], points[i + 1], points[i + 2], points[i + 3], _t);
        }

        public SplinePoint[] GetSpacedPointsReversed(float _spacing)
        {
            List<SplinePoint> spacedPoints = new List<SplinePoint>();

            Vector3 prevPoint = GetPointOnCurve(1f);
            float sqrSpacing = _spacing * _spacing;
            float tDiv = 1f / (float)outputResolution;

            // Search for Points Along Spline
            spacedPoints.Add(new SplinePoint(prevPoint, Quaternion.identity));
            for (float t = 1f; t >= 0f; t-=tDiv)
            {
                Vector3 nextPoint = GetPointOnCurve(t);
                if(Vector3.SqrMagnitude(nextPoint - prevPoint) >= sqrSpacing)
                {
                    prevPoint = nextPoint;
                    spacedPoints.Add(new SplinePoint(prevPoint, Quaternion.identity));
                }
            }

            // Return Empty Array If No Points Were Gathered
            if (spacedPoints.Count <= 1)
                return new SplinePoint[0];

            // Get Final Point For Gap Comparison
            prevPoint = GetPointOnCurve(0f);
            float error = Vector3.Distance(prevPoint, spacedPoints[spacedPoints.Count - 1].position) / (float)(spacedPoints.Count);

            // Orient Links and Fill Gap Error
            for (int p = spacedPoints.Count - 1; p >= 0; p--)
            {
                Vector3 direction = (prevPoint - spacedPoints[p].position).normalized;

                if (evenlyDistributPoints)
                    spacedPoints[p].position += ((error * p) * direction);

                spacedPoints[p].rotation = Quaternion.FromToRotation(Vector3.up, direction);
                prevPoint = spacedPoints[p].position;
            }

            // Return Point List
            return spacedPoints.ToArray();
        }

        private void UpdateCurveLength(int _resolution = 1000)
        {
            float itt = 1f / (float)_resolution;
            Vector3 pPnt = GetPointOnCurve(0);
            curveLength = 0;

            for(int i = 1; i <= _resolution; i++)
            {
                Vector3 tPnt = GetPointOnCurve(i * itt);
                curveLength += Vector3.Distance(pPnt, tPnt);
                pPnt = tPnt;
            }
        }
        
        public static Vector3 GetBezierPoint(Vector3 _p0, Vector3 _p1, Vector3 _p2, Vector3 _p3, float _t)
        {
            _t = Mathf.Clamp01(_t);
            float oneMinusT = 1f - _t;

            return oneMinusT * oneMinusT * oneMinusT * _p0 +
                3f * oneMinusT * oneMinusT * _t * _p1 +
                3f * oneMinusT * _t * _t * _p2 +
                _t * _t * _t * _p3;
        }
    }
}