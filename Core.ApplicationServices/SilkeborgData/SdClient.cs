using Core.ApplicationServices.SdKoersel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace Core.ApplicationServices
{
    public class SdClient : ISdClient
    {
        private AnsaettelseKoerselOpret20170501PortTypeClient _portTypeClient;
        private ILogger _logger;
        private readonly IConfiguration _configuration;

        public SdClient (ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            try
            {
                if (Boolean.Parse(configuration["SD:Enabled"]))
                {
                    _portTypeClient = new AnsaettelseKoerselOpret20170501PortTypeClient();
                    _portTypeClient.ClientCredentials.UserName.UserName = configuration["SD:Username"];
                    _portTypeClient.ClientCredentials.UserName.Password = configuration["SD:Password"];
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{this.GetType().ToString()}, sendDataToSd(), Error when initiating SD client", e);
                throw e; 
            }
        }

        public AnsaettelseKoerselOpret20170501Type SendRequest(AnsaettelseKoerselOpretInputType requestData)
        {
            try
            {
                if (!Boolean.Parse(_configuration["SD:Enabled"]))
                {
                    return null;
                }
                var request = new AnsaettelseKoerselOpret20170501OperationRequest();
                request.AnsaettelseKoerselOpret = requestData;
                return _portTypeClient.AnsaettelseKoerselOpret20170501Operation(request).AnsaettelseKoerselOpret20170501;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{this.GetType().ToString()}, SendRequest(), Error when sending data to SD");
                throw e;
            }
        }
    }
}
