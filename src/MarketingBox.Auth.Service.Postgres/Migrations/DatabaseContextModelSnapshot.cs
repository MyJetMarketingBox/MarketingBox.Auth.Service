﻿// <auto-generated />
using System;
using MarketingBox.Auth.Service.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MarketingBox.Auth.Service.Postgres.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("auth-service")
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MarketingBox.Auth.Service.Domain.Models.User", b =>
                {
                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.Property<string>("ExternalUserId")
                        .HasColumnType("text");

                    b.Property<string>("EmailEncrypted")
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("Salt")
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("TenantId", "ExternalUserId");

                    b.HasIndex("TenantId", "EmailEncrypted")
                        .IsUnique();

                    b.HasIndex("TenantId", "Username")
                        .IsUnique();

                    b.ToTable("user", "auth-service");
                });

            modelBuilder.Entity("MarketingBox.Auth.Service.Domain.Models.UserLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("ChangeType")
                        .HasColumnType("integer");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("ModifiedByUserId")
                        .HasColumnType("bigint");

                    b.Property<long>("ModifiedForUserId")
                        .HasColumnType("bigint");

                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("user-log", "auth-service");
                });
#pragma warning restore 612, 618
        }
    }
}
