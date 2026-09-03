using UnityEditor;
using UnityEngine;
using Tools.DialogueSystem.Elements;

namespace Tools.DialogueSystem.Utilities
{
    public class DSNodeFlasher
    {
        private DSNode node;
        private float delay;
        private int remaining;
        private bool isRed;
        private double lastTime;

        public void Flash(DSNode node, float delaySeconds, int loopCount)
        {
            this.node = node;
            delay = delaySeconds;
            remaining = loopCount * 2; // red + reset
            isRed = false;
            lastTime = EditorApplication.timeSinceStartup;

            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
        }

        private void Tick()
        {
            if (EditorApplication.timeSinceStartup - lastTime < delay)
                return;

            lastTime = EditorApplication.timeSinceStartup;

            if (remaining <= 0)
            {
                EditorApplication.update -= Tick;
                node.ResetStyle();
                return;
            }

            if (isRed)
                node.ResetStyle();
            else
                node.SetErrorStyle(Color.red);

            isRed = !isRed;
            remaining--;
        }
    }
}

