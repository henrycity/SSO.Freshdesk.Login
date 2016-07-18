using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace NPS.SSO.Freshdesk.Login.Settings
{
    public class SSOFreshdeskClientSettings
    {
        public string BaseUrl { get; set; }
        public string SecretKey { get; set; }
        public SSOFreshdeskClientSettings()
        {
            BaseUrl = ConfigurationManager.AppSettings["sso-freshdesk-url"];
            SecretKey = ConfigurationManager.AppSettings["sso-freshdesk-key"];
            if (string.IsNullOrEmpty(BaseUrl))
            {
                throw new ArgumentException($"AppSetting value for key {BaseUrl} is null or empty", BaseUrl);
            }
            if (string.IsNullOrEmpty(SecretKey))
            {
                throw new ArgumentException($"AppSetting value for key {SecretKey} is null or empty", SecretKey);
            }
        }
    }
}