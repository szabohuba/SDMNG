﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SDMNG.Data;

#nullable disable

namespace SDMNG.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250405133937_FixDatabaseIssue")]
    partial class FixDatabaseIssue
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("SDMNG.Models.Attachment", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("BusId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ContactId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UploadDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("BusId");

                    b.HasIndex("ContactId");

                    b.ToTable("Attachments");
                });

            modelBuilder.Entity("SDMNG.Models.Bus", b =>
                {
                    b.Property<string>("BusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("BusNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("BusType")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<string>("ContactId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("BusId");

                    b.HasIndex("ContactId")
                        .IsUnique();

                    b.ToTable("Buses");
                });

            modelBuilder.Entity("SDMNG.Models.Contact", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Zipcode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("SDMNG.Models.Schedule", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("ArrivalTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("BusId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DepartureTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TransportRouteId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("BusId");

                    b.HasIndex("TransportRouteId")
                        .IsUnique();

                    b.ToTable("Schedules");
                });

            modelBuilder.Entity("SDMNG.Models.Stop", b =>
                {
                    b.Property<string>("StopId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("Latitude")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Longitude")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("StopName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("TransportRouteId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("StopId");

                    b.HasIndex("TransportRouteId");

                    b.ToTable("Stops");
                });

            modelBuilder.Entity("SDMNG.Models.Ticket", b =>
                {
                    b.Property<string>("TicketId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ContactId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("PurchaseDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ScheduleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("SeatNumber")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("TicketId");

                    b.HasIndex("ContactId")
                        .IsUnique();

                    b.HasIndex("ScheduleId");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("SDMNG.Models.TransportRoute", b =>
                {
                    b.Property<string>("TransportRoutesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("EndLocation")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("StartLocation")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("TransportRoutesName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("TransportRoutesId");

                    b.ToTable("TransportRoutes");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("SDMNG.Models.Contact", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("SDMNG.Models.Contact", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SDMNG.Models.Contact", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("SDMNG.Models.Contact", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SDMNG.Models.Attachment", b =>
                {
                    b.HasOne("SDMNG.Models.Bus", "Bus")
                        .WithMany("Attachments")
                        .HasForeignKey("BusId");

                    b.HasOne("SDMNG.Models.Contact", "Contact")
                        .WithMany("Attachments")
                        .HasForeignKey("ContactId");

                    b.Navigation("Bus");

                    b.Navigation("Contact");
                });

            modelBuilder.Entity("SDMNG.Models.Bus", b =>
                {
                    b.HasOne("SDMNG.Models.Contact", "Contact")
                        .WithOne("Bus")
                        .HasForeignKey("SDMNG.Models.Bus", "ContactId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Contact");
                });

            modelBuilder.Entity("SDMNG.Models.Schedule", b =>
                {
                    b.HasOne("SDMNG.Models.Bus", "Bus")
                        .WithMany("Schedules")
                        .HasForeignKey("BusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SDMNG.Models.TransportRoute", "TransportRoute")
                        .WithOne("Schedule")
                        .HasForeignKey("SDMNG.Models.Schedule", "TransportRouteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bus");

                    b.Navigation("TransportRoute");
                });

            modelBuilder.Entity("SDMNG.Models.Stop", b =>
                {
                    b.HasOne("SDMNG.Models.TransportRoute", "TransportRoute")
                        .WithMany("Stops")
                        .HasForeignKey("TransportRouteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TransportRoute");
                });

            modelBuilder.Entity("SDMNG.Models.Ticket", b =>
                {
                    b.HasOne("SDMNG.Models.Contact", "Passanger")
                        .WithOne("Ticket")
                        .HasForeignKey("SDMNG.Models.Ticket", "ContactId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SDMNG.Models.Schedule", "Schedule")
                        .WithMany("Tickets")
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Passanger");

                    b.Navigation("Schedule");
                });

            modelBuilder.Entity("SDMNG.Models.Bus", b =>
                {
                    b.Navigation("Attachments");

                    b.Navigation("Schedules");
                });

            modelBuilder.Entity("SDMNG.Models.Contact", b =>
                {
                    b.Navigation("Attachments");

                    b.Navigation("Bus")
                        .IsRequired();

                    b.Navigation("Ticket")
                        .IsRequired();
                });

            modelBuilder.Entity("SDMNG.Models.Schedule", b =>
                {
                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("SDMNG.Models.TransportRoute", b =>
                {
                    b.Navigation("Schedule")
                        .IsRequired();

                    b.Navigation("Stops");
                });
#pragma warning restore 612, 618
        }
    }
}
