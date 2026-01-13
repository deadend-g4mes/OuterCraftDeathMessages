using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Reflection;
using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using OWML.Utils;
using UnityEngine;

namespace OuterCraftDeathMessages
{
    public class OuterCraftDeathMessages : ModBehaviour
    {
        public static OuterCraftDeathMessages Instance;

        public static DeathType deathType;

        public static string DeathMessage = "working";

        public static bool ShowText = false;

        public static bool _inTimeLoop = true;

        public static int _displayHeight;
        public static int _displayWidth;
        public static float _xPos;
        public static float _yPos;
        public static float _width;
        public static float _height;
        public Font _hudFont;

        public static Dictionary<DeathType, List<string>> deathMessages = new Dictionary<DeathType, List<string>>()
        {
            {DeathType.Impact, new List<string> { "fell from a great height", "took a nosedive", "couldn't stick the landing"} },
            {DeathType.Asphyxiation, new List<string> { "couldn't hold their breath", "choked on their own spit"} },
            {DeathType.Supernova, new List<string> { "was caught in a supernova", "couldn't escape the stellar explosion", "was obliterated by a dying star"} },
            {DeathType.Digestion, new List<string> { "Was swallowed by a fish", "became food not friend"} },
            {DeathType.DreamExplosion, new List<string> { "was caught in a dream explosion", "couldn't escape the blast", "was torn apart in a dream"} },
            {DeathType.BlackHole, new List<string> { "couldn't escape the singularity"} },
            {DeathType.Lava, new List<string> { "was melted by lava", "couldn't withstand the heat", "was consumed by molten rock"} },
            {DeathType.CrushedByElevator, new List<string> { "was crushed by an elevator", "couldn't avoid the falling platform", "was flattened by machinery"} },
            {DeathType.Meditation, new List<string> { "was lost in meditation", "was consumed by inner thoughts"} },
            {DeathType.Crushed, new List<string> { "was flattened by a heavy load"} },
            {DeathType.Default, new List<string> { "met an untimely end", "couldn't survive the ordeal", "was defeated by the environment"} },
            {DeathType.TimeLoop, new List<string> { "couldn't escape the temporal anomaly"} },
            {DeathType.Energy, new List<string> {"couldn't handle the heat", "flew too close to the sun"}},
            {DeathType.Dream, new List<string> {"couldn't escape the krueger", "failed to lucid dream"}}
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
            ModHelper.Console.WriteLine($"My mod {nameof(OuterCraftDeathMessages)} is loaded! {DateTime.Now:HH:mm:ss}", MessageType.Success);

            new Harmony("RattmanTheStupid.OuterCraftDeathMessages").PatchAll(Assembly.GetExecutingAssembly());

            // Example of accessing game code.
            OnCompleteSceneLoad(OWScene.TitleScreen, OWScene.TitleScreen); // We start on title screen
            LoadManager.OnCompleteSceneLoad += OnCompleteSceneLoad;

            _hudFont = Resources.Load<Font>(@"fonts/english - latin/SpaceMono-Regular_Dynamic");

            GlobalMessenger<DeathType>.AddListener("PlayerDeath", DrawDeathText);
            GlobalMessenger<GraphicSettings>.AddListener("GraphicSettingsUpdated", GetDisplaySettings);
        }

        public void OnCompleteSceneLoad(OWScene previousScene, OWScene newScene)
        {
            if (newScene != OWScene.SolarSystem) return;
            ModHelper.Console.WriteLine($"Loaded into solar system! {DateTime.Now:HH:mm:ss}", MessageType.Success);
        }

        public void IsInTimeLoop()
        {
            _inTimeLoop = false;
        }

        public void AddTextToScene()
        {
            ShowText = false;
        }

        public void OnGUI()
        {
           if (LoadManager.GetCurrentScene() != OWScene.SolarSystem)
            {
                return;
            }
            if (!ShowText)
            {
                return;
            }

            var style = new GUIStyle
            {
                font = _hudFont,
                fontSize = 30,
                wordWrap = true
            };
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;

            GUI.Label(new Rect(_xPos, _yPos, _width, _height), DeathMessage, style);
        }

        public void DrawDeathText(DeathType type)
        {
            Invoke(nameof(AddTextToScene), 3);
        }

        public override void Configure(IModConfig config)
        {
            RecalculatePosition();
        }

        public void GetDisplaySettings(GraphicSettings settings)
        {
            _displayHeight = settings.displayResHeight;
            _displayWidth = settings.displayResWidth;
            RecalculatePosition();
        }

        private void RecalculatePosition()
        {
            _yPos = 0;
            _height = _displayHeight;
            _xPos =  0;
            _width = _displayWidth;
        }
    }

    [HarmonyPatch]
    public class MyPatchClass {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerData), nameof(PlayerData.SetLastDeathType))]
        public static void PlayerData_SetLastDeathType_Prefix(DeathType deathType) {
            OuterCraftDeathMessages.Instance.ModHelper.Console.WriteLine($"{deathType}");
            var messageList = OuterCraftDeathMessages.deathMessages.TryGetValue(deathType, out List<string> messages);
            if (messageList)
            {
                var random = new System.Random();
                var randomMessage = messages[random.Next(messages.Count)];
                OuterCraftDeathMessages.DeathMessage = randomMessage;
                OuterCraftDeathMessages.ShowText = true;
            }
        }
    }

}
