using System;
using UnityEngine;

namespace Tools.DialogueSystem.Data
{
    [Serializable]
    public abstract class DSData
    {
        public string Guid { get; internal set; } = "UniqueID";
        [field: SerializeField] public string Title { get; internal set; } = "New Dialogue Entry";
        [field: SerializeField] public string Description { get; internal set; } = "Description";

        protected DSData(string title, string description)
        {
            Title = title;
            Description = description;
        }

        public void SetName(string name) => Title = name;
    }
}