using System;
using System.Collections.Generic;
using System.Linq;

namespace ADProvider.Models
{
    [Serializable]
    public class UserFullName
    {
        public UserFullName(string firstName, string middleName, string surName)
        {
            FirstName = firstName;
            MiddleName = middleName;
            SurName = surName;
        }
        public string FirstName { get; private set; }
        public string MiddleName { get; private set; }
        public string SurName { get; private set; }

        override public string ToString() => $"User<{SurName} {FirstName} {MiddleName}>";

        public bool PartialEqual(UserFullName other)
        {
            Dictionary<char, int> cnt = new Dictionary<char, int>();

            var line1 = FirstName + MiddleName + SurName;
            var line2 = other.FirstName + other.MiddleName + other.SurName;

            foreach (var ch in line1)
                if (cnt.ContainsKey(ch))
                    cnt[ch]++;
                else
                    cnt.Add(ch, 1);

            foreach (var ch in line2)
                if (cnt.ContainsKey(ch))
                    cnt[ch]--;
                else
                    cnt.Add(ch, -1);

            return cnt.Sum((kv) => Math.Abs(kv.Value)) == 1;
        }
        static public bool operator ==(UserFullName first, UserFullName second)
        {
            if ((object)first != null && (object)second == null) return false;
            if ((object)first == null && (object)second != null) return false;
            if ((object)first == null && (object)second == null) return true;
            if ((first.FirstName == second.FirstName) &&
            (first.MiddleName == second.MiddleName) &&
            (first.SurName == second.SurName))
                return true;

            return false;
        }
        static public bool operator !=(UserFullName first, UserFullName second)
        {
            return !(first == second);
        }
           
    }
}
