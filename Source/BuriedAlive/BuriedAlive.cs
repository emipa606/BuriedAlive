using System.Reflection;
using HarmonyLib;
using Verse;

namespace BuriedAlive;

[StaticConstructorOnStartup]
public static class BuriedAlive
{
    public static readonly bool MassGravesLoaded;
    public static readonly MethodInfo MassGraveAcceptCorpses;

    static BuriedAlive()
    {
        MassGravesLoaded = ModLister.GetActiveModWithIdentifier("pyrce.mass.graves.continued") != null;

        new Harmony("buriedalive.1trickPonyta").PatchAll();
        if (!MassGravesLoaded)
        {
            return;
        }

        MassGraveAcceptCorpses =
            AccessTools.PropertyGetter("MassGravesContinued.Building_MassGrave:CanAcceptCorpses");
        Log.Message(MassGraveAcceptCorpses != null
            ? "[Buried Alive!] Adding support for Mass Graves."
            : "[Buried Alive!] Failed to add support for Mass Graves.");
    }
}