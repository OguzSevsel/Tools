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
        public string _name = "Name";

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

        public string Guid { get; internal set; } = "UniqueID";
        public string Description = "Description";
        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

        protected DSData(string title, string description)
        {
            this.Name = title;
            this.Description = description;
        }

        protected void Notify(
            [CallerMemberName] string propertyName = null)
        {
            propertyChanged?.Invoke(
                this,
                new BindablePropertyChangedEventArgs(propertyName)
            );
        }

        public void SetName(string name) => this.Name = name;
    }
}