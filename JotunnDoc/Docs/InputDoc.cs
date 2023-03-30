﻿using UnityEngine.SceneManagement;

namespace JotunnDoc.Docs
{
    public class InputDoc : Doc
    {
        public InputDoc() : base("input/input-list.md")
        {
            SceneManager.sceneLoaded += DocInputs;
        }

        public void DocInputs(Scene scene, LoadSceneMode mode)
        {
            if (Generated)
            {
                return;
            }

            if (scene.name != "main")
            {
                return;
            }

            Jotunn.Logger.LogInfo("Documenting inputs");

            AddHeader(1, "Input list");
            AddText("All of the inputs currently in the game, and their default values.");
            AddText($"This file is automatically generated from Valheim {Version.GetVersionString(true)} using the JotunnDoc mod found on our GitHub.");
            AddTableHeader("Name", "Keycode", "Axis", "Gamepad");

            var buttons = ZInput.instance.m_buttons;

            foreach (var pair in buttons)
            {
                ZInput.ButtonDef button = pair.Value;
                AddTableRow(pair.Key, button.m_key.ToString(), button.m_axis, button.m_gamepad.ToString());
            }

            Save();
        }
    }
}
