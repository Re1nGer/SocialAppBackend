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
public class UserControllerTest
{
    private readonly ApiFactory _factory;

    public UserControllerTest(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetUserList_ReturnsCorrectUserList()
    {
        //Arrange
        var userId = new Guid();
        using (var scope = _factory.Services.CreateScope())
        {
            var database = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            var user = new User
            {
                Id = userId,
                Email = "test@domain.com",
                Username = "testUsername",
                RegisteredAt = DateTime.UtcNow,
            };
            
            await database.Users.AddAsync(user);

            await database.SaveChangesAsync();
        }
        
        // Act
        var client = _factory.CreateClient();
        
        var token = _factory.GenerateJwt(userId.ToString());

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var response = await client.GetAsync("/api/v1/user/list");

        var responseString = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var userList = JsonConvert.DeserializeObject<List<User>>(responseString);
        
        // Assert
        userList.Should().NotBeNull();

        userList.Should().ContainSingle(user => user.Email == "test@domain.com" && user.Username == "testUsername");
    }
    
    [Fact]
    public async Task GetUser_ReturnsCorrectUserData()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var post1Id = Guid.NewGuid();
        var post2Id = Guid.NewGuid();
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

            var post1 = new UserPost
            {
                Id = post1Id,
                UserId = userId,
                User = user,
                LowResMediaUrl = "post1_lowResMediaUrl.jpg",
                Message = "This is the first post.",
                Likes = new (),
                Comments = new ()
            };

            var post2 = new UserPost
            {
                Id = post2Id,
                UserId = userId,
                User = user,
                LowResMediaUrl = "post2_lowResMediaUrl.jpg",
                Message = "This is the second post.",
                Likes = new (),
                Comments = new ()
            };

            await database.Users.AddAsync(user);
            await database.UserPosts.AddRangeAsync(post1, post2);
            await database.SaveChangesAsync();
        }

        // Act
        var client = _factory.CreateClient();

        var token = _factory.GenerateJwt(userId.ToString());

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/v1/user");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseString = await response.Content.ReadAsStringAsync();

        var userResponse = JsonConvert.DeserializeObject<UserResponse>(responseString);

        // Add more specific assertions based on the expected user data
        userResponse.Should().NotBeNull();
        userResponse.Username.Should().Be("testUsername");
        userResponse.HighResImageLink.Should().Be("highResImageLink.jpg");
        userResponse.LowResImageLink.Should().Be("lowResImageLink.jpg");
        userResponse.ProfileBackgroundImagelink.Should().Be("profileBackgroundImageLink.jpg");
        userResponse.Intro.Should().Be("This is a test user.");
        userResponse.UserPosts.Should().NotBeNull();
        userResponse.UserPosts.Should().HaveCount(2); // Assuming two test posts were added
        userResponse.UserPosts.Should().ContainSingle(post => post.Id == post1Id && post.Message == "This is the first post.");
        userResponse.UserPosts.Should().ContainSingle(post => post.Id == post2Id && post.Message == "This is the second post.");
    }
    
    [Fact]
    public async Task GetUserById_ReturnsCorrectUserData()
    {
        // Arrange
        var userId = Guid.NewGuid(); // Replace with a valid GUID
        var currentUserId = Guid.NewGuid(); // Replace with a valid GUID
        var postId = Guid.NewGuid();
        using (var scope = _factory.Services.CreateScope())
        {
            var database = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


            var user = new User
            {
                Id = userId,
                Username = "testUsername",
                Email = "test@domain.com"
            };

            var post1 = new UserPost
            {
                Id = postId,
                UserId = userId,
                LowResMediaUrl = "post1_lowResMediaUrl.jpg",
                Message = "This is the first post.",
                User = user
            };
            
            var request = new UserRequest
            {
                SenderUserId = currentUserId,
                UserReceivingRequestId = userId,
                Status = "Accepted",
                SendUser = user
            };

            await database.Users.AddAsync(user);
            await database.UserPosts.AddAsync(post1);
            await database.UserRequests.AddAsync(request);

            // Add other necessary data to the database
            await database.SaveChangesAsync();
        }

        // Act
        var client = _factory.CreateClient();

        var token = _factory.GenerateJwt(currentUserId.ToString());

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"/api/v1/user/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseString = await response.Content.ReadAsStringAsync();

        var userResponse = JsonConvert.DeserializeObject<UserResponse>(responseString);

        userResponse.Should().NotBeNull();
        userResponse.Username.Should().Be("testUsername");
        userResponse.UserPosts.Should().NotBeNull();
        userResponse.UserRequests.Should().NotBeNull();
        userResponse.IsFollowing.Should().BeFalse(); 
    }
}