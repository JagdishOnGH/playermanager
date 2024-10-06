﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PlayerManagementSystem.EfContext;

#nullable disable

namespace PlayerManagementSystem.Migrations
{
    [DbContext(typeof(EfDbContext))]
    partial class EfDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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
                    b.Property<Guid>("PalikaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

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
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.Property<int?>("TeamsTeamId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("TeamsTeamId");

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

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("PalikaId")
                        .HasColumnType("uuid");

                    b.Property<string>("TeamName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("WardId")
                        .HasColumnType("uuid");

                    b.HasKey("TeamId");

                    b.HasIndex("PalikaId");

                    b.HasIndex("WardId");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("PlayerManagementSystem.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PlayerManagementSystem.Models.Ward", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("PalikaId")
                        .HasColumnType("uuid");

                    b.Property<int>("wardNo")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PalikaId");

                    b.ToTable("Wards");
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

                    b.HasOne("PlayerManagementSystem.Models.Teams", null)
                        .WithMany("PersonalDetails")
                        .HasForeignKey("TeamsTeamId");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("PlayerManagementSystem.Models.Teams", b =>
                {
                    b.HasOne("PlayerManagementSystem.Models.Palika", null)
                        .WithMany("Teams")
                        .HasForeignKey("PalikaId");

                    b.HasOne("PlayerManagementSystem.Models.Ward", null)
                        .WithMany("wardTeams")
                        .HasForeignKey("WardId");
                });

            modelBuilder.Entity("PlayerManagementSystem.Models.Ward", b =>
                {
                    b.HasOne("PlayerManagementSystem.Models.Palika", null)
                        .WithMany("Wards")
                        .HasForeignKey("PalikaId");
                });

            modelBuilder.Entity("PlayerManagementSystem.Models.Palika", b =>
                {
                    b.Navigation("Teams");

                    b.Navigation("Wards");
                });

            modelBuilder.Entity("PlayerManagementSystem.Models.PersonalDetails", b =>
                {
                    b.Navigation("Addresses");
                });

            modelBuilder.Entity("PlayerManagementSystem.Models.Teams", b =>
                {
                    b.Navigation("PersonalDetails");
                });

            modelBuilder.Entity("PlayerManagementSystem.Models.Ward", b =>
                {
                    b.Navigation("wardTeams");
                });
#pragma warning restore 612, 618
        }
    }
}
