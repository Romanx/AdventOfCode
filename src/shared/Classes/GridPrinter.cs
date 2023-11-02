using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Shared.Grid;

namespace Shared
{
    public static class GridPrinter
    {
        public static string Print<T>(IReadOnlyDictionary<Point2d, T> map, char empty = '#')
            where T : struct
            => Print(map, empty, static (map, writer, empty) => new GridPrinter<T>(map, writer, empty));

        public static string Print<T>(
            IReadOnlyDictionary<Point2d, T> map,
            char empty,
            Func<IReadOnlyDictionary<Point2d, T>, IGridWriter, char, GridPrinter<T>> printerFactory)
            where T : struct
        {
            var builder = new StringBuilder();
            var writer = new StringGridWriter(builder);
            var printer = printerFactory(map, writer, empty);
            printer.Scan();
            return builder.ToString().Trim();
        }

        public static string Print<T>(T[,] array)
            where T : struct
            => Print(array, static (array, writer) => new ArrayPrinter<T>(array, writer));

        public static string Print<T>(
            T[,] array,
            Func<T[,], IGridWriter, ArrayPrinter<T>> printerFactory)
            where T : struct
        {
            var builder = new StringBuilder();
            var writer = new StringGridWriter(builder);
            var printer = printerFactory(array, writer);
            printer.Scan();
            return builder.ToString();
        }

        public static string Print(IEnumerable<Point2d> points, char identifier)
            => Print(points, identifier, static (points, identifier, writer) => new GridPointPrinter(new HashSet<Point2d>(points), identifier, writer));

        public static string Print(
            IEnumerable<Point2d> points,
            char identifier,
            Func<IEnumerable<Point2d>, char, IGridWriter, GridPointPrinter> printerFactory)
        {
            var builder = new StringBuilder();
            var writer = new StringGridWriter(builder);
            var printer = printerFactory(points, identifier, writer);
            printer.Scan();
            return builder.ToString().Trim();
        }

        public static void Write<T>(IReadOnlyDictionary<Point2d, T> map, TextWriter streamWriter, char empty = '#')
            where T : struct
            => Write(map, streamWriter, empty, static (map, writer, empty) => new GridPrinter<T>(map, writer, empty));

        public static void Write<T>(
            IReadOnlyDictionary<Point2d, T> map,
            TextWriter streamWriter,
            char empty,
            Func<IReadOnlyDictionary<Point2d, T>, IGridWriter, char, GridPrinter<T>> printerFactory)
            where T : struct
        {
            var writer = new TextGridWriter(streamWriter);
            var printer = printerFactory(map, writer, empty);
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
        private readonly string empty;

        public GridPrinter(
            IReadOnlyDictionary<Point2d, T> map,
            IGridWriter writer,
            char empty)
            : base(Area2d.Create(map.Keys))
        {
            _map = map;
            _writer = writer;
            this.empty = empty.ToString();
        }

        public override void OnEndOfRow() => _writer.AppendLine();

        public override void OnPosition(Point2d point)
        {
            if (_map.TryGetValue(point, out var item))
            {
                if (item.GetType().IsEnum)
                {
                    _writer.Append(EnumHelpers.ToDisplayName(item));
                }
                else
                {
                    _writer.Append(item.ToString() ?? string.Empty);
                }
            }
            else
            {
                _writer.Append(empty);
            }
        }
    }

    public class GridPointPrinter : GridScanner
    {
        private readonly HashSet<Point2d> _points;
        private readonly string _id;
        private readonly IGridWriter _writer;

        public GridPointPrinter(HashSet<Point2d> points, char id, IGridWriter writer)
            : base(Area2d.Create(points))
        {
            _points = points;
            _id = $"{id}";
            _writer = writer;
        }

        public override void OnEndOfRow() => _writer.AppendLine();

        public override void OnPosition(Point2d point)
        {
            if (_points.Contains(point))
            {
                _writer.Append(_id);
            }
            else
            {
                _writer.Append(".");
            }
        }
    }

    public class ArrayPrinter<T> : GridScanner
        where T : notnull
    {
        private readonly T[,] array;
        private readonly IGridWriter _writer;

        public ArrayPrinter(T[,] array, IGridWriter writer)
            : base(Area2d.Create(array))
        {
            this.array = array;
            _writer = writer;
        }

        public override void OnEndOfRow() => _writer.AppendLine();

        public override void OnPosition(Point2d point)
        {
            var value = array[point.Y, point.X];

            if (value.GetType().IsEnum)
            {
                _writer.Append(EnumHelpers.ToDisplayName(value));
            }
            else
            {
                _writer.Append(value.ToString() ?? string.Empty);
            }
        }
    }

    public interface IGridWriter
    {
        public void AppendLine();

        public void Append(string c);
    }
}
