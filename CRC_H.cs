using System.Collections.Generic;
using System.Linq;

public sealed class CRC_H {

    private readonly int _polynom;

    public static readonly CRC_H Default = new CRC_H(0xA001);

    public CRC_H(int polynom) {
        _polynom = polynom;
    }

    public int CalcCrc16(IEnumerable<byte> buffer) {
        return CalcCrc16(buffer, 0, buffer.Count(), _polynom, 0);
    }

    public int CalcCrc16(IEnumerable<byte> buffer, int offset, int bufLen, int polynom, int preset) {
        preset &= 0xFFFF;
        polynom &= 0xFFFF;
        var crc = preset;
        for (var i = offset; i < bufLen; i++) {
            var data = buffer.ElementAt(i);
            crc ^= data;
            for (var j = 0; j < 8; j++) {
                crc = (crc & 0x0001) != 0 ? (crc >> 1) ^ polynom : crc >> 1;
            }
        }
        return crc & 0xFFFF;
    }
}