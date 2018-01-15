using System;
using System.Threading.Tasks;
using Models;
using Models.DTO;
using Models.EFDB;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using ServerApi.Services.SkautIS;

namespace ServerApi.Services.Users
{
    public class RegisterUserService
    {
        private readonly SkautIsUserService _userDetailsService = new SkautIsUserService();

        private readonly ServerApiContext _db = new ServerApiContext();

        public async Task<(UserDetails userDetails, Guid deviceId, byte[] privateKey)> RegisterUserAsync(Guid loginId)
        {
            var userDetails = await _userDetailsService.GetUserDetailsAsync(loginId);

            var deviceGuid = Guid.NewGuid();
            byte[] publicKeyEncoded = null;
            byte[] privateKeyEncoded = null;

            var user = await _db.Users.FindAsync(userDetails.UserId);
            if (user == null)
            {
                var unit = await _db.Units.FindAsync(userDetails.UnitId);

                if(unit == null)
                {
                    unit = new Unit
                    {
                        Id = userDetails.UnitId,
                        Name = "<placeholder>",
                        LastModified = DateTime.UtcNow,
                    };

                    _db.Units.Add(unit);
                }

                var keyPair = GenerateRsaKeyPair();
                publicKeyEncoded = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyPair.Public)
                    .ToAsn1Object().GetDerEncoded();
                privateKeyEncoded = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keyPair.Private)
                    .ToAsn1Object().GetDerEncoded();

                user = new Models.EFDB.User
                {
                    Id = userDetails.UserId,
                    Category = userDetails.CategoryId,
                    Name = userDetails.UserName,
                    LastModified = DateTime.Now,
                    PrivateKeyEncoded = privateKeyEncoded,
                    PublicKeyEncoded = publicKeyEncoded,
                    UnitId = userDetails.UnitId,
                    Unit = unit,
                    UnitAdmin = userDetails.IsUnitAdmin
                };
                _db.Users.Add(user);
            }
            else
            {
                privateKeyEncoded = user.PrivateKeyEncoded;
            }

            _db.Devices.Add(new Device { User = user, Id = deviceGuid, LastModified = DateTime.UtcNow, UserId = user.Id });

            try
            {
                _db.SaveChanges();
            }
            catch(Exception e)
            {

            }

            return (userDetails, deviceGuid, privateKeyEncoded);
        }

        private AsymmetricCipherKeyPair GenerateRsaKeyPair(int keySize = 2048)
        {
            CryptoApiRandomGenerator randomGenerator = new CryptoApiRandomGenerator();
            SecureRandom secureRandom = new SecureRandom(randomGenerator);
            var keyGenerationParameters = new KeyGenerationParameters(secureRandom, keySize);
       
            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            return keyPairGenerator.GenerateKeyPair();
        }
    }
}