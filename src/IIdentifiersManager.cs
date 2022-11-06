using System;

namespace Aadev.JTF
{
    public interface IIdentifiersManager
    {
        IIdentifiersManager? Parent { get; }
        bool RegisterNode(JtIdentifier id, JtNode node);
        bool UnregisterNode(JtIdentifier id);
        JtNode? GetNodeById(JtIdentifier id);
        bool ContainsNode(JtIdentifier id);
        JtNode[] GetRegisteredNodes();

        event EventHandler<NodeIdentifierEventArgs>? NodeRegistered;
        event EventHandler<NodeIdentifierEventArgs>? NodeUnregistered;
    }
    public sealed class NodeIdentifierEventArgs : EventArgs
    {
        public NodeIdentifierEventArgs(JtNode node, JtIdentifier id)
        {
            Node = node;
            Id = id;
        }

        public JtNode Node { get; }
        public JtIdentifier Id { get; }
    }
}