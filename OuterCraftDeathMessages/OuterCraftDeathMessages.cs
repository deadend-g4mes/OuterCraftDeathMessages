using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;

namespace OuterCraftDeathMessages
{
    public class OuterCraftDeathMessages : ModBehaviour
    {
        public static OuterCraftDeathMessages Instance;

        public static DeathType deathType;

        public static Dictionary<DeathType, List<string>> deathMessages = new Dictionary<DeathType, List<string>>()
        {
            { DeathType.Impact, new List<string> { "fell from a great height", "took a nosedive", "couldn't stick the landing" } },
            { DeathType.Asphyxiation, new List<string> { "couldn't hold their breath" } },
            { DeathType.Energy, new List<string> { "was burned to a crisp", "played with fire", "got too hot to handle" } },
            {DeathType.Supernova, new List<string> { "was caught in a supernova", "couldn't escape the stellar explosion", "was obliterated by a dying star" } },
            { DeathType.Digestion, new List<string> { "Was swallowed by a fish"} },
            {DeathType.DreamExplosion, new List<string> { "was caught in a dream explosion", "couldn't escape the blast", "was torn apart in a dream" } },
            {DeathType.BlackHole, new List<string> { "couldn't escape the singularity"} },
            {DeathType.Lava, new List<string> { "was melted by lava", "couldn't withstand the heat", "was consumed by molten rock" } },
            {DeathType.CrushedByElevator, new List<string> { "was crushed by an elevator", "couldn't avoid the falling platform", "was flattened by machinery" } },
            {DeathType.Meditation, new List<string> { "was lost in meditation", "was consumed by inner thoughts" } },
            {DeathType.Crushed, new List<string> { "was flattened by a heavy load" } },
            // Add more death types and messages as needed
        };

        public void Awake()
        {
            Instance = this;
            // You won't be able to access OWML's mod helper in Awake.
            // So you probably don't want to do anything here.
            // Use Start() instead.
        }

        public void Start()
        {
            // Starting here, you'll have access to OWML's mod helper.
            ModHelper.Console.WriteLine($"My mod {nameof(OuterCraftDeathMessages)} is loaded!", MessageType.Success);

            new Harmony("RattmanTheStupid.OuterCraftDeathMessages").PatchAll(Assembly.GetExecutingAssembly());

            // Example of accessing game code.
            OnCompleteSceneLoad(OWScene.TitleScreen, OWScene.TitleScreen); // We start on title screen
            LoadManager.OnCompleteSceneLoad += OnCompleteSceneLoad;
        }

        public void OnCompleteSceneLoad(OWScene previousScene, OWScene newScene)
        {
            if (newScene != OWScene.SolarSystem) return;
            ModHelper.Console.WriteLine("Loaded into solar system!", MessageType.Success);
        }
    }

    [HarmonyPatch]
    public class MyPatchClass {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerData), nameof(PlayerData.SetLastDeathType))]
        public static void PlayerData_SetLastDeathType_Prefix(DeathType deathType) {
            var messageList = OuterCraftDeathMessages.deathMessages.TryGetValue(deathType, out List<string> messages);
            if (messageList)
            {
                var random = new Random();
                var randomMessage = messages[random.Next(messages.Count)];
                OuterCraftDeathMessages.Instance.ModHelper.Console.WriteLine(randomMessage);
            }
        }
    }

}
