﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Persistance;

#nullable disable

namespace Persistance.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230320211618_blockedUsers")]
    partial class blockedUsers
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entities.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DateUpdated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("PostId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.ToTable("UserComments");
                });

            modelBuilder.Entity("Domain.Entities.Like", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("PostId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.ToTable("UserLikes");
                });

            modelBuilder.Entity("Domain.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<string>("Intro")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastLogin")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("Picture")
                        .HasColumnType("text");

                    b.Property<string>("Profile")
                        .HasColumnType("text");

                    b.Property<DateTime>("RegisteredAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Domain.Entities.UserBlocked", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("UserBlockedId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UsersBlocked");
                });

            modelBuilder.Entity("Domain.Entities.UserFollower", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("FollowerId")
                        .HasColumnType("integer");

                    b.Property<int?>("FollowerUserId")
                        .HasColumnType("integer");

                    b.Property<int>("FollowingId")
                        .HasColumnType("integer");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("FollowerUserId");

                    b.HasIndex("FollowingId");

                    b.ToTable("UserFollowers");
                });

            modelBuilder.Entity("Domain.Entities.UserMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("SourceId")
                        .HasColumnType("integer");

                    b.Property<int>("TargetId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("SourceId");

                    b.HasIndex("TargetId");

                    b.ToTable("UserMessages");
                });

            modelBuilder.Entity("Domain.Entities.UserPost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<List<int>>("CommentIds")
                        .HasColumnType("integer[]");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<List<int>>("LikeIds")
                        .HasColumnType("integer[]");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserPosts");
                });

            modelBuilder.Entity("Domain.Entities.UserReceivingRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("TargetUserId")
                        .HasColumnType("integer");

                    b.Property<int>("UserRequestId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("TargetUserId");

                    b.HasIndex("UserRequestId")
                        .IsUnique();

                    b.ToTable("UserReceivingRequests");
                });

            modelBuilder.Entity("Domain.Entities.UserRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("SenderUserId")
                        .HasColumnType("integer");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("UserReceivingRequestId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SenderUserId");

                    b.ToTable("UserRequests");
                });

            modelBuilder.Entity("Domain.Entities.Comment", b =>
                {
                    b.HasOne("Domain.Entities.UserPost", "UserPost")
                        .WithMany("Comments")
                        .HasForeignKey("PostId");

                    b.Navigation("UserPost");
                });

            modelBuilder.Entity("Domain.Entities.Like", b =>
                {
                    b.HasOne("Domain.Entities.UserPost", "UserPost")
                        .WithMany("Likes")
                        .HasForeignKey("PostId");

                    b.Navigation("UserPost");
                });

            modelBuilder.Entity("Domain.Entities.UserBlocked", b =>
                {
                    b.HasOne("Domain.Entities.User", "User")
                        .WithMany("UsersBlocked")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Entities.UserFollower", b =>
                {
                    b.HasOne("Domain.Entities.User", "FollowerUser")
                        .WithMany()
                        .HasForeignKey("FollowerUserId");

                    b.HasOne("Domain.Entities.User", "FollowingUser")
                        .WithMany("Followers")
                        .HasForeignKey("FollowingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FollowerUser");

                    b.Navigation("FollowingUser");
                });

            modelBuilder.Entity("Domain.Entities.UserMessage", b =>
                {
                    b.HasOne("Domain.Entities.User", "SourceUser")
                        .WithMany("UserMessageSources")
                        .HasForeignKey("SourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.User", "TargetUser")
                        .WithMany("UserMessageTargets")
                        .HasForeignKey("TargetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SourceUser");

                    b.Navigation("TargetUser");
                });

            modelBuilder.Entity("Domain.Entities.UserPost", b =>
                {
                    b.HasOne("Domain.Entities.User", "User")
                        .WithMany("UserPosts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Entities.UserReceivingRequest", b =>
                {
                    b.HasOne("Domain.Entities.User", "TargetUser")
                        .WithMany("UserReceivingRequests")
                        .HasForeignKey("TargetUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.UserRequest", "UserRequest")
                        .WithOne("UserReceivingRequest")
                        .HasForeignKey("Domain.Entities.UserReceivingRequest", "UserRequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TargetUser");

                    b.Navigation("UserRequest");
                });

            modelBuilder.Entity("Domain.Entities.UserRequest", b =>
                {
                    b.HasOne("Domain.Entities.User", "SendUser")
                        .WithMany("UserRequests")
                        .HasForeignKey("SenderUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SendUser");
                });

            modelBuilder.Entity("Domain.Entities.User", b =>
                {
                    b.Navigation("Followers");

                    b.Navigation("UserMessageSources");

                    b.Navigation("UserMessageTargets");

                    b.Navigation("UserPosts");

                    b.Navigation("UserReceivingRequests");

                    b.Navigation("UserRequests");

                    b.Navigation("UsersBlocked");
                });

            modelBuilder.Entity("Domain.Entities.UserPost", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Likes");
                });

            modelBuilder.Entity("Domain.Entities.UserRequest", b =>
                {
                    b.Navigation("UserReceivingRequest");
                });
#pragma warning restore 612, 618
        }
    }
}
