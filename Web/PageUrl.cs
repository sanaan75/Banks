namespace Web
{
    public static class PageUrl
    {
        public const string Dashboard = "/_App/Index";
        public const string Login = "/Login";
        public const string Logout = "/Logout";
        public const string Home = "/_App/Index";

        public const string Users = "/_App/Users/Index";
        public const string AddUser = "/_App/Users/Create";
        public const string ListUsers = "/_App/Users/Index";
        public const string InfoUser = "/_App/Users/Info";
        public const string UpdateUser = "/_App/Users/Update";
        public const string SearchUsers = "InfoUsers";

        public const string Journals = "/_App/Journals/Index";
        public const string AddJournal = "/_App/Journals/Create";
        public const string ListJournals = "/_App/Journals/Index";
        public const string UpdateJournal = "/_App/Journals/Edit";
        public const string SearchJournals = "/_App/Journals/Search";

        public const string JournalsRecords = "/_App/Journals/Records/Index";
        public const string AddJournalsRecord = "/_App/Journals/Records/Create";
        public const string UpdateJournalsRecord = "/_App/Journals/Records/Edit";

        public const string ImportFromExcel = "/_App/Journals/ImportExcel";
        public const string ReadExcelCloritive = "/_App/Journals/ReadExcelCloritive";
        public const string UpdateIF = "/_App/Journals/UpdateIF";
        public const string UpdateIF2 = "/_App/Journals/UpdateIF2";
        public const string UpdateISC = "/_App/Journals/UpdateISC";
        public const string UpdateByYear = "/_App/Journals/UpdateByYear";
        public const string UpdateMIF = "/_App/Journals/UpdateMIF";

        public const string Reports = ListReports;
        public const string AddReport = "/_App/Reports/Add";
        public const string ListReports = "/_App/Reports/Index";
        public const string InfoReport = "/_App/Reports/Info";
        public const string SearchReports = "/_App/Reports/All";

        public const string UserGroups = ListUserGroups;
        public const string AddUserGroup = "/_App/UserGroups/Add";
        public const string AddDefaultUserGroup = "/_App/UserGroups/AddDefault";
        public const string ListUserGroups = "/_App/UserGroups/Index";
        public const string InfoUserGroup = "/_App/UserGroups/Info";
        public const string SearchUserGroups = "/_App/UserGroups/All";

        public const string Orgs = ListUsers;
        public const string AddOrg = "/_App/Orgs/Add";
        public const string ListOrgs = "/_App/Orgs/Index";
        public const string InfoOrg = "/_App/Orgs/Info";
        public const string UpdateOrg = "/_App/Orgs/Update";
        public const string SearchOrgs = "InfoUsers";
    }
}