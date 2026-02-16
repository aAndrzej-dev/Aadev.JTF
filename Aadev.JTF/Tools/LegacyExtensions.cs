using System.Text;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.Tools;
internal static class LegacyExtensions
{
    internal static string ConvertLegacyCondition(JArray legacyCondition)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < legacyCondition.Count; i++)
        {
            if (i != 0)
                sb.Append("||");
            JObject item = (JObject)legacyCondition[i];
            sb.Append($"'$({(string?)item["id"]})'");
            switch (((string?)item["type"])?.ToLowerInvariant())
            {
                case "equal":
                    sb.Append("==");
                    break;
                case "notEqual":
                    sb.Append("!=");
                    break;
                case "less":
                    sb.Append('<');
                    break;
                case "bigger":
                    sb.Append('>');
                    break;
                default:
                    throw new JtfException($"Invalid condition type `{item["type"]}`");
            }

            sb.Append($"'{(string?)item["value"]}'");
        }

        return sb.ToString();
    }
}
