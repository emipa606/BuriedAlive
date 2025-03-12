using HarmonyLib;
using Verse;

namespace BuriedAlive;

[StaticConstructorOnStartup]
public static class BuriedAlive
{
    static BuriedAlive()
    {
        new Harmony("buriedalive.1trickPonyta").PatchAll();
        Log.Message("[Buried Alive!] Loaded.");
    }
}