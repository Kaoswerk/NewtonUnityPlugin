﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NewtonPlugin
{

    abstract public class NewtonCollider : MonoBehaviour
    {
        private List<Line> lines = null;
        Color col = new Color(0.6f, 1.0f, 0.6f);

        public Vector3 Scale = Vector3.one;

        void OnDrawGizmosSelected()
        {
            //if (this.GetType() == typeof(NewtonTreeCollider))
            //    return; // Don't render tree collider, too heavy...

            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

            if (lines == null)
            {
                lines = new List<Line>();
                UpdateLines();
            }

            DrawLines();

        }

        void OnValidate()
        {
            //if (this.GetType() == typeof(NewtonTreeCollider))
            //    return; // Don't render tree collider, too heavy...

            if (lines == null)
                lines = new List<Line>();
            else
                lines.Clear();

            UpdateLines();
        }

        void UpdateLines()
        {

            //Debug.Log("Building debug lines for " + this.name);

            //GCHandle gch = GCHandle.Alloc(lines);
            //IntPtr collider = CreateCollider(false);
            //if (collider != IntPtr.Zero)
            //{
            //    Matrix4x4 offsetMatrix = Matrix4x4.identity;
            //    NewtonAPI.NewtonCollisionForEachPolygonDo(collider, ref offsetMatrix, NewtonCollisionIterator, GCHandle.ToIntPtr(gch));
            //    gch.Free();
            //    NewtonAPI.NewtonDestroyCollision(collider);
            //}

        }

        void DrawLines()
        {
            Gizmos.color = col;

            foreach (Line line in lines)
            {
                Gizmos.DrawLine(line.pA, line.pB);
            }
        }


        float[] points = new float[64 * 3]; // Reuse the same buffer

        void NewtonCollisionIterator(IntPtr userData, int vertexCount, IntPtr faceArray, int faceId)
        {
            GCHandle gch = GCHandle.FromIntPtr(userData);

            Debug.Log(userData.ToString());

            List<Line> lines = (List<Line>)gch.Target;

            //float[] points = new float[vertexCount * 3];
            Marshal.Copy(faceArray, points, 0, vertexCount * 3);

            Vector3 pA = Vector3.zero;
            Vector3 pB = Vector3.zero;
            Line line;
            for (int i = 0; i < vertexCount - 1; i++)
            {
                pA = new Vector3(points[i * 3], points[i * 3 + 1], points[i * 3 + 2]);
                pB = new Vector3(points[(i + 1) * 3], points[(i + 1) * 3 + 1], points[(i + 1) * 3 + 2]);
                line = new Line();
                line.pA = pA;
                line.pB = pB;
                lines.Add(line);
            }
            pA = new Vector3(points[0], points[1], points[2]);
            line = new Line();
            line.pA = pA;
            line.pB = pB;
            lines.Add(line);

        }

        abstract public IntPtr CreateCollider(bool applyOffset);

    }

    public struct Line
    {
        public Vector3 pA;
        public Vector3 pB;
    }

}
