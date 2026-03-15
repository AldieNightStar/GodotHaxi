using System.Collections.Generic;
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

    public static Dictionary<string, string> ParseColonArguments(string src, char divider = ';')
    {
        string currentParam = "null";
        var sb = new StringBuilder();
        var dict = new Dictionary<string, string>();
        var valueState = false;
        var escaped = false;

        foreach (char c in src)
        {
            if (valueState)
            { // Value state
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

                if (c == divider)
                {
                    var value = sb.ToString();
                    sb.Clear();
                    dict[currentParam] = value;
                    valueState = false;
                    escaped = false;
                }
                else
                {
                    sb.Append(c);
                }
            }
            else
            { // Key state
                if (c == ':')
                {
                    currentParam = sb.ToString();
                    sb.Clear();
                    valueState = true;
                    escaped = false;
                }
                else
                {
                    sb.Append(c);
                }
            }
        }

        if (valueState && sb.Length > 0) dict[currentParam] = sb.ToString();

        return dict;
    }

    public static string GetColonArguments(Dictionary<string, string> args)
    {
        var sb = new StringBuilder();
        foreach (var (k, v) in args)
        {
            if (sb.Length>0) sb.Append(';');
            var escapedV = Escape(v).Replace(";", "\\;");
            sb.Append($"{k}:{escapedV}");
        }
        return sb.ToString();
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