using System;

public class PacketReader<T> {

    public PacketReader(string data, Func<string, T> converter, int unit_size = 1) {
        Data = data;
        UnitSize = unit_size;
        Converter = converter;
    }

    public string Data { get; private set; }
    public int UnitSize { get; }
    public Func<string, T> Converter { get; set; }

    public T Read(int length, Func<string, T> converter = null) {
        ValidateLength(length);
        var value = Data.Substring(0, UnitSize * length);
        Data = Data.Substring(UnitSize * length);
        return Process(value, converter);
    }

    public N CustomRead <N> (int length, Func<string, N> converter) {
        ValidateLength(length);
        var value = Data.Substring(0, UnitSize * length);
        Data = Data.Substring(UnitSize * length);
        return converter(value);
    }

    public T ReadEnd (int length, Func<string, T> converter = null) {
        ValidateLength(length);
        var value = Data.Substring(Data.Length - (length*UnitSize));
        Data = Data.Substring(0, Data.Length - (length*UnitSize));
        return Process(value, converter);
    }

    void ValidateLength (int length) {
        if (length < 0) throw new Exception($"Read failed because the length of the data is invalid.");
        if (length > Data.Length) throw new Exception($"Length of substring was greater than the string length.");
    }

    T Process(string value, Func<string, T> converter = null) {
        if (converter == null) {
            return Converter(value);
        } else {
            return converter(value);
        }
    }


}

public class PacketReader : PacketReader<string> {

    public PacketReader(string data, int index = 0, int unit_size = 1) : base(data, (x) => x, unit_size) {
    }

}