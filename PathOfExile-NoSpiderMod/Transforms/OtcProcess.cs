using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PathOfExile_NoSpiderMod.Transforms;

public static class OtcProcess
{
    static readonly Regex[] regexes = new Regex[]
    {
        new Regex(@"[\t]+?.*?""HideAllMeshSegments\(\);""\r\n", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline),
        new Regex(@"[\t]+?.*?""HideMeshSegment\(.*?\);""\r\n", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline),
        new Regex(@"[\t]+?HideAllMeshSegments\(\);\r\n", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline),
        new Regex(@"[\t]+?HideMeshSegment\(.*?\);\r\n", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline),
        new Regex(@" HideAllMeshSegments\(\);", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline),
        new Regex(@" HideMeshSegment\(.*?\);", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline),
        new Regex(@"HideAllMeshSegments\(\); ", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline),
        new Regex(@"HideMeshSegment\(.*?\); ", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline),
        // ./Metadata/Monsters/LeagueHarvest/Green/PlagueSpider.otc introduced "HideMeshSegments"
        new Regex(@"[\t]+?.*?""HideMeshSegments\(.*?\);""\r\n", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline),
        new Regex(@"[\t]+?HideMeshSegments\(\);\r\n", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline),
        new Regex(@" HideMeshSegments\(.*?\);", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline),
        new Regex(@"HideMeshSegments\(.*?\); ", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline),
        new Regex(@"HideMeshSegments\(.*?\);", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline),
    };

    static string Process(string source)
    {
        foreach (var regex in regexes)
        {
            source = regex.Replace(source, "");
        }

        return source;
    }

    public static string Do(string source, bool lifeChange, bool barrel = true)
    {
        source = Process(source);

        if (!lifeChange || source.Contains("extends \"nothing\""))
            return source;

        const string barrelPath = "Metadata/Chests/Barrels/Barrel1.ao";
        const string cratePath = "Metadata/Chests/Crates/Crate1.ao";

        string ao = barrel ? barrelPath : cratePath;
        string aoAlt = barrel ? cratePath : barrelPath;

        string addAttached = $"AddAttached( {ao}, <root> )";
        string detachAll = $"DetachAllOfType( {ao} )";
        string addAttachedAlt = $"AddAttached( {aoAlt}, <root> )";
        string detachAllAlt = $"DetachAllOfType( {aoAlt} )";

        if (source.Contains("Life\r\n{\r\n", StringComparison.OrdinalIgnoreCase))
        {
            string resultLife = Regex.Match(source, "Life\r\n\\{\r\n.*?}", RegexOptions.IgnoreCase | RegexOptions.Singleline).Value;

            if (!resultLife.Contains(addAttached) && !resultLife.Contains(addAttachedAlt))
            {
                if (resultLife.Contains("\ton_spawned_alive", StringComparison.OrdinalIgnoreCase))
                {
                    if (resultLife.Contains("\ton_spawned_alive = \""))
                    {
                        resultLife = resultLife.Replace("\ton_spawned_alive = \"", "\ton_spawned_alive = \"" + addAttached + "; ", true, null);
                    }
                    else if (resultLife.Contains("\ton_spawned_alive = \r\n\t\""))
                    {
                        resultLife = resultLife.Replace("\ton_spawned_alive = \r\n\t\"", "\ton_spawned_alive = \r\n\t\"\r\n\t\t" + addAttached + ";");
                    }
                    else if (resultLife.Contains("\ton_spawned_alive =\r\n\t\""))
                    {
                        resultLife = resultLife.Replace("\ton_spawned_alive =\r\n\t\"", "\ton_spawned_alive =\r\n\t\"\r\n\t\t" + addAttached + ";");
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    resultLife = resultLife.Replace("{\r\n", "{\r\n\ton_spawned_alive = \"" + addAttached + ";\"\r\n", true, null);
                }
            }

            if (!resultLife.Contains(detachAll) && !resultLife.Contains(detachAllAlt))
            {
                if (resultLife.Contains("\ton_death", StringComparison.OrdinalIgnoreCase))
                {
                    if (resultLife.Contains("\ton_death = \""))
                    {
                        resultLife = resultLife.Replace("\ton_death = \"", "\ton_death = \"" + detachAll + "; ", true, null);
                    }
                    else if (resultLife.Contains("\ton_death = \r\n\t\""))
                    {
                        resultLife = resultLife.Replace("\ton_death = \r\n\t\"", "\ton_death = \r\n\t\"\r\n\t\t" + detachAll + ";");
                    }
                    else if (resultLife.Contains("\ton_death =\r\n\t\""))
                    {
                        resultLife = resultLife.Replace("\ton_death =\r\n\t\"", "\ton_death =\r\n\t\"\r\n\t\t" + detachAll + ";");
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    resultLife = resultLife.Replace("{\r\n", "{\r\n\ton_death = \"" + detachAll + ";\"\r\n", true, null);
                }
            }

            source = Regex.Replace(source, "Life\r\n\\{\r\n.*?}", resultLife, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }
        else
        {
            if (source.EndsWith("\r\n"))
            {
                source += "\r\n";
            }
            else
            {
                source += "\r\n\r\n";
            }

            source += "Life\r\n{\r\n\ton_spawned_alive = \"";
            source += addAttached;
            source += ";\"\r\n\ton_death = \"";
            source += detachAll;
            source += ";\"\r\n}";
        }

        return source;
    }
}
