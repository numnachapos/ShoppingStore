﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShoppingStore.Infrastructure.Data;

#nullable disable

namespace ShoppingStore.Infrastructure.Migrations
{
    [DbContext(typeof(ShoppingStoreContext))]
    partial class ShoppingStoreContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ShoppingStore.Domain.Entities.Article", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<string>("SKU")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Articles");

                    b.HasData(
                        new
                        {
                            Id = new Guid("bfd7a329-8080-4729-8cd4-b99e06335360"),
                            Name = "Red T-Shirt",
                            Price = 9.9900000000000002,
                            SKU = "SKU001"
                        },
                        new
                        {
                            Id = new Guid("34f5228e-eed2-4483-bd92-ea5bd2749a09"),
                            Name = "Blue T-Shirt",
                            Price = 11.99,
                            SKU = "SKU002"
                        },
                        new
                        {
                            Id = new Guid("dbe85208-c181-4d23-9b43-8201d8bff7c8"),
                            Name = "Green Blouson",
                            Price = 99.989999999999995,
                            SKU = "SKU003"
                        });
                });

            modelBuilder.Entity("ShoppingStore.Domain.Entities.CartItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ArticleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<Guid>("ShoppingCartId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ArticleId");

                    b.HasIndex("ShoppingCartId");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("ShoppingStore.Domain.Entities.ShoppingCart", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Carts");
                });

            modelBuilder.Entity("ShoppingStore.Domain.Entities.CartItem", b =>
                {
                    b.HasOne("ShoppingStore.Domain.Entities.Article", "Article")
                        .WithMany()
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ShoppingStore.Domain.Entities.ShoppingCart", "ShoppingCart")
                        .WithMany("Items")
                        .HasForeignKey("ShoppingCartId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Article");

                    b.Navigation("ShoppingCart");
                });

            modelBuilder.Entity("ShoppingStore.Domain.Entities.ShoppingCart", b =>
                {
                    b.Navigation("Items");
                });
#pragma warning restore 612, 618
        }
    }
}
