namespace Entities.Permissions;

public enum Permission
{
    Journals = 10,
    Journals_Create = 10_01,
    Journals_List = 10_02,
    Journals_Details = 10_03,

    Users = 20,
    Users_Create = 20_01,
    Users_List = 20_02,
    Users_Details = 20_03,

    Reports = 30,
    Reports_Create = 30_01,
    Reports_List = 30_02,
    Reports_Details = 30_03,

    Permissions = 40,
    Permissions_Create = 40_01,
    Permissions_List = 40_02,
    Permissions_Details = 40_03
}