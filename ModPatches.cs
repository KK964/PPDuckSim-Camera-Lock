using HarmonyLib;
using UnityEngine;

namespace PPDuckSim_Camera_Lock
{
    internal class ModPatches
    {
        [HarmonyPatch(typeof(SmoothCameraOrbit), "LateUpdate")]
        public class InputRemoverPatch
        {
            static bool Prefix(
                ref SmoothCameraOrbit __instance,
                ref float ___xDeg,
                ref float ___yDeg,
                ref float ___currentDistance,
                ref float ___desiredDistance,
                ref Quaternion ___currentRotation,
                ref Quaternion ___desiredRotation,
                ref Quaternion ___rotation,
                ref Vector3 ___position,
                ref float ___idleSmooth
                )
            {
                if (Mod.Instance == null) return true;
                if (!Mod.Instance.lockToFloatingCam.Value) return true;
                ___idleSmooth += (Time.deltaTime + ___idleSmooth) * 0.005f;
                ___idleSmooth = Mathf.Clamp(___idleSmooth, 0f, 1f);
                ___xDeg += __instance.xSpeed * 0.001f * ___idleSmooth;
                ___yDeg = ClampAngle(___yDeg, __instance.yMinLimit, __instance.yMaxLimit);
                ___desiredRotation = Quaternion.Euler(___yDeg, ___xDeg, 0f);
                ___currentRotation = __instance.transform.rotation;
                ___rotation = Quaternion.Lerp(___currentRotation, ___desiredRotation, Time.deltaTime * __instance.zoomDampening * 2f);
                __instance.transform.rotation = ___rotation;
                ___desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * (float)__instance.zoomRate * Mathf.Abs(___desiredDistance);
                ___desiredDistance = Mathf.Clamp(___desiredDistance, __instance.minDistance, __instance.maxDistance);
                ___currentDistance = Mathf.Lerp(___currentDistance, ___desiredDistance, Time.deltaTime * __instance.zoomDampening);
                ___position = __instance.target.position - (___rotation * Vector3.forward * ___currentDistance + __instance.targetOffset);
                __instance.transform.position = ___position;
                return false;
            }

            private static float ClampAngle(float angle, float min, float max)
            {
                if (angle < -360f)
                {
                    angle += 360f;
                }
                if (angle > 360f)
                {
                    angle -= 360f;
                }
                return Mathf.Clamp(angle, min, max);
            }
        }

        [HarmonyPatch(typeof(SmoothMouseLook), "Update")]
        public class InputRemoverPatch2
        {
            static bool Prefix(ref SmoothMouseLook __instance, ref bool ___autoCamera, ref float ___lastInputAt)
            {
                if (Mod.Instance == null) return true;
                if (!Mod.Instance.lockToFloatingCam.Value) return true;
                if (__instance.GestPause.IsPaused || !__instance.generalManager.CanStartAutocamera)
                {
                    return true;
                }

                ___autoCamera = true;
                ___lastInputAt = 0;
                return true;
            }
        }

        [HarmonyPatch(typeof(SmoothMouseLook), "ComputePointToLookAt")]
        public class ChairRemover
        {
            static bool Prefix(
                ref SmoothMouseLook __instance, 
                ref bool ___autoCameraOnHotspot,
                ref float ___randomZoomOffset,
                ref float ___randomXOffset,
                ref float ___randomYOffset,
                ref float ___changeTargetAt)
            {
                if (Mod.Instance == null) return true;
                if (!Mod.Instance.disableChairs.Value) return true;
                __instance.generalManager.SelectRandomDuck();
                ___autoCameraOnHotspot = false;
                ___randomZoomOffset = 0f;
                ___randomXOffset = 0f;
                ___randomYOffset = 0f;
                ___changeTargetAt = Time.time + Random.Range(__instance.RandomRangeToChangeTarget.x, __instance.RandomRangeToChangeTarget.y);
                return false;
            }
        }
    }
}
