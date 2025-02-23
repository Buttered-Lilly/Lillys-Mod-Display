using HarmonyLib;
using UnityEngine;
using Mirror;
using Steamworks;
using System.Text;
using MelonLoader;
using TMPro;


namespace Lillys_Mod_Display
{
    public class ModDisplayCore : MonoBehaviour
    {
        public static ModDisplayCore DisplayCoreInstance;
        public List<string> ModList = new List<string>();
        public string ModListString = "";
        public Func<string, bool> Logger;

        protected Callback<LobbyChatMsg_t> messageRecived;

        public IDictionary<string, List<string>> usersMods;

        TextMeshProUGUI modlister;

        void Start()
        {
            DisplayCoreInstance = this;
            messageRecived = Callback<LobbyChatMsg_t>.Create(onMessage);
            usersMods = new Dictionary<string,List<string>>();
            ModListString += "MODLIST";
            foreach (string name in ModList)
            {
                ModListString += ",";
                ModListString += name;
            }
            Logger("in core: " + ModListString);
        }

        [HarmonyPatch(typeof(WhoListDataEntry), "Select_Entry")]
        public static class setList
        {
            [HarmonyPrefix]
            private static void Postfix(ref WhoListDataEntry __instance)
            {
                try
                {
                    string list = "Mods:\n\n";

                    foreach (string mod in DisplayCoreInstance.usersMods[__instance._player._steamID])
                    {
                        list += mod;
                        list += "\n";
                    }

                    DisplayCoreInstance.modlister.text = list;
                }
                catch (Exception e)
                {
                    DisplayCoreInstance.modlister.text = "No List Found";
                }
            }
        }

        [HarmonyPatch(typeof(Player), "OnGameConditionChange")]
        public static class playerLoaded
        {
            [HarmonyPrefix]
            private static void Prefix(ref Player __instance)
            {
                try
                {
                    if (__instance.Network_currentGameCondition == GameCondition.IN_GAME)
                    {
                        byte[] bytes = Encoding.ASCII.GetBytes(ModDisplayCore.DisplayCoreInstance.ModListString);
                        CSteamID steamID = new CSteamID(SteamLobby._current._currentLobbyID);
                        SteamMatchmaking.SendLobbyChatMsg(steamID, bytes, bytes.Length);
                    }
                    if(__instance.Network_currentGameCondition == GameCondition.IN_GAME && DisplayCoreInstance.modlister == null)
                    {
                        DisplayCoreInstance.Logger("adding list");
                        DisplayCoreInstance.usersMods = new Dictionary<string, List<string>>();
                        GameObject g = Instantiate(new GameObject(), GameObject.Find("_GameUI_TabMenu/Canvas_InGameMenu/Dolly_MenuIndex/_cell_whoMenu/_steamworksPanel/").transform);
                        DisplayCoreInstance.modlister = g.AddComponent<TextMeshProUGUI>();
                        g.transform.localPosition = new Vector3(-280, -25, 0);
                        g.transform.localScale = Vector3.one * 0.75f;
                        DisplayCoreInstance.modlister.text = "";
                        DisplayCoreInstance.modlister.enableAutoSizing = true;
                        DisplayCoreInstance.modlister.paragraphSpacing = 55;
                    }
                }
                catch (Exception e)
                {
                    DisplayCoreInstance.Logger(e.Message);
                    DisplayCoreInstance.Logger("missing");
                }
            }
        }


        void onMessage(LobbyChatMsg_t callback)
        {
            try
            {
                /*if ((CSteamID)callback.m_ulSteamIDUser == SteamUser.GetSteamID())
                {
                    return;
                }*/
                int bufferSize = 32000;
                byte[] data = new byte[bufferSize];
                CSteamID sender;
                EChatEntryType chatType;
                SteamMatchmaking.GetLobbyChatEntry((CSteamID)callback.m_ulSteamIDLobby, (int)callback.m_iChatID, out sender, data, bufferSize, out chatType);
                string message = Encoding.ASCII.GetString(data);
                //Logger(message);
                if (message.Contains("MODLIST"))
                {
                    //Logger(message);
                    List<string> mods = message.Split(',').ToList();
                    mods.Remove("MODLIST");
                    findPlayer((CSteamID)callback.m_ulSteamIDUser, mods);
                }
            }
            catch (Exception e)
            {
                //Logger(e.Message);
                //Logger("recive failed");
            }
        }



        void findPlayer(CSteamID steamID, List<string> mods)
        {
            try
            {
                usersMods.Add(steamID.ToString(), mods);
            }
            catch(Exception e)
            {
                //Logger(e.Message);
                //Logger("Dic Error");
            }
        }
    }
}