﻿// <auto-generated />
using System;
using Infrastructure.Data.Postgreesql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Data.Postgreesql.Migrations
{
    [DbContext(typeof(TILTContext))]
    [Migration("20240208151230_DriveEnableCollumn")]
    partial class DriveEnableCollumn
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("tilt")
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Infrastructure.Data.Postgreesql.Features.Security.Entities.RefreshTokens", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("LastLogin")
                        .HasColumnType("timestamp")
                        .HasColumnName("LastLogin");

                    b.Property<string>("RefreshToken")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)")
                        .HasColumnName("RefeshToken");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("RefeshTokens", "security");
                });

            modelBuilder.Entity("Infrastructure.Data.Postgreesql.Features.Users.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp")
                        .HasColumnName("Created");

                    b.Property<string>("Document")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("character varying(11)")
                        .HasColumnName("Document");

                    b.Property<bool>("DriveEnable")
                        .HasColumnType("boolean");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)")
                        .HasColumnName("Email");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)")
                        .HasColumnName("Name");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)")
                        .HasColumnName("Password");

                    b.Property<string>("Photo")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)")
                        .HasColumnName("Photo");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Updated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValue(new DateTime(2024, 2, 8, 12, 12, 30, 344, DateTimeKind.Local).AddTicks(4020))
                        .HasColumnName("Updated");

                    b.Property<string>("ValidationCode")
                        .HasMaxLength(6)
                        .HasColumnType("character varying(6)")
                        .HasColumnName("ValidationCode");

                    b.Property<string>("ValidationSalt")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)")
                        .HasColumnName("ValidationSalt");

                    b.Property<string>("VehicleModel")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)")
                        .HasColumnName("VehicleModel");

                    b.Property<string>("VehiclePlate")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)")
                        .HasColumnName("VehiclePlate");

                    b.Property<short?>("VehicleYear")
                        .HasColumnType("smallint")
                        .HasColumnName("VehicleYear");

                    b.Property<string>("VerificationCode")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users", "tilt");
                });

            modelBuilder.Entity("Infrastructure.Data.Postgreesql.Features.Security.Entities.RefreshTokens", b =>
                {
                    b.HasOne("Infrastructure.Data.Postgreesql.Features.Users.Entities.User", "User")
                        .WithOne()
                        .HasForeignKey("Infrastructure.Data.Postgreesql.Features.Security.Entities.RefreshTokens", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
