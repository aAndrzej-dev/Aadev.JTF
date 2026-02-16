using System;
using System.Collections.Generic;
using System.Linq;

namespace Aadev.JTF;

public sealed class IdentifiersManager
{
    private readonly Dictionary<JtIdentifier, JtNode> registeredNodes = new Dictionary<JtIdentifier, JtNode>();
    public IdentifiersManager? Parent { get; }
    internal IdentifiersManager(IdentifiersManager? parent)
    {
        Parent = parent;
    }

    public event EventHandler<NodeIdentifierEventArgs>? NodeRegistered;
    public event EventHandler<NodeIdentifierEventArgs>? NodeUnregistered;

    public bool RegisterNode(JtIdentifier id, JtNode node)
    {
        if (registeredNodes.TryAdd(id, node))
        {
            NodeRegistered?.Invoke(this, new NodeIdentifierEventArgs(node, id));
            return true;
        }

        return false;
    }
    public bool UnregisterNode(JtIdentifier id)
    {
        if (registeredNodes.Remove(id, out JtNode? node))
        {
            NodeUnregistered?.Invoke(this, new NodeIdentifierEventArgs(node, id));
            return true;
        }

        return false;
    }

    public JtNode? GetNodeById(JtIdentifier id) => registeredNodes.TryGetValue(id, out JtNode? value) ? value : null;

    public bool ContainsNode(JtIdentifier id) => registeredNodes.ContainsKey(id);
    public JtNode[] GetRegisteredNodes() => registeredNodes.Values.ToArray();
    public IEnumerable<JtNode> EnumerateRegisteredNodes() => registeredNodes.Values;

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