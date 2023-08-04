using System.Text;
using System.Text.RegularExpressions;
using LibBundle3.Nodes;
using LibBundledGGPK;
using LibGGPK3.Records;
using PathOfExile_NoSpiderMod.Checks;
using PathOfExile_NoSpiderMod.Helper;
using PathOfExile_NoSpiderMod.Transforms;

namespace PathOfExile_NoSpiderMod;
class Program
{
    const RegexOptions regexOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase;

    static readonly CheckGroup testGroup = new("Test", ReplacementType.None, new Regex[]
    {
        Regexes.TestGroupRegex()
    });

    static readonly CheckGroup crabGroup = new("Crabs", ReplacementType.Crate, new Regex[]
    {
        Regexes.CrabGroupRegex()
    }, new Regex[]
    {
        Regexes.CrabGroupEx1Regex(),
        Regexes.CrabGroupEx2Regex()
    });

    static readonly CheckGroup wetasGroup = new("Wetas", ReplacementType.Barrel, new Regex[]
    {
        Regexes.WetaGroupRegex()
    });

    static readonly CheckGroup mainGroup = new("Main", ReplacementType.Barrel, new Regex[]
    {
        Regexes.MainGroupRegex(),
        Regexes.MainGroupArakaaliRegex(),
        Regexes.MainGroupRazorLegRegex()
    }, new Regex[]
    {
        Regexes.MainGroupExRegex()
    });

    static readonly CheckGroup[] allGroups = new CheckGroup[]
    {
        testGroup,
        crabGroup,
        wetasGroup,
        mainGroup
    };

    static readonly Dictionary<string, FileData> files = new(StringComparer.OrdinalIgnoreCase);

    const string barrelPath = "metadata/chests/barrels/barrel1.ao";
    const string cratePath = "metadata/chests/crates/crate1.ao";

    static bool writeTestFiles = false;

    static void Main(string[] appArgs)
    {
        Console.WriteLine("Hello, World!");

        string filePath;
        if (appArgs.TryGetIndex("-p", out int pArgIndex, true))
        {
            filePath = appArgs[pArgIndex + 1];
        }
        else
        {
            System.Console.WriteLine("путь пж");
            filePath = Console.ReadLine() ?? "";
        }

        if (!File.Exists(filePath))
        {
            System.Console.WriteLine("Увы, такого файла нет.");
            return;
        }

        bool replace = appArgs.Contains("-r", StringComparer.OrdinalIgnoreCase);
        if (replace)
        {
            System.Console.WriteLine("Будет менять...");
        }

        writeTestFiles = appArgs.Contains("-t", StringComparer.OrdinalIgnoreCase);
        if (writeTestFiles)
        {
            System.Console.WriteLine("Будет писать тестовые файлы...");
        }

        var filesDict = files;

        using var ggpk = new BundledGGPK(filePath, true);

        var meta = (DirectoryNode)ggpk.Index.FindNode("metadata")!;

        var b1 = (FileNode)ggpk.Index.FindNode(barrelPath)!;
        var c1 = (FileNode)ggpk.Index.FindNode(cratePath)!;

        var dirs = meta.Children.OfType<DirectoryNode>().Select(d => d.Name).ToArray();
        System.Console.WriteLine(string.Join(", ", dirs));

        var monsters = (DirectoryNode)ggpk.Index.FindNode("monsters", meta)!;
        var critters = (DirectoryNode)ggpk.Index.FindNode("critters", meta)!;
        var pet = (DirectoryNode)ggpk.Index.FindNode("pet", meta)!;
        var npc = (DirectoryNode)ggpk.Index.FindNode("npc", meta)!;
        var effects = (DirectoryNode)ggpk.Index.FindNode("effects", meta)!;

        DoDir(monsters, TransformFile);
        DoDir(critters, TransformFile);
        DoDir(pet, TransformFile);
        DoDir(npc, TransformFile);
        DoDir(effects, TransformFile);

        DoDir(monsters, SearchForEnemies);
        DoDir(critters, SearchForEnemies);
        DoDir(effects, SearchForEnemies);

        IEnumerable<IGrouping<string, FileData>> stats = files.Values.GroupBy(f => f.whoMatched);
        System.Console.WriteLine($"Всего файлов {files.Count}");

        foreach (var stat in stats)
        {
            System.Console.WriteLine($"{stat.Key}: {stat.Count()}");
        }

        if (replace)
        {
            System.Console.WriteLine("Заменяем.");

            ggpk.Index.Replace(files.Keys, FileContentResolveFunction, false);
        }

        ggpk.Dispose();
    }

    static ReadOnlySpan<byte> FileContentResolveFunction(string filePath)
    {
        System.Console.WriteLine($"Меняем {filePath}");

        return Encoding.Unicode.GetBytes(files[filePath].newContent);
    }

    static void DoDir(DirectoryNode directoryNode, Action<FileNode> action)
    {
        var subFiles = directoryNode.Children.OfType<FileNode>().ToArray();
        foreach (var file in subFiles)
        {
            action(file);
        }

        var subDirectories = directoryNode.Children.OfType<DirectoryNode>().ToArray();
        foreach (var subDir in subDirectories)
        {
            DoDir(subDir, action);
        }
    }

    static void SearchForEnemies(FileNode fileNode)
    {
        if (!fileNode.Name.EndsWith(".aoc"))
            return;

        var aocSource = Encoding.Unicode.GetString(fileNode.Record.Read().ToArray());
        var lines = aocSource.Split("\r\n");

        Match? match = null;
        foreach (var line in lines)
        {
            match = Regexes.SkinLineCheckRegex().Match(line);
            if (match.Success)
                break;
        }

        if (match?.Success != true)
            return;

        string skinPath = match.Groups["SkinPath"].Value;
        if (!skinPath.Contains("spider", StringComparison.OrdinalIgnoreCase))
            return;

        if (!files.ContainsKey(fileNode.Record.Path))
        {
            string otcPath = Path.ChangeExtension(fileNode.Record.Path, "otc");

            ITreeNode? otcNode = fileNode.Record.BundleRecord.Index.FindNode(otcPath);

            if (otcNode is FileNode fileOtc)
            {
                string otcSource = Encoding.Unicode.GetString(fileOtc.Record.Read().ToArray());
                string otcResult = OtcProcess.Do(otcSource, true);
                if (!otcResult.StartsWith("version"))
                    throw new Exception();

                if (otcSource != otcResult)
                {
                    files[fileNode.Record.Path] = new FileData(otcResult, "Searchment");

                    if (writeTestFiles)
                    {
                        var t1 = File.WriteAllTextAsync("1.json", otcSource);
                        var t2 = File.WriteAllTextAsync("2.json", otcResult);

                        Task.WaitAll(t1, t2);

                        System.Console.WriteLine(fileNode.Record.Path);
                        System.Console.WriteLine("ентер пж");
                        Console.ReadLine();
                    }
                }
            }
            else
            {
                System.Console.WriteLine("Otc не найден.");
            }

            string aocResult = AocProcess.Do(aocSource, false);
            if (!aocResult.StartsWith("version"))
                throw new Exception();

            if (aocSource != aocResult)
            {
                if (writeTestFiles)
                {
                    var t1 = File.WriteAllTextAsync("1.json", aocSource);
                    var t2 = File.WriteAllTextAsync("2.json", aocResult);

                    Task.WaitAll(t1, t2);

                    System.Console.WriteLine(fileNode.Record.Path);
                    System.Console.WriteLine("ентер пж");
                    Console.ReadLine();
                }

                files[fileNode.Record.Path] = new FileData(aocResult, "Searchment");
            }
            else
            {
                System.Console.WriteLine("Странное...");
            }

            System.Console.WriteLine($"Свежие новости!\nФайл: {fileNode.Record.Path}\nСкин: {skinPath}");
        }
    }

    static void TransformFile(FileNode fileNode)
    {
        bool aoc = fileNode.Name.EndsWith(".aoc", StringComparison.OrdinalIgnoreCase);

        if (!aoc && !fileNode.Name.EndsWith(".otc", StringComparison.OrdinalIgnoreCase))
            return;

        CheckGroup? group = null;
        foreach (var someGroup in allGroups)
        {
            if (someGroup.Checked(fileNode.Record.Path))
            {
                group = someGroup;
                break;
            }
        }
        if (group == null)
            return;

        string source = Encoding.Unicode.GetString(fileNode.Record.Read().ToArray());

        if (!source.StartsWith("version"))
            throw new Exception();

        string result;

        if (aoc)
        {
            bool arakaali = fileNode.Record.Path.StartsWith("metadata/monsters/arakaali/");

            result = AocProcess.Do(source, arakaali);
        }
        else
        {
            bool lifeChange = group.replacement != ReplacementType.None;
            bool barrel = group.replacement == ReplacementType.Barrel;

            result = OtcProcess.Do(source, lifeChange, barrel);
        }

        if (source != result)
        {
            if (writeTestFiles)
            {
                var t1 = File.WriteAllTextAsync("1.json", source);
                var t2 = File.WriteAllTextAsync("2.json", result);

                Task.WaitAll(t1, t2);

                System.Console.WriteLine(fileNode.Record.Path);
                System.Console.WriteLine("ентер пж");
                Console.ReadLine();
            }

            if (!result.StartsWith("version"))
                throw new Exception();

            System.Console.WriteLine($"Заменяем {fileNode.Record.Path}");

            files[fileNode.Record.Path] = new FileData(result, $"Group: {group.name}");
        }
        else
        {
            System.Console.WriteLine($"Ничего {fileNode.Record.Path}");
        }
    }
}
