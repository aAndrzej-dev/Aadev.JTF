﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Aadev.JTF
{
    internal sealed class IdentifiersManager : IIdentifiersManager
    {
        private readonly Dictionary<JtIdentifier, JtNode> registeredNodes = new Dictionary<JtIdentifier, JtNode>();
        public IIdentifiersManager? Parent { get; }
        public IdentifiersManager(IIdentifiersManager? parent)
        {
            Parent = parent;
        }

        public event EventHandler<NodeIdentifierEventArgs>? NodeRegistered;
        public event EventHandler<NodeIdentifierEventArgs>? NodeUnregistered;

        public bool RegisterNode(JtIdentifier id, JtNode node)
        {
            if (registeredNodes.ContainsKey(id))
                return false;
            NodeRegistered?.Invoke(this, new NodeIdentifierEventArgs(node, id));
            registeredNodes.Add(id, node);
            return true;
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

        public JtNode? GetNodeById(JtIdentifier id) => registeredNodes.ContainsKey(id) ? registeredNodes[id] : null;

        public bool ContainsNode(JtIdentifier id) => registeredNodes.ContainsKey(id);
        public JtNode[] GetRegisteredNodes() => registeredNodes.Values.ToArray();

    }
}