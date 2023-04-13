using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PathOfExile_NoSpiderMod.Transforms;

public static class AocProcess
{
    public static readonly Regex regexArakaali = new("\r\nSkinMesh\r\n\\{\r\n\t.*?\r\n}(\r\n){0,1}", RegexOptions.Compiled | RegexOptions.Singleline);
    // tut mb stoit smestit gruppu chtobi konec bil ne 2 empty strings. inogda u nih tak? inogda ne tak.
    static readonly Regex regexDefault = new("(?'result'^.*?}(\r\n\r\n|$))", RegexOptions.Compiled | RegexOptions.Singleline);

    static string ProcessArakaali(string source)
    {
        return regexArakaali.Replace(source, "");
    }

    static string Process(string source)
    {
        return regexDefault.Match(source).Groups["result"].Value;
    }

    public static string Do(string source, bool arakaali)
    {
        // return source;
        if (arakaali)
            return ProcessArakaali(source);
        return Process(source);
    }
}
