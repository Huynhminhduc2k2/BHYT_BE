﻿// <auto-generated />
using System;
using BHYT_BE.Internal.Repository.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BHYT_BE.Migrations.InsurancePaymentHistoryDB
{
    [DbContext(typeof(InsurancePaymentHistoryDBContext))]
    partial class InsurancePaymentHistoryDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BHYT_BE.Internal.Models.InsurancePaymentHistory", b =>
                {
                    b.Property<int>("PaymentHistoryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("PaymentHistoryID"));

                    b.Property<decimal>("AmountPaid")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("InsuranceID")
                        .HasColumnType("integer");

                    b.Property<DateTime>("PaymentDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("PaymentMethod")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("PaymentHistoryID");

                    b.ToTable("InsuranceHistories");
                });
#pragma warning restore 612, 618
        }
    }
}