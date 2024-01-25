﻿// <auto-generated />
using System;
using BHYT_BE.Internal.Repository.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BHYT_BE.Migrations.InsuranceHistoryDB
{
    [DbContext(typeof(InsuranceHistoryDBContext))]
    partial class InsuranceHistoryDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BHYT_BE.Internal.Models.InsuranceHistory", b =>
                {
                    b.Property<int>("InsuranceHistoryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("InsuranceHistoryID"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<int>("InsuranceID")
                        .HasColumnType("integer");

                    b.Property<int>("NewStatus")
                        .HasColumnType("integer");

                    b.Property<int>("OldStatus")
                        .HasColumnType("integer");

                    b.Property<string>("Remark")
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.HasKey("InsuranceHistoryID");

                    b.ToTable("InsuranceHistories");
                });
#pragma warning restore 612, 618
        }
    }
}