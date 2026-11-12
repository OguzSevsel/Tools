using System;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.Data
{
    [Serializable]
    public abstract class DSData : INotifyBindablePropertyChanged
    {
        [SerializeField, SerializeReference] private string _name = "Name";

        [CreateProperty]
        public string Name
        {
            get => _name;
            private set
            {
                if (_name == value)
                {
                    return;
                }

                _name = value;
                Notify();
            }
        }

        [SerializeField, SerializeReference] private string _description = "Description";

        [CreateProperty]
        public string Description
        {
            get => _description;
            private set
            {
                if (_description == value)
                {
                    return;
                }

                _description = value;
                Notify();
            }
        }

        [field: SerializeField] public string Guid { get; internal set; } = "UniqueID";

        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

        protected DSData(string title, string description)
        {
            this.Name = title;
            this.Description = description;
        }

        protected void Notify([CallerMemberName] string propertyName = null)
        {
            propertyChanged?.Invoke(
                this,
                new BindablePropertyChangedEventArgs(propertyName)
            );
        }

        public void SetName(string name) => this.Name = name;
    }
}