using GameNetcodeStuff;
using LethalMenu.Types;
using LethalMenu.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;


namespace LethalMenu.Cheats
{
    internal class ESP : Cheat
    {

        private Dictionary<int, Material> materials = new Dictionary<int, Material>();
        private int _color;

        private Material chamsMaterial = new Material(Shader.Find("Hidden/Internal-Colored"))
        {
            hideFlags = HideFlags.DontSaveInEditor | HideFlags.HideInHierarchy
        };


        public void ApplyChams(MonoBehaviour obj)
        {
            if(obj == null) return;

            List<Renderer> renderers = obj.GetComponentsInChildren<Renderer>().ToList();

            
            if(obj.GetType() == typeof(DoorLock))
                renderers.AddRange(obj.GetComponentsInParent<Renderer>());


            renderers.ForEach(r =>
            {
                if (!materials.ContainsKey(r.GetInstanceID()))
                {
                    materials.Add(r.GetInstanceID(), r.material);
                    chamsMaterial.SetColor("_Color", Settings.c_chams.GetColor());
                    r.material = chamsMaterial;
                }
            });
        }

        public void RemoveChams(MonoBehaviour obj)
        {
            if(obj == null) return;

            obj.GetComponentsInChildren<Renderer>().ToList().ForEach(r =>
            {
                if(materials.TryGetValue(r.GetInstanceID(), out Material material))
                {
                    r.material = material;
                    materials.Remove(r.GetInstanceID());
                }
            });
        }

        private IEnumerator CleanUpMaterials()
        {
            while (true)
            {
                yield return new WaitForSeconds(15f);

                List<int> keep = new List<int>();

                LethalMenu.items.ForEach(i => i.GetComponentsInChildren<Renderer>().ToList().ForEach(r => keep.Add(r.GetInstanceID())));
                LethalMenu.landmines.ForEach(l => l.GetComponentsInChildren<Renderer>().ToList().ForEach(r => keep.Add(r.GetInstanceID())));
                LethalMenu.turrets.ForEach(t => t.GetComponentsInChildren<Renderer>().ToList().ForEach(r => keep.Add(r.GetInstanceID())));
                LethalMenu.doors.ForEach(d => d.GetComponentsInChildren<Renderer>().ToList().ForEach(r => keep.Add(r.GetInstanceID())));
                LethalMenu.players.ForEach(p => p.GetComponentsInChildren<Renderer>().ToList().ForEach(r => keep.Add(r.GetInstanceID())));
                LethalMenu.enemies.ForEach(e => e.GetComponentsInChildren<Renderer>().ToList().ForEach(r => keep.Add(r.GetInstanceID())));
                LethalMenu.steamValves.ForEach(s => s.GetComponentsInChildren<Renderer>().ToList().ForEach(r => keep.Add(r.GetInstanceID())));
                LethalMenu.bigDoors.ForEach(b => b.GetComponentsInChildren<Renderer>().ToList().ForEach(r => keep.Add(r.GetInstanceID())));
                LethalMenu.doorLocks.ForEach(d => d.GetComponentsInChildren<Renderer>().ToList().ForEach(r => keep.Add(r.GetInstanceID())));
                
                if(LethalMenu.shipDoor != null)
                    LethalMenu.shipDoor.GetComponentsInChildren<Renderer>().ToList().ForEach(r => keep.Add(r.GetInstanceID()));
                
                if(LethalMenu.breaker != null)
                    LethalMenu.breaker.GetComponentsInChildren<Renderer>().ToList().ForEach(r => keep.Add(r.GetInstanceID()));

                materials.Keys.ToList().FindAll(k => !keep.Contains(k)).ForEach(k => materials.Remove(k));

            }
        }

        public ESP()
        {
            
            chamsMaterial.SetInt("_SrcBlend", 5);
            chamsMaterial.SetInt("_DstBlend", 10);
            chamsMaterial.SetInt("_Cull", 0);
            chamsMaterial.SetInt("_ZTest", 8); // Render through walls
            chamsMaterial.SetInt("_ZWrite", 0);
            chamsMaterial.SetColor("_Color", Settings.c_chams.GetColor());
            
            LethalMenu.Instance.StartCoroutine(CleanUpMaterials());
        }

        public override void OnGui()
        {
            if (!(bool)StartOfRound.Instance) return;

            PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;

            if (player == null) return;

            try
            {
                if (Hack.ObjectESP.IsEnabled()) this.DisplayObjects();
                if (Hack.EnemyESP.IsEnabled()) this.DisplayEnemyAI();
                if (Hack.PlayerESP.IsEnabled()) this.DisplayPlayers();
                if (Hack.DoorESP.IsEnabled()) this.DisplayEntranceExitDoors();
                if (Hack.LandmineESP.IsEnabled()) this.DisplayLandmines();
                if (Hack.TurretESP.IsEnabled()) this.DisplayTurrets();
                if (Hack.ShipESP.IsEnabled()) this.DisplayShip();
                if (Hack.BigDoorESP.IsEnabled()) this.DisplayBigDoors();
                if (Hack.SteamHazardESP.IsEnabled()) this.DisplaySteamHazards();
                if (Hack.DoorLockESP.IsEnabled()) this.DisplayDoorLocks();
                if (Hack.BreakerESP.IsEnabled()) this.DisplayBreaker();
            }
            catch (Exception e)
            {
                LethalMenu.debugMessage = e.Message + "\n" + e.StackTrace;
            }

            

        }
            
        private void DisplayTurrets()
        {
            foreach (Turret turret in LethalMenu.turrets)
            {
                if (turret != null && turret.IsSpawned)
                {
                    string text = "Turret";
                    float distanceToPlayer = this.GetDistanceToPlayer(turret.transform.position);
                    if (distanceToPlayer > Settings.f_espDistance) continue;

                    Vector3 screen;

                    

                    if (!WorldToScreen(turret.transform.position, out screen)) continue;

                    TerminalAccessibleObject termObj = turret.GetComponent<TerminalAccessibleObject>();

                    text += " [ " + termObj.objectCode + " ]";

                    if (Settings.b_chamsTurret && distanceToPlayer >= Settings.f_chamDistance) ApplyChams(turret);
                    else RemoveChams(turret);

                    VisualUtil.DrawDistanceString(screen, text, Settings.c_turretESP, distanceToPlayer);
                }
            }
        }

        private void DisplayShip()
        {
            if (LethalMenu.shipDoor == null) return;
            Vector3 screen;
            float distanceToPlayer = this.GetDistanceToPlayer(LethalMenu.shipDoor.transform.position);
            if (distanceToPlayer > Settings.f_espDistance || !WorldToScreen(LethalMenu.shipDoor.transform.position, out screen)) return;

            if (Settings.b_chamsShip && distanceToPlayer >= Settings.f_chamDistance) ApplyChams(LethalMenu.shipDoor);
            else RemoveChams(LethalMenu.shipDoor);
            
            VisualUtil.DrawDistanceString(screen, "Ship", Settings.c_shipESP, distanceToPlayer);



        }

        private void DisplayBreaker()
        {
            if (LethalMenu.breaker == null) return;
            Vector3 screen;
            float distanceToPlayer = this.GetDistanceToPlayer(LethalMenu.breaker.transform.position);
            if (distanceToPlayer > Settings.f_espDistance || !WorldToScreen(LethalMenu.breaker.transform.position, out screen)) return;

            if (Settings.b_chamsBreaker && distanceToPlayer >= Settings.f_chamDistance) ApplyChams(LethalMenu.breaker);
            else RemoveChams(LethalMenu.breaker);
            
            VisualUtil.DrawDistanceString(screen, "Breaker Box", Settings.c_breakerESP, distanceToPlayer);

        }

        private void DisplayEntranceExitDoors()
        {
            foreach (EntranceTeleport door in LethalMenu.doors)
            {
                if (door == null) continue;
                Vector3 screen;
                string text = door.isEntranceToBuilding ? "Entrance" : "Exit";
                float distanceToPlayer = this.GetDistanceToPlayer(door.transform.position);
                if (distanceToPlayer > Settings.f_espDistance || !WorldToScreen(door.transform.position, out screen)) continue;


                if (Settings.b_chamsEntranceExit && distanceToPlayer >= Settings.f_chamDistance) ApplyChams(door);
                else RemoveChams(door);
                
                VisualUtil.DrawDistanceString(screen, text, Settings.c_entranceExitESP, distanceToPlayer); 
               
                
            }
        }

        private void DisplayLandmines()
        {
            foreach (Landmine landmine in LethalMenu.landmines)
            {
                if (landmine == null || !landmine.IsSpawned || landmine.hasExploded) continue;
                string text = "Landmine";
                float distanceToPlayer = this.GetDistanceToPlayer(landmine.transform.position);
                if (distanceToPlayer > Settings.f_espDistance) continue;

                Vector3 screen;
                if (!WorldToScreen(landmine.transform.position, out screen)) continue;

                TerminalAccessibleObject termObj = landmine.GetComponent<TerminalAccessibleObject>();

                text += " [ " + termObj.objectCode + " ]";

                if (Settings.b_chamsLandmine && distanceToPlayer >= Settings.f_chamDistance) ApplyChams(landmine);
                else RemoveChams(landmine);

                VisualUtil.DrawDistanceString(screen, text, Settings.c_landmineESP, distanceToPlayer);
            }
        }

        private void DisplayPlayers()
        {
            foreach (PlayerControllerB player in LethalMenu.players)
            {
                
                Vector3 screen;
                if (player == null || player.isPlayerDead || player.IsLocalPlayer || player.disconnectedMidGame || !WorldToScreen(player.playerEye.transform.position, out screen) || player.playerClientId == GameNetworkManager.Instance.localPlayerController.playerClientId) continue;

                
                string playerUsername = player.playerUsername;
                float distanceToPlayer = this.GetDistanceToPlayer(player.playerEye.transform.position);
                if (distanceToPlayer > Settings.f_espDistance) continue;

                if (Settings.b_chamsPlayer && distanceToPlayer >= Settings.f_chamDistance) ApplyChams(player);
                else RemoveChams(player);

                VisualUtil.DrawDistanceString(screen, playerUsername, Settings.c_playerESP, distanceToPlayer);
            }
        }

        private void DisplayEnemyAI()
        {

            foreach (EnemyAI enemyAi in LethalMenu.enemies)
            {
                if(!enemyAi.GetEnemyAIType().IsESPEnabled()) continue;

                Vector3 screen;

                if(enemyAi == null) continue;

                if (!WorldToScreen(enemyAi.transform.position, out screen) || enemyAi.isEnemyDead) continue;
                string enemyName = enemyAi.enemyType.enemyName;
                float distanceToPlayer = this.GetDistanceToPlayer(enemyAi.transform.position);
                if (distanceToPlayer > Settings.f_espDistance) continue;

                if (Settings.b_chamsEnemy && distanceToPlayer >= Settings.f_chamDistance) ApplyChams(enemyAi);
                else RemoveChams(enemyAi);

                VisualUtil.DrawDistanceString(screen, enemyName, Settings.c_enemyESP, distanceToPlayer);
            }
        }

        private void DisplayDeadBody(Vector3 screen, float distance, RagdollGrabbableObject body)
        {
            PlayerControllerB player = StartOfRound.Instance.allPlayerScripts[body.ragdoll.playerObjectId];
            RGBAColor color = Settings.c_playerESP;
            string text = player.playerUsername + "\n" + Settings.c_causeOfDeath.AsString(body.ragdoll.causeOfDeath.ToString());

            if(Settings.b_chamsPlayer && distance >= Settings.f_chamDistance) ApplyChams(body);
            else RemoveChams(body);

            VisualUtil.DrawDistanceString(screen, text, color, distance);
        }
        private void DisplayObjects()
        {
            foreach (GrabbableObject item in LethalMenu.items)
            {
                
                RGBAColor color = Settings.c_objectESP;
                Vector3 screen;
                if (item == null || item.isHeld || item.isPocketed || !item.IsSpawned || !WorldToScreen(item.transform.position, out screen)) continue;

                string text = "Object";
                float distanceToPlayer = this.GetDistanceToPlayer(item.transform.position);
                if (distanceToPlayer > Settings.f_espDistance) continue;

                if (item.GetType() == typeof(RagdollGrabbableObject))
                {
                    DisplayDeadBody(screen, distanceToPlayer, (RagdollGrabbableObject)item);
                    continue;
                }
                
                if(Settings.b_useScrapTiers)
                {
                    int index = Array.FindLastIndex<int>(Settings.i_scrapValueThresholds, x => x <= item.scrapValue);
                    if(index > -1) color = Settings.c_scrapValueColors[index];
                }



                if (item.itemProperties != null)
                {
                    if(item.itemProperties.itemName != null) text = item.itemProperties.itemName;
                    text += $" ({item.scrapValue}) " + item.itemProperties.weight;
                }

                if (Settings.b_chamsObject && distanceToPlayer >= Settings.f_chamDistance) ApplyChams(item);
                else RemoveChams(item);

                VisualUtil.DrawDistanceString(screen, text, color, distanceToPlayer);
            }
        }

        public void DisplaySteamHazards()
        {
            foreach (SteamValveHazard valve in LethalMenu.steamValves)
            {
                Vector3 screen;
                if (valve == null || !valve.triggerScript.interactable || !WorldToScreen(valve.transform.position, out screen)) continue;

                string text = "Steam Valve";
                float distanceToPlayer = this.GetDistanceToPlayer(valve.transform.position);

                if (Settings.b_chamsSteamHazard && distanceToPlayer >= Settings.f_chamDistance) ApplyChams(valve);
                else RemoveChams(valve);

                VisualUtil.DrawDistanceString(screen, text, Settings.c_steamHazardESP, distanceToPlayer);



            }
        }

        public void DisplayBigDoors()
        {
            foreach (TerminalAccessibleObject door in LethalMenu.bigDoors)
            {
                Vector3 screen;

                if (door == null || !WorldToScreen(door.transform.position, out screen)) continue;


                string text = "Big Door";
                float distanceToPlayer = this.GetDistanceToPlayer(door.transform.position);
                if (distanceToPlayer > Settings.f_espDistance) continue;

                text += " [ " + door.objectCode + " ]";

                if(Settings.b_chamsBigDoor && distanceToPlayer >= Settings.f_chamDistance) ApplyChams(door);
                else RemoveChams(door);

                
                VisualUtil.DrawDistanceString(screen, text, Settings.c_bigDoorESP, distanceToPlayer);
            }
        }

        public void DisplayDoorLocks()
        {
            foreach (DoorLock door in LethalMenu.doorLocks)
            {
                Vector3 screen;

                if (door == null || !WorldToScreen(door.transform.position, out screen)) continue;

                string text = "Locked Door";
                float distanceToPlayer = this.GetDistanceToPlayer(door.transform.position);
                if (distanceToPlayer > Settings.f_espDistance) continue;

                if (Settings.b_chamsDoorLock && distanceToPlayer >= Settings.f_chamDistance) ApplyChams(door);
                else RemoveChams(door);

                
                if(door.isLocked)
                    VisualUtil.DrawDistanceString(screen, text, Settings.c_doorLockESP, distanceToPlayer);
            }
        }

    }
}
