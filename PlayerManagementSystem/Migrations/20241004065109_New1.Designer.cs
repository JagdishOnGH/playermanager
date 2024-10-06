﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PlayerManagementSystem.EfContext;

#nullable disable

namespace PlayerManagementSystem.Migrations
{
    [DbContext(typeof(EfDbContext))]
    [Migration("20241004065109_New1")]
    partial class New1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0-rc.1.24451.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PlayerManagementSystem.Models.Address", b =>
                {
                    b.Property<int>("AddressId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("AddressId"));

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("District")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsPermanent")
                        .HasColumnType("boolean");

                    b.Property<string>("Palika")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("PersonalDetailsId")
                        .HasColumnType("integer");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Tole")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Ward")
                        .HasColumnType("integer");

                    b.HasKey("AddressId");

                    b.HasIndex("PersonalDetailsId");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("PlayerManagementSystem.Models.Palika", b =>
                {
                    b.Property<int>("PalikaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("PalikaId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("PalikaId");

                    b.ToTable("Palikas");
                });

            modelBuilder.Entity("PlayerManagementSystem.Models.PersonalDetails", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateOnly>("Dob")
                        .HasColumnType("date");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Gender")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PhoneNo")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ProfilePicUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.Property<int?>("TeamId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("TeamId");

                    b.ToTable("PersonalDetails");
                });

            modelBuilder.Entity("PlayerManagementSystem.Models.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("RoleId"));

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("RoleId");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("PlayerManagementSystem.Models.Teams", b =>
                {
                    b.Property<int>("TeamId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("TeamId"));

                    b.Property<int>("AssociationId")
                        .HasColumnType("integer");

                    b.Property<string>("TeamName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("TeamOf")
                        .HasColumnType("integer");

                    b.HasKey("TeamId");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("PlayerManagementSystem.Models.Address", b =>
                {
                    b.HasOne("PlayerManagementSystem.Models.PersonalDetails", null)
                        .WithMany("Addresses")
                        .HasForeignKey("PersonalDetailsId");
                });

            modelBuilder.Entity("PlayerManagementSystem.Models.PersonalDetails", b =>
                {
                    b.HasOne("PlayerManagementSystem.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PlayerManagementSystem.Models.Teams", "Team")
                        .WithMany("PersonalDetails")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Role");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("PlayerManagementSystem.Models.PersonalDetails", b =>
                {
                    b.Navigation("Addresses");
                });

            modelBuilder.Entity("PlayerManagementSystem.Models.Teams", b =>
                {
                    b.Navigation("PersonalDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
