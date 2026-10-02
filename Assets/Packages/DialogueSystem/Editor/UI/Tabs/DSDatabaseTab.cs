using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem
{
    public class DSDatabaseTab : Tab
    {
        public DSDatabaseTab(VisualTreeAsset tabAsset)
        {
            tabAsset.CloneTree(this);
        }
    }
}
