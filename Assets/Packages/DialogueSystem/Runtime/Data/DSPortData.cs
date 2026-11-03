using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tools.DialogueSystem
{
    [Serializable]
    public class DSPortData
    {
        public string NodeId;
        public List<string> OutputIds;
        public string PortName;

        public DSPortData(string nodeId, string portId)
        {
            this.OutputIds = new List<string>();
            this.NodeId = nodeId;
            this.PortName = portId;
        }
    }
}

