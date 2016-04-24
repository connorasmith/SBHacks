using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PicoGames.Utilities
{
    public class Triangulate
    {
        private static Vector3[] shape;
        public static int[] Edge(Vector3[] _shape)
        {
            shape = _shape;
            List<int> output = new List<int>();

            int n = shape.Length;
            if (n < 3)
                return output.ToArray();

            int[] V = new int[n];
            if(Area() > 0)
            {
                for (int v = 0; v < n; v++)
                    V[v] = v;
            }
            else
            {
                for (int v = 0; v < n; v++)
                    V[v] = (n - 1) - v;
            }

            int nv = n;
            int count = 2 * nv;
            for (int m = 0, v = nv - 1; nv > 2; )
            {
                if ((count--) <= 0)
                    return output.ToArray();

                int u = v;
                if (nv <= u)
                    u = 0;

                v = u + 1;
                if (nv <= v)
                    v = 0;

                int w = v + 1;
                if (nv <= w)
                    w = 0;

                if(Snip(u, v, w, nv, V))
                {
                    int a, b, c, s, t;

                    a = V[u];
                    b = V[v];
                    c = V[w];

                    output.Add(a);
                    output.Add(b);
                    output.Add(c);

                    m++;
                    for (s = v, t = v + 1; t < nv; s++, t++)
                        V[s] = V[t];

                    nv--;
                    count = 2 * nv;
                }
            }

            output.Reverse();
            return output.ToArray();
        }

        private static float Area()
        {
            int n = shape.Length;
            float A = 0f;

            for(int p = n - 1, q = 0; q < n; p = q++)
            {
                Vector3 pval = shape[p];
                Vector3 qval = shape[q];
                A += pval.x * qval.y - qval.x * pval.y;
            }

            return (A * 0.5f);
        }

        private static bool OverlapsPoint(Vector3 _t1, Vector3 _t2, Vector3 _t3, Vector3 _p1)
        {
            float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
            float cCross, bCross, aCross;

            ax = _t3.x - _t2.x; ay = _t3.y - _t2.y;
            bx = _t1.x - _t3.x; by = _t1.y - _t3.y;
            cx = _t2.x - _t1.x; cy = _t2.y - _t1.y;

            apx = _p1.x - _t1.x; apy = _p1.y - _t1.y;
            bpx = _p1.x - _t2.x; bpy = _p1.y - _t2.y;
            cpx = _p1.x - _t3.x; cpy = _p1.y - _t3.y;

            aCross = ax * bpy - ay * bpx;
            cCross = cx * apy - cy * apx;
            bCross = bx * cpy - by * cpx;

            return ((aCross >= 0f) && (bCross >= 0f) && (cCross >= 0f));
        }

        private static bool Snip(int u, int v, int w, int n, int[] V)
        {
            Vector3 A = shape[V[u]];
            Vector3 B = shape[V[v]];
            Vector3 C = shape[V[w]];

            if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
                return false;

            for(int p = 0; p < n; p++)
            {
                if ((p == u) || (p == v) || (p == w))
                    continue;

                Vector3 P = shape[V[p]];
                if (OverlapsPoint(A, B, C, P))
                    return false;
            }

            return true;
        }
    }
}