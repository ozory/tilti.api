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
    [Migration("20240307234545_PaymentToken")]
    partial class PaymentToken
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

            modelBuilder.Entity("Infrastructure.Data.Postgreesql.Features.Orders.Entities.Order", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("AcceptanceTime")
                        .HasColumnType("timestamp")
                        .HasColumnName("AcceptanceTime");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<DateTime?>("CancelationTime")
                        .HasColumnType("timestamp")
                        .HasColumnName("CancelationTime");

                    b.Property<DateTime?>("CompletionTime")
                        .HasColumnType("timestamp")
                        .HasColumnName("CompletionTime");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp")
                        .HasColumnName("Created");

                    b.Property<long?>("DriverId")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("RequestedTime")
                        .HasColumnType("timestamp")
                        .HasColumnName("RequestedTime");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Updated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValue(new DateTime(2024, 3, 7, 20, 45, 45, 782, DateTimeKind.Local).AddTicks(2100))
                        .HasColumnName("Updated");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("DriverId");

                    b.HasIndex("UserId");

                    b.ToTable("Orders", "tilt");
                });

            modelBuilder.Entity("Infrastructure.Data.Postgreesql.Features.Plans.Entities.Plan", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric")
                        .HasColumnName("Amount");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp")
                        .HasColumnName("Created");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)")
                        .HasColumnName("Name");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Updated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValue(new DateTime(2024, 3, 7, 20, 45, 45, 784, DateTimeKind.Local).AddTicks(8050))
                        .HasColumnName("Updated");

                    b.HasKey("Id");

                    b.ToTable("Plans", "tilt");
                });

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

            modelBuilder.Entity("Infrastructure.Data.Postgreesql.Features.Subscriptions.Entities.Subscription", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp")
                        .HasColumnName("Created");

                    b.Property<DateTime>("DueDate")
                        .HasColumnType("timestamp")
                        .HasColumnName("DueDate");

                    b.Property<long>("PlanId")
                        .HasColumnType("bigint");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Updated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValue(new DateTime(2024, 3, 7, 20, 45, 45, 784, DateTimeKind.Local).AddTicks(6120))
                        .HasColumnName("Updated");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("PlanId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Subscriptions", "tilt");
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

                    b.Property<string>("PaymentToken")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)")
                        .HasColumnName("PaymentToken");

                    b.Property<string>("Photo")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)")
                        .HasColumnName("Photo");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Updated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValue(new DateTime(2024, 3, 7, 20, 45, 45, 784, DateTimeKind.Local).AddTicks(9350))
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

            modelBuilder.Entity("Infrastructure.Data.Postgreesql.Features.Orders.Entities.Order", b =>
                {
                    b.HasOne("Infrastructure.Data.Postgreesql.Features.Users.Entities.User", "Driver")
                        .WithMany("DriverOrders")
                        .HasForeignKey("DriverId");

                    b.HasOne("Infrastructure.Data.Postgreesql.Features.Users.Entities.User", "User")
                        .WithMany("UserOrders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsMany("Infrastructure.Data.Postgreesql.Features.Orders.Entities.Address", "Addresses", b1 =>
                        {
                            b1.Property<long>("OrderId")
                                .HasColumnType("bigint");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            b1.Property<int>("AddressType")
                                .HasColumnType("integer");

                            b1.Property<string>("City")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Complment")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<double>("Latitude")
                                .HasColumnType("double precision");

                            b1.Property<double>("Longitude")
                                .HasColumnType("double precision");

                            b1.Property<string>("Neighborhood")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Number")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Street")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("ZipCode")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("OrderId", "Id");

                            b1.ToTable("Orders", "tilt");

                            b1.ToJson("Addresses");

                            b1.WithOwner()
                                .HasForeignKey("OrderId");
                        });

                    b.Navigation("Addresses");

                    b.Navigation("Driver");

                    b.Navigation("User");
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

            modelBuilder.Entity("Infrastructure.Data.Postgreesql.Features.Subscriptions.Entities.Subscription", b =>
                {
                    b.HasOne("Infrastructure.Data.Postgreesql.Features.Plans.Entities.Plan", "Plan")
                        .WithMany("Subscriptions")
                        .HasForeignKey("PlanId");

                    b.HasOne("Infrastructure.Data.Postgreesql.Features.Users.Entities.User", "User")
                        .WithOne()
                        .HasForeignKey("Infrastructure.Data.Postgreesql.Features.Subscriptions.Entities.Subscription", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Plan");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Infrastructure.Data.Postgreesql.Features.Plans.Entities.Plan", b =>
                {
                    b.Navigation("Subscriptions");
                });

            modelBuilder.Entity("Infrastructure.Data.Postgreesql.Features.Users.Entities.User", b =>
                {
                    b.Navigation("DriverOrders");

                    b.Navigation("UserOrders");
                });
#pragma warning restore 612, 618
        }
    }
}
