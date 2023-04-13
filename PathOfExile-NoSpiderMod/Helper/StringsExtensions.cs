using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PathOfExile_NoSpiderMod.Helper;

public static class StringsExtensions
{
    public static bool TryGetIndex<T>(this T[] array, T target, out int index, bool ignoreCase = false)
    {
        index = Array.IndexOf(array, target);

        return index != -1;
    }
}
