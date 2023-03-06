using ADProvider;
using ADProvider.Models;
using System;
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
