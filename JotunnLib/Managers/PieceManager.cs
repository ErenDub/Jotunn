﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JotunnLib.Managers
{
    public class PieceManager : Manager
    {
        public static PieceManager Instance { get; private set; }
        public event EventHandler PieceTableRegister;
        public event EventHandler PieceRegister;
        internal GameObject PieceTableContainer;

        private bool loaded = false;
        private Dictionary<string, PieceTable> pieceTables = new Dictionary<string, PieceTable>();
        private Dictionary<string, string> pieceTableNameMap = new Dictionary<string, string>()
        {
            { "Cultivator", "_CultivatorPieceTable" },
            { "Hammer", "_HammerPieceTable" },
            { "Hoe", "_HoePieceTable" }
        };

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Error, two instances of singleton: " + this.GetType().Name);
                return;
            }

            Instance = this;
        }

        internal override void Init()
        {
            PieceTableContainer = new GameObject("PieceTables");
            PieceTableContainer.transform.parent = JotunnLib.RootObject.transform;
            UnityEngine.Object.DontDestroyOnLoad(PieceTableContainer);

            Debug.Log("Initialized PieceTableManager");
        }

        internal override void Register()
        {
            // TODO: Split register and load logic
        }

        internal override void Load()
        {
            if (loaded)
            {
                return;
            }

            // Clear piece tables and re-load
            pieceTables.Clear();
            
            foreach (Transform child in PieceTableContainer.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            List<string> loadedTables = new List<string>();
            Debug.Log("---- Loading piece tables ----");

            foreach (PieceTable table in Resources.FindObjectsOfTypeAll(typeof(PieceTable)))
            {
                string name = table.gameObject.name;
                pieceTables.Add(name, table);
                loadedTables.Add(name);

                Debug.Log("Loaded existing piece table: " + name);
            }

            PieceTableRegister?.Invoke(null, EventArgs.Empty);

            foreach (var pair in pieceTables)
            {
                PieceTable table = pair.Value;
                string name = table.gameObject.name;

                if (loadedTables.Contains(name))
                {
                    continue;
                }

                pieceTables.Add(name, table);

                Debug.Log("Registered piece table: " + name);
            }

            Debug.Log("---- Loading pieces ----");
            PieceRegister?.Invoke(null, EventArgs.Empty);
            loaded = true;
        }

        public void RegisterPieceTable(string name)
        {
            if (pieceTables.ContainsKey(name))
            {
                Debug.Log("Cannot register piece table with existing name" + name);
                return;
            }

            GameObject obj = new GameObject(name);
            obj.transform.parent = PieceTableContainer.transform;

            PieceTable table = obj.AddComponent<PieceTable>();
            pieceTables.Add(name, table);
        }

        public void RegisterPiece(string pieceTable, string prefabName)
        {
            PieceTable table = getPieceTable(pieceTable);
            GameObject prefab = PrefabManager.Instance.GetPrefab(prefabName);

            if (!table)
            {
                Debug.LogError("Piece table does not exist: " + pieceTable);
                return;
            }

            if (!prefab)
            {
                Debug.LogError("Prefab does not exist: " + prefabName);
                return;
            }

            if (!prefab.GetComponent<Piece>())
            {
                Debug.LogError("Prefab does not have Piece component: " + prefabName);
                return;
            }

            table.m_pieces.Add(prefab);
            Debug.Log("Registered piece: " + prefabName + " to " + pieceTable);
        }

        internal void RegisterPiece(string pieceTable, GameObject prefab)
        {
            PieceTable table = getPieceTable(pieceTable);

            if (!table)
            {
                Debug.LogError("Piece table does not exist: " + pieceTable);
                return;
            }

            table.m_pieces.Add(prefab);
            Debug.Log("Registered piece: " + prefab.name + " to " + pieceTable);
        }

        private PieceTable getPieceTable(string name)
        {
            if (pieceTables.ContainsKey(name))
            {
                return pieceTables[name];
            }

            if (pieceTableNameMap.ContainsKey(name))
            {
                return pieceTables[pieceTableNameMap[name]];
            }

            return null;
        }
    }
}