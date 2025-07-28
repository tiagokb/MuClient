public enum LoginAttemptResponseEnum : byte
{
    InvalidPassword = 0x00,
    Okay = 0x01,
    AccountInvalid = 0x02,
    AccountAlreadyConnected = 0x03,
    ServerIsFull = 0x04,
    AccountBlocked = 0x05,
    WrongVersion = 0x06,
    ConnectionError = 0x07,
    ConnectionClosed3Fails = 0x08,
    UnkownError = 0xFF // fallback
}