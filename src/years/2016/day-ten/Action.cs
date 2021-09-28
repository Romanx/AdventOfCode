namespace DayTen2016
{
    record Action();

    record Assignment(int BotNumber, int Value) : Action();

    record Transfer(int BotNumber, Target LowTarget, Target HighTarget) : Action();

    record Target(TargetType TargetType, int Number);
}
