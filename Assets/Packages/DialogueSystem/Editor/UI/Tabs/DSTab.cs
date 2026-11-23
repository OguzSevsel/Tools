using System.Collections.Generic;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.Utilities;
using UnityEditor;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSTab : Tab
    {
        public ScrollView scrollView;
        public VisualElement cardContainer;
        public VisualElement buttonContainer;
        public Button deleteAllButton;
        public Button selectAllButton;

        public VisualTreeAsset cardAsset;
        public Button createButton;

        public Dictionary<IDSData, VisualElement> Items { get; set; }
        public List<IDSData> selectedItems { get; }

        public DSTab(VisualTreeAsset tabAsset, VisualTreeAsset cardAsset)
        {
            Items = new Dictionary<IDSData, VisualElement>();
            selectedItems = new List<IDSData>();
            this.cardAsset = cardAsset;
            RegisterEvents();
        }

        public void CreateItem(IDSData item)
        {
            VisualElement cardElement = CreateCard(cardAsset, item);
            AddCard(cardContainer, Items, item, cardElement);
            DSDatabaseManager.Instance.Current.Register(item);
        }

        public void DeleteItem(IDSData item, VisualElement container)
        {
            var cardElement = Items[item];
            container.Remove(cardElement);
            Items.Remove(item);
            DSDatabaseManager.Instance.Current.Unregister(item);
        }

        private void OnDatabaseOpened(DSDatabase database)
        {
            foreach (DSAudioClip audioClip in database.AudioClips)
            {
                VisualElement card = CreateCard(cardAsset, audioClip);
                AddCard(cardContainer, Items, audioClip, card);
            }
        }

        private void OnDatabaseClosed(DSDatabase database)
        {
            cardContainer.Clear();
            Items.Clear();
        }

        private void RegisterEvents()
        {
            DSDatabaseManager.Instance.DatabaseOpened += OnDatabaseOpened;
            DSDatabaseManager.Instance.DatabaseClosed += OnDatabaseClosed;
            DSEditorView.OnWindowCloses += UnRegisterEvents;
        }

        private void UnRegisterEvents()
        {
            DSDatabaseManager.Instance.DatabaseOpened -= OnDatabaseOpened;
            DSDatabaseManager.Instance.DatabaseClosed -= OnDatabaseClosed;
            DSEditorView.OnWindowCloses -= UnRegisterEvents;
        }

        private VisualElement CreateCard(VisualTreeAsset cardAsset, IDSData item)
        {
            VisualElement cardElement = new();
            cardAsset.CloneTree(cardElement);

            var checkBox = cardElement.Q<Toggle>("SelectionCheckBox");
            var image = cardElement.Q<Image>("Image");

            cardElement.RegisterCallback<MouseEnterEvent>((evt) =>
            {
                if (evt.currentTarget is VisualElement element)
                {
                    element.SetDropShadow();
                    if (!checkBox.value)
                    {
                        checkBox.SetVisible(true);
                        image.SetVisible(false);
                    }
                }
            });
            cardElement.RegisterCallback<MouseLeaveEvent>((evt) =>
            {
                if (evt.currentTarget is VisualElement element)
                {
                    element.SetDropShadow(true);
                    if (!checkBox.value)
                    {
                        checkBox.SetVisible(false);
                        image.SetVisible(true);
                    }
                }
            });
            cardElement.RegisterCallback<MouseDownEvent>((evt) =>
            {
                checkBox.value = !checkBox.value;

                if (checkBox.value)
                {
                    if (!selectedItems.Contains(item))
                    {
                        selectedItems.Add(item);
                    }
                }
                else
                {
                    if (selectedItems.Contains(item))
                    {
                        selectedItems.Remove(item);
                    }
                }

                if (selectedItems.Count > 0)
                {
                    buttonContainer.SetVisible(true);
                }
                else
                {
                    buttonContainer.SetVisible(false);
                }
            });

            cardElement.dataSource = item;
            return cardElement;
        }

        public void HandleDeleteSelectedButtonClicked()
        {
            var deletedItems = new List<IDSData>();

            if (selectedItems.Count > 0)
            {
                bool confirm = EditorUtility.DisplayDialog(
                    $"Delete Items",
                    $"Are you sure you want to delete {selectedItems.Count} Items?",
                    "Delete",
                    "Cancel"
                );

                if (!confirm) return;

                foreach (var item in selectedItems)
                {
                    DeleteItem(item, cardContainer);
                    deletedItems.Add(item);
                }
            }

            foreach (var actor in deletedItems)
            {
                selectedItems.Remove(actor);
            }
            buttonContainer.SetVisible(false);
        }

        public void HandleSelectAllButtonClicked()
        {
            if (selectedItems.Count >= Items.Count)
            {
                foreach (var kvp in Items)
                {
                    var actor = kvp.Key;
                    var card = kvp.Value;

                    var checkBox = card.Q<Toggle>("SelectionCheckBox");
                    var image = card.Q<Image>("Image");

                    checkBox.value = false;

                    image.SetVisible(true);
                    checkBox.SetVisible(false);

                    if (selectedItems.Contains(actor))
                    {
                        selectedItems.Remove(actor);
                    }
                }
                buttonContainer.SetVisible(false);
            }
            else
            {
                foreach (var kvp in Items)
                {
                    var actor = kvp.Key;
                    var card = kvp.Value;

                    var checkBox = card.Q<Toggle>("SelectionCheckBox");
                    var image = card.Q<Image>("Image");

                    checkBox.value = true;

                    image.SetVisible(false);
                    checkBox.SetVisible(true);

                    if (!selectedItems.Contains(actor))
                    {
                        selectedItems.Add(actor);
                    }
                }
            }
        }

        private void AddCard(VisualElement container, Dictionary<IDSData, VisualElement> items, IDSData item, VisualElement cardElement)
        {
            container.Add(cardElement);
            items.Add(item, cardElement);

            Button deleteButton = cardElement.Q<Button>("DeleteButton");
            deleteButton.clicked += () =>
            {
                bool confirm = EditorUtility.DisplayDialog(
                    $"Delete {item.Name}",
                    $"Are you sure you want to delete {item.Name}?",
                    "Delete",
                    "Cancel"
                );

                if (!confirm) return;

                container.Remove(cardElement);
                items.Remove(item);
                DSDatabaseManager.Instance.Current.Unregister(item);
            };
        }
    }
}
