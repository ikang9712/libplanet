#nullable enable
using System;
using System.Collections.Generic;

namespace Libplanet.Store.Trie
{
    /// <summary>
    /// An interface to access key-value store.
    /// </summary>
    public interface IKeyValueStore : IDisposable
    {
        public byte[] Get(byte[] key);

        public void Set(byte[] key, byte[] value);

        /// <summary>
        /// Sets all values in the given dictionary.
        /// </summary>
        /// <param name="values">A values to set.</param>
        public void Set(IDictionary<byte[], byte[]> values);

        public void Delete(byte[] key);

        public bool Exists(byte[] key);

        /// <summary>
        /// Lists all keys that have been stored in the storage.
        /// </summary>
        /// <returns>All keys in an arbitrary order.  The order might be vary for each call.
        /// </returns>
        public IEnumerable<byte[]> ListKeys();
    }
}
