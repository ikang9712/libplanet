using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Libplanet.Crypto;
using Libplanet.Net;
using Xunit;

namespace Libplanet.Tests.Net
{
    public class PeerTest
    {
        public static IEnumerable<object[]> GetPeers()
        {
            var signer = new PrivateKey();
            AppProtocolVersion ver = AppProtocolVersion.Sign(signer, 1);
            AppProtocolVersion ver2 = AppProtocolVersion.Sign(
                signer: signer,
                version: 2,
                extra: Bencodex.Types.Dictionary.Empty.Add("foo", 123).Add("bar", 456)
            );
            yield return new object[]
            {
                new Peer(
                    new PublicKey(new byte[]
                    {
                        0x04, 0xb5, 0xa2, 0x4a, 0xa2, 0x11, 0x27, 0x20, 0x42, 0x3b,
                        0xad, 0x39, 0xa0, 0x20, 0x51, 0x82, 0x37, 0x9d, 0x6f, 0x2b,
                        0x33, 0xe3, 0x48, 0x7c, 0x9a, 0xb6, 0xcc, 0x8f, 0xc4, 0x96,
                        0xf8, 0xa5, 0x48, 0x34, 0x40, 0xef, 0xbb, 0xef, 0x06, 0x57,
                        0xac, 0x2e, 0xf6, 0xc6, 0xee, 0x05, 0xdb, 0x06, 0xa9, 0x45,
                        0x32, 0xfd, 0xa7, 0xdd, 0xc4, 0x4a, 0x16, 0x95, 0xe5, 0xce,
                        0x1a, 0x3d, 0x3c, 0x76, 0xdb,
                    })),
            };
        }

        [Theory]
        [MemberData(nameof(GetPeers))]
        public void Serializable(Peer peer)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, peer);
                byte[] serialized = stream.ToArray();
                stream.Seek(0, SeekOrigin.Begin);
                Peer deserialized = (Peer)formatter.Deserialize(stream);
                Assert.IsType(peer.GetType(), deserialized);
                Assert.Equal(peer, deserialized);
            }
        }

        [Theory]
        [MemberData(nameof(GetPeers))]
        public void Serialize(Peer peer)
        {
            Bencodex.Types.Dictionary serialized = peer.ToBencodex();
            var deserialized = new Peer(serialized);

            Assert.Equal(peer, deserialized);
        }
    }
}
