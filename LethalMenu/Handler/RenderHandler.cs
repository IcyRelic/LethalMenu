using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LethalMenu.Handler
{
    public class RenderHandler
    {
        private static Dictionary<int, Material[]> materials = new Dictionary<int, Material[]>();
        private static Material m_chamMaterial;
        private static int _color;
        private Renderer r;

        public RenderHandler(Renderer renderer)
        {
            r = renderer;
        }

        public static void SetupChamMaterial()
        {
            m_chamMaterial = new Material(Shader.Find("Hidden/Internal-Colored"))
            {
                hideFlags = HideFlags.DontSaveInEditor | HideFlags.HideInHierarchy
            };

            m_chamMaterial.SetInt("_SrcBlend", 5);
            m_chamMaterial.SetInt("_DstBlend", 10);
            m_chamMaterial.SetInt("_Cull", 0);
            m_chamMaterial.SetInt("_ZTest", 8);
            m_chamMaterial.SetInt("_ZWrite", 0);
            m_chamMaterial.SetColor("_Color", Settings.c_chams.GetColor());
            _color = Shader.PropertyToID("_Color");

            LethalMenu.Instance.StartCoroutine(CleanUpMaterials());
        }

        public void RemoveCham()
        {
            if (r == null) return;

            if (materials.TryGetValue(r.GetInstanceID(), out Material[] mats))
            {
                r.SetMaterials(mats.ToList());
                materials.Remove(r.GetInstanceID());
            }
        }
        public void ApplyCham()
        {
            if (r == null) return;

            if (!materials.ContainsKey(r.GetInstanceID()))
            {
                materials.Add(r.GetInstanceID(), r.materials);
                r.SetMaterials(Enumerable.Repeat(m_chamMaterial, r.materials.Length).ToList());
            }

            UpdateChamColor();
        }
        private void UpdateChamColor() => r.materials.ToList().ForEach(m => m.SetColor(_color, Settings.c_chams.GetColor()));

        public static RenderHandler GetHandler(Renderer renderer)
        {
            return new RenderHandler(renderer);
        }
        private static IEnumerator CleanUpMaterials()
        {
            while (true)
            {
                yield return new WaitForSeconds(15f);

                List<int> keep = new List<int>();
                Object.FindObjectsOfType<Renderer>().ToList().ForEach(r => keep.Add(r.GetInstanceID()));

                materials.Keys.ToList().FindAll(k => !keep.Contains(k)).ForEach(k => materials.Remove(k));
            }
        }
    }

    public static class RendererExtensions
    {
        public static RenderHandler Handle(this Renderer renderer)
        {
            return RenderHandler.GetHandler(renderer);
        }
    }
}
