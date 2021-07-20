using AutoFixture;
using Elasticsearch.Net;
using Nest;
using ReferenceDataApi.V1.Gateways;
using ReferenceDataApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace ReferenceDataApi.Tests
{
    public class ElasticSearchFixture<TStartup> : IDisposable where TStartup : class
    {
        private readonly Fixture _fixture = new Fixture();

        public HttpClient Client { get; private set; }
        public IElasticClient ElasticSearchClient => _factory?.ElasticSearchClient;

        public string NodeUri => Environment.GetEnvironmentVariable("ElasticSearchDomainUrl");
        public string Category => "SomeCategory";
        public string SubCategory => "SomeSubCategory";
        public string Unknown => "Unknown";
        public List<QueryableReferenceData> Data { get; private set; } = new List<QueryableReferenceData>();

        private readonly MockWebApplicationFactory<TStartup> _factory;
        private readonly string _index = ElasticSearchGateway.EsIndex;

        private readonly List<Action> _cleanup = new List<Action>();

        public ElasticSearchFixture()
        {
            EnsureEnvVarConfigured("ElasticSearchDomainUrl", "http://localhost:9200");

            _factory = new MockWebApplicationFactory<TStartup>();
            Client = _factory.CreateClient();

            EnsureIndexExists(ElasticSearchClient, _index);
            AddDataToIndexAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                foreach (var action in _cleanup)
                    action();

                if (null != _factory)
                    _factory.Dispose();
                _disposed = true;
            }
        }

        private static void EnsureEnvVarConfigured(string name, string defaultValue)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name)))
                Environment.SetEnvironmentVariable(name, defaultValue);
        }

        private static void EnsureIndexExists(IElasticClient esClient, string index)
        {
            var settingsDoc = File.ReadAllTextAsync("./Data/referencedata-index.json")
                                  .GetAwaiter()
                                  .GetResult();

            if (esClient.Indices.Exists(index).Exists)
                esClient.Indices.Delete(index);

            esClient.LowLevel.Indices.CreateAsync<BytesResponse>(index, settingsDoc)
                                     .GetAwaiter()
                                     .GetResult();
        }

        public void AddDataToIndexAsync()
        {
            var tags = new[] { "animal", "vegetable", "mineral" };
            // Add the Category/SubCategory combo used in tests
            Data.AddRange(_fixture.Build<QueryableReferenceData>()
                                  .With(x => x.Category, Category)
                                  .With(x => x.SubCategory, SubCategory)
                                  .With(x => x.Tags, tags)
                                  .CreateMany(10));
            // Add other data for the named category
            AddData(Category);

            // Add lots of other data for other categories
            for (int i = 1; i <= 5; i++)
                AddData($"Category-{i}");

            ElasticSearchClient.IndexManyAsync(Data, _index).GetAwaiter().GetResult();
            _cleanup.Add(() => { ElasticSearchClient.DeleteManyAsync(Data, _index).GetAwaiter().GetResult(); });

            Thread.Sleep(1000);
        }

        private void AddData(string category = null)
        {
            for (int i = 1; i <= 5; i++)
            {
                var categoryToUse = category ?? $"Category-{i}";
                var subToUse = $"Sub-Category-{i}";
                Data.AddRange(_fixture.Build<QueryableReferenceData>()
                                     .With(x => x.Category, categoryToUse)
                                     .With(x => x.SubCategory, subToUse)
                                     .CreateMany(5));
            }
        }

        public void GivenDataAlreadyExists()
        {
            if (Data.Count == 0)
                AddDataToIndexAsync();
        }
    }

    [CollectionDefinition("ElasticSearch collection", DisableParallelization = true)]
    public class DynamoDbCollection : ICollectionFixture<ElasticSearchFixture<Startup>>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
