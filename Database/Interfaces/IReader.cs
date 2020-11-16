using System;

namespace Database.Interfaces {

    public interface IReader {

        bool IsDBNull (int i);
        bool Read();
        string GetString(int i);
        int GetInt32 (int i);
        DateTime GetDateTime(int i);
        double GetDouble (int i);
        bool GetBoolean (int i);

    }

}