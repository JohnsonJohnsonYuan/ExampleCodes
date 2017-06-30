/// <summary>
/// Convert byte array to hex string
/// </summary
public static string ToHexString(this byte[] hex) {
    if (hex == null) {
        return null;
    }
    if (hex.Length == 0) {
        return string.Empty;
    }
    var s = new StringBuilder();
    foreach (byte b in hex) {
        s.Append(b.ToString("x2"));
    }
    return s.ToString();
}

/// <summary>
/// Convert hex string to bytes
/// ef321e3f => using 4 times to convert: ef 32 1e 3f
/// </summary
public static byte[] ToHexBytes(this string hex)
{
    if (hex == null) {
        return null;
    }
    if (hex.Length == 0) {
        return new byte[0];
    }
    int l = hex.Length / 2;
    var b = new byte[l];
    for (int i = 0; i < l; ++i) {
        b[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
    }
    return b;
}