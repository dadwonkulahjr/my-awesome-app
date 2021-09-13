using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExampleEmpty.UI.DataProtectionTokenManager
{
    public class CustomEmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
    {
       
    }

    public class CustomEmailConfirmationTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
    {
        public CustomEmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider, IOptions<CustomEmailConfirmationTokenProviderOptions> options, ILogger<DataProtectorTokenProvider<TUser>> logger)
            :base(dataProtectionProvider, options, logger)
        {

        }
    }
}
