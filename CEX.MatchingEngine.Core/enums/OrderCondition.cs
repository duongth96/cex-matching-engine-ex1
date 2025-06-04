namespace CEX.MatchingEngine.Core.enums;

public enum OrderCondition : byte
{
    None = 0,
    IOC = 1, //ImmediateOrCancel
    BOC = 2, //BookOrCancel
    FOK = 4, //FillOrKill
}