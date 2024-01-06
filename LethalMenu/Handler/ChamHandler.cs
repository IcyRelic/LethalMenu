using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LethalMenu.Handler
{
    public class ChamHandler
    {
        private static Dictionary<int, List<Renderer>> renderers = new Dictionary<int, List<Renderer>>();
        private static Dictionary<int, Material[]> materials = new Dictionary<int, Material[]>();
        private static Material m_chamMaterial;
        private static int _color;

        private Object @object;

        public ChamHandler(Object obj)
        {
            @object = obj;
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

        private List<Renderer> GetRenderers()
        {
            if (renderers.TryGetValue(@object.GetInstanceID(), out List<Renderer> r)) return r;

            List<Renderer> newRenderers = new List<Renderer>();

            if(@object == null) return newRenderers;

            
            if (@object is GameObject) newRenderers.AddRange(((GameObject)@object).GetComponentsInChildren<Renderer>());
            if (@object is Component) newRenderers.AddRange(((Component)@object).GetComponentsInChildren<Renderer>());
            if (@object is DoorLock) newRenderers.AddRange(((DoorLock)@object).GetComponentsInParent<Renderer>());



            renderers.Add(@object.GetInstanceID(), newRenderers);

            return newRenderers;
        }

        public void RemoveCham()
        {
            GetRenderers().ForEach(r =>
            {
                if (materials.ContainsKey(r.GetInstanceID()))
                {
                    r.SetMaterials(materials[r.GetInstanceID()].ToList());
                    materials.Remove(r.GetInstanceID());
                }
            });
        }

        public void ApplyCham()
        {
            if (@object == null) return;

            GetRenderers().ForEach(r =>
            {
                if (!materials.ContainsKey(r.GetInstanceID()))
                {
                    materials.Add(r.GetInstanceID(), r.materials);
                    r.SetMaterials(Enumerable.Repeat(m_chamMaterial, r.materials.Length).ToList());
                }

                UpdateChamColor(r);
            }); 
        }
        private void UpdateChamColor(Renderer r) => r.materials.ToList().ForEach(m => m.SetColor(_color, Settings.c_chams.GetColor()));

        public static ChamHandler GetHandler(Object obj)
        {
            return new ChamHandler(obj);
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

        public static explicit operator ChamHandler(PlayerHandler v)
        {
            throw new NotImplementedException();
        }
    }

    public static class RendererExtensions
    {
        public static ChamHandler GetChamHandler(this Object obj)
        {
            return ChamHandler.GetHandler(obj);
        }
    }
}
