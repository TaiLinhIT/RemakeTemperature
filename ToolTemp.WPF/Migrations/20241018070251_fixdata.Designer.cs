﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ToolTemp.WPF.Models;

#nullable disable

namespace ToolTemp.WPF.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20241018070251_fixdata")]
    partial class fixdata
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ToolTemp.WPF.Models.BusDataTemp", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AddressMachine")
                        .HasColumnType("int");

                    b.Property<int>("Baudrate")
                        .HasColumnType("int");

                    b.Property<string>("Channel")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Factory")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsWarning")
                        .HasColumnType("bit");

                    b.Property<string>("Line")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<double>("Max")
                        .HasColumnType("float");

                    b.Property<double>("Min")
                        .HasColumnType("float");

                    b.Property<string>("Port")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("Sensor_Typeid")
                        .HasColumnType("int");

                    b.Property<string>("Sensor_ant")
                        .IsRequired()
                        .HasMaxLength(63)
                        .HasColumnType("nvarchar(63)");

                    b.Property<string>("Sensor_kind")
                        .IsRequired()
                        .HasMaxLength(63)
                        .HasColumnType("nvarchar(63)");

                    b.Property<double>("Temp")
                        .HasColumnType("float");

                    b.Property<DateTime>("UploadDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("dv_BusDataTemp", (string)null);
                });

            modelBuilder.Entity("ToolTemp.WPF.Models.Device", b =>
                {
                    b.Property<string>("DevId")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasColumnName("devid");

                    b.Property<int>("ActiveId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("TypeId")
                        .HasColumnType("int");

                    b.HasKey("DevId");

                    b.ToTable("devices", (string)null);
                });

            modelBuilder.Entity("ToolTemp.WPF.Models.Factory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Address")
                        .HasColumnType("int");

                    b.Property<string>("FactoryCode")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Line")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("dv_Factory_Configs", (string)null);
                });

            modelBuilder.Entity("ToolTemp.WPF.Models.Style", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal?>("CompensateVaild")
                        .HasColumnType("decimal(18,6)")
                        .HasColumnName("Compensate_Vaild");

                    b.Property<string>("Devid")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<decimal>("Max")
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("Min")
                        .HasColumnType("decimal(18,6)");

                    b.Property<string>("NameStyle")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("StandardTemp")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("Standard_temp");

                    b.HasKey("Id");

                    b.ToTable("dv_style", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}