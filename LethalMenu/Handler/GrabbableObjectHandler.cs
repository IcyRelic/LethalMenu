using GameNetcodeStuff;
using LethalMenu.Types;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

namespace LethalMenu.Handler
{
    internal class GrabbableObjectHandler
    {

    }

    public static class GrabbableObjectExtensions
    {
        public static Bounds GetBounds(this GameObject gameObject)
        {
            Renderer renderer = gameObject.GetComponentInChildren<Renderer>();
            if (renderer != null) return renderer.bounds;
            Collider collider = gameObject.GetComponentInChildren<Collider>();
            if (collider != null) return collider.bounds;
            return new Bounds(Vector3.zero, Vector3.zero);
        }

        public static bool IsDefaultShipItem(this GrabbableObject GrabbableObject)
        {
            if (GrabbableObject == null) return false;
            string[] Items = ["ClipboardManual", "StickyNoteItem"];
            return Items.Contains(GrabbableObject.name);
        }

        public static void PlaceOnDesk(this GrabbableObject GrabbableObject, float wait = 0.1f) => LethalMenu.Instance.StartCoroutine(PlaceOnDeskWait(GrabbableObject, wait));

        public static IEnumerator PlaceOnDeskWait(this GrabbableObject GrabbableObject, float wait = 0.1f)
        {
            PlayerControllerB player = LethalMenu.players.Where(p => p != null && !p.isPlayerDead).FirstOrDefault();
            if (LethalMenu.depositItemsDesk == null) yield break;
            player.currentlyHeldObjectServer = GrabbableObject;
            LethalMenu.depositItemsDesk.PlaceItemOnCounter(player);
            yield return new WaitForSeconds(0.1f);
        }

        public static void ShootGunAsEnemy(this ShotgunItem shotgun, EnemyAI enemy)
        {
            shotgun.gunShootAudio.volume = 0.15f;
            shotgun.shotgunRayPoint = enemy.transform;
            shotgun.ShootGunAndSync(false);
        }
    }
}
