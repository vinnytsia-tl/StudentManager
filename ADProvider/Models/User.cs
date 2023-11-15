using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ADProvider.Models
{
    [Serializable]
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
            ZnzExist = false;
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

        public void UpdateGroup(UserPrincipal user, string group)
        {
            DirectoryEntry de = user.GetUnderlyingObject() as DirectoryEntry;

            Group = group;
            de.Properties["department"].Value = group;
            user.Save();
        }

        public void DeferredUpdateGroup(string group)
        {
            Group = group;
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

        public void SetZnzData(UserFullName nameUA, string group)
        {
            ZnzNameUA = nameUA;
            ZnzGroup = group;
            ZnzExist = true;

            if (ZnzGroup == Group && ZnzNameUA == NameUA)
            {
                ZnzStatus = Loaded ? ZnzSyncStatus.InSync : ZnzSyncStatus.Create;
                return;
            }

            string groupName = Group.Split('-')[0];
            int groupNumber = int.Parse(Group.Split('-')[1][0].ToString());

            string znzGroupName = ZnzGroup.Split('-')[0];
            int znzGroupNumber = int.Parse(ZnzGroup.Split('-')[1][0].ToString());

            if (ZnzNameUA != NameUA)
                ZnzStatus |= ZnzSyncStatus.NameFix;

            if (znzGroupNumber - groupNumber == 1)
                ZnzStatus |= ZnzSyncStatus.GroupNext;

            if (znzGroupName == groupName && ((ZnzStatus & ZnzSyncStatus.NameFix) != 0))
                return;

            if (znzGroupName == groupName && ((ZnzStatus & ZnzSyncStatus.GroupNext) != 0))
                return;
                 
            if (znzGroupName != groupName && ((ZnzStatus & ZnzSyncStatus.GroupNext) != 0))
            {
                ZnzStatus |= ZnzSyncStatus.GroupUpdate;
                return;
            }

            //; throw new ArgumentException
            Console.WriteLine($"User {nameUA} Strange group update {Group} -> {ZnzGroup}");
        }

        public UserFullName ZnzNameUA { get; private set; }
        public string ZnzGroup { get; private set; }
        public bool ZnzExist { get; private set; }
        public ZnzSyncStatus ZnzStatus { get; private set; }

        public void NameFromZnz()
        {
            NameUA = ZnzNameUA;
            Group = ZnzGroup;
            NameEN = Transliteration.Transliterate(NameUA);
        }

        public User DeepClone()
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, this);
                ms.Position = 0;

                return (User)formatter.Deserialize(ms);
            }
        }
    }
}
