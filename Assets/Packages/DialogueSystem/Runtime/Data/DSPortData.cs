using System;

namespace Tools.DialogueSystem
{
    public enum PortType
    {
        Input,
        Output
    }

    [Serializable]
    public class DSPortData
    {
        public string InNodeId;
        public string OutNodeId;
        public string PortName;

        public DSPortData(string inputNodeId, string outputNodeId, string portId)
        {
            InNodeId = inputNodeId;
            OutNodeId = outputNodeId;
            PortName = portId;
        }
    }
}

