using System;
using UnityEngine;

namespace Tools.DialogueSystem.Data
{
    [Serializable]
    public abstract class DSData
    {
        public string Guid { get; internal set; } = "UniqueID";
        public string Name = "New Dialogue Entry";
        public string Description = "Description";

        protected DSData(string title, string description)
        {
            this.Name = title;
            this.Description = description;
        }

        public void SetName(string name) => this.Name = name;
    }
}