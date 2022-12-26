namespace API.Helpers;

public class Auth
{
    public enum Roles
    {
        Administrator,
        Manager,
        Employee
    }

    public const Roles DEFAULT_ROLE = Roles.Employee;
}
