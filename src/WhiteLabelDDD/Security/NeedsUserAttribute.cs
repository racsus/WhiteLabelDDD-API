using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WhiteLabel.WebAPI.Security
{
    public class NeedsUserAttribute : TypeFilterAttribute, IAllowAnonymous
    {
        public NeedsUserAttribute() : base(typeof(NeedsUserFilter))
        {

        }
    }
}
