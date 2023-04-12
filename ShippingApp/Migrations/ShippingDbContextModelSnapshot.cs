﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShippingApp.Data;

#nullable disable

namespace ShippingApp.Migrations
{
    [DbContext(typeof(ShippingDbContext))]
    partial class ShippingDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ShippingApp.Models.Checkpoint", b =>
                {
                    b.Property<Guid>("checkpointId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("checkpointId");

                    b.ToTable("Checkpoints");
                });

            modelBuilder.Entity("ShippingApp.Models.ContainerType", b =>
                {
                    b.Property<Guid>("containerTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("containerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("price")
                        .HasColumnType("real");

                    b.HasKey("containerTypeId");

                    b.ToTable("ContainerTypes");
                });

            modelBuilder.Entity("ShippingApp.Models.Driver", b =>
                {
                    b.Property<Guid>("driverId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("isAvailable")
                        .HasColumnType("bit");

                    b.Property<string>("location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("driverId");

                    b.ToTable("Drivers");
                });

            modelBuilder.Entity("ShippingApp.Models.ProductType", b =>
                {
                    b.Property<Guid>("productTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("isFragile")
                        .HasColumnType("bit");

                    b.Property<float>("price")
                        .HasColumnType("real");

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
