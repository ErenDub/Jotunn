﻿using System.Collections.Generic;
using System.Linq;
using Jotunn.Entities;
using UnityEngine;

namespace JotunnDoc.Docs
{
    public class CharacterDoc : Doc
    {
        public CharacterDoc() : base("prefabs/character-list.md")
        {
            On.Player.OnSpawned += DocCharacters;
        }

        private void DocCharacters(On.Player.orig_OnSpawned orig, Player self)
        {
            orig(self);

            if (Generated)
            {
                return;
            }

            Jotunn.Logger.LogInfo("Documenting characters");

            AddHeader(1, "Character list");
            AddText("All of the Character prefabs currently in the game.");
            AddText("This file is automatically generated from Valheim using the JotunnDoc mod found on our GitHub.");
            AddTableHeader("Name", "Components");

            List<GameObject> allPrefabs = new List<GameObject>();
            allPrefabs.AddRange(ZNetScene.instance.m_nonNetViewPrefabs);
            allPrefabs.AddRange(ZNetScene.instance.m_prefabs);

            foreach (GameObject obj in allPrefabs.Where(x => !CustomPrefab.IsCustomPrefab(x.name) && x.GetComponent<Character>() != null).OrderBy(x => x.name))
            {
                string components = "<ul>";

                foreach (Component comp in obj.GetComponents<Component>())
                {
                    components += "<li>" + comp.GetType().Name + "</li>";
                }

                components += "</ul>";

                AddTableRow(
                    obj.name,
                    components
                );
            }

            Save();
        }
    }
}