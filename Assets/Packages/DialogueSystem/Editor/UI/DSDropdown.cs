using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using Tools.DialogueSystem.Data;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSPopUpField<T> : PopupField<T> where T : DSData
    {
        public DSPopUpField(string title)
        {
            label = title;
            index = 0;
            value = null;
        }

        public void SetItems(List<T> items)
        {
            choices = items;
        }
    }

    [UxmlElement]
    public partial class DSActorPopUpField : DSPopUpField<DSActor>
    {
        public DSActorPopUpField() : base("Actor")
        {
            formatListItemCallback = FormatItem;
            formatSelectedValueCallback = FormatItem;
        }

        private string FormatItem(DSActor data)
        {
            if (data != null)
                return data.Name;

            return choices.Count > 0
                ? "Click To Select an Actor"
                : "There is no Actor in Database.";
        }
    }

    [UxmlElement]
    public partial class DSConversantPopUpField : DSPopUpField<DSActor>
    {
        public DSConversantPopUpField() : base("Conversant")
        {
            formatListItemCallback = FormatItem;
            formatSelectedValueCallback = FormatItem;
        }

        private string FormatItem(DSActor data)
        {
            if (data != null)
                return data.Name;

            return choices.Count > 0
                ? "Click To Select a Conversant"
                : "There is no Conversant in Database.";
        }
    }

    [UxmlElement]
    public partial class DSAudioPopUpField : DSPopUpField<DSAudioClip>
    {
        public DSAudioPopUpField() : base("Audio")
        {
            formatListItemCallback = FormatItem;
            formatSelectedValueCallback = FormatItem;
        }

        private string FormatItem(DSAudioClip data)
        {
            if (data != null)
                return data.Name;

            return choices.Count > 0
                ? "Click To Select an Audio"
                : "There is no Audio in Database.";
        }
    }

    [UxmlElement]
    public partial class DSDialoguePopUpField : DSPopUpField<DSDialogueText>
    {
        public DSDialoguePopUpField() : base("Dialogue")
        {
            formatListItemCallback = FormatItem;
            formatSelectedValueCallback = FormatItem;
        }

        private string FormatItem(DSDialogueText data)
        {
            if (data != null)
                return data.Name;

            return choices.Count > 0
                ? "Click To Select a Dialogue"
                : "There is no Dialogue in Database.";
        }
    }

    public abstract class DSDataConverter<T> : UxmlAttributeConverter<T>
    where T : DSData
    {
        protected abstract List<T> GetItems();

        public override string ToString(T value)
        {
            return value != null ? value.Guid : string.Empty;
        }

        public override T FromString(string value)
        {
            if (!Guid.TryParse(value, out Guid id))
                return null;

            return GetItems()?.Find(x => x.Guid == id.ToString());
        }
    }

    public class DSActorConverter : DSDataConverter<DSActor>
    {
        protected override List<DSActor> GetItems()
        {
            return DSDatabaseManager.Instance?.Current?.Actors;
        }
    }

    public class DSAudioConverter : DSDataConverter<DSAudioClip>
    {
        protected override List<DSAudioClip> GetItems()
        {
            return DSDatabaseManager.Instance?.Current?.AudioClips;
        }
    }

    public class DSDialogueConverter : DSDataConverter<DSDialogueText>
    {
        protected override List<DSDialogueText> GetItems()
        {
            return DSDatabaseManager.Instance?.Current?.DialogueTexts;
        }
    }
}