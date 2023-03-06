using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using ADProvider.Models;

namespace ADProvider
{
    public class StudentManager
    {
        private List<User> students;
        private ADConfig adConfig;

        private UserPrincipal studentsContainerContext;
        private GroupPrincipal studentGroupContext;

        public delegate void ProgressCallBack(ActionStatus status, double progress);

        public StudentManager(ADConfig config)
        {
            students = new List<User>();
            adConfig = config;
        }

        public void LoadADContexts(ProgressCallBack progress = null)
        {
            progress?.Invoke(ActionStatus.SendingADRequest, 0);

            PrincipalContext upc = new PrincipalContext(ContextType.Domain, adConfig.Domain,
                adConfig.Containers.Student, adConfig.Admin.User, adConfig.Admin.Password); 
            PrincipalContext gpc = new PrincipalContext(ContextType.Domain, adConfig.Domain,
                adConfig.Containers.Group, adConfig.Admin.User, adConfig.Admin.Password);

            studentsContainerContext = new UserPrincipal(upc);
            studentGroupContext = GroupPrincipal.FindByIdentity(gpc, IdentityType.DistinguishedName,
                adConfig.Groups.Students);
        }

        public void LoadFromAD(ProgressCallBack progress = null)
        {
            progress?.Invoke(ActionStatus.SendingADRequest, 0);

            PrincipalSearcher ups = new PrincipalSearcher(studentsContainerContext);
            var adUsers = ups.FindAll().ToArray();
            foreach (UserPrincipal adUser in adUsers)
            {
                // Ignore Generic Student user
                // because it does not have necessary attributes
                if (adUser.SamAccountName == "student")
                    continue;

                User user = new User(adUser);

                students.Add(user);
                progress?.Invoke(ActionStatus.Loading, 1.0 * students.Count / adUsers.Length);
            }

            progress?.Invoke(ActionStatus.Done, 0);
        }

        public void AssignCorrespondigGroups()
        {
            throw new Exception("Not implemented");
        }

        public List<User> GetStudents()
        {
            return students;
        }

        private string findProperSAN(string[] sans)
        {
            // SamAccountName limited to 20 characters
            // due to pre-Windows 2000 restrictions
            // TODO: Fix by switching to UserPrincipalName
            var limited_sans = sans.Where(san => san.Length <= 20);

            // Remove used SAN when First & Last Names is the same
            var used_sans = new HashSet<string>(students.Select(st => st.SamAccountName));
            var accepted_sans = limited_sans.Where(san => !used_sans.Contains(san));

            return accepted_sans.First();
        }

        public User AddStudent(UserFullName name, string group, bool simple_password = true)
        {
            User st = new User(name, group);
            st.Password = Helpers.GetPassword(simple_password);
            st.SamAccountName = findProperSAN(st.GenerateSams());

            students.Add(st);
            return st;
        }

        public void Commit()
        {
            throw new Exception("Not implemented");
        }

//        public void CreateOrUpdateDomain(PrincipalContext context, GroupPrincipal groupPrincipal = null)
//        {
//            int done = 0;
//
//            foreach (var st in students)
//            {
//                UserPrincipal up = new UserPrincipal(context);
//
//                up.SamAccountName = st.SamAccountName;
//                up.UserPrincipalName = st.SamAccountName + "@" + context.Name;
//
//                up.GivenName = st.NameUA.FirstName;
//                up.MiddleName = st.NameUA.MiddleName;
//                up.Surname = st.NameUA.SurName;
//
//                up.DisplayName = $"{st.NameUA.SurName} {st.NameUA.FirstName[0]}. {st.NameUA.MiddleName[0]}.";
//                up.Name = $"{st.NameUA.SurName} {st.NameUA.FirstName} {st.NameUA.MiddleName} ({st.Group})";
//
//                up.Enabled = true;
//                up.SetPassword(st.Password);
//                up.ExpirePasswordNow();
//                up.Save();
//
//                DirectoryEntry de = up.GetUnderlyingObject() as DirectoryEntry;
//                if (de != null)
//                {
//                    var initials = de.Properties["initials"];
//                    initials.Add($"{st.NameUA.FirstName[0]}. {st.NameUA.MiddleName[0]}.");
//
//                    var department = de.Properties["department"];
//                    department.Add(st.Group);
//                }
//
//                up.Save();
//
//                if (groupPrincipal != null)
//                {
//                    groupPrincipal.Members.Add(up);
//                    groupPrincipal.Save();
//                }
//
//                done++;
//
//                Console.WriteLine($"INFO: Added {up.Name}({up.SamAccountName}) user. Total: {done}/{students.Count}");
//            }
//
//        }
    }
}
