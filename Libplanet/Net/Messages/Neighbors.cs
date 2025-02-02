#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bencodex;
using Destructurama.Attributed;

namespace Libplanet.Net.Messages
{
    internal class Neighbors : Message
    {
        private static readonly Codec Codec = new Codec();

        public Neighbors(IEnumerable<BoundPeer> found)
        {
            Found = found.ToImmutableList();
        }

        public Neighbors(byte[][] dataFrames)
        {
            var codec = new Codec();
            int foundCount = BitConverter.ToInt32(dataFrames[0], 0);
            Found = dataFrames.Skip(1).Take(foundCount)
                .Select(ba => new BoundPeer((Bencodex.Types.Dictionary)codec.Decode(ba)))
                .ToImmutableList();
        }

        [LogAsScalar]
        public IImmutableList<BoundPeer> Found { get; }

        public override MessageType Type => MessageType.Neighbors;

        public override IEnumerable<byte[]> DataFrames
        {
            get
            {
                var frames = new List<byte[]>();
                frames.Add(BitConverter.GetBytes(Found.Count));
                frames.AddRange(Found.Select(boundPeer => Codec.Encode(boundPeer.ToBencodex())));
                return frames;
            }
        }
    }
}
