﻿using System;
using Comments.Services;
using Comments.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Comments.Models;
using Newtonsoft.Json;

namespace Comments.Common
{
	public class OrganisationCookieAuthenticationOptions : AuthenticationSchemeOptions
	{
		public const string DefaultScheme = "OrganisationCookieScheme";
		public string Scheme => DefaultScheme;
		public string AuthenticationType => DefaultScheme;
	}

	/// <summary>
	/// This AuthenticationHandler is here for the Organisational commenting cookie.
	///
	/// If you have 1 or more organisational commenting cookies (each specific to a consultation), then the cookies are checked here to see if they're valid.
	///
	/// If so, you'll be authenticated. However you'll only be authorised for the specific consultations you have cookies for.
	/// </summary>
	public class OrganisationCookieAuthenticationHandler : AuthenticationHandler<OrganisationCookieAuthenticationOptions>
	{
		private readonly IOrganisationService _organisationService;

		public OrganisationCookieAuthenticationHandler(IOptionsMonitor<OrganisationCookieAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IOrganisationService organisationService)
			: base(options, logger, encoder, clock)
		{
			_organisationService = organisationService;
		}


		protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			var unvalidatedSessionCookies = Request.Cookies.GetSessionCookies();

			if (!unvalidatedSessionCookies.Any())
			{
				return AuthenticateResult.NoResult();
			}
			
			var validatedSessions = _organisationService.CheckValidCodesForConsultation(new Session(unvalidatedSessionCookies));

			if (!validatedSessions.Any())
			{
				return AuthenticateResult.NoResult(); //return AuthenticateResult.Fail("Access denied.");
			}

			var validatedSessionsSerialised = JsonConvert.SerializeObject(validatedSessions);

			var claims = new List<Claim>
				{
					new Claim(Constants.OrgansationAuthentication.ValidatedSessionsClaim, validatedSessionsSerialised, null, Constants.OrgansationAuthentication.Issuer)
				};
			var identity = new ClaimsIdentity(claims, Options.AuthenticationType);
			var identities = new List<ClaimsIdentity> { identity };
			var principal = new ClaimsPrincipal(identities);
			var ticket = new AuthenticationTicket(principal, Options.Scheme);

			return AuthenticateResult.Success(ticket);
		}
	}
}
