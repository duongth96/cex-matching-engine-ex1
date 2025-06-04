namespace CEX.MatchingEngine.Core.enums;

public enum CancelReason : byte
{
    UserRequested = 1,
    NoLiquidity, //MarketOrderNoLiquidity
    ImmediateOrCancel,
    FillOrKill,
    BookOrCancel,
    ValidityExpired,
    LessThanStepSize, //MarketOrderCannotMatchLessThanStepSize
    InvalidOrder,
    SelfMatch,
}