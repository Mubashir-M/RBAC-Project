public class LoginResponseDto
{
    public required string token { get; set; }
    public required UserDto user { get; set; }
}

public class UserDto
{
    public required int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required IEnumerable<LoginRoleDto> Roles { get; set; }
}

public class LoginRoleDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required IEnumerable<PermissionDto> Permissions { get; set; }
}

public class PermissionDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
}