using System;
using UnityEngine;

namespace Tools.DialogueSystem.Data
{
    [Serializable]
    public abstract class DSData
    {
        public string Guid { get; internal set; } = "UniqueID";
        [field: SerializeField] public string Name { get; internal set; } = "New Dialogue Entry";
        [field: SerializeField] public string Description { get; internal set; } = "Description";

        protected DSData(string title, string description)
        {
            this.Name = title;
            this.Description = description;
        }

        public void SetName(string name) => this.Name = name;
    }
}