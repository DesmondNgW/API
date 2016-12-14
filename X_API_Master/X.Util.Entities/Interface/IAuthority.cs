using System.Collections.Generic;

namespace X.Util.Entities.Interface
{
    public interface IAuthority
    {
        bool SetAuthority(AuthorityRequest requests);

        bool SetAuthority(List<AuthorityRequest> requests);

        bool GetAuthority(AuthorityRequest request);

        List<AuthorityRequest> GetAuthority(string userId);
    }
}
