using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADProvider.Models
{
    public class User
    {
        public User(string firstName, string middleName, string surname, string group) :
            this(new UserFullName(firstName, middleName, surname), group)
        { }
        public User(UserFullName nameUA, string group)
        {
            NameUA = nameUA;
            NameEN = Transliteration.Transliterate(NameUA);
            Group = group;
        }
        public User(UserPrincipal user)
        {
            Loaded = true;
            DirectoryEntry de = user.GetUnderlyingObject() as DirectoryEntry;

            SamAccountName = user.SamAccountName;
            NameUA = new UserFullName(user.GivenName, user.MiddleName, user.Surname);
            NameEN = Transliteration.Transliterate(NameUA);

            Group = de.Properties["department"].Value.ToString();
            LastPasswordSet = user.LastPasswordSet;
        }

        public UserFullName NameUA { get; private set; }
        public UserFullName NameEN { get; private set; }
        public string SamAccountName { get; set; }
        public string Password { get; set; }
        // LastPasswordSet == null => Expired
        public DateTime? LastPasswordSet { get; private set; }
        public string Group { get; private set; }
        public bool Loaded { get; private set; }
        public string Magic { get => Helpers.Crc8(NameUA.SurName + NameUA.MiddleName + NameUA.FirstName + Group); }

        public string Name => $"{NameUA.SurName} {NameUA.FirstName} {NameUA.MiddleName} ({Group})";
        public string DisplayName => $"{NameUA.SurName} {NameUA.FirstName[0]}. {NameUA.MiddleName[0]}.";
        public string[] GenerateSams()
        {
            var fn = Helpers.NameFix(NameEN.FirstName).ToLower();
            var mn = Helpers.NameFix(NameEN.MiddleName).ToLower();
            var sn = Helpers.NameFix(NameEN.SurName).ToLower();

            return new string[] {
                fn + "." + sn,
                fn.First() + "." + sn,
                fn.First() + "." + mn.First() + "." + sn,
                fn.First() + sn,
                fn.First() + "." + sn + Magic,
            };
        }

    }
}
