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
    [Migration("20240313204435_Email")]
    partial class Email
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

            modelBuilder.HasSequence("Sequence-Orders")
                .IncrementsBy(10);

            modelBuilder.HasSequence("Sequence-Users")
                .IncrementsBy(10);

            modelBuilder.Entity("Domain.Features.Orders.Entities.Order", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnOrder(0);

                    NpgsqlPropertyBuilderExtensions.UseHiLo(b.Property<long>("Id"), "Sequence-Orders");

                    b.Property<DateTime?>("AcceptanceTime")
                        .HasColumnType("timestamp")
                        .HasColumnName("AcceptanceTime")
                        .HasColumnOrder(8);

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric")
                        .HasColumnName("Amount")
                        .HasColumnOrder(4);

                    b.Property<DateTime?>("CancelationTime")
                        .HasColumnType("timestamp")
                        .HasColumnName("CancelationTime")
                        .HasColumnOrder(10);

                    b.Property<DateTime?>("CompletionTime")
                        .HasColumnType("timestamp")
                        .HasColumnName("CompletionTime")
                        .HasColumnOrder(9);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp")
                        .HasColumnName("Created")
                        .HasColumnOrder(5);

                    b.Property<long>("CreatedBy")
                        .HasColumnType("bigint");

                    b.Property<int>("DistanceInKM")
                        .HasColumnType("integer");

                    b.Property<long?>("DriverId")
                        .HasColumnType("bigint")
                        .HasColumnOrder(2);

                    b.Property<int>("DurationInSeconds")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("RequestedTime")
                        .HasColumnType("timestamp")
                        .HasColumnName("RequestedTime")
                        .HasColumnOrder(7);

                    b.Property<ushort>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("Status")
                        .HasColumnOrder(3);

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValue(new DateTime(2024, 3, 13, 17, 44, 35, 728, DateTimeKind.Local).AddTicks(2420))
                        .HasColumnName("Updated")
                        .HasColumnOrder(6);

                    b.Property<long?>("UpdatedBy")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnOrder(1);

                    b.HasKey("Id");

                    b.HasIndex("DriverId");

                    b.HasIndex("UserId");

                    b.ToTable("Orders", "tilt");
                });

            modelBuilder.Entity("Domain.Features.Plans.Entities.Plan", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnOrder(0);

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric")
                        .HasColumnName("Amount")
                        .HasColumnOrder(4);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp")
                        .HasColumnName("Created")
                        .HasColumnOrder(5);

                    b.Property<long>("CreatedBy")
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Description")
                        .HasColumnOrder(3);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)")
                        .HasColumnName("Name")
                        .HasColumnOrder(2);

                    b.Property<ushort>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("Status")
                        .HasColumnOrder(1);

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValue(new DateTime(2024, 3, 13, 17, 44, 35, 731, DateTimeKind.Local).AddTicks(8950))
                        .HasColumnName("Updated")
                        .HasColumnOrder(6);

                    b.Property<long?>("UpdatedBy")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("Plans", "tilt");
                });

            modelBuilder.Entity("Domain.Features.Subscriptions.Entities.Subscription", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnOrder(0);

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp")
                        .HasColumnName("Created");

                    b.Property<long>("CreatedBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("DueDate")
                        .HasColumnType("timestamp")
                        .HasColumnName("DueDate");

                    b.Property<string>("PaymentToken")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)")
                        .HasColumnName("PaymentToken");

                    b.Property<long>("PlanId")
                        .HasColumnType("bigint");

                    b.Property<ushort>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("Status")
                        .HasColumnOrder(1);

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValue(new DateTime(2024, 3, 13, 17, 44, 35, 731, DateTimeKind.Local).AddTicks(1530))
                        .HasColumnName("Updated");

                    b.Property<long?>("UpdatedBy")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("PlanId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Subscriptions", "tilt");
                });

            modelBuilder.Entity("Domain.Features.Users.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnOrder(0);

                    NpgsqlPropertyBuilderExtensions.UseHiLo(b.Property<long>("Id"), "Sequence-Users");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp")
                        .HasColumnName("Created")
                        .HasColumnOrder(13);

                    b.Property<long>("CreatedBy")
                        .HasColumnType("bigint");

                    b.Property<string>("Document")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("character varying(11)")
                        .HasColumnName("Document")
                        .HasColumnOrder(4);

                    b.Property<bool>("DriveEnable")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)")
                        .HasColumnName("Name")
                        .HasColumnOrder(1);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)")
                        .HasColumnName("Password")
                        .HasColumnOrder(3);

                    b.Property<string>("PaymentToken")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)")
                        .HasColumnName("PaymentToken")
                        .HasColumnOrder(16);

                    b.Property<string>("PaymentUserIdentifier")
                        .HasColumnType("text");

                    b.Property<string>("Photo")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)")
                        .HasColumnName("Photo")
                        .HasColumnOrder(7);

                    b.Property<ushort>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("Status")
                        .HasColumnOrder(2);

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValue(new DateTime(2024, 3, 13, 17, 44, 35, 736, DateTimeKind.Local).AddTicks(4240))
                        .HasColumnName("Updated")
                        .HasColumnOrder(14);

                    b.Property<long?>("UpdatedBy")
                        .HasColumnType("bigint");

                    b.Property<string>("VerificationCode")
                        .HasMaxLength(6)
                        .HasColumnType("character varying(6)")
                        .HasColumnName("ValidationCode")
                        .HasColumnOrder(6);

                    b.Property<string>("VerificationSalt")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)")
                        .HasColumnName("ValidationSalt")
                        .HasColumnOrder(15);

                    b.HasKey("Id");

                    b.ToTable("Users", "tilt");
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

            modelBuilder.Entity("Domain.Features.Orders.Entities.Order", b =>
                {
                    b.HasOne("Domain.Features.Users.Entities.User", "Driver")
                        .WithMany("DriverOrders")
                        .HasForeignKey("DriverId");

                    b.HasOne("Domain.Features.Users.Entities.User", "User")
                        .WithMany("UserOrders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsMany("Domain.ValueObjects.Address", "Addresses", b1 =>
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

            modelBuilder.Entity("Domain.Features.Subscriptions.Entities.Subscription", b =>
                {
                    b.HasOne("Domain.Features.Plans.Entities.Plan", "Plan")
                        .WithMany("Subscriptions")
                        .HasForeignKey("PlanId");

                    b.HasOne("Domain.Features.Users.Entities.User", "User")
                        .WithOne("Subscription")
                        .HasForeignKey("Domain.Features.Subscriptions.Entities.Subscription", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Plan");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Features.Users.Entities.User", b =>
                {
                    b.OwnsOne("Domain.ValueObjects.Email", "Email", b1 =>
                        {
                            b1.Property<long>("UserId")
                                .HasColumnType("bigint");

                            b1.Property<string>("Value")
                                .HasMaxLength(250)
                                .HasColumnType("character varying(250)")
                                .HasColumnName("Email");

                            b1.HasKey("UserId");

                            b1.ToTable("Users", "tilt");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.OwnsOne("Domain.ValueObjects.Transport", "Transport", b1 =>
                        {
                            b1.Property<long>("UserId")
                                .HasColumnType("bigint");

                            b1.Property<string>("Description")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("Description")
                                .HasColumnOrder(9);

                            b1.Property<string>("Model")
                                .IsRequired()
                                .HasMaxLength(250)
                                .HasColumnType("character varying(250)")
                                .HasColumnName("VehicleModel")
                                .HasColumnOrder(10);

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasMaxLength(250)
                                .HasColumnType("character varying(250)")
                                .HasColumnName("VehicleName")
                                .HasColumnOrder(8);

                            b1.Property<string>("Plate")
                                .IsRequired()
                                .HasMaxLength(250)
                                .HasColumnType("character varying(250)")
                                .HasColumnName("VehiclePlate")
                                .HasColumnOrder(11);

                            b1.Property<short>("Year")
                                .HasColumnType("smallint")
                                .HasColumnName("VehicleYear")
                                .HasColumnOrder(12);

                            b1.HasKey("UserId");

                            b1.ToTable("Users", "tilt");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.Navigation("Email")
                        .IsRequired();

                    b.Navigation("Transport");
                });

            modelBuilder.Entity("Infrastructure.Data.Postgreesql.Features.Security.Entities.RefreshTokens", b =>
                {
                    b.HasOne("Domain.Features.Users.Entities.User", "User")
                        .WithOne()
                        .HasForeignKey("Infrastructure.Data.Postgreesql.Features.Security.Entities.RefreshTokens", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Features.Plans.Entities.Plan", b =>
                {
                    b.Navigation("Subscriptions");
                });

            modelBuilder.Entity("Domain.Features.Users.Entities.User", b =>
                {
                    b.Navigation("DriverOrders");

                    b.Navigation("Subscription");

                    b.Navigation("UserOrders");
                });
#pragma warning restore 612, 618
        }
    }
}