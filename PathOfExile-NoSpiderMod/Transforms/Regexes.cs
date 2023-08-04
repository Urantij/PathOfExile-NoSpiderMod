using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PathOfExile_NoSpiderMod.Transforms;

public partial class Regexes
{
    // TODO Разобраться, выходит ли регекс из матча при первом находе.
    // Если нет, мб можно добавить .*$

    // Можно вырезать Metadata.

    const RegexOptions regexOptions = RegexOptions.IgnoreCase;

    [GeneratedRegex(@"^metadata/npc/act1/.*doorman", regexOptions)]
    public static partial Regex TestGroupRegex();

    [GeneratedRegex(@"^metadata/monsters/.*crab", regexOptions)]
    public static partial Regex CrabGroupRegex();

    [GeneratedRegex(@"^metadata/monsters/.*nessacrab", regexOptions)]
    public static partial Regex CrabGroupEx1Regex();

    [GeneratedRegex(@"^metadata/monsters/.*shieldcrab", regexOptions)]
    public static partial Regex CrabGroupEx2Regex();

    [GeneratedRegex(@"^metadata/.*weta", regexOptions)]
    public static partial Regex WetaGroupRegex();

    [GeneratedRegex(@"^metadata/.*spider", regexOptions)]
    public static partial Regex MainGroupRegex();

    [GeneratedRegex(@"^metadata/monsters/arakaali/", regexOptions)]
    public static partial Regex MainGroupArakaaliRegex();

    [GeneratedRegex(@"^metadata/.*razorleg", regexOptions)]
    public static partial Regex MainGroupRazorLegRegex();

    [GeneratedRegex(@"^metadata/monsters/arakaali/turrets/arakaaliturret", regexOptions)]
    public static partial Regex MainGroupExRegex();

    [GeneratedRegex("(\t|^)skin = \"(?'SkinPath'.*?)\"", regexOptions)]
    public static partial Regex SkinLineCheckRegex();
}
