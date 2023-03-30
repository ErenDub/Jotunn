﻿using System.Collections.Generic;
using Jotunn.Managers;
using Jotunn.Utils;

namespace JotunnDoc.Docs
{
    public class PieceTableDoc : Doc
    {
        public PieceTableDoc() : base("pieces/piece-table-list.md")
        {
            PieceManager.OnPiecesRegistered += DocPieceTables;
        }

        public void DocPieceTables()
        {
            if (Generated)
            {
                return;
            }

            Jotunn.Logger.LogInfo("Documenting piece tables");

            AddHeader(1, "Piece table list");
            AddText("All of the piece tables currently in the game.");
            AddText($"This file is automatically generated from Valheim {Version.GetVersionString(true)} using the JotunnDoc mod found on our GitHub.");
            AddTableHeader("GameObject Name", "Jotunn Alias", "Piece Count");

            var pieceTables = ReflectionHelper.GetPrivateField<Dictionary<string, PieceTable>>(PieceManager.Instance, "PieceTableMap");
            var nameMap = ReflectionHelper.GetPrivateField<Dictionary<string, string>>(PieceManager.Instance, "PieceTableNameMap");

            foreach (var pair in pieceTables)
            {
                string alias = "";

                if (nameMap.ContainsValue(pair.Key))
                {
                    foreach (string key in nameMap.Keys)
                    {
                        if (nameMap[key] == pair.Key)
                        {
                            alias = key;
                            break;
                        }
                    }
                }

                AddTableRow(pair.Key, alias, pair.Value.m_pieces.Count.ToString());
            }

            Save();
        }
    }
}
