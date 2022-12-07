using MoreLinq.Extensions;

namespace DaySeven2022;

static class OutputParser
{
    public static Directory ParseFromOutput(string[] commandOutput)
    {
        Directory root = new("/", null, new List<IFileSystemItem>());

        Directory? current = null;

        for (var i = 0; i < commandOutput.Length; i++)
        {
            var line = commandOutput[i].AsSpan();

            if (line[0] is not '$')
            {
                throw new InvalidOperationException("Unhandled command!");
            }

            if (line.StartsWith("$ cd "))
            {
                var arg = line[5..];

                if (arg is "/")
                {
                    current = root;
                }
                else if (arg is "..")
                {
                    ArgumentNullException.ThrowIfNull(current);
                    current = current.Parent;
                }
                else
                {
                    ArgumentNullException.ThrowIfNull(current);
                    foreach (var directory in current.Children.OfType<Directory>())
                    {
                        if (directory.Name.AsSpan().Equals(arg, StringComparison.Ordinal))
                        {
                            current = directory;
                            break;
                        }
                    }
                }
            }
            else if (line is "$ ls")
            {
                var start = i + 1;
                var end = commandOutput.Length; ;

                for (var x = i + 1; x < commandOutput.Length; x++)
                {
                    var next = commandOutput[x].AsSpan();
                    if (next[0] is '$')
                    {
                        end = x;
                        break;
                    }
                }

                ParseDirectoryItemList(commandOutput.AsSpan()[start..end], current!);
                i = end - 1;
            }
        }

        return root;
    }

    private static void ParseDirectoryItemList(ReadOnlySpan<string> lines, Directory current)
    {
        foreach (var line in lines)
        {
            var span = line.AsSpan();

            if (span.StartsWith("dir"))
            {
                var dirName = new string(span[4..]);
                var directory = new Directory(
                    dirName,
                    current, 
                    new List<IFileSystemItem>());

                current.Children.Add(directory);
            }
            else
            {
                var splitter = span.IndexOf(' ');
                var size = long.Parse(span[..splitter]);
                var name = new string(span[(splitter + 1)..]);

                var item = new File(name, current, size);
                current.Children.Add(item);
            }
        }
    }
}

record Directory(
    string Name,
    Directory? Parent,
    List<IFileSystemItem> Children) : IFileSystemItem
{
    public long Size => CalculateSize(Children);

    private static long CalculateSize(List<IFileSystemItem> children)
    {
        var size = 0L;
        foreach (var item in children)
        {
            size += item.Size;
        }

        return size;
    }

    public IEnumerable<Directory> Flatten()
    {
        yield return this;
        foreach (var directory in Children.OfType<Directory>().SelectMany(d => d.Flatten()))
        {
            yield return directory;
        }
    }
}

readonly record struct File(
    string Name,
    Directory Parent,
    long Size) : IFileSystemItem;

interface IFileSystemItem 
{
    public long Size { get; }
}
