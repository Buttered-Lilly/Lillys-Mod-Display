using BepInEx;
using UnityEngine;
using HarmonyLib;
using BepInEx.Bootstrap;


namespace Lillys_Mod_Display
{
    [BepInPlugin("516b9dfb-3ee3-4483-9be3-f685db5717ac", "Lillys Mod Display", "1.0.0")]
    public class Bepin : BaseUnityPlugin
    {
        ModDisplayCore disCore;
        private void Awake()
        {
            if (ModDisplayCore.DisplayCoreInstance != null)
                return;
            GameObject g = GameObject.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube));
            g.hideFlags = UnityEngine.HideFlags.HideAndDontSave;
            disCore = g.AddComponent<ModDisplayCore>();
            disCore.Logger = logger;
            foreach (var mod in Chainloader.PluginInfos)
            {
                try
                {
                    disCore.ModList.Add(mod.Value.Metadata.Name);
                }
                catch (Exception e)
                {

                }
            }
            var harmony = new HarmonyLib.Harmony("Lillys Mod Display");
            harmony.PatchAll();
        }

        public bool logger(string mesg)
        {
            Logger.LogInfo($"{mesg}");
            return true;
        }
    }
}
