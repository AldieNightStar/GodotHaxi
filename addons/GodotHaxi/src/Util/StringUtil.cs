using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GodotHaxi;

public class StringUtil
{
    public static string Unescape(string s)
    {
        var sb = new StringBuilder(capacity: s.Length);
        var escaped = false;
        foreach (char c in s)
        {
            if (escaped)
            {
                escaped = false;
                sb.Append(_getEscaped(c));
                continue;
            }

            if (c == '\\')
            {
                escaped = true;
                continue;
            }

            sb.Append(c);
        }
        return sb.ToString();
    }

    public static string Escape(string s)
    {
        return s.Replace("\\", "\\\\")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n")
            .Replace("\t", "\\t")
            .Replace("\0", "\\0");
    }

    public static List<string> ParseDivided(char divider, string src)
    {
        var list = new List<string>();
        var escaped = false;
        var sb = new StringBuilder();
        foreach (char c in src)
        {
            if (escaped)
            {
                escaped = false;
                sb.Append(c);
                continue;
            }

            if (c == '\\')
            {
                escaped = true;
                continue;
            }

            if (c == divider)
            {
                list.Add(sb.ToString());
                sb.Clear();
                continue;
            }

            sb.Append(c);
        }
        if (sb.Length > 0) list.Add(sb.ToString());
        return list;
    }

    private static char _getEscaped(char c)
    {
        if (c == 'n') return '\n';
        else if (c == 'r') return '\r';
        else if (c == '0') return '\0';
        else if (c == 't') return '\t';
        return c;
    }
}