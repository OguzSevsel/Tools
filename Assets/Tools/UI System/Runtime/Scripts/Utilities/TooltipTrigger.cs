using UnityEngine;
using UnityEngine.EventSystems;

namespace TooltipSystem
{
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public string Header;
        public string Content;

        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipSystem.Show(Content, Header);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipSystem.Hide();
        }

        private void OnMouseEnter()
        {
            TooltipSystem.Show(Content, Header);
        }

        private void OnMouseExit()
        {
            TooltipSystem.Hide();
        }
    } 
}
