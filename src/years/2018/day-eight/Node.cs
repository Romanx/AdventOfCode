namespace DayEight2018;

record Node
{
    public Node(Node[] children, int[] metadata)
    {
        Children = children;
        Metadata = metadata;

        Value = CalculateValue(children, metadata);
    }

    public Node[] Children { get; }

    public int[] Metadata { get; }

    public int Value { get; }

    private static int CalculateValue(Node[] children, int[] metadata)
    {
        if (children.Length == 0)
        {
            return metadata.Sum();
        }

        var total = 0;
        foreach (var meta in metadata)
        {
            if (meta is 0)
                continue;

            if (meta > children.Length)
                continue;

            var child = children[meta - 1];
            total += child.Value;
        }

        return total;
    }
}
