using System.Collections.Generic;
using System.Linq;

namespace Aadev.JTF
{
    internal class BlankIdentifiersManager : IIdentifiersManager
    {
        private readonly Dictionary<string, JtNode> registeredNodes = new Dictionary<string, JtNode>();
        public bool RegisterNode(string id, JtNode node)
        {
            if (registeredNodes.ContainsKey(id))
                return false;
            registeredNodes.Add(id, node);
            return true;
        }
        public bool UnregisterNode(string id) => registeredNodes.Remove(id);
        public JtNode? GetNodeById(string id)
        {
            if (registeredNodes.ContainsKey(id))
                return registeredNodes[id];
            return null;
        }

        public bool ContainsNode(string id) => registeredNodes.ContainsKey(id);
        public JtNode[] GetRegisteredNodes() => registeredNodes.Values.ToArray();
    }
}