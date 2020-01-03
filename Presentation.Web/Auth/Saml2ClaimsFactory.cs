using Core.DomainModel;
using Core.DomainServices;
using Microsoft.AspNetCore.Identity;
using Sustainsys.Saml2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Presentation.Web.Auth
{
    public class Saml2ClaimsFactory : IUserClaimsPrincipalFactory<IdentityPerson>
    {
        IUserClaimsPrincipalFactory<IdentityPerson> _inner;
        ExternalLoginInfo _externalLoginInfo;

        public Saml2ClaimsFactory(
            IUserClaimsPrincipalFactory<IdentityPerson> inner,
            ExternalLoginInfo externalLoginInfo)
        {
            _inner = inner;
            _externalLoginInfo = externalLoginInfo;
        }

        public async Task<ClaimsPrincipal> CreateAsync(IdentityPerson user)
        {
            var principal = await _inner.CreateAsync(user);

            var logoutInfo = _externalLoginInfo.Principal.FindFirst(Saml2ClaimTypes.LogoutNameIdentifier);
            var sessionIndex = _externalLoginInfo.Principal.FindFirst(Saml2ClaimTypes.SessionIndex);

            var identity = principal.Identities.Single();
            identity.AddClaim(logoutInfo);
            identity.AddClaim(sessionIndex);
            return principal;
        }
    }
}
