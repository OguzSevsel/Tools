using System;
using UnityEditor;
using UnityEngine;

namespace Tools.DialogueSystem.Data
{
    [Serializable]
    public abstract class DSData
    {
        public string Guid { get; internal set; } = "UniqueID";
        [field: SerializeField] public string Name { get; internal set; } = "Data";

        public void SetName(string name)
        {
            this.Name = name;
        }
    }
}