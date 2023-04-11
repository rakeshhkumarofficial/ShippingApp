﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShippingApp.Data;

#nullable disable

namespace ShippingApp.Migrations
{
    [DbContext(typeof(ShippingDbContext))]
    [Migration("20230411062536_ProductType")]
    partial class ProductType
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ShippingApp.Models.ContainerType", b =>
                {
                    b.Property<Guid>("containerTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("containerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("containerWeight")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("containerTypeId");

                    b.ToTable("ContainerTypes");
                });

            modelBuilder.Entity("ShippingApp.Models.ProductType", b =>
                {
                    b.Property<Guid>("productTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("isFragile")
                        .HasColumnType("bit");

                    b.Property<string>("type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("productTypeId");

                    b.ToTable("ProductTypes");
                });
#pragma warning restore 612, 618
        }
    }
}
