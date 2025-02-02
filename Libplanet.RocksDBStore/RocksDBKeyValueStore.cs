#nullable enable
using System.Collections.Generic;
using Libplanet.Store.Trie;
using RocksDbSharp;

namespace Libplanet.RocksDBStore
{
    /// <summary>
    /// The <a href="https://rocksdb.org/">RocksDB</a> <see cref="IKeyValueStore"/> implementation.
    /// This stores data in the RocksDB.
    /// </summary>
    public class RocksDBKeyValueStore : IKeyValueStore
    {
        private readonly RocksDb _keyValueDb;
        private bool _disposed = false;

        /// <summary>
        /// Creates a new <see cref="RocksDBKeyValueStore"/>.
        /// </summary>
        /// <param name="path">The path of the storage file will be saved.</param>
        public RocksDBKeyValueStore(string path)
        {
            var options = new DbOptions()
                .SetCreateIfMissing();

            _keyValueDb = RocksDBUtils.OpenRocksDb(options, path);
        }

        /// <inheritdoc/>
        public byte[] Get(byte[] key) => _keyValueDb.Get(key) ?? throw new KeyNotFoundException(
            $"There were no elements that correspond to the key (hex: {ByteUtil.Hex(key)}).");

        /// <inheritdoc/>
        public void Set(byte[] key, byte[] value)
        {
            _keyValueDb.Put(key, value);
        }

        /// <inheritdoc/>
        public void Set(IDictionary<byte[], byte[]> values)
        {
            using var writeBatch = new WriteBatch();

            foreach (KeyValuePair<byte[], byte[]> kv in values)
            {
                writeBatch.Put(kv.Key, kv.Value);
            }

            _keyValueDb.Write(writeBatch);
        }

        /// <inheritdoc/>
        public void Delete(byte[] key)
        {
            _keyValueDb.Remove(key);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _keyValueDb.Dispose();
                _disposed = true;
            }
        }

        /// <inheritdoc/>
        public bool Exists(byte[] key) => !(_keyValueDb.Get(key) is null);

        /// <inheritdoc/>
        public IEnumerable<byte[]> ListKeys()
        {
            using Iterator it = _keyValueDb.NewIterator();
            for (it.SeekToFirst(); it.Valid(); it.Next())
            {
                yield return it.Key();
            }
        }
    }
}
