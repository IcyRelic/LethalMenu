using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using LethalMenu.Types;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LethalMenu.Handler;

public class ChamHandler
{
    //private static Dictionary<int, List<Renderer>> renderers = new Dictionary<int, List<Renderer>>();
    private static readonly Dictionary<int, Material[]> Materials = new();
    private static Material _mChamMaterial;
    private static int _color;

    // Shader properties
    private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
    private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
    private static readonly int Cull = Shader.PropertyToID("_Cull");
    private static readonly int ZTest = Shader.PropertyToID("_ZTest");
    private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");
    private static readonly int Color1 = Shader.PropertyToID("_Color");

    private readonly Object _object;

    private ChamHandler(Object obj)
    {
        _object = obj;
    }

    public static void SetupChamMaterial()
    {
        _mChamMaterial = new Material(Shader.Find("Hidden/Internal-Colored"))
        {
            hideFlags = HideFlags.DontSaveInEditor | HideFlags.HideInHierarchy
        };

        _mChamMaterial.SetInt(SrcBlend, 5);
        _mChamMaterial.SetInt(DstBlend, 10);
        _mChamMaterial.SetInt(Cull, 0);
        _mChamMaterial.SetInt(ZTest, 8);
        _mChamMaterial.SetInt(ZWrite, 0);
        _mChamMaterial.SetColor(Color1, Settings.c_chams.GetColor());
        _color = Shader.PropertyToID("_Color");

        LethalMenu.Instance.StartCoroutine(CleanUpMaterials());
    }

    private List<Renderer> GetRenderers()
    {
        var renderers = new List<Renderer>();

        if (_object == null) return renderers;


        switch (_object)
        {
            case GameObject gameObject:
                renderers.AddRange(gameObject.GetComponentsInChildren<Renderer>());
                break;
            case Component component:
                renderers.AddRange(component.GetComponentsInChildren<Renderer>());
                break;
        }

        if (_object is DoorLock @lock) renderers.AddRange(@lock.GetComponentsInParent<Renderer>());

        return renderers;
    }

    public void RemoveCham()
    {
        GetRenderers().ForEach(r =>
        {
            if (!Materials.ContainsKey(r.GetInstanceID())) return;

            r.SetMaterials(Materials[r.GetInstanceID()].ToList());
            Materials.Remove(r.GetInstanceID());
        });
    }

    public void ProcessCham(float distance)
    {
        if (_object == null) return;

        var e = _object switch
        {
            GrabbableObject => Settings.b_chamsObject,
            Landmine => Settings.b_chamsLandmine,
            GameObject when _object.name.StartsWith("TurretContainer") => Settings.b_chamsTurret,
            PlayerControllerB => Settings.b_chamsPlayer,
            EnemyAI enemy => enemy.GetEnemyAIType().IsEspEnabled() && Settings.b_chamsEnemy,
            SteamValveHazard => Settings.b_chamsSteamHazard,
            TerminalAccessibleObject { isBigDoor: true } => Settings.b_chamsBigDoor,
            DoorLock => Settings.b_chamsDoorLock,
            HangarShipDoor => Settings.b_chamsShip,
            BreakerBox => Settings.b_chamsBreaker,
            _ => false
        };

        if (e && distance >= Settings.f_chamDistance) ApplyCham();
        else RemoveCham();

        if (_object is GrabbableObject { isHeld: true } ||
            (_object is SteamValveHazard hazard && !hazard.triggerScript.interactable))
            RemoveCham();
    }

    public void ApplyCham()
    {
        if (_object == null) return;

        GetRenderers().ForEach(r =>
        {
            if (!r) return;

            if (Materials.ContainsKey(r.GetInstanceID())) return;
            if (r.materials == null) return;

            Materials.Add(r.GetInstanceID(), r.materials);
            r.SetMaterials(Enumerable.Repeat(_mChamMaterial, r.materials.Length).ToList());
            UpdateChamColor(r);
        });
    }

    private void UpdateChamColor(Renderer r)
    {
        if (!r || r.materials == null) return;
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

            var keep = new List<int>();
            Object.FindObjectsOfType<Renderer>().ToList().ForEach(r => keep.Add(r.GetInstanceID()));

            Materials.Keys.ToList().FindAll(k => !keep.Contains(k)).ForEach(k => { Materials.Remove(k); });
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