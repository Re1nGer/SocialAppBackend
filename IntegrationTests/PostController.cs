using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Persistance;
using SocialApp.Models;
using Xunit;

namespace IntegrationTests;

[Collection(nameof(PostgressCollection))]
public class PostControllerTest
{
    private readonly ApiFactory _factory;

    public PostControllerTest(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetPostList_ReturnsCorrectPostList()
    {
        //Arrange
        var postId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        using (var scope = _factory.Services.CreateScope())
        {
            var database = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var user = new User
            {
                Id = userId,
                Email = "test@domain.com",
                Username = "testUsername",
                RegisteredAt = DateTime.UtcNow,
                HighResImageLink = "highResImageLink.jpg",
                LowResImageLink = "lowResImageLink.jpg",
                ProfileBackgroundImagelink = "profileBackgroundImageLink.jpg",
                Intro = "This is a test user.",
            };
            
            var post = new UserPost
            {
                Id = postId,
                LowResMediaUrl = "Some Dummy Url",
                Message = "Some Message",
                CreatedAt = DateTime.UtcNow,
                User = user,
                UserId = userId
            };

            await database.Users.AddAsync(user);
            
            await database.UserPosts.AddAsync(post);

            await database.SaveChangesAsync();
        }

        // Act
        var client = _factory.CreateClient();

        var token = _factory.GenerateJwt(userId.ToString());

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/v1/post/list");

        var responseString = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var userList = JsonConvert.DeserializeObject<List<PostModel>>(responseString);

        // Assert
        userList.Should().NotBeNull();

        userList.Should().ContainSingle(post => post.Message == "Some Message" && post.MediaUrl == "Some Dummy Url");
    }
}
    