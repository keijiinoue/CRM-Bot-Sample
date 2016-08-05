
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;

namespace CrmActivityBot.Services
{
    static public class ADALService
    {     
        static public AuthenticationContext AuthContext = new AuthenticationContext(Settings.AuthUri, false);
        static public async Task<string> GetAccessToken(string resource = "")
        {           
            AuthenticationResult result = null;
            
            result = await AuthContext.AcquireTokenAsync(resource, Settings.ClientId, new UserCredential(Settings.UserName, Settings.Password));
            
            return result.AccessToken;
        }
    }
}
