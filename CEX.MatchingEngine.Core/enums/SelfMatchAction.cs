namespace CEX.MatchingEngine.Core.enums;

public enum SelfMatchAction : byte
{
    Match = 0,
    Reduce, //Decrease old order
    Reject, //Cancel if old order existed
    Replace, //Cancel old order
}