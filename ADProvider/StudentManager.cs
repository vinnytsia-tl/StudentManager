﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.RegularExpressions;
using ADProvider.Models;
using Microsoft.VisualBasic.FileIO;

namespace ADProvider
{
    public partial class StudentManager
    {
        private List<User> students;
        private ADConfig adConfig;

        private PrincipalContext studentsContainerPrincipalContext;
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

            studentsContainerPrincipalContext = new PrincipalContext(ContextType.Domain, adConfig.Domain,
                adConfig.Containers.Student, adConfig.Admin.User, adConfig.Admin.Password);
            PrincipalContext gpc = new PrincipalContext(ContextType.Domain, adConfig.Domain,
                adConfig.Containers.Group, adConfig.Admin.User, adConfig.Admin.Password);

            studentsContainerContext = new UserPrincipal(studentsContainerPrincipalContext);
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

            progress?.Invoke(ActionStatus.Done, 1);
        }

        public enum ProcessZnzUserStatus
        {
            Unknown,
            Exist,
            Add1,
            AddX,
            Partial
        }

        private (ProcessZnzUserStatus status, User user) processZnzUserFields(string[] znzUserFields, bool checkPartial = false)
        {
            var group = znzUserFields[1];
            int groupNumber = int.Parse(group.Split('-')[1][0].ToString());
            UserFullName znzUser = new UserFullName(znzUserFields[3], znzUserFields[4], znzUserFields[2]);

            var existingStudents = students.FindAll((u) => u.NameUA == znzUser);
            if (existingStudents.Count > 1) { throw new Exception(); }
            if (existingStudents.Count == 1)
            {
                var existingStudent = existingStudents.First();
                existingStudent.SetZnzData(znzUser, group);
                return (ProcessZnzUserStatus.Exist, existingStudent);
            }

            // if (existingStudents.Count == 0)
            // Maybe new user
            if (groupNumber == 1)
            {
                var user = new User(znzUser, group);
                user.SetZnzData(znzUser, group);
                students.Add(user);
                return (ProcessZnzUserStatus.Add1, user);
            }

            var existingPartialStudents = students.FindAll((u) => u.NameUA.PartialEqual(znzUser));
            if (existingPartialStudents.Count == 0)
            {
                var user = new User(znzUser, group);
                user.SetZnzData(znzUser, group);
                students.Add(user);
                return (ProcessZnzUserStatus.AddX, user);
            }

            if (!checkPartial || existingPartialStudents.Count > 1)
                return (ProcessZnzUserStatus.Partial, null);

            var partialStudents = existingPartialStudents.First();
            partialStudents.SetZnzData(znzUser, group);
            return (ProcessZnzUserStatus.Exist, partialStudents);
        }

        public struct ZnzSyncResults
        {
            public string[] znzUserFields;
            public ProcessZnzUserStatus znzUserStatus;
            public User user;
        }

        public List<ZnzSyncResults> SyncWithZnz(string filePath)
        {
            var encoding = Encoding.GetEncoding(1251);
            //var encoding = Encoding.UTF8;

            var results = new List<ZnzSyncResults>();

            using (TextFieldParser parser = new TextFieldParser(filePath, encoding))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                string header = parser.ReadLine();
                if (header != "Школа,Клас,Прізвище,Ім'я,По-батькові,Дата народження,Стать,Місто,Вулиця,Будинок,Квартира,")
                    throw new ArgumentOutOfRangeException($"ZNZ file headers is not valid: {header}");

                List<string[]> znzUserFieldsForReload = new List<string[]>();
                while (!parser.EndOfData)
                {
                    string[] znzUserFields = parser.ReadFields();
                    //total++;

                    var res = processZnzUserFields(znzUserFields);
                    if (res.status == ProcessZnzUserStatus.Partial)
                        znzUserFieldsForReload.Add(znzUserFields);
                    else
                        results.Add(new ZnzSyncResults() { user = res.user, znzUserFields = znzUserFields, znzUserStatus = res.status });
                }

                foreach (var znzUserFields in znzUserFieldsForReload)
                {
                    var res = processZnzUserFields(znzUserFields);
                    results.Add(new ZnzSyncResults() { user = res.user, znzUserFields = znzUserFields, znzUserStatus = res.status });
                }
            }

            //Console.WriteLine($"Total: {total}\nExists: {exists}\nToAdd1: {toadd1}\nToAddX: {toaddx}\nPartial: {partial}");
            return results;
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

        private (User, UserPrincipal) FindStudentPair(string samAccountName)
        {
            var student = students.Find((u) => u.SamAccountName == samAccountName);
            var adStudent = UserPrincipal.FindByIdentity(studentsContainerPrincipalContext, IdentityType.SamAccountName, samAccountName);

            return (student, adStudent);
        }

        public User UpdateGroup(string samAccountName, string group)
        {
            var (student, adStudent) = FindStudentPair(samAccountName);

            student.UpdateGroup(adStudent, group);
            return student;
        }

        public List<UserValidateFailInfo> ValidateUsersName(ProgressCallBack progress = null)
        {
            List<UserValidateFailInfo> userValidateFailInfos = new List<UserValidateFailInfo>();

            for (int i = 0; i < students.Count; i++)
            {
                var student = students[i];
                if (!student.Loaded)
                    continue;

                var adStudent = UserPrincipal.FindByIdentity(
                    studentsContainerPrincipalContext, IdentityType.SamAccountName, student.SamAccountName);

                if (adStudent.Name != student.Name || adStudent.DisplayName != student.DisplayName)
                    userValidateFailInfos.Add(new UserValidateFailInfo()
                    {
                        SamAccountName = student.SamAccountName,
                        CorrectDisplayName = student.DisplayName,
                        CorrectName = student.Name,
                        CurrentDisplayName = adStudent.DisplayName,
                        CurrentName = adStudent.Name
                    });

                progress?.Invoke(ActionStatus.Processing, 1.0 * i / students.Count);
            }

            progress?.Invoke(ActionStatus.Done, 1);
            return userValidateFailInfos;
        }

        public void FixUserValidateFails(List<UserValidateFailInfo> infos, ProgressCallBack progress = null)
        {
            for (int i = 0; i < infos.Count; i++)
            {
                var info = infos[i];

                UserPrincipal up = UserPrincipal.FindByIdentity(
                    studentsContainerPrincipalContext, IdentityType.SamAccountName, info.SamAccountName);

                up.DisplayName = info.CorrectDisplayName;
                up.Save();

                var dirEntry = (DirectoryEntry)up.GetUnderlyingObject();
                dirEntry.Rename($"CN={info.CorrectName}");
                dirEntry.CommitChanges();

                progress?.Invoke(ActionStatus.Processing, 1.0 * i / students.Count);
            }

            progress?.Invoke(ActionStatus.Done, 1);
        }

        public void Commit()
        {
            throw new Exception("Not implemented");

            foreach (var student in students)
            {
                UserPrincipal adStudent = null;
                if (student.Loaded)
                {
                    adStudent = UserPrincipal.FindByIdentity(studentsContainerPrincipalContext, IdentityType.SamAccountName, student.SamAccountName);
                }

                adStudent.GivenName = student.NameUA.FirstName;
                adStudent.MiddleName = student.NameUA.MiddleName;
                adStudent.Surname = student.NameUA.SurName;

                adStudent.DisplayName = student.DisplayName;

                if (student.Loaded)
                {
                    adStudent.Save();

                    var dirEntry = (DirectoryEntry)adStudent.GetUnderlyingObject();
                    dirEntry.Rename($"CN={student.Name}");
                    dirEntry.CommitChanges();
                }
                else
                {
                    adStudent.Name = student.Name;
                }
            }
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
