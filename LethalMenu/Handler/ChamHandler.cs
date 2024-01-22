using GameNetcodeStuff;
using HarmonyLib;
using LethalMenu.Util;
using Steamworks.Ugc;
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
        //private static Dictionary<int, List<Renderer>> renderers = new Dictionary<int, List<Renderer>>();
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
            List<Renderer> renderers = new List<Renderer>();

            if(@object == null) return renderers;

            
            if (@object is GameObject) renderers.AddRange(((GameObject)@object).GetComponentsInChildren<Renderer>());
            if (@object is Component) renderers.AddRange(((Component)@object).GetComponentsInChildren<Renderer>());
            if (@object is DoorLock) renderers.AddRange(((DoorLock)@object).GetComponentsInParent<Renderer>());

            return renderers;
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

        public void ProcessCham(float distance)
        {
            if(@object == null) return;

            bool e = false;

            if (@object is GrabbableObject) e = Settings.b_chamsObject;
            if (@object is Landmine) e = Settings.b_chamsLandmine;
            if (@object is GameObject && @object.name.StartsWith("TurretContainer")) e = Settings.b_chamsTurret;
            if (@object is PlayerControllerB) e = Settings.b_chamsPlayer;
            if (@object is EnemyAI) e = Settings.b_chamsEnemy;
            if (@object is SteamValveHazard) e = Settings.b_chamsSteamHazard;
            if (@object is TerminalAccessibleObject && ((TerminalAccessibleObject)@object).isBigDoor) e = Settings.b_chamsBigDoor;
            if (@object is DoorLock) e = Settings.b_chamsDoorLock;
            if (@object is HangarShipDoor) e = Settings.b_chamsShip;
            if (@object is BreakerBox) e = Settings.b_chamsBreaker;


            if(e && distance >= Settings.f_chamDistance) ApplyCham();
            else RemoveCham();

            if (   (@object is GrabbableObject && ((GrabbableObject)@object).isHeld)
                || (@object is SteamValveHazard && !((SteamValveHazard)@object).triggerScript.interactable)
                
            ) RemoveCham();

        }

        public void ApplyCham()
        {
            if (@object == null) return;

            GetRenderers().ForEach(r =>
            {
                if(r == null) return;

                if (!materials.ContainsKey(r.GetInstanceID()))
                {
                    if(r.materials == null) return;

                    materials.Add(r.GetInstanceID(), r.materials);
                    r.SetMaterials(Enumerable.Repeat(m_chamMaterial, r.materials.Length).ToList());
                    UpdateChamColor(r);
                }

            }); 
        }

        private void UpdateChamColor(Renderer r)
        {
            if(r == null || r.materials == null) return;
            r.materials.ToList().ForEach(m => m.SetColor(_color, Settings.c_chams.GetColor()));

        }


        public static ChamHandler GetHandler(Object obj)
        {
            return new ChamHandler(obj);
        }

        private static IEnumerator CleanUpMaterials()
        {
            while (true)
            {
                yield return new WaitForSeconds(15f);
                int cnt = 0;
                List<int> keep = new List<int>();
                Object.FindObjectsOfType<Renderer>().ToList().ForEach(r => keep.Add(r.GetInstanceID()));

                materials.Keys.ToList().FindAll(k => !keep.Contains(k)).ForEach(k => { materials.Remove(k); cnt++; }) ;

            }
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
