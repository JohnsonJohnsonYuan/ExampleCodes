/// ����������:
/// Source: https://stackoverflow.com/questions/3825390/effective-way-to-find-any-files-encoding
/// ��һ���������ļ��ж�ȡ
/// �ڶ���������Stream�ж�ȡ, Read����Ϊchar����

/// 1. GetFrom File
/// <summary>
/// Determines a text file's encoding by analyzing its byte order mark (BOM).
/// Defaults to ASCII when detection of the text file's endianness fails.
/// </summary>
/// <param name="filename">The text file to analyze.</param>
/// <returns>The detected encoding.</returns>
public static Encoding GetEncoding(string filename)
{
    // Read the BOM
    var bom = new byte[4];
    using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
    {
        file.Read(bom, 0, 4);
    }

    // Analyze the BOM
    if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
    if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
    if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
    if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
    if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
    return Encoding.Default;
}

/// 2. Get From Stream
/// <summary>
/// Determines a text file's encoding by analyzing its byte order mark (BOM).
/// Defaults to ASCII when detection of the text file's endianness fails.
/// </summary>
/// <param name="filename">The text file to analyze.</param>
/// <returns>The detected encoding.</returns>
public static Encoding GetEncoding(Stream stream)
{
    // Read the BOM
    var bom = new char[4];
    using (var reader = new StreamReader(stream))
    {
        reader.Read(bom, 0, 4);
    }

    // Analyze the BOM
    if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
    if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
    if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
    if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
    if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
    return Encoding.Default;
}

/// <summary>
/// StreamReader.CurrentEncoding���ж�
/// �������Ե��ļ�ʱasciiʱ����utf-8, ���Ƽ�ʹ��
/// </summary>
/// <param name="filename">The path to the file
private static Encoding GetEncoding(string filename)
{
    // This is a direct quote from MSDN:  
    // The CurrentEncoding value can be different after the first
    // call to any Read method of StreamReader, since encoding
    // autodetection is not done until the first call to a Read method.

    using (var reader = new StreamReader(filename, Encoding.Default, true))
    {
        if (reader.Peek() >= 0) // you need this!
            reader.Read();

        return reader.CurrentEncoding;
    }
}