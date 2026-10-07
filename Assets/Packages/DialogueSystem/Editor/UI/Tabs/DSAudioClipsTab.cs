using System;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.Utilities;
using UnityEditor;
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


        public DSAudioClipsTab(VisualTreeAsset tabAsset, VisualTreeAsset cardAsset)
        {
            tabAsset.CloneTree(this);
            label = "Audio Clips";
            this.cardAsset = cardAsset;

            audioClipContainer = this.Q<VisualElement>("AudioClipsCardContainer");
            audioClipScrollView = this.Q<ScrollView>("AudioClipsScrollView");
            createAudioClipButton = this.Q<Button>("CreateAudioClipButton");
            createAudioClipButton.clicked += OnCreateAudioClipsButtonClicked;
        }

        private void OnCreateAudioClipsButtonClicked()
        {
            VisualElement cardElement = new VisualElement();
            cardAsset.CloneTree(cardElement);
            cardElement.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            cardElement.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
            DSAudioClip audipClip = new DSAudioClip("New Clip", "New Description", null);
            cardElement.dataSource = audipClip;
            audioClipContainer.Add(cardElement);
        }

        private void OnMouseLeave(MouseLeaveEvent evt)
        {
            if (evt.currentTarget is VisualElement element)
            {
                element.SetDropShadow(true);
            }
        }

        private void OnMouseEnter(MouseEnterEvent evt)
        {
            if (evt.currentTarget is VisualElement element)
            {
                element.SetDropShadow();
            }
        }
    }
}