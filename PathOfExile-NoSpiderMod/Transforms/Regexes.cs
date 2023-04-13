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

    [GeneratedRegex(@"^Metadata/NPC/Act1/.*Doorman", regexOptions)]
    public static partial Regex TestGroupRegex();

    [GeneratedRegex(@"^Metadata/Monsters/.*crab", regexOptions)]
    public static partial Regex CrabGroupRegex();

    [GeneratedRegex(@"^Metadata/Monsters/.*NessaCrab", regexOptions)]
    public static partial Regex CrabGroupEx1Regex();

    [GeneratedRegex(@"^Metadata/Monsters/.*ShieldCrab", regexOptions)]
    public static partial Regex CrabGroupEx2Regex();

    [GeneratedRegex(@"^Metadata/.*Weta", regexOptions)]
    public static partial Regex WetaGroupRegex();

    [GeneratedRegex(@"^Metadata/.*Spider", regexOptions)]
    public static partial Regex MainGroupRegex();

    [GeneratedRegex(@"^Metadata/Monsters/Arakaali/", regexOptions)]
    public static partial Regex MainGroupArakaaliRegex();

    [GeneratedRegex(@"^Metadata/.*RazorLeg", regexOptions)]
    public static partial Regex MainGroupRazorLegRegex();

    [GeneratedRegex(@"^Metadata/Monsters/Arakaali/Turrets/ArakaaliTurret", regexOptions)]
    public static partial Regex MainGroupExRegex();

    [GeneratedRegex("(\t|^)skin = \"(?'SkinPath'.*?)\"", regexOptions)]
    public static partial Regex SkinLineCheckRegex();
}
