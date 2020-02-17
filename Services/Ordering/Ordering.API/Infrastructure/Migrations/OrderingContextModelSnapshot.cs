﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Ordering.Infrastructure;

namespace Ordering.API.Infrastructure.Migrations
{
    [DbContext(typeof(OrderingContext))]
    partial class OrderingContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("Relational:Sequence:.buyerseq", "'buyerseq', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("Relational:Sequence:.orderitemseq", "'orderitemseq', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("Relational:Sequence:.orderseq", "'orderseq', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("Relational:Sequence:.paymentmethodseq", "'paymentmethodseq', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Ordering.Domain.AggregatesModel.BuyerAggregate.Buyer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "buyerseq")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<string>("IdentityGuid")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("IdentityGuid")
                        .IsUnique();

                    b.ToTable("Buyers");
                });

            modelBuilder.Entity("Ordering.Domain.AggregatesModel.BuyerAggregate.PaymentMethod", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "paymentmethodseq")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<int>("BuyerId");

                    b.Property<string>("CardHolderName")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("CardNumber")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.Property<int>("CardType");

                    b.Property<DateTime>("Expiration");

                    b.HasKey("Id");

                    b.HasIndex("BuyerId");

                    b.ToTable("PaymentMethods");
                });

            modelBuilder.Entity("Ordering.Domain.AggregatesModel.OrderAggregate.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "orderseq")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<int?>("BuyerId");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Currency")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue("NZD");

                    b.Property<decimal>("CurrencyRate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(18, 8)")
                        .HasDefaultValue(1m);

                    b.Property<int?>("PaymentMethodId");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("BuyerId");

                    b.HasIndex("PaymentMethodId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Ordering.Domain.AggregatesModel.OrderAggregate.OrderItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "orderitemseq")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<string>("ISBN13");

                    b.Property<int>("OrderId");

                    b.Property<int>("ProductId");

                    b.Property<string>("ProductName");

                    b.Property<decimal>("UnitPrice");

                    b.Property<int>("Units");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("Ordering.Infrastructure.Idempotency.ClientRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<DateTime>("Time");

                    b.HasKey("Id");

                    b.ToTable("requests");
                });

            modelBuilder.Entity("Ordering.Domain.AggregatesModel.BuyerAggregate.PaymentMethod", b =>
                {
                    b.HasOne("Ordering.Domain.AggregatesModel.BuyerAggregate.Buyer")
                        .WithMany("PaymentMethods")
                        .HasForeignKey("BuyerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Ordering.Domain.AggregatesModel.OrderAggregate.Order", b =>
                {
                    b.HasOne("Ordering.Domain.AggregatesModel.BuyerAggregate.Buyer")
                        .WithMany()
                        .HasForeignKey("BuyerId");

                    b.HasOne("Ordering.Domain.AggregatesModel.BuyerAggregate.PaymentMethod")
                        .WithMany()
                        .HasForeignKey("PaymentMethodId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.OwnsOne("Ordering.Domain.AggregatesModel.OrderAggregate.Address", "Address", b1 =>
                        {
                            b1.Property<int>("OrderId");

                            b1.Property<string>("City");

                            b1.Property<string>("Country");

                            b1.Property<string>("State");

                            b1.Property<string>("Street");

                            b1.Property<string>("ZipCode");

                            b1.HasKey("OrderId");

                            b1.ToTable("Orders");

                            b1.HasOne("Ordering.Domain.AggregatesModel.OrderAggregate.Order")
                                .WithOne("Address")
                                .HasForeignKey("Ordering.Domain.AggregatesModel.OrderAggregate.Address", "OrderId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("Ordering.Domain.AggregatesModel.OrderAggregate.OrderItem", b =>
                {
                    b.HasOne("Ordering.Domain.AggregatesModel.OrderAggregate.Order")
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
