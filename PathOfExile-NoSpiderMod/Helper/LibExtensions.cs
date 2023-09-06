using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibBundle3.Nodes;

namespace PathOfExile_NoSpiderMod.Helper;

public static class LibExtensions
{
    // https://github.com/aianlinb/LibGGPK3/blob/8f3a3a403ada6e5d52f88d139377824ab64e55fd/LibBundle3/Index.cs#L545C31-L545C31
    public static ITreeNode? FindNodeNoCase(this LibBundle3.Index index, string path, DirectoryNode? root = null)
    {
        root ??= index.Root;
        var SplittedPath = path.Split('/', '\\');
        foreach (var name in SplittedPath)
        {
            if (name == "")
                return root;
            var next = root.Children.FirstOrDefault(n => n.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (next is not DirectoryNode dn)
                return next;
            root = dn;
        }
        return root;
    }
}
