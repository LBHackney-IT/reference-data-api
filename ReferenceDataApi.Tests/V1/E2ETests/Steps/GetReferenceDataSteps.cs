using FluentAssertions;
using ReferenceDataApi.V1.Boundary.Response;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReferenceDataApi.Tests.V1.E2ETests.Steps
{
    public class GetReferenceDataSteps : BaseSteps
    {
        public GetReferenceDataSteps(HttpClient httpClient) : base(httpClient)
        { }

        public async Task WhenReferenceDataIsRequested(string category, bool? includeInactive)
        {
            await WhenReferenceDataIsRequested(category, null, includeInactive).ConfigureAwait(false);
        }
        public async Task WhenReferenceDataIsRequested(string category, string subCategory, bool? includeInactive)
        {
            string route = $"api/v1/reference-data?category={category}";
            if (!string.IsNullOrEmpty(subCategory)) route += $"&subCategory={subCategory}";
            if (includeInactive.HasValue) route += $"&includeInactive={includeInactive}";

            var uri = new Uri(route, UriKind.Relative);
            _lastResponse = await _httpClient.GetAsync(uri).ConfigureAwait(false);
        }

        public async Task ThenTheCategoryReferenceDataIsReturned(ElasticSearchFixture<Startup> testFixture, string category, bool? includeInactive)
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await _lastResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var response = JsonSerializer.Deserialize<ReferenceDataResponseObject>(responseContent, CreateJsonOptions());

            var expected = testFixture.Data.Where(r => (r.Category == category))
                                                        .Select(x => x);
            if (!includeInactive.HasValue || !includeInactive.Value)
                expected = expected.Where(x => x.IsActive);

            var expectedSubCategories = expected.GroupBy(y => y.SubCategory);
            response.Count.Should().Be(expectedSubCategories.Count());
            foreach (var subCategory in expectedSubCategories)
            {
                response.First(x => x.Key == subCategory.Key).Key.Should().Be(subCategory.Key);
                response.First(x => x.Key == subCategory.Key).Value.Should().BeEquivalentTo(subCategory);
            }
        }

        public async Task ThenTheSubCategoryReferenceDataIsReturned(ElasticSearchFixture<Startup> testFixture, string category, string subCategory, bool? includeInactive)
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await _lastResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var response = JsonSerializer.Deserialize<ReferenceDataResponseObject>(responseContent, CreateJsonOptions());

            var expected = testFixture.Data.Where(r => (r.Category == category) && (r.SubCategory == subCategory))
                                           .Select(x => x);

            if (!includeInactive.HasValue || !includeInactive.Value)
                expected = expected.Where(x => x.IsActive);

            response.Count.Should().Be(1);
            response.First().Key.Should().Be(subCategory);
            response.First().Value.Should().BeEquivalentTo(expected);
        }

        public async Task ThenNoDataIsReturned()
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await _lastResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var response = JsonSerializer.Deserialize<ReferenceDataResponseObject>(responseContent, CreateJsonOptions());

            response.Should().BeEmpty();
        }
    }
}
