namespace ADProvider.Models
{
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
    }
}
