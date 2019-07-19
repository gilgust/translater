using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Translation.V2;

namespace Translater
{
    public class GoogleTranslater
    {
        private static readonly Lazy<GoogleTranslater> lazy = new Lazy<GoogleTranslater>(() => new GoogleTranslater());
        private readonly TranslationClient _translater;
        private GoogleTranslater()
        {
            var creditial = GoogleCredential.FromFile("key.json");
            _translater = TranslationClient.Create(creditial);
        }

        public TranslationClient Translater => _translater;
        public static GoogleTranslater GetInstance() { return lazy.Value; }
    }
}
