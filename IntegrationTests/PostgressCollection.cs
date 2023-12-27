using Xunit;

namespace IntegrationTests;

[CollectionDefinition(nameof(PostgressCollection))]
public class PostgressCollection : ICollectionFixture<ApiFactory> { }