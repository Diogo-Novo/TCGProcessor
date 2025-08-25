using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using TCGProcessor.Models;

namespace TCGProcessor.Data;

public partial class OsMgxPricingSystemContext : DbContext
{
    public OsMgxPricingSystemContext()
    {
    }

    public OsMgxPricingSystemContext(DbContextOptions<OsMgxPricingSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<PsPricingSheet> PsPricingSheets { get; set; }

    public virtual DbSet<PsPricingSheetItem> PsPricingSheetItems { get; set; }

    public virtual DbSet<PsPricingSheetStatus> PsPricingSheetStatuses { get; set; }

    public virtual DbSet<PsTemporaryPricingItem> PsTemporaryPricingItems { get; set; }

    public virtual DbSet<PsTemporaryPricingItemGroup> PsTemporaryPricingItemGroups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<PsPricingSheet>(entity =>
        {
            entity.HasKey(e => e.PsId).HasName("PRIMARY");

            entity
                .ToTable("PS_PricingSheet")
                .HasCharSet("latin1")
                .UseCollation("latin1_swedish_ci");

            entity.HasIndex(e => e.PsSheetStatus, "FK_PS_Status_idx");

            entity.Property(e => e.PsId).HasColumnName("PS_Id");
            entity.Property(e => e.PsActualAmountPaid)
                .HasPrecision(10, 2)
                .HasColumnName("PS_ActualAmountPaid");
            entity.Property(e => e.PsCreatedBy).HasColumnName("PS_CreatedBy");
            entity.Property(e => e.PsCreatedIn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("PS_CreatedIn");
            entity.Property(e => e.PsIsCash).HasColumnName("PS_IsCash");
            entity.Property(e => e.PsItemCount).HasColumnName("PS_ItemCount");
            entity.Property(e => e.PsLastUpdated)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("PS_LastUpdated");
            entity.Property(e => e.PsQuickAccessReference)
                .HasMaxLength(50)
                .HasColumnName("PS_QuickAccessReference");
            entity.Property(e => e.PsSheetName)
                .HasMaxLength(100)
                .HasColumnName("PS_SheetName");
            entity.Property(e => e.PsSheetNotes)
                .HasMaxLength(500)
                .HasColumnName("PS_SheetNotes");
            entity.Property(e => e.PsSheetStatus).HasColumnName("PS_SheetStatus");
            entity.Property(e => e.PsTotalCashValue)
                .HasPrecision(10, 2)
                .HasColumnName("PS_TotalCashValue");
            entity.Property(e => e.PsTotalSellValue)
                .HasPrecision(10, 2)
                .HasColumnName("PS_TotalSellValue");
            entity.Property(e => e.PsTotalTradeValue)
                .HasPrecision(10, 2)
                .HasColumnName("PS_TotalTradeValue");

            entity.HasOne(d => d.PsSheetStatusNavigation).WithMany(p => p.PsPricingSheets)
                .HasForeignKey(d => d.PsSheetStatus)
                .HasConstraintName("FK_PS_Status");
        });

        modelBuilder.Entity<PsPricingSheetItem>(entity =>
        {
            entity.HasKey(e => e.PsiId).HasName("PRIMARY");

            entity
                .ToTable("PS_PricingSheetItems")
                .HasCharSet("latin1")
                .UseCollation("latin1_swedish_ci");

            entity.HasIndex(e => e.PsiPricingSheet, "FK_PSI_Sheet_idx");

            entity.Property(e => e.PsiId).HasColumnName("PSI_Id");
            entity.Property(e => e.PsiCashValue)
                .HasPrecision(10, 2)
                .HasColumnName("PSI_CashValue");
            entity.Property(e => e.PsiCreatedBy).HasColumnName("PSI_CreatedBy");
            entity.Property(e => e.PsiItemName)
                .HasMaxLength(100)
                .HasColumnName("PSI_ItemName");
            entity.Property(e => e.PsiItemNotes)
                .HasMaxLength(500)
                .HasColumnName("PSI_ItemNotes");
            entity.Property(e => e.PsiPricingSheet).HasColumnName("PSI_PricingSheet");
            entity.Property(e => e.PsiSaleValue)
                .HasPrecision(10, 2)
                .HasColumnName("PSI_SaleValue");
            entity.Property(e => e.PsiSerialNumber)
                .HasMaxLength(80)
                .HasColumnName("PSI_SerialNumber");
            entity.Property(e => e.PsiTradeValue)
                .HasPrecision(10, 2)
                .HasColumnName("PSI_TradeValue");
            entity.Property(e => e.PsiWeBuyReference)
                .HasMaxLength(100)
                .HasColumnName("PSI_WeBuyReference");

            entity.HasOne(d => d.PsiPricingSheetNavigation).WithMany(p => p.PsPricingSheetItems)
                .HasForeignKey(d => d.PsiPricingSheet)
                .HasConstraintName("FK_PSI_Sheet");
        });

        modelBuilder.Entity<PsPricingSheetStatus>(entity =>
        {
            entity.HasKey(e => e.PssId).HasName("PRIMARY");

            entity.ToTable("PS_PricingSheetStatus");

            entity.Property(e => e.PssId).HasColumnName("PSS_Id");
            entity.Property(e => e.PssIsFinalized).HasColumnName("PSS_IsFinalized");
            entity.Property(e => e.PssStatusName)
                .HasMaxLength(100)
                .HasColumnName("PSS_StatusName");
        });

        modelBuilder.Entity<PsTemporaryPricingItem>(entity =>
        {
            entity.HasKey(e => e.TpiId).HasName("PRIMARY");

            entity.ToTable("PS_TemporaryPricingItems");

            entity.HasIndex(e => e.TpiGroupId, "TPI_GroupID_FK_idx");

            entity.Property(e => e.TpiId).HasColumnName("TPI_Id");
            entity.Property(e => e.TpiCashPrice)
                .HasPrecision(10, 2)
                .HasColumnName("TPI_CashPrice");
            entity.Property(e => e.TpiCreatedIn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("TPI_CreatedIn");
            entity.Property(e => e.TpiGroupId).HasColumnName("TPI_GroupId");
            entity.Property(e => e.TpiImported)
                .HasDefaultValueSql("'0'")
                .HasColumnName("TPI_Imported");
            entity.Property(e => e.TpiProductCategoryName)
                .HasMaxLength(250)
                .HasColumnName("TPI_ProductCategoryName");
            entity.Property(e => e.TpiProductName)
                .HasMaxLength(250)
                .HasColumnName("TPI_ProductName");
            entity.Property(e => e.TpiSellPrice)
                .HasPrecision(10, 2)
                .HasColumnName("TPI_SellPrice");
            entity.Property(e => e.TpiTradePrice)
                .HasPrecision(10, 2)
                .HasColumnName("TPI_TradePrice");
            entity.Property(e => e.TpiWeBuyId)
                .HasMaxLength(250)
                .HasColumnName("TPI_WeBuyId");

            entity.HasOne(d => d.TpiGroup).WithMany(p => p.PsTemporaryPricingItems)
                .HasForeignKey(d => d.TpiGroupId)
                .HasConstraintName("TPI_GroupID_FK");
        });

        modelBuilder.Entity<PsTemporaryPricingItemGroup>(entity =>
        {
            entity.HasKey(e => e.TpigId).HasName("PRIMARY");

            entity.ToTable("PS_TemporaryPricingItemGroup");

            entity.Property(e => e.TpigId).HasColumnName("TPIG_Id");
            entity.Property(e => e.TpigCreatedIn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("TPIG_CreatedIn");
            entity.Property(e => e.TpigImported)
                .HasDefaultValueSql("'0'")
                .HasColumnName("TPIG_Imported");
            entity.Property(e => e.TpigItemCount).HasColumnName("TPIG_ItemCount");
            entity.Property(e => e.TpigTotalCashValue)
                .HasPrecision(10, 2)
                .HasColumnName("TPIG_TotalCashValue");
            entity.Property(e => e.TpigTotalSellValue)
                .HasPrecision(10, 2)
                .HasColumnName("TPIG_TotalSellValue");
            entity.Property(e => e.TpigTotalTradeValue)
                .HasPrecision(10, 2)
                .HasColumnName("TPIG_TotalTradeValue");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
