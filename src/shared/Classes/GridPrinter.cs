﻿using System;
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

        public static string Print<T>(
            IReadOnlyDictionary<Point2d, T> map,
            Func<IReadOnlyDictionary<Point2d, T>, IGridWriter, GridPrinter<T>> printerFactory)
            where T : struct
        {
            var builder = new StringBuilder();
            var writer = new StringGridWriter(builder);
            var printer = printerFactory(map, writer);
            printer.Scan();
            return builder.ToString().Trim();
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
            var item = _map[point];
            if (item.GetType().IsEnum)
            {
                _writer.Append(EnumHelpers.ToDisplayName(item));
            }
            else
            {
                _writer.Append(item.ToString() ?? string.Empty);
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


    public interface IGridWriter
    {
        public void AppendLine();

        public void Append(string c);
    }
}
