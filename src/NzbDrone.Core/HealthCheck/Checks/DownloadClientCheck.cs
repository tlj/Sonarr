﻿using System;
using System.Linq;
using NLog;
using NzbDrone.Core.Download;

namespace NzbDrone.Core.HealthCheck.Checks
{
    public class DownloadClientCheck : HealthCheckBase
    {
        private readonly IProvideDownloadClient _downloadClientProvider;
        private readonly Logger _logger;

        public DownloadClientCheck(IProvideDownloadClient downloadClientProvider, Logger logger)
        {
            _downloadClientProvider = downloadClientProvider;
            _logger = logger;
        }

        public override HealthCheck Check()
        {
            var downloadClients = _downloadClientProvider.GetDownloadClients().ToList();

            if (!downloadClients.Any())
            {
                return new HealthCheck(GetType(), HealthCheckResult.Warning, "No download client is available");
            }

            try
            {
                foreach (var downloadClient in downloadClients)
                {
                    downloadClient.GetItems();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Unable to communicate with download client: ", ex);
               return new HealthCheck(GetType(), HealthCheckResult.Error, "Unable to communicate with download client: " + ex.Message);
            }

            return new HealthCheck(GetType());
        }
    }
}
