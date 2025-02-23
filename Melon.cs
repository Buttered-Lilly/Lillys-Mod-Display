using MelonLoader;
using Mirror.SimpleWeb;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[assembly: MelonInfo(typeof(Lillys_Mod_Display.MelonLoad), "Lillys Mod Display", "1.0.0", "Lilly", null)]
[assembly: MelonGame("KisSoft", "ATLYSS")]
[assembly: MelonOptionalDependencies("BepInEx")]

namespace Lillys_Mod_Display
{
    public class MelonLoad : MelonMod
    {
        ModDisplayCore disCore = null;
        public override void OnInitializeMelon()
        {
            if (ModDisplayCore.DisplayCoreInstance != null)
                return;
            GameObject g = GameObject.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube));
            g.hideFlags = UnityEngine.HideFlags.HideAndDontSave;
            disCore = g.AddComponent<ModDisplayCore>();
            foreach (var melon in MelonBase.RegisteredMelons)
            {
                try
                {
                    disCore.ModList.Add(melon.Info.Name);
                }
                catch (Exception e)
                {
                    MelonLogger.Msg(e);
                }
            }
            disCore.Logger = logger;
        }

        public bool logger(string mesg)
        {
            MelonLogger.Msg(mesg);
            return true;
        }
    }
}
