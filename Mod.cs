using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MelonLoader;
using UnityEngine;
using HarmonyLib;

namespace PPDuckSim_Camera_Lock
{
    public class Mod : MelonMod
    {
        public static Mod Instance { get { return Mod._instance; } }
        private static Mod _instance { get; set; }

        public MelonPreferences_Category preferences;
        public MelonPreferences_Entry<float> maxCameraDistance;
        public MelonPreferences_Entry<bool> disableChairs;
        public MelonPreferences_Entry<bool> lockToFloatingCam;

        public override void OnInitializeMelon()
        {
            _instance = this;
            preferences = MelonPreferences.CreateCategory("CameraLockCategory");
            maxCameraDistance = preferences.CreateEntry<float>("MaxCameraDistance", 40f);
            disableChairs = preferences.CreateEntry<bool>("DisableChairs", true);
            lockToFloatingCam = preferences.CreateEntry<bool>("LockToFloatingCam", true);

            HarmonyLib.Harmony harmony = new HarmonyLib.Harmony("dev.kk964.cameralock");
            harmony.PatchAll();
        }

        public override void OnUpdate()
        {
            if (_instance == null) return;
            SmoothMouseLook smoothMouseLook = GameObject.FindObjectOfType<SmoothMouseLook>();
            if (smoothMouseLook != null)
            {
                smoothMouseLook.zoomMax = maxCameraDistance.Value;
            }
        }
    }
}
