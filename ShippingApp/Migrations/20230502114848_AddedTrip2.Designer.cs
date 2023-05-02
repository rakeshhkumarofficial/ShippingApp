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
    [Migration("20230502114848_AddedTrip2")]
    partial class AddedTrip2
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

                    b.Property<Guid>("checkpointLocation")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("isAvailable")
                        .HasColumnType("bit");

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

            modelBuilder.Entity("ShippingApp.Models.ShippmentDriverMapping", b =>
                {
                    b.Property<Guid>("mapId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("checkpoint1Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("checkpoint2Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("containerType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("driverId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("isAccepted")
                        .HasColumnType("bit");

                    b.Property<bool>("isActive")
                        .HasColumnType("bit");

                    b.Property<string>("productType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("shipmentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<float>("shipmentWeight")
                        .HasColumnType("real");

                    b.Property<DateTime>("time")
                        .HasColumnType("datetime2");

                    b.HasKey("mapId");

                    b.ToTable("Shippers");
                });

            modelBuilder.Entity("ShippingApp.Models.Trip", b =>
                {
                    b.Property<Guid>("tripId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("checkpoint1Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("checkpoint2Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("driverId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("tripId");

                    b.ToTable("Trips");
                });
#pragma warning restore 612, 618
        }
    }
}
