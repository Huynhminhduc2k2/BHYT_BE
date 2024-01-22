﻿// <auto-generated />
using System;
using BHYT_BE.Internal.Repository.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BHYT_BE.Migrations.InsuranceRequestDB
{
    [DbContext(typeof(InsuranceRequestDBContext))]
    partial class InsuranceRequestDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BHYT_BE.Internal.Models.InsuranceRequest", b =>
                {
                    b.Property<int>("InsuranceRequestID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("InsuranceRequestID"));

                    b.Property<DateTime?>("ApprovalDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("InsuranceID")
                        .HasColumnType("integer");

                    b.Property<bool?>("IsApproved")
                        .HasColumnType("boolean");

                    b.Property<string>("MedicalRecord")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RejectionReason")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("RequestDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("RequestedAmount")
                        .HasColumnType("numeric");

                    b.Property<string>("RequestedBy")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("InsuranceRequestID");

                    b.ToTable("InsuranceRequests");
                });
#pragma warning restore 612, 618
        }
    }
}
