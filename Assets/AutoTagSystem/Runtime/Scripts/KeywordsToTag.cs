using System.Collections.Generic;
using UnityEngine;

namespace Tools.AutoTagSystem
{
    [CreateAssetMenu(fileName = "Keywords", menuName = "Keywords", order = 0)]
    public class KeywordsToTag : ScriptableObject
    {
        public List<string> Keywords;
    }
}

