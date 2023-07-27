namespace ADProvider
{
    public partial class StudentManager
    {
        public class UserValidateFailInfo
        {
            public string SamAccountName { get; set; }
            public string CurrentDisplayName { get; set; }
            public string CorrectDisplayName { get; set; }
            public string CurrentName { get; set; }
            public string CorrectName { get; set; }
        }

    }
}
