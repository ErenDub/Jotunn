﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jotunn.Managers;
using Jotunn.Utils;
using UnityEngine;

namespace JotunnDoc.Docs
{
    public class ShaderDoc : Doc
    {
        public ShaderDoc() : base("prefabs/shader-list.md")
        {
            PrefabManager.OnPrefabsRegistered += DocShaders;
        }

        private void DocShaders()
        {
            if (Generated)
            {
                return;
            }

            Jotunn.Logger.LogInfo("Documenting prefab shaders");

            AddHeader(1, "Shader list");
            AddText("All shaders and their properties currently in the game.");
            AddText("This file is automatically generated from Valheim using the JotunnDoc mod found on our GitHub.");
            AddTableHeader("Shader", "Properties");

            IEnumerable<Shader> shaders = PrefabManager.Cache.GetPrefabs(typeof(Shader)).Values.Cast<Shader>();

            foreach (Shader shady in shaders.OrderBy(x => x.name))
            {
                StringBuilder propsb = new StringBuilder();

                if (shady.GetPropertyCount() > 0)
                {
                    propsb.Append("<dl>");
                    for (int i = 0; i < shady.GetPropertyCount(); ++i)
                    {
                        propsb.Append("<dd>");
                        propsb.Append(shady.GetPropertyName(i));
                        string desc = shady.GetPropertyDescription(i);
                        if (!string.IsNullOrEmpty(desc))
                        {
                            propsb.Append($" ({desc})");
                        }
                        propsb.Append("</dd>");
                    }
                    propsb.Append("</dl>");
                }

                AddTableRow(shady.name, propsb.ToString());
            }

            Save();
        }
    }
}
