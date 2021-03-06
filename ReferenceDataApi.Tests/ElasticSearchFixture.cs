using AutoFixture;
using Elasticsearch.Net;
using Nest;
using ReferenceDataApi.V1.Gateways;
using ReferenceDataApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            WaitForESInstance(ElasticSearchClient);
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

        private static void WaitForESInstance(IElasticClient elasticSearchClient)
        {
            Exception ex = null;
            var timeout = DateTime.UtcNow.AddSeconds(5); // 5 second timeout (make configurable?)
            while (DateTime.UtcNow < timeout)
            {
                try
                {
                    var pingResponse = elasticSearchClient.Ping();
                    if (pingResponse.IsValid)
                        return;
                    else
                        ex = pingResponse.OriginalException;
                }
                catch (Exception e)
                {
                    ex = e;
                }

                Thread.Sleep(200);
            }

            if (ex != null)
                throw ex;
        }

        private static void EnsureEnvVarConfigured(string name, string defaultValue)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name)))
                Environment.SetEnvironmentVariable(name, defaultValue);
        }

        private static void EnsureIndexExists(IElasticClient esClient, string index)
        {
            try
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
            catch (System.Exception e)
            {
                var esNodes = string.Join(';', esClient.ConnectionSettings.ConnectionPool.Nodes.Select(x => x.Uri));
                var message = $"ES call to NodeUris: {esNodes} failed with message: {e.Message}.";
                throw new Exception(message, e);
            }
        }

        public void AddDataToIndexAsync()
        {
            // Add the Category/SubCategory combo used in tests
            AddData(Category, SubCategory, 10, 10);

            // This dataset is to test an exact match on category and sub category
            AddData($"{Category}-different", $"{SubCategory}-different", 10, 10);

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
                AddData(categoryToUse, subToUse, 5, 5);
            }
        }

        private void AddData(string category, string subCategory, int active, int inactive)
        {
            Data.AddRange(_fixture.Build<QueryableReferenceData>()
                                    .With(x => x.Category, category)
                                    .With(x => x.SubCategory, subCategory)
                                    .With(x => x.IsActive, true)
                                    .CreateMany(active));
            Data.AddRange(_fixture.Build<QueryableReferenceData>()
                                    .With(x => x.Category, category)
                                    .With(x => x.SubCategory, subCategory)
                                    .With(x => x.IsActive, false)
                                    .CreateMany(inactive));
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
