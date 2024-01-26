using LethalMenu.Menu.Core;
using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalMenu.Util
{


    public class MenuUtil
    {
        private static readonly string encryptionKey = "743e5f0a42c826b72f8921491ee523b5";
        public static bool resizing = false;
        public static float MouseX => Mouse.current.position.value.x;
        public static float MouseY => Screen.height - Mouse.current.position.value.y;
        public static float maxWidth = Screen.width - (Screen.width * 0.1f);
        public static float maxHeight = Screen.height - (Screen.height * 0.1f);
        private static int oldWidth, oldHeight;


        public static void BeginResizeMenu()
        {
            if (resizing) return;
            WarpCursor();
            resizing = true;
            oldWidth = Settings.i_menuWidth;
            oldHeight = Settings.i_menuHeight;
        }

        public static void WarpCursor()
        {
            float currentX = HackMenu.Instance.windowRect.x + HackMenu.Instance.windowRect.width;
            //get an inverted screen position for the height
            float currentY = Screen.height - (HackMenu.Instance.windowRect.y + HackMenu.Instance.windowRect.height);

            Mouse.current.WarpCursorPosition(new Vector2(currentX, currentY));


        }
        
        public static void ResizeMenu()
        {
            if(!resizing) return;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                resizing = false;
                Settings.Config.SaveConfig();
                return;
            }

            if(Mouse.current.rightButton.wasPressedThisFrame)
            {
                resizing = false;
                Settings.i_menuWidth = oldWidth;
                Settings.i_menuHeight = oldHeight;
                //HackMenu.Instance.Resize();
                Settings.Config.SaveConfig();
                return;
            }

            


            float currentX = HackMenu.Instance.windowRect.x + HackMenu.Instance.windowRect.width;
            float currentY = HackMenu.Instance.windowRect.y + HackMenu.Instance.windowRect.height;

            Settings.i_menuWidth = (int) Mathf.Clamp(MouseX - HackMenu.Instance.windowRect.x, 500, maxWidth);
            Settings.i_menuHeight = (int) Mathf.Clamp(MouseY - HackMenu.Instance.windowRect.y, 250, maxHeight);
            HackMenu.Instance.Resize();
        }
    
        public static void ShowCursor()
        {
            LethalMenu.localPlayer.playerActions.Disable();
            Cursor.visible = true;
            Settings.clm_lastCursorState = Cursor.lockState;
            Cursor.lockState = CursorLockMode.Confined;
        }

        public static void HideCursor()
        {
            LethalMenu.localPlayer.playerActions.Enable();
            Cursor.visible = false;
            Cursor.lockState = Settings.clm_lastCursorState;
        }

        public static string Encrypt(string text)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(encryptionKey.PadRight(32, '\0').Substring(0, 32));
                aesAlg.IV = new byte[16];

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(text);
                    }

                    return $"encrypted_{encryptionKey}_{Convert.ToBase64String(msEncrypt.ToArray())}";
                }
            }
        }

        public static string Decrypt(string encryptedText, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key.PadRight(32, '\0').Substring(0, 32));
                aesAlg.IV = new byte[16];

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        public static bool IsEncrypted(string text)
        {
            return text.StartsWith("encrypted_");
        }
        public static void ProcessEncryptedMessage(string message)
        {
            if(!IsEncrypted(message)) return;

            string key = message.Split('_')[1];
            string encryptedText = message.Split('_')[2];

            string decryptedText = Decrypt(encryptedText, key);

            if(decryptedText.Contains("|")) {
                string steamID = decryptedText.Split('|')[0];
                string version = decryptedText.Split('|')[1];

                if(!LethalMenu.lmUsers.ContainsKey(ulong.Parse(steamID)))
                    LethalMenu.lmUsers.Add(ulong.Parse(steamID), version);
            } 
            

            
        }
    }
}
