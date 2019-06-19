﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebAppWMHBattleReporter.Data;

namespace WebAppWMHBattleReporter.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190619175257_RemoveUnnecessaryRequirementsOnBattleReportsAttributes")]
    partial class RemoveUnnecessaryRequirementsOnBattleReportsAttributes
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");

                    b.HasDiscriminator<string>("Discriminator").HasValue("IdentityUser");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("WebAppWMHBattleReporter.Models.BattleReport", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ConfirmationKey");

                    b.Property<bool>("ConfirmedByOpponent");

                    b.Property<DateTime>("DatePlayed");

                    b.Property<string>("EndCondition")
                        .IsRequired();

                    b.Property<int>("GameSize");

                    b.Property<string>("LosersUsername");

                    b.Property<string>("LosingCaster");

                    b.Property<string>("LosingFaction");

                    b.Property<string>("LosingTheme");

                    b.Property<string>("OpponentsArmyList")
                        .IsRequired();

                    b.Property<int>("OpponentsArmyPoints");

                    b.Property<string>("OpponentsCaster")
                        .IsRequired();

                    b.Property<int>("OpponentsControlPoints");

                    b.Property<string>("OpponentsFaction")
                        .IsRequired();

                    b.Property<string>("OpponentsTheme")
                        .IsRequired();

                    b.Property<string>("OpponentsUsername")
                        .IsRequired();

                    b.Property<string>("PostersArmyList")
                        .IsRequired();

                    b.Property<int>("PostersArmyPoints");

                    b.Property<string>("PostersCaster")
                        .IsRequired();

                    b.Property<int>("PostersControlPoints");

                    b.Property<string>("PostersFaction")
                        .IsRequired();

                    b.Property<string>("PostersTheme")
                        .IsRequired();

                    b.Property<string>("PostersUsername")
                        .IsRequired();

                    b.Property<string>("Scenario")
                        .IsRequired();

                    b.Property<string>("WinnersUsername");

                    b.Property<string>("WinningCaster");

                    b.Property<string>("WinningFaction");

                    b.Property<string>("WinningTheme");

                    b.HasKey("Id");

                    b.ToTable("BattleReports");
                });

            modelBuilder.Entity("WebAppWMHBattleReporter.Models.Caster", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("FactionId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("NumberOfGamesLost");

                    b.Property<int>("NumberOfGamesPlayed");

                    b.Property<int>("NumberOfGamesWon");

                    b.Property<float>("Winrate");

                    b.HasKey("Id");

                    b.HasIndex("FactionId");

                    b.ToTable("Casters");
                });

            modelBuilder.Entity("WebAppWMHBattleReporter.Models.Faction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("NumberOfGamesLost");

                    b.Property<int>("NumberOfGamesPlayed");

                    b.Property<int>("NumberOfGamesWon");

                    b.Property<float>("Winrate");

                    b.HasKey("Id");

                    b.ToTable("Factions");
                });

            modelBuilder.Entity("WebAppWMHBattleReporter.Models.Theme", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("FactionId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("NumberOfGamesLost");

                    b.Property<int>("NumberOfGamesPlayed");

                    b.Property<int>("NumberOfGamesWon");

                    b.Property<float>("Winrate");

                    b.HasKey("Id");

                    b.HasIndex("FactionId");

                    b.ToTable("Themes");
                });

            modelBuilder.Entity("WebAppWMHBattleReporter.Models.ApplicationUser", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.IdentityUser");

                    b.Property<int>("NumberOfGamesLost");

                    b.Property<int>("NumberOfGamesPlayed");

                    b.Property<int>("NumberOfGamesWon");

                    b.Property<string>("Region")
                        .IsRequired();

                    b.Property<float>("Winrate");

                    b.HasDiscriminator().HasValue("ApplicationUser");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WebAppWMHBattleReporter.Models.Caster", b =>
                {
                    b.HasOne("WebAppWMHBattleReporter.Models.Faction", "Faction")
                        .WithMany()
                        .HasForeignKey("FactionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WebAppWMHBattleReporter.Models.Theme", b =>
                {
                    b.HasOne("WebAppWMHBattleReporter.Models.Faction", "Faction")
                        .WithMany()
                        .HasForeignKey("FactionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
