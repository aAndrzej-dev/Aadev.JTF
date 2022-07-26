namespace Aadev.JTF
{
    public interface IIdentifiersManager
    {
        bool RegisterNode(string id, JtNode node);
        bool UnregisterNode(string id);
        JtNode? GetNodeById(string id);
        bool ContainsNode(string id);
        JtNode[] GetRegisteredNodes();
    }
}