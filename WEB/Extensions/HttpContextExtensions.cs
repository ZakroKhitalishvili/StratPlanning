﻿using Application.DTOs;
using Core.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.Extensions
{
    /// <summary>
    /// This class contains extension methods for <c>HttpContext</c> class.
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Signs an user in a website and saves claims (Role, Email, Name, Position, Id).
        /// </summary>
        /// <param name="context">This object</param>
        /// <param name="user">User info</param>
        /// <param name="rememberMe">determiens whether to remember user on website</param>
        public async static void LogIn(this HttpContext context, UserDTO user, bool rememberMe)
        {
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                        new Claim(ClaimTypes.Role, user.Role),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(CustomClaimTypes.Position, user.Position?.Title??string.Empty),
                        new Claim(CustomClaimTypes.Id, user.Id.ToString())
                };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            DateTimeOffset? expiresUtc = null;
            bool isPersistent = false;
            if (rememberMe)
            {
                expiresUtc = DateTimeOffset.UtcNow.AddDays(7); // on remembemrMe, an user is stayed logged in for 7 days
                isPersistent = true;
            }

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                // Refreshing the authentication session should be allowed.

                ExpiresUtc = expiresUtc,
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                IsPersistent = isPersistent,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = login.ReturnUrl
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            await context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        /// <summary>
        /// Logs user out
        /// </summary>
        /// <param name="context">This object</param>
        public async static void LogOut(this HttpContext context)
        {
            await context.SignOutAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme);
        }

        /// <summary>
        ///   Gets current logged user's id
        /// </summary>
        /// <param name="context">this object</param>
        /// <returns>Current user Id</returns>
        public static int GetUserId(this HttpContext context)
        {
            return int.Parse(context.User.FindFirst(CustomClaimTypes.Id).Value);
        }

        /// <summary>
        ///  Gets current logged user's email
        /// </summary>
        /// <param name="context">This object</param>
        /// <returns>Current user email</returns>
        public static string GetUserEmail(this HttpContext context)
        {
            return context.User.FindFirst(ClaimTypes.Email).Value;
        }

        /// <summary>
        /// Resets logged user info - claims
        /// </summary>
        /// <param name="context">this object</param>
        /// <param name="user">Current user updated info</param>
        public async static void UpdateUser(this HttpContext context, UserDTO user)
        {
            var claimsIdentity = context.User.Identity as ClaimsIdentity;

            var emailClaim = claimsIdentity.FindFirst(ClaimTypes.Email);
            var nameClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            var roleClaim = claimsIdentity.FindFirst(ClaimTypes.Role);
            var positionClaim = claimsIdentity.FindFirst(CustomClaimTypes.Position);

            claimsIdentity.RemoveClaim(emailClaim);
            claimsIdentity.RemoveClaim(nameClaim);
            claimsIdentity.RemoveClaim(roleClaim);
            claimsIdentity.RemoveClaim(positionClaim);

            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, $"{ user.FirstName } {user.LastName}"));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, user.Role));
            claimsIdentity.AddClaim(new Claim(CustomClaimTypes.Position, user.Position?.Title ?? string.Empty));

            var result = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = result.Properties;

            await context.SignOutAsync(
                 CookieAuthenticationDefaults.AuthenticationScheme);

            await context.SignInAsync(
                   CookieAuthenticationDefaults.AuthenticationScheme,
                   new ClaimsPrincipal(claimsIdentity),
                   authProperties);
        }

        /// <summary>
        /// Gets date when current logged user session expires
        /// </summary>
        /// <param name="context">this object</param>
        /// <returns>Expiring date</returns>
        public async static Task<DateTime?> GetExpiration(this HttpContext context)
        {
            var result = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = result.Properties;

            return authProperties.ExpiresUtc?.LocalDateTime;
        }
    }
}
