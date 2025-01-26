using System.ComponentModel;

namespace ApiAfterDotnetCourse.Data.Enums;

public enum UserType
{
    [Description("Admin")]
    Admin = 1,

    [Description("Felhasználó")]
    User = 2
}
