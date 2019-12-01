﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SplineMesh {
    /// <summary>
    /// Example of component to show the deformation of a mesh in a changing
    /// interval in spline space.
    /// 
    /// This component is only for demo purpose and is not intended to be used as-is.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Spline))]
    public class TrackContortion : MonoBehaviour {
        private Spline spline;
        private float distance = 0;
        private float contortionLength = 0;
        private List<MeshBender> meshBenders = new List<MeshBender>();

        [HideInInspector]
        public List<GameObject> generatedList = new List<GameObject>();

        public List<Mesh> meshes;
        public List<Material> materials;
        public List<Vector3> translations;
        public List<Vector3> rotations;
        public List<Vector3> scales;


        public float textureOffsetScale = 8.09f;
        public float Speed = 0;
        public int Repeat = 0;

        private void OnEnable() {
            distance = 0;
            Init();
#if UNITY_EDITOR
            EditorApplication.update += EditorUpdate;
#endif
        }

        void OnDisable() {
#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
#endif
        }

        private void OnValidate() {
            Init();
        }

        void EditorUpdate() {
            distance += Time.deltaTime * Speed;
            if (distance >= spline.Length) {
                distance = 0;
            }
            Contort();
        }

        private void Contort() {
            if (generatedList != null)
            {                
                float contortionStart = distance - contortionLength / 2;
                float contortionEnd = distance + contortionLength / 2;

                // fix indexes
                if (contortionStart < 0)
                {
                    contortionStart = 0;
                    //contortionEnd = contortionStart + contortionLength;
                }
                // fix indexes
                if (contortionEnd >= spline.Length)
                {
                    contortionEnd = spline.Length;
                    //contortionStart = contortionEnd - contortionLength;
                }

                for (int i = 0; i < meshBenders.Count; i++)
                {
                    meshBenders[i].SetInterval(spline, contortionStart, contortionEnd);
                    meshBenders[i].ComputeIfNeeded();
                }

                // Move the texture on the mesh backwards to fake a sense of speed
                for (int i = 0; i < generatedList.Count; i++)
                {
                    float offsetX = 0;
                    float offsetY = Time.time * Speed / textureOffsetScale;

                    // Works in editor, but applies changes directly to the assets, which is bad
                    //generatedList[0].GetComponent<Renderer>().sharedMaterial.mainTextureOffset = new Vector2(offsetX, offsetY);

                    // Doesn't work in editor
                    generatedList[0].GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offsetX, offsetY);
                }
            }
        }

        private void Init() {
            for (int i = 0; i < meshes.Count; i++)
            {
                string generatedName = i + ". generated by " + GetType().Name;
                var generatedTranform = transform.Find(generatedName);

                GameObject generated = generatedTranform != null ? generatedTranform.gameObject : UOUtility.Create(generatedName, gameObject,
                    typeof(MeshFilter),
                    typeof(MeshRenderer),
                    typeof(MeshBender));

                // some harmless bug TODO
                if (generatedList.Count < i + 1)
                    generatedList.Add(generated);
                else
                    generatedList[i] = generated;

                generatedList[i].GetComponent<MeshRenderer>().material = materials[i];

                MeshBender meshBender = generatedList[i].GetComponent<MeshBender>();

                if (meshBenders.Count < i + 1)
                    meshBenders.Add(meshBender);
                else
                    meshBenders[i] = meshBender;

                meshBenders[i] = generatedList[i].GetComponent<MeshBender>();
                spline = GetComponent<Spline>();

                meshBenders[i].Source = SourceMesh.Build(meshes[i])
                    .Translate(translations[i])
                    .Rotate(Quaternion.Euler(rotations[i]))
                    .Scale(scales[i]);
                meshBenders[i].Mode = MeshBender.FillingMode.Repeat;
                meshBenders[i].SetInterval(spline, 0);
            }

            // First mesh determines lenght of generation
            contortionLength = meshBenders[0].Source.Length * ((float)(Repeat) + 0.1f);
        }
    }
}
