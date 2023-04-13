using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PathOfExile_NoSpiderMod.Transforms;

namespace PathOfExile_NoSpiderMod.Checks;

public class CheckGroup
{
    public readonly string name;
    public readonly ReplacementType replacement;

    readonly Regex[] restrict;
    readonly Regex[] exclude;

    public CheckGroup(string name, ReplacementType replacement, Regex[] restrict, Regex[] exclude)
    {
        this.name = name;
        this.replacement = replacement;

        this.restrict = restrict;
        this.exclude = exclude;
    }

    public CheckGroup(string name, ReplacementType replacement, Regex[] restrict)
        : this(name, replacement, restrict, Array.Empty<Regex>())
    {
    }

    public bool Checked(string test)
    {
        bool included = restrict.Any(r => r.IsMatch(test));

        if (included)
        {
            return !exclude.Any() || exclude.All(r => !r.IsMatch(test));
        }

        return included;
    }
}
