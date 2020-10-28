// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using LightningDB;

namespace KeyValueStore
{
    public abstract class LightningDataStore : IDisposable
    {
        protected const ushort MaxKeySizeBytes = 511;

        private const int DefaultMaxReaders = 126;
        private const int DefaultMaxDatabases = 5;
        private const long DefaultMapSize = 10_485_760;

        protected static readonly DatabaseConfiguration Config = new DatabaseConfiguration
            {Flags = DatabaseOpenFlags.None};

        protected Lazy<LightningEnvironment> Env;

        public string FilePath { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Init(string path)
        {
            if (Env != default && Env.IsValueCreated)
                return;
            Env ??= new Lazy<LightningEnvironment>(() =>
            {
                var config = new EnvironmentConfiguration
                {
                    MaxDatabases = DefaultMaxDatabases,
                    MaxReaders = DefaultMaxReaders,
                    MapSize = DefaultMapSize
                };
                var environment = new LightningEnvironment(path, config);
                environment.Open();
                CreateIfNotExists(environment);
                return environment;
            });
            FilePath = path;
        }

        private static void CreateIfNotExists(LightningEnvironment environment)
        {
            using var tx = environment.BeginTransaction();
            using (tx.OpenDatabase(null, Config))
            {
                tx.Commit();
            }
        }

        public long GetLength()
        {
            using var tx = Env.Value.BeginTransaction(TransactionBeginFlags.ReadOnly);
            using var db = tx.OpenDatabase(configuration: Config);
            var count = tx.GetEntriesCount(db); // entries also contains handles to databases
            return count;
        }

        public void Clear()
        {
            using var tx = Env.Value.BeginTransaction();
            var db = tx.OpenDatabase(configuration: Config);
            tx.TruncateDatabase(db);
            tx.Commit();
        }

        public void Destroy()
        {
            using (var tx = Env.Value.BeginTransaction())
            {
                var db = tx.OpenDatabase(configuration: Config);
                tx.DropDatabase(db);
                tx.Commit();
            }

            if (Env != null && Env.IsValueCreated)
                Env.Value.Dispose();

            try
            {
                Directory.Delete(FilePath, true);
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());
            }
        }

        [ExcludeFromCodeCoverage /* is never called with false in the base class */]
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            if (Env != null && Env.IsValueCreated)
                Env.Value.Dispose();
        }
    }
}