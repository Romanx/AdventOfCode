namespace DayTwentyOne2016
{
    enum ScrambleType
    {
        SwapPosition,
        SwapLetter,
        RotateDirection,
        RotatePosition,
        ReversePosition,
        MovePosition
    }

    enum Direction
    {
        Left,
        Right
    }

    abstract record ScrambleFunction(ScrambleType Type)
    {
        public abstract void ApplyScramble(Span<char> password);

        public abstract void Descramble(Span<char> password);
    }

    record SwapPosition(int X, int Y) : ScrambleFunction(ScrambleType.SwapPosition)
    {
        public override void ApplyScramble(Span<char> password)
        {
            var xChar = password[X];
            var yChar = password[Y];
            password[X] = yChar;
            password[Y] = xChar;
        }

        public override void Descramble(Span<char> password)
            => ApplyScramble(password);
    }

    record SwapLetter(char X, char Y) : ScrambleFunction(ScrambleType.SwapLetter)
    {
        public override void ApplyScramble(Span<char> password)
        {
            for (var i = 0; i < password.Length; i++)
            {
                if (password[i] == X)
                {
                    password[i] = Y;
                }
                else if (password[i] == Y)
                {
                    password[i] = X;
                }
            }
        }

        public override void Descramble(Span<char> password)
            => ApplyScramble(password);
    }

    record RotateDirection(Direction Direction, int Steps) : ScrambleFunction(ScrambleType.RotateDirection)
    {
        public override void ApplyScramble(Span<char> password)
        {
            Span<char> @new = stackalloc char[password.Length];

            if (Direction is Direction.Left)
            {
                var start = password[(Steps % password.Length)..];
                var end = password[..(Steps % password.Length)];

                start.CopyTo(@new);
                end.CopyTo(@new[start.Length..]);
            }
            else if (Direction is Direction.Right)
            {
                var start = password[^(Steps % password.Length)..];
                var end = password[..^(Steps % password.Length)];

                start.CopyTo(@new);
                end.CopyTo(@new[start.Length..]);
            }

            @new.CopyTo(password);
        }

        public override void Descramble(Span<char> password)
            => new RotateDirection(Direction is Direction.Left ? Direction.Right : Direction.Left, Steps).ApplyScramble(password);
    }

    record RotatePosition(char Character) : ScrambleFunction(ScrambleType.RotatePosition)
    {
        public override void ApplyScramble(Span<char> password)
        {
            var index = password.IndexOf(Character);

            var times = 1 + index;
            if (index >= 4)
            {
                times += 1;
            }

            new RotateDirection(Direction.Right, times)
                .ApplyScramble(password);
        }

        public override void Descramble(Span<char> password)
        {
            // Create some scratch storage
            Span<char> testValue = stackalloc char[password.Length];
            Span<char> scrabledScratch = stackalloc char[password.Length];

            // Try the length of the string in shifting
            for (var offset = 0; offset < password.Length; offset++)
            {
                // Copy the scrambled password fresh
                password.CopyTo(testValue);

                // Apply a left scramble to it with this offset to find the candidate before value.
                new RotateDirection(Direction.Left, offset).ApplyScramble(testValue);

                // Taking the candidate before value, apply a scramble to it.
                testValue.CopyTo(scrabledScratch);
                ApplyScramble(scrabledScratch);

                // If the candidate scrambled is the same as the original scrambled password then it
                // must be the correct before value.
                if (scrabledScratch.SequenceEqual(password))
                {
                    testValue.CopyTo(password);
                    return;
                }
            }

            throw new InvalidOperationException("Something broke!");
        }
    }

    record ReversePosition(int X, int Y) : ScrambleFunction(ScrambleType.ReversePosition)
    {
        public override void ApplyScramble(Span<char> password)
        {
            var slice = password[X..(Y + 1)];
            slice.Reverse();
        }

        public override void Descramble(Span<char> password)
            => ApplyScramble(password);
    }

    record MovePosition(int X, int Y) : ScrambleFunction(ScrambleType.MovePosition)
    {
        public override void ApplyScramble(Span<char> password)
        {
            var arr = new List<char>(password.ToArray());
            var value = arr[X];

            arr.RemoveAt(X);
            arr.Insert(Y, value);

            arr.ToArray().CopyTo(password);
        }

        public override void Descramble(Span<char> password)
            => new MovePosition(Y, X).ApplyScramble(password);
    }
}
