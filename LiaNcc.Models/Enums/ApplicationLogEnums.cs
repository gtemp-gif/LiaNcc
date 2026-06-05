namespace LiaNcc.Models.Enums
{
    public static class ApplicationLogLevel
    {
        public const string Trace = "Trace";
        public const string Debug = "Debug";
        public const string Information = "Information";
        public const string Warning = "Warning";
        public const string Error = "Error";
        public const string Critical = "Critical";
    }

    public static class ApplicationEventType
    {
        public const string Login = "Login";
        public const string Logout = "Logout";
        public const string Create = "Create";
        public const string Update = "Update";
        public const string Delete = "Delete";
        public const string Upload = "Upload";
        public const string Download = "Download";
        public const string Email = "Email";
        public const string Booking = "Booking";
        public const string Contact = "Contact";
        public const string System = "System";
    }
}
