using System;
using MyYamlParser;

namespace MarketingBox.Auth.Service.Settings
{
    public class SettingsModel
    {
        [YamlProperty("MarketingBoxAuthService.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("MarketingBoxAuthService.JaegerUrl")]
        public string JaegerUrl { get; set; }

        [YamlProperty("MarketingBoxAuthService.MarketingBoxServiceBusHostPort")]
        public string MarketingBoxServiceBusHostPort { get; set; }

        [YamlProperty("MarketingBoxAuthService.MyNoSqlReaderHostPort")]
        public string MyNoSqlReaderHostPort { get; set; }

        [YamlProperty("MarketingBoxAuthService.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; set; }

        [YamlProperty("MarketingBoxAuthService.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }
        
        [YamlProperty("MarketingBoxAuthService.EncryptionSalt")]
        public string EncryptionSalt { get; set; }

        [YamlProperty("MarketingBoxAuthService.EncryptionSecret")]
        public string EncryptionSecret { get; set; }

        [YamlProperty("MarketingBoxAuthService.JwtTtl")]
        public string JwtTtl { get; set; }

        [YamlProperty("MarketingBoxAuthService.JwtSecret")]
        public string JwtSecret { get; set; }

        [YamlProperty("MarketingBoxAuthService.JwtAudience")]
        public string JwtAudience { get; set; }
    }
}
