using CodiceApp.EventTracking;
using System;
using System.Collections.Generic;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSAudioClipsTab : Tab
    {
        private VisualElement audioClipContainer;
        private Button createAudioClipButton;
        private ScrollView audioClipScrollView;
        private VisualTreeAsset cardAsset;
        public Dictionary<DSAudioClip, VisualElement> AudioClips { get; set; }

        public DSAudioClipsTab(VisualTreeAsset tabAsset, VisualTreeAsset cardAsset)
        {
            AudioClips = new Dictionary<DSAudioClip, VisualElement>();
            tabAsset.CloneTree(this);
            label = "Audio Clips";
            this.cardAsset = cardAsset;

            audioClipContainer = this.Q<VisualElement>("AudioClipsCardContainer");
            audioClipScrollView = this.Q<ScrollView>("AudioClipsScrollView");
            createAudioClipButton = this.Q<Button>("CreateAudioClipButton");
            createAudioClipButton.clicked += OnCreateAudioClipsButtonClicked;
            RegisterEvents();
        }

        private void OnDatabaseOpened(DSDatabase database)
        {
            foreach (var audioClip in database.AudioClips)
            {
                var card = CreateCard(cardAsset, audioClip);

                AddCard(audioClipContainer, AudioClips, audioClip, card);
            }
        }

        private void OnDatabaseClosed(DSDatabase database)
        {
            foreach (KeyValuePair<DSAudioClip, VisualElement> cardPair in AudioClips)
            {
                var card = cardPair.Value;
                var text = cardPair.Key;

                audioClipContainer.Remove(card);
            }

            AudioClips.Clear();
        }

        private void OnCreateAudioClipsButtonClicked()
        {
            DSAudioClip audioClip = new DSAudioClip("New Clip", "New Description", null);
            var cardElement = CreateCard(cardAsset, audioClip);
            AddCard(audioClipContainer, AudioClips, audioClip, cardElement);
            DSDatabaseManager.Current.Register(audioClip);
        }

        #region Utils

        private void RegisterEvents()
        {
            DSDatabaseManager.DatabaseOpened += OnDatabaseOpened;
            DSDatabaseManager.DatabaseClosed += OnDatabaseClosed;
            DSEditorView.OnWindowCloses += UnRegisterEvents;
        }

        private void UnRegisterEvents()
        {
            DSDatabaseManager.DatabaseOpened -= OnDatabaseOpened;
            DSDatabaseManager.DatabaseClosed -= OnDatabaseClosed;
            DSEditorView.OnWindowCloses -= UnRegisterEvents;
        }

        private VisualElement CreateCard(VisualTreeAsset cardAsset, DSAudioClip audioClip)
        {
            VisualElement cardElement = new VisualElement();
            cardAsset.CloneTree(cardElement);
            cardElement.RegisterCallback<MouseEnterEvent>((evt) =>
            {
                if (evt.currentTarget is VisualElement element)
                {
                    element.SetDropShadow();
                }
            });
            cardElement.RegisterCallback<MouseLeaveEvent>((evt) =>
            {
                if (evt.currentTarget is VisualElement element)
                {
                    element.SetDropShadow(true);
                }
            });

            cardElement.dataSource = audioClip;
            return cardElement;
        }

        private void AddCard(VisualElement container, Dictionary<DSAudioClip, VisualElement> audioClips, DSAudioClip audioClip, VisualElement cardElement)
        {
            container.Add(cardElement);
            Button deleteButton = cardElement.Q<Button>("DeleteButton");
            deleteButton.clicked += () => {
                container.Remove(cardElement);
                audioClips.Remove(audioClip);
                DSDatabaseManager.Current.Unregister(audioClip);
            };
        }

        #endregion
    }
}