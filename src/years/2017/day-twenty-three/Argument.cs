using System;
using System.Collections.Generic;

namespace DayTwentyThree2017
{
    internal readonly struct Argument
    {
        private readonly int? _value;
        private readonly char? _register;

        private Argument(int? value, char? register)
        {
            _value = value;
            _register = register;
        }

        public static Argument Register(char register) => new(null, register);

        public static Argument Value(int value) => new(value, null);

        public static Argument Parse(ReadOnlySpan<char> str)
        {
            if (str.Length == 1 && char.IsLetter(str[0]))
            {
                return Register(str[0]);
            }

            return Value(int.Parse(str));
        }

        public long GetValue(Dictionary<char, long> registers)
        {
            if (_value is not null)
            {
                return _value.Value;
            }

            return registers.TryGetValue(_register!.Value, out var registerVal)
                ? registerVal
                : 0L;
        }

        public int GetValue()
        {
            if (_value is not null)
            {
                return _value.Value;
            }

            throw new InvalidOperationException("Cannot get value if not a value");
        }

        public static implicit operator char(Argument arg)
        {
            if (arg._register is null)
            {
                throw new InvalidOperationException("Cannot cast if not a register");
            }

            return arg._register.Value;
        }

        public override string ToString()
        {
            if (_value is null)
            {
                return _register!.ToString()!;
            }

            return _value.Value.ToString();
        }
    }
}
