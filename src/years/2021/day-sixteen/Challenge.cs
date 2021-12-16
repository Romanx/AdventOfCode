using System.Buffers.Binary;
using System.Buffers.Text;
using System.Runtime.InteropServices;

namespace DaySixteen2021;

public class Challenge : ChallengeSync
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2021, 12, 16), "Packet Decoder");

    public override void PartOne(IInput input, IOutput output)
    {
        var str = input.Content.AsString().Trim();
        var binary = ConvertHexToBinary(str);

        var packet = PacketParser.ParsePacket(binary);
        var version = CalculateTotalVersion(packet);

        output.WriteProperty("Total Version", version);

        static int CalculateTotalVersion(Packet packet)
        {
            if (packet is LiteralPacket lp)
            {
                return lp.Version;
            }
            else if (packet is OperatorPacket op)
            {
                var sum = op.Version;
                foreach (var inner in op.InnerPackets)
                {
                    sum += CalculateTotalVersion(inner);
                }
                return sum;
            }

            throw new InvalidOperationException("What!?");
        }
    }

    public override void PartTwo(IInput input, IOutput output)
    {
        var str = input.Content.AsString();
        var binary = ConvertHexToBinary(str);

        var packet = PacketParser.ParsePacket(binary);

        var result = packet.Calculate();

        output.WriteProperty("Result of Packet", result);
    }

    static ReadOnlyMemory<char> ConvertHexToBinary(ReadOnlySpan<char> input)
    {
        var arr = new char[input.Length * 4];
        var span = arr.AsSpan();

        for (var i = 0; i < input.Length; i++)
        {
            ReadOnlySpan<char> bits = input[i] switch
            {
                '0' => "0000",
                '1' => "0001",
                '2' => "0010",
                '3' => "0011",
                '4' => "0100",
                '5' => "0101",
                '6' => "0110",
                '7' => "0111",
                '8' => "1000",
                '9' => "1001",
                'A' => "1010",
                'B' => "1011",
                'C' => "1100",
                'D' => "1101",
                'E' => "1110",
                'F' => "1111",
                _ => throw new InvalidOperationException($"Not a Hexidecimal value: '{input[i]}'")
            };

            bits.CopyTo(span[..4]);
            span = span[4..];
        }

        return arr;
    }
}

abstract record Packet(int Version, int Type)
{
    public abstract long Calculate();
}

record LiteralPacket(int Version, int Type, long Value) : Packet(Version, Type)
{
    public override long Calculate() => Value;
}

record OperatorPacket(int Version, int Type, int LengthType, Packet[] InnerPackets) : Packet(Version, Type)
{
    public override long Calculate()
    {
        return Type switch
        {
            0 => InnerPackets.Sum(ip => ip.Calculate()),
            1 => InnerPackets.Aggregate(1L, (acc, ip) => acc * ip.Calculate()),
            2 => InnerPackets.Min(ip => ip.Calculate()),
            3 => InnerPackets.Max(ip => ip.Calculate()),
            5 => InnerPackets[0].Calculate() > InnerPackets[1].Calculate() ? 1 : 0,
            6 => InnerPackets[0].Calculate() < InnerPackets[1].Calculate() ? 1 : 0,
            7 => InnerPackets[0].Calculate() == InnerPackets[1].Calculate() ? 1 : 0,
            _ => throw new NotImplementedException($"Not implemented that type yet: '{Type}'"),
        };
    }
}
