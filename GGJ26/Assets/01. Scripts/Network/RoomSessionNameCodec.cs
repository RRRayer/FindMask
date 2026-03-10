using System;

public static class RoomSessionNameCodec
{
    private const string PasswordSeparator = "#";
    private const string SessionIdSeparator = "@";
    private const string ModeMarker = "::m=";

    public static string Encode(string roomName, string password, string mode)
    {
        string safeRoom = string.IsNullOrWhiteSpace(roomName) ? "room" : roomName.Trim();
        string safeMode = NormalizeMode(mode);
        string session = safeRoom;

        if (string.IsNullOrEmpty(password) == false)
        {
            session += PasswordSeparator + password;
        }

        session += ModeMarker + safeMode;
        return session;
    }

    public static string DecodeMode(string sessionName)
    {
        if (string.IsNullOrWhiteSpace(sessionName))
        {
            return GameModeRuntime.Classic;
        }

        int marker = sessionName.LastIndexOf(ModeMarker, StringComparison.Ordinal);
        if (marker < 0)
        {
            return GameModeRuntime.Classic;
        }

        string mode = sessionName.Substring(marker + ModeMarker.Length).Trim();
        return NormalizeMode(mode);
    }

    public static string DecodeDisplayRoomName(string sessionName)
    {
        if (string.IsNullOrWhiteSpace(sessionName))
        {
            return string.Empty;
        }

        string withoutMode = RemoveModeSuffix(sessionName);
        string withoutSessionId = RemoveSessionIdSuffix(withoutMode);
        int separator = withoutSessionId.IndexOf(PasswordSeparator, StringComparison.Ordinal);
        if (separator >= 0)
        {
            return withoutSessionId.Substring(0, separator);
        }

        return withoutSessionId;
    }

    public static bool HasPassword(string sessionName)
    {
        if (string.IsNullOrWhiteSpace(sessionName))
        {
            return false;
        }

        string withoutMode = RemoveModeSuffix(sessionName);
        string withoutSessionId = RemoveSessionIdSuffix(withoutMode);
        int separator = withoutSessionId.IndexOf(PasswordSeparator, StringComparison.Ordinal);
        return separator >= 0 && separator < withoutSessionId.Length - 1;
    }

    public static bool MatchesRoomAndPassword(string sessionName, string roomName, string password)
    {
        string safeRoom = roomName == null ? string.Empty : roomName.Trim();
        string safePassword = password == null ? string.Empty : password.Trim();
        string withoutMode = RemoveModeSuffix(sessionName);
        string withoutSessionId = RemoveSessionIdSuffix(withoutMode);

        int separator = withoutSessionId.IndexOf(PasswordSeparator, StringComparison.Ordinal);
        string baseRoom = separator >= 0 ? withoutSessionId.Substring(0, separator) : withoutSessionId;
        string pass = separator >= 0 && separator < withoutSessionId.Length - 1 ? withoutSessionId.Substring(separator + 1) : string.Empty;

        return string.Equals(baseRoom, safeRoom, StringComparison.Ordinal) &&
               string.Equals(pass, safePassword, StringComparison.Ordinal);
    }

    public static string DecodePassword(string sessionName)
    {
        if (string.IsNullOrWhiteSpace(sessionName))
        {
            return string.Empty;
        }

        string withoutMode = RemoveModeSuffix(sessionName);
        string withoutSessionId = RemoveSessionIdSuffix(withoutMode);
        int separator = withoutSessionId.IndexOf(PasswordSeparator, StringComparison.Ordinal);
        if (separator < 0 || separator >= withoutSessionId.Length - 1)
        {
            return string.Empty;
        }

        return withoutSessionId.Substring(separator + 1);
    }

    private static string RemoveModeSuffix(string sessionName)
    {
        int marker = sessionName.LastIndexOf(ModeMarker, StringComparison.Ordinal);
        if (marker < 0)
        {
            return sessionName;
        }

        return sessionName.Substring(0, marker);
    }

    private static string RemoveSessionIdSuffix(string sessionName)
    {
        int separator = sessionName.LastIndexOf(SessionIdSeparator, StringComparison.Ordinal);
        if (separator < 0)
        {
            return sessionName;
        }

        return sessionName.Substring(0, separator);
    }

    private static string NormalizeMode(string mode)
    {
        return string.Equals(mode, GameModeRuntime.Deathmatch, StringComparison.OrdinalIgnoreCase)
            ? GameModeRuntime.Deathmatch
            : GameModeRuntime.Classic;
    }
}
