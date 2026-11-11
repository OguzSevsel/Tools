using Tools.DialogueSystem.Data;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI.Inspector
{
    public interface IDSNodeInspector
    {
        void Create();
        void LoadFields(DSNodeData data);
        VisualElement GetVisualElement();
    }
}
