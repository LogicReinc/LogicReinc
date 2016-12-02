using LogicReinc.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Security.TokenSystem
{
    /// <summary>
    /// A version of TokenReinc using the same format as V1 and V2 but instead of using hashing as validation, it uses a lookup table.
    /// Which counters statistic-based hacking.
    /// </summary>
    public class TokenSystem
    {
        private const string _ValidCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private int duration = 3600;

        private RNGCryptoServiceProvider rngCrypt = new RNGCryptoServiceProvider();
        private ConcurrentDictionary<string, TokenLookup> lookupTable { get; set; } = new ConcurrentDictionary<string, TokenLookup>();

        //Properties
        public int Duration => duration;

        //Constructors
        public TokenSystem(int tokenDuration = 3600)
        {
            duration = tokenDuration;
        }

        //Privates
        private string GenerateCryptoKey()
        {
            byte[] key = new byte[64];
            for (int i = 0; i < 64; i++)
            {
                byte[] keyPart = new byte[1];
                rngCrypt.GetBytes(keyPart);
                key[i] = (byte)_ValidCharacters[(keyPart[0] % _ValidCharacters.Length)];
            }
            return Encoding.ASCII.GetString(key);
        }


        //Publics


        //Verification
        public bool VerifyToken(string tokenUnique, string token)
        {
            token = token.UrlDecode();
            if (!lookupTable.ContainsKey(token))
                return false;
            TokenLookup lookup = lookupTable[token];
            if (lookup.TokenUnique != tokenUnique)
                return false;
            if (lookup.Expires < DateTime.Now)
                return false;
            return true;
        }

        //Creation
        public Token CreateToken(string tokenUnique, object data)
        {
            TokenLookup lookup = new TokenLookup()
            {
                AccessToken = GenerateCryptoKey(),
                RefreshToken = GenerateCryptoKey(),
                Expires = DateTime.Now.Add(TimeSpan.FromSeconds(Duration)),
                Data = data,
                TokenUnique = tokenUnique
            };
            lookupTable.AddOrUpdate(lookup.AccessToken, lookup, (key, val) => lookup);
            return lookup.ToToken();
        }
        public Token UseRefreshToken(string tokenUnique, string accessToken, string refreshToken)
        {
            accessToken = accessToken.UrlDecode();
            refreshToken = refreshToken.UrlDecode();
            if (!lookupTable.ContainsKey(accessToken))
                return null;
            TokenLookup lookup = lookupTable[accessToken];
            if (lookup.RefreshToken != refreshToken)
                return null;

            lookupTable.TryRemove(accessToken, out lookup);

            lookup.AccessToken = GenerateCryptoKey().UrlEncode();
            lookup.RefreshToken = GenerateCryptoKey().UrlEncode();
            lookupTable.AddOrUpdate(lookup.AccessToken, lookup, (key, val) => lookup);

            return lookup.ToToken();
        }

        //Utility
        public object GetTokenData(string token)
        {
            token = token.UrlDecode();
            if (!lookupTable.ContainsKey(token))
                return null;
            return lookupTable[token].Data;
        }



        //
        class TokenLookup
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
            public string TokenUnique { get; set; }
            public object Data { get; set; }
            public DateTime Expires { get; set; }

            public Token ToToken()
            {
                return new Token()
                {
                    AccessToken = AccessToken.UrlEncode(),
                    RefreshToken = RefreshToken.UrlEncode(),
                    Duration = (int)Expires.Subtract(DateTime.Now).TotalSeconds,
                    Data = Data
                };
            }
        }
    }
}
