using ADProvider;
using ADProvider.Models;
using System;
using System.Linq;
using System.Text;

namespace StudentManagerCLI
{
    internal class Program
    {
        static ProgressBar progressBar = new ProgressBar();
        static void callBack(ActionStatus status, double progress)
        {
            progressBar.Report(status.ToString(), progress);
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.GetEncoding("UTF-8");
            Console.InputEncoding = Encoding.GetEncoding("UTF-8");

            ADConfig config = ADConfig.LoadConfig();
            StudentManager sm = new StudentManager(config);


            sm.LoadADContexts(callBack);
            sm.LoadFromAD(callBack);
            var x = sm.SyncWithZnz(@"C:\znz2023.txt", callBack);

            foreach (var y in x)
            {
                if (y.znzUserStatus == StudentManager.ProcessZnzUserStatus.AddX)
                    Console.WriteLine($"{y.znzUserStatus} {y.user.NameUA} {y.user.ZnzNameUA}");
                if (y.znzUserStatus == StudentManager.ProcessZnzUserStatus.Add1)
                    Console.WriteLine($"{y.znzUserStatus} {y.user.NameUA} {y.user.ZnzNameUA}");
                if (y.znzUserStatus == StudentManager.ProcessZnzUserStatus.Partial)
                    Console.WriteLine($"{y.znzUserStatus} {y.user.NameUA} {y.user.ZnzNameUA}");

                
            }
            
            var data = x.Select((u) => $"{u.user.NameUA};{u.user.Group};{u.znzUserStatus};{u.user.ZnzStatus};{string.Join("; ", u.znzUserFields)}");
            System.IO.File.WriteAllLines("sync.csv", data);
           
            var g = sm.MoveToGraduate(callBack);
            data = g.Select((u) => $"{u.NameUA};{u.Group};Graduate");
            System.IO.File.AppendAllLines("sync.csv", data);


            foreach (var y in g)
            {
                Console.WriteLine($"Graduate: {y.NameUA} {y.Group}");
            }

            sm.Commit(callBack);
            var res = sm.GetStudents().Select((u) => $"{u.NameUA};{u.NameEN};{u.DisplayName};{u.Group};{u.SamAccountName};{u.Password};{u.ZnzStatus}");
            System.IO.File.AppendAllLines("import.csv", res);

            //sm.FixUserValidateFails(x, callBack);
            return;
        }

//        public void SaveToFile(string file)
//        {
//            List<string> data = new List<string>();
//
//            data.Add("NameUA.SurName;NameUA.FirstName;NameUA.MiddleName;" +
//                    "NameEN.SurName;NameEN.FirstName;NameEN.MiddleName;" +
//                    "Group;SamAccountName;Password;Magic");
//            foreach (var st in students)
//            {
//                var line = $"{st.NameUA.SurName};{st.NameUA.FirstName};{st.NameUA.MiddleName};" +
//                    $"{st.NameEN.SurName};{st.NameEN.FirstName};{st.NameEN.MiddleName};" +
//                    $"{st.Group};{st.SamAccountName};{st.Password};{st.Magic}";
//
//                data.Add(line);
//            }
//
//            File.WriteAllLines(file, data);
//        }

    }
}
