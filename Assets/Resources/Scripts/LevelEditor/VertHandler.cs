﻿using FlipFall;
using FlipFall.Levels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Created handles and moves the moveArea verticies based on the handler's positions.
/// </summary>

namespace FlipFall.Editor
{
    [ExecuteInEditMode]
    public class VertHandler : MonoBehaviour
    {
        public GameObject handlePrefab;
        public GameObject handleParent;
        public Camera editorCamera;

        public int handleSize = 50;
        public bool showHandles = true;

        private Mesh mesh;
        private Vector3[] verts;
        private Vector3 vertPos;
        private GameObject[] handles;

        private bool handleDrag = false;
        private bool handlesShown = true;

        public static List<Handle> selectedHandles = new List<Handle>();
        public static Handle quickDragHandle;

        private void Awake()
        {
            selectedHandles = new List<Handle>();
            DestroyHandles();
        }

        private void Start()
        {
            Main.onSceneChange.AddListener(SceneChanged);

            if (LevelPlacer.generatedLevel != null)
            {
                //mesh = LevelEditor.editLevel.mergedMesh;

                mesh = LevelPlacer.generatedLevel.moveArea.meshFilter.mesh;

                handlesShown = true;

                verts = mesh.vertices;

                // crate handles
                if (showHandles)
                {
                    foreach (Vector3 vert in verts)
                    {
                        vertPos = LevelPlacer.generatedLevel.moveArea.transform.TransformPoint(vert);
                        GameObject handle = Instantiate(handlePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
                        handle.name = "handle";
                        handle.tag = "handle";
                        handle.layer = LayerMask.NameToLayer("Handles");
                        handle.transform.localScale = new Vector3(1, 1, 1);

                        if (handleParent != null)
                            handle.transform.parent = handleParent.transform;
                        else
                            handle.transform.parent = transform;

                        handle.transform.position = vertPos;

                        print(vertPos);
                    }
                }
            }
        }

        // destory handles
        private void OnDisable()
        {
            DestroyHandles();
        }

        private void DestroyHandles()
        {
            GameObject[] handles = GameObject.FindGameObjectsWithTag("handle");
            foreach (GameObject handle in handles)
            {
                DestroyImmediate(handle);
                handlesShown = false;
            }
        }

        private void SceneChanged(Main.Scene s)
        {
            showHandles = false;
        }

        public void OnClick()
        {
            handleDrag = true;
        }

        public void OnRelease()
        {
            handleDrag = false;
        }

        // update selection vertices based on handler position
        private void Update()
        {
            if (showHandles && !handlesShown)
                Start();
            else if (showHandles && LevelPlacer.generatedLevel != null && handlesShown)
            {
                handles = GameObject.FindGameObjectsWithTag("handle");
                for (int i = 0; i < verts.Length; i++)
                {
                    Vector3 localHandle = LevelPlacer.generatedLevel.moveArea.transform.InverseTransformPoint(handles[i].transform.position);
                    if (verts[i] != localHandle)
                        LevelEditor.changesAreSaved = false;
                    verts[i] = localHandle;
                }
                mesh.vertices = verts;
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();

                LevelPlacer.generatedLevel.moveArea.meshFilter.mesh = mesh;
            }
            else if (handlesShown)
                DestroyHandles();
        }

        private void VertexAdd()
        {
            if (Input.touchCount == 1)
            {
            }
        }
    }

    // compares for clockwise vertex positioning around a point
    public class ClockwiseComparer : IComparer<Vector3>
    {
        private Vector3 m_Origin;

        #region Properties

        /// <summary>
        ///     Gets or sets the origin.
        /// </summary>
        /// <value>The origin.</value>
        public Vector3 origin { get { return m_Origin; } set { m_Origin = value; } }

        #endregion Properties

        /// <summary>
        ///     Initializes a new instance of the ClockwiseComparer class.
        /// </summary>
        /// <param name="origin">Origin.</param>
        public ClockwiseComparer(Vector3 origin)
        {
            m_Origin = origin;
        }

        #region IComparer Methods

        /// <summary>
        ///     Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="first">First.</param>
        /// <param name="second">Second.</param>
        public int Compare(Vector3 v1, Vector3 v2)
        {
            return IsClockwise(v2, v1, m_Origin);
        }

        #endregion IComparer Methods

        /// <summary>
        ///     Returns 1 if first comes before second in clockwise order.
        ///     Returns -1 if second comes before first.
        ///     Returns 0 if the points are identical.
        /// </summary>
        /// <param name="first">First.</param>
        /// <param name="second">Second.</param>
        /// <param name="origin">Origin.</param>
        public static int IsClockwise(Vector3 first, Vector3 second, Vector3 origin)
        {
            if (first == second)
                return 0;

            Vector3 firstOffset = first - origin;
            Vector3 secondOffset = second - origin;

            float angle1 = Mathf.Atan2(firstOffset.x, firstOffset.y);
            float angle2 = Mathf.Atan2(secondOffset.x, secondOffset.y);

            if (angle1 < angle2)
                return 1;

            if (angle1 > angle2)
                return -1;

            // Check to see which point is closest
            return (firstOffset.sqrMagnitude < secondOffset.sqrMagnitude) ? 1 : -1;
        }
    }
}