using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Shared.Grid;

namespace Shared
{
    public static class GridPrinter
    {
        public static string Print<T>(IReadOnlyDictionary<Point2d, T> map)
            where T : struct
            => Print(map, static (map, writer) => new GridPrinter<T>(map, writer));

        public static string Print<T>(IReadOnlyDictionary<Point2d, T> map, Func<IReadOnlyDictionary<Point2d, T>, IGridWriter, GridPrinter<T>> printerFactory)
            where T : struct
        {
            var builder = new StringBuilder();
            var writer = new StringGridWriter(builder);
            var printer = printerFactory(map, writer);
            printer.Scan();
            return builder.ToString().Trim();
        }

        public static void Write<T>(IReadOnlyDictionary<Point2d, T> map, TextWriter streamWriter)
            where T : struct
            => Write(map, streamWriter, static (map, writer) => new GridPrinter<T>(map, writer));

        public static void Write<T>(IReadOnlyDictionary<Point2d, T> map, TextWriter streamWriter, Func<IReadOnlyDictionary<Point2d, T>, IGridWriter, GridPrinter<T>> printerFactory)
            where T : struct
        {
            var writer = new TextGridWriter(streamWriter);
            var printer = printerFactory(map, writer);
            printer.Scan();
        }

        private readonly struct StringGridWriter : IGridWriter
        {
            private readonly StringBuilder _builder;

            public StringGridWriter(StringBuilder builder)
            {
                _builder = builder;
            }

            public void Append(string c) => _builder.Append(c);

            public void AppendLine() => _builder.AppendLine();
        }

        private readonly struct TextGridWriter : IGridWriter
        {
            private readonly TextWriter _textWriter;

            public TextGridWriter(TextWriter textWriter)
            {
                _textWriter = textWriter;
            }

            public void Append(string c) => _textWriter.Write(c);

            public void AppendLine() => _textWriter.WriteLine();
        }
    }

    public class GridPrinter<T> : GridScanner where T : struct
    {
        protected readonly IReadOnlyDictionary<Point2d, T> _map;
        protected readonly IGridWriter _writer;

        public GridPrinter(IReadOnlyDictionary<Point2d, T> map, IGridWriter writer)
            : base(Area2d.Create(map.Keys))
        {
            _map = map;
            _writer = writer;
        }

        public override void OnEndOfRow() => _writer.AppendLine();

        public override void OnPosition(Point2d point)
        {
            _writer.Append(EnumHelpers.ToDisplayName(_map[point]));
        }
    }


    public interface IGridWriter
    {
        public void AppendLine();

        public void Append(string c);
    }
}
