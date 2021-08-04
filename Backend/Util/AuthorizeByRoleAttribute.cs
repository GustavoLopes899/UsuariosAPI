using Microsoft.AspNetCore.Authorization;
using System;

/// <summary>
/// Specifies that the class or method that this attribute is applied to requires role-based authorization. <br />
/// To authorize users with either role A or role B, use:
/// <code>
/// [AuthorizeByRole("A", "B")]
/// </code>
/// To only authorize users with both role A and role B, use:
/// <code>
/// [AuthorizeByRole("A")] <br />
/// [AuthorizeByRole("B")]
/// </code>
/// </summary>
public class AuthorizeByRoleAttribute : AuthorizeAttribute
{
    public AuthorizeByRoleAttribute(params string[] roles)
    {
        this.Roles = String.Join(",", roles);
    }
}