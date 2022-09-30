namespace DaySixteen2021;

static class PacketParser
{
    public static Packet ParsePacket(ReadOnlyMemory<char> binary)
    {
        var (packet, _) = ParsePacketInner(binary);

        return packet;
    }

    private static (Packet Packet, ReadOnlyMemory<char> Remaining) ParsePacketInner(ReadOnlyMemory<char> binary)
    {
        var span = binary.Span;
        var version = Convert.ToInt32(new string(span[0..3]), 2);
        var type = Convert.ToInt32(new string(span[3..6]), 2);
        var rest = binary[6..];

        if (type == 4)
        {
            var (packet, remaining) = ParseLiteralPacket(version, type, rest);
            return (packet, remaining);
        }
        else
        {
            var lengthTypeId = rest.Span[0];
            if (lengthTypeId == '0')
            {
                var packetLength = rest[1..16];
                rest = rest[16..];
                var length = Convert.ToInt32(new string(packetLength.Span), 2);
                var subpackets = rest[..length];
                rest = rest[length..];

                var packet = ParseOperatorPacketTypeZero(version, type, subpackets);
                return (packet, rest);
            }
            else
            {
                var packetLength = rest[1..12];
                rest = rest[12..];
                var numberOfPackets = Convert.ToInt32(new string(packetLength.Span), 2);

                var (packet, remaining) = ParseOperatorPacketTypeOne(version, type, numberOfPackets, rest);
                return (packet, remaining);
            }
        }

        throw new InvalidOperationException("Not handled yet!");
    }

    static (LiteralPacket Packet, ReadOnlyMemory<char> Remaining) ParseLiteralPacket(int version, int type, ReadOnlyMemory<char> binary)
    {
        var length = 0;

        var valueBits = new List<char>();
        var span = binary.Span;

        var foundLast = false;
        var index = 0;
        while (foundLast is false)
        {
            if (span[index] == '0')
            {
                foundLast = true;
            }

            var slice = span[(index + 1)..(index + 5)];
            for (var i = 0; i < slice.Length; i++)
            {
                valueBits.Add(slice[i]);
            }

            index += 5;
            length += 5;
        }

        var value = Convert.ToInt64(string.Join("", valueBits), 2);

        //if (length % 4 != 0)
        //{
        //    length += (length % 4);
        //}

        var rest = binary[length..];

        return (new LiteralPacket(version, type, value), rest);
    }

    static OperatorPacket ParseOperatorPacketTypeZero(
        int version,
        int type,
        ReadOnlyMemory<char> subpackets)
    {
        var packets = new List<Packet>();
        while (subpackets.Length > 0)
        {
            var (packet, remaining) = ParsePacketInner(subpackets);

            packets.Add(packet);
            subpackets = remaining;
        }

        return new OperatorPacket(version, type, 0, packets.ToArray());
    }

    static (OperatorPacket Packet, ReadOnlyMemory<char> Remaining) ParseOperatorPacketTypeOne(
        int version,
        int type,
        int packetCount,
        ReadOnlyMemory<char> binary)
    {
        var packets = new Packet[packetCount];
        var body = binary;

        for (var i = 0; i < packets.Length; i++)
        {
            (var packet, body) = ParsePacketInner(body);
            packets[i] = packet;
        }

        return (new OperatorPacket(version, type, 1, packets), body);
    }
}
