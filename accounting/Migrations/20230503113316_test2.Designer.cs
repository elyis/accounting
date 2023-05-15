﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using accounting.src.Data;

#nullable disable

namespace accounting.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230503113316_test2")]
    partial class test2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("accounting.src.Models.AmountOfAccountedMaterials", b =>
                {
                    b.Property<Guid>("MaterialAccountingId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("MaterialId")
                        .HasColumnType("uuid");

                    b.Property<float>("Amount")
                        .HasColumnType("real");

                    b.HasKey("MaterialAccountingId", "MaterialId");

                    b.HasIndex("MaterialId");

                    b.ToTable("AmountOfAccountedMaterials");
                });

            modelBuilder.Entity("accounting.src.Models.ConsumptionOfMaterialPerProduct", b =>
                {
                    b.Property<Guid>("MaterialId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.Property<float>("Amount")
                        .HasColumnType("real");

                    b.HasKey("MaterialId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("ConsumptionOfMaterialPerProduct");
                });

            modelBuilder.Entity("accounting.src.Models.Material", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<float>("Amount")
                        .HasColumnType("real");

                    b.Property<string>("Image")
                        .HasColumnType("text");

                    b.Property<Guid?>("MaterialAccountingId")
                        .HasColumnType("uuid");

                    b.Property<string>("Measure")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("MaterialAccountingId");

                    b.ToTable("Materials");
                });

            modelBuilder.Entity("accounting.src.Models.MaterialAccounting", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.ToTable("MaterialAccountings");
                });

            modelBuilder.Entity("accounting.src.Models.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Image")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<float>("Price")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("accounting.src.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Image")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Patronymic")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RecoveryCode")
                        .HasColumnType("text");

                    b.Property<DateTime?>("RecoveryCodeValidBefore")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Token")
                        .HasColumnType("text");

                    b.Property<bool>("WasPasswordResetRequest")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("accounting.src.Models.AmountOfAccountedMaterials", b =>
                {
                    b.HasOne("accounting.src.Models.MaterialAccounting", "MaterialAccounting")
                        .WithMany("AccountedMaterials")
                        .HasForeignKey("MaterialAccountingId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("accounting.src.Models.Material", "Material")
                        .WithMany("AccountedMaterials")
                        .HasForeignKey("MaterialId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Material");

                    b.Navigation("MaterialAccounting");
                });

            modelBuilder.Entity("accounting.src.Models.ConsumptionOfMaterialPerProduct", b =>
                {
                    b.HasOne("accounting.src.Models.Material", "Material")
                        .WithMany("Products")
                        .HasForeignKey("MaterialId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("accounting.src.Models.Product", "Product")
                        .WithMany("Materials")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Material");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("accounting.src.Models.Material", b =>
                {
                    b.HasOne("accounting.src.Models.MaterialAccounting", null)
                        .WithMany("Materials")
                        .HasForeignKey("MaterialAccountingId");
                });

            modelBuilder.Entity("accounting.src.Models.MaterialAccounting", b =>
                {
                    b.HasOne("accounting.src.Models.User", "CreatedBy")
                        .WithMany("MaterialAccountings")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreatedBy");
                });

            modelBuilder.Entity("accounting.src.Models.Material", b =>
                {
                    b.Navigation("AccountedMaterials");

                    b.Navigation("Products");
                });

            modelBuilder.Entity("accounting.src.Models.MaterialAccounting", b =>
                {
                    b.Navigation("AccountedMaterials");

                    b.Navigation("Materials");
                });

            modelBuilder.Entity("accounting.src.Models.Product", b =>
                {
                    b.Navigation("Materials");
                });

            modelBuilder.Entity("accounting.src.Models.User", b =>
                {
                    b.Navigation("MaterialAccountings");
                });
#pragma warning restore 612, 618
        }
    }
}
