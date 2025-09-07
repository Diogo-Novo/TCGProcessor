using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using TCGProcessor.Models;
namespace TCGProcessor.Data;

public partial class OSMGXTCGSystemDbContext : DbContext
{
    public OSMGXTCGSystemDbContext()
    {
    }

    public OSMGXTCGSystemDbContext(DbContextOptions<OSMGXTCGSystemDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TcgmJobQueue> TcgmJobQueues { get; set; }

    public virtual DbSet<TcgmJobQueueDatum> TcgmJobQueueData { get; set; }

    public virtual DbSet<TcgmMtgCard> TcgmMtgCards { get; set; }

    public virtual DbSet<TcgmMtgCardSet> TcgmMtgCardSets { get; set; }

    public virtual DbSet<TcgmMtgImportConfig> TcgmMtgImportConfigs { get; set; }

    public virtual DbSet<TcgmMtgMarketPriceHistory> TcgmMtgMarketPriceHistories { get; set; }

    public virtual DbSet<TcgmMtgSale> TcgmMtgSales { get; set; }

    public virtual DbSet<TcgmMtgStockList> TcgmMtgStockLists { get; set; }

    public virtual DbSet<TcgmPkmCard> TcgmPkmCards { get; set; }

    public virtual DbSet<TcgmPkmCardSet> TcgmPkmCardSets { get; set; }

    public virtual DbSet<TcgmPkmMarketPriceHistory> TcgmPkmMarketPriceHistories { get; set; }

    public virtual DbSet<TcgmPkmSale> TcgmPkmSales { get; set; }

    public virtual DbSet<TcgmPkmStockList> TcgmPkmStockLists { get; set; }

    public virtual DbSet<TcgmTillBasket> TcgmTillBaskets { get; set; }

    public virtual DbSet<TcgmTillBasketItem> TcgmTillBasketItems { get; set; }

    public virtual DbSet<TcgmUploadSession> TcgmUploadSessions { get; set; }

    public virtual DbSet<TcgmUploadedFile> TcgmUploadedFiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=82.165.235.79;port=32768;user=root;password=QfEM687Rx5;database=OS_MGX_TCGManager", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.32-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<TcgmJobQueue>(entity =>
        {
            entity.HasKey(e => e.JqId).HasName("PRIMARY");

            entity.ToTable("TCGM_JobQueue");

            entity.Property(e => e.JqId).HasColumnName("JQ_Id");
            entity.Property(e => e.JqCompletedAt)
                .HasColumnType("datetime")
                .HasColumnName("JQ_CompletedAt");
            entity.Property(e => e.JqCreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("JQ_CreatedAt");
            entity.Property(e => e.JqErrorMessage)
                .HasColumnType("text")
                .HasColumnName("JQ_ErrorMessage");
            entity.Property(e => e.JqJobType)
                .HasColumnType("text")
                .HasColumnName("JQ_JobType");
            entity.Property(e => e.JqProcessedEntries).HasColumnName("JQ_ProcessedEntries");
            entity.Property(e => e.JqResultId).HasColumnName("JQ_ResultId");
            entity.Property(e => e.JqStartedAt)
                .HasColumnType("datetime")
                .HasColumnName("JQ_StartedAt");
            entity.Property(e => e.JqStatus)
                .HasMaxLength(100)
                .HasDefaultValueSql("'Pending'")
                .HasColumnName("JQ_Status");
            entity.Property(e => e.JqTotalEntries).HasColumnName("JQ_TotalEntries");
            entity.Property(e => e.JqUpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("JQ_UpdatedAt");
        });

        modelBuilder.Entity<TcgmJobQueueDatum>(entity =>
        {
            entity.HasKey(e => e.JqdJobQueueId).HasName("PRIMARY");

            entity.ToTable("TCGM_JobQueueData");

            entity.Property(e => e.JqdJobQueueId)
                .ValueGeneratedNever()
                .HasColumnName("JQD_JobQueueId");
            entity.Property(e => e.JqdData).HasColumnName("JQD_Data");

            entity.HasOne(d => d.JqdJobQueue).WithOne(p => p.TcgmJobQueueDatum)
                .HasForeignKey<TcgmJobQueueDatum>(d => d.JqdJobQueueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TCGM_JobQueueData_ibfk_1");
        });

        modelBuilder.Entity<TcgmMtgCard>(entity =>
        {
            entity.HasKey(e => e.CId).HasName("PRIMARY");

            entity.ToTable("TCGM_MTG_Cards");

            entity.HasIndex(e => e.CRarity, "idx_C_Rarity");

            entity.HasIndex(e => e.CScryfallId, "idx_C_ScryfallId");

            entity.HasIndex(e => new { e.CSetId, e.CCardNumber }, "idx_C_Set_Number");

            entity.Property(e => e.CId).HasColumnName("C_Id");
            entity.Property(e => e.CCardMarkerUrl)
                .HasMaxLength(500)
                .HasColumnName("C_CardMarkerURL");
            entity.Property(e => e.CCardName)
                .HasMaxLength(200)
                .HasColumnName("C_CardName");
            entity.Property(e => e.CCardNumber)
                .HasMaxLength(20)
                .HasColumnName("C_CardNumber");
            entity.Property(e => e.CImageUrl)
                .HasMaxLength(500)
                .HasColumnName("C_ImageURL");
            entity.Property(e => e.CIsFoil).HasColumnName("C_IsFoil");
            entity.Property(e => e.CRarity)
                .HasMaxLength(50)
                .HasColumnName("C_Rarity");
            entity.Property(e => e.CScryfallId)
                .HasMaxLength(50)
                .HasColumnName("C_ScryfallId");
            entity.Property(e => e.CSetId).HasColumnName("C_SetId");

            entity.HasOne(d => d.CSet).WithMany(p => p.TcgmMtgCards)
                .HasForeignKey(d => d.CSetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TCGM_MTG_Cards_ibfk_1");
        });

        modelBuilder.Entity<TcgmMtgCardSet>(entity =>
        {
            entity.HasKey(e => e.CsId).HasName("PRIMARY");

            entity.ToTable("TCGM_MTG_CardSet");

            entity.HasIndex(e => e.CsScryfallId, "CS_ScryfallId").IsUnique();

            entity.HasIndex(e => e.CsSetCode, "idx_CS_SetCode");

            entity.HasIndex(e => e.CsSetName, "idx_SetName").IsUnique();

            entity.Property(e => e.CsId).HasColumnName("CS_Id");
            entity.Property(e => e.CsReleaseDate).HasColumnName("CS_ReleaseDate");
            entity.Property(e => e.CsScryfallId)
                .HasMaxLength(50)
                .HasColumnName("CS_ScryfallId");
            entity.Property(e => e.CsSetCode)
                .HasMaxLength(50)
                .HasColumnName("CS_SetCode");
            entity.Property(e => e.CsSetIconSvgurl)
                .HasMaxLength(500)
                .HasColumnName("CS_SetIconSVGUrl");
            entity.Property(e => e.CsSetName)
                .HasMaxLength(100)
                .HasColumnName("CS_SetName");
        });

        modelBuilder.Entity<TcgmMtgImportConfig>(entity =>
        {
            entity.HasKey(e => e.IcId).HasName("PRIMARY");

            entity.ToTable("TCGM_MTG_ImportConfig");

            entity.Property(e => e.IcId).HasColumnName("IC_Id");
            entity.Property(e => e.IcIncludeCommon).HasColumnName("IC_IncludeCommon");
            entity.Property(e => e.IcIncludeMythic)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("IC_IncludeMythic");
            entity.Property(e => e.IcIncludeRare)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("IC_IncludeRare");
            entity.Property(e => e.IcIncludeUncommon).HasColumnName("IC_IncludeUncommon");
            entity.Property(e => e.IcMinPrice)
                .HasPrecision(10, 2)
                .HasColumnName("IC_MinPrice");
        });

        modelBuilder.Entity<TcgmMtgMarketPriceHistory>(entity =>
        {
            entity.HasKey(e => e.MphId).HasName("PRIMARY");

            entity.ToTable("TCGM_MTG_MarketPriceHistory");

            entity.HasIndex(e => new { e.MphCardId, e.MphDate }, "uq_MPH_Card_Date").IsUnique();

            entity.Property(e => e.MphId).HasColumnName("MPH_Id");
            entity.Property(e => e.MphCardId).HasColumnName("MPH_CardId");
            entity.Property(e => e.MphDate).HasColumnName("MPH_Date");
            entity.Property(e => e.MphPriceEur)
                .HasPrecision(10, 2)
                .HasColumnName("MPH_PriceEur");
            entity.Property(e => e.MphPriceEurEtched)
                .HasPrecision(10, 2)
                .HasColumnName("MPH_PriceEurEtched");
            entity.Property(e => e.MphPriceEurFoil)
                .HasPrecision(10, 2)
                .HasColumnName("MPH_PriceEurFoil");
            entity.Property(e => e.MphPriceGbp)
                .HasPrecision(10, 2)
                .HasColumnName("MPH_PriceGbp");
            entity.Property(e => e.MphPriceGbpEtched)
                .HasPrecision(10, 2)
                .HasColumnName("MPH_PriceGbpEtched");
            entity.Property(e => e.MphPriceGbpFoil)
                .HasPrecision(10, 2)
                .HasColumnName("MPH_PriceGbpFoil");
            entity.Property(e => e.MphPriceUsd)
                .HasPrecision(10, 2)
                .HasColumnName("MPH_PriceUsd");
            entity.Property(e => e.MphPriceUsdEtched)
                .HasPrecision(10, 2)
                .HasColumnName("MPH_PriceUsdEtched");
            entity.Property(e => e.MphPriceUsdFoil)
                .HasPrecision(10, 2)
                .HasColumnName("MPH_PriceUsdFoil");

            entity.HasOne(d => d.MphCard).WithMany(p => p.TcgmMtgMarketPriceHistories)
                .HasForeignKey(d => d.MphCardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TCGM_MTG_MarketPriceHistory_ibfk_1");
        });

        modelBuilder.Entity<TcgmMtgSale>(entity =>
        {
            entity.HasKey(e => e.SId).HasName("PRIMARY");

            entity.ToTable("TCGM_MTG_Sales");

            entity.HasIndex(e => e.SCardId, "S_CardId");

            entity.Property(e => e.SId).HasColumnName("S_Id");
            entity.Property(e => e.SCardId).HasColumnName("S_CardId");
            entity.Property(e => e.SQuantity).HasColumnName("S_Quantity");
            entity.Property(e => e.SSaleDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("S_SaleDate");
            entity.Property(e => e.SSalePrice)
                .HasPrecision(10, 2)
                .HasColumnName("S_SalePrice");
            entity.Property(e => e.STotalAmount)
                .HasPrecision(10, 2)
                .HasComputedColumnSql("`S_Quantity` * `S_SalePrice`", true)
                .HasColumnName("S_TotalAmount");

            entity.HasOne(d => d.SCard).WithMany(p => p.TcgmMtgSales)
                .HasForeignKey(d => d.SCardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TCGM_MTG_Sales_ibfk_1");
        });

        modelBuilder.Entity<TcgmMtgStockList>(entity =>
        {
            entity.HasKey(e => e.SlId).HasName("PRIMARY");

            entity.ToTable("TCGM_MTG_StockList");

            entity.HasIndex(e => e.SlCardId, "idx_SL_Card");

            entity.Property(e => e.SlId).HasColumnName("SL_Id");
            entity.Property(e => e.SlCardId).HasColumnName("SL_CardId");
            entity.Property(e => e.SlDateAcquired)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("SL_DateAcquired");
            entity.Property(e => e.SlPurchasePrice)
                .HasPrecision(10, 2)
                .HasColumnName("SL_PurchasePrice");

            entity.HasOne(d => d.SlCard).WithMany(p => p.TcgmMtgStockLists)
                .HasForeignKey(d => d.SlCardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TCGM_MTG_StockList_ibfk_1");
        });

        modelBuilder.Entity<TcgmPkmCard>(entity =>
        {
            entity.HasKey(e => e.CId).HasName("PRIMARY");

            entity.ToTable("TCGM_PKM_Cards");

            entity.HasIndex(e => e.CRarity, "idx_C_Rarity");

            entity.HasIndex(e => new { e.CSetId, e.CCardNumber }, "idx_C_Set_Number");

            entity.Property(e => e.CId).HasColumnName("C_Id");
            entity.Property(e => e.CCardName)
                .HasMaxLength(200)
                .HasColumnName("C_CardName");
            entity.Property(e => e.CCardNumber)
                .HasMaxLength(20)
                .HasColumnName("C_CardNumber");
            entity.Property(e => e.CImageUrl)
                .HasMaxLength(500)
                .HasColumnName("C_ImageURL");
            entity.Property(e => e.CRarity)
                .HasMaxLength(50)
                .HasColumnName("C_Rarity");
            entity.Property(e => e.CSetId).HasColumnName("C_SetId");

            entity.HasOne(d => d.CSet).WithMany(p => p.TcgmPkmCards)
                .HasForeignKey(d => d.CSetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TCGM_PKM_Cards_ibfk_1");
        });

        modelBuilder.Entity<TcgmPkmCardSet>(entity =>
        {
            entity.HasKey(e => e.CsId).HasName("PRIMARY");

            entity.ToTable("TCGM_PKM_CardSet");

            entity.HasIndex(e => e.CsSetCode, "idx_CS_SetCode");

            entity.HasIndex(e => e.CsSetName, "idx_SetName").IsUnique();

            entity.Property(e => e.CsId).HasColumnName("CS_Id");
            entity.Property(e => e.CsReleaseDate).HasColumnName("CS_ReleaseDate");
            entity.Property(e => e.CsSetCode)
                .HasMaxLength(50)
                .HasColumnName("CS_SetCode");
            entity.Property(e => e.CsSetName)
                .HasMaxLength(100)
                .HasColumnName("CS_SetName");
        });

        modelBuilder.Entity<TcgmPkmMarketPriceHistory>(entity =>
        {
            entity.HasKey(e => e.MphId).HasName("PRIMARY");

            entity.ToTable("TCGM_PKM_MarketPriceHistory");

            entity.HasIndex(e => new { e.MphCardId, e.MphDate }, "uq_MPH_Card_Date").IsUnique();

            entity.Property(e => e.MphId).HasColumnName("MPH_Id");
            entity.Property(e => e.MphCardId).HasColumnName("MPH_CardId");
            entity.Property(e => e.MphDate).HasColumnName("MPH_Date");
            entity.Property(e => e.MphPrice)
                .HasPrecision(10, 2)
                .HasColumnName("MPH_Price");

            entity.HasOne(d => d.MphCard).WithMany(p => p.TcgmPkmMarketPriceHistories)
                .HasForeignKey(d => d.MphCardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TCGM_PKM_MarketPriceHistory_ibfk_1");
        });

        modelBuilder.Entity<TcgmPkmSale>(entity =>
        {
            entity.HasKey(e => e.SId).HasName("PRIMARY");

            entity.ToTable("TCGM_PKM_Sales");

            entity.HasIndex(e => e.SCardId, "S_CardId");

            entity.Property(e => e.SId).HasColumnName("S_Id");
            entity.Property(e => e.SCardId).HasColumnName("S_CardId");
            entity.Property(e => e.SQuantity).HasColumnName("S_Quantity");
            entity.Property(e => e.SSaleDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("S_SaleDate");
            entity.Property(e => e.SSalePrice)
                .HasPrecision(10, 2)
                .HasColumnName("S_SalePrice");
            entity.Property(e => e.STotalAmount)
                .HasPrecision(10, 2)
                .HasComputedColumnSql("`S_Quantity` * `S_SalePrice`", true)
                .HasColumnName("S_TotalAmount");

            entity.HasOne(d => d.SCard).WithMany(p => p.TcgmPkmSales)
                .HasForeignKey(d => d.SCardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TCGM_PKM_Sales_ibfk_1");
        });

        modelBuilder.Entity<TcgmPkmStockList>(entity =>
        {
            entity.HasKey(e => e.SlId).HasName("PRIMARY");

            entity.ToTable("TCGM_PKM_StockList");

            entity.HasIndex(e => e.SlCardId, "idx_SL_Card");

            entity.Property(e => e.SlId).HasColumnName("SL_Id");
            entity.Property(e => e.SlCardId).HasColumnName("SL_CardId");
            entity.Property(e => e.SlDateAcquired)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("SL_DateAcquired");
            entity.Property(e => e.SlPurchasePriceEur)
                .HasPrecision(10, 2)
                .HasColumnName("SL_PurchasePriceEur");
            entity.Property(e => e.SlPurchasePriceGbp)
                .HasPrecision(10, 2)
                .HasColumnName("SL_PurchasePriceGbp");
            entity.Property(e => e.SlQuantity).HasColumnName("SL_Quantity");

            entity.HasOne(d => d.SlCard).WithMany(p => p.TcgmPkmStockLists)
                .HasForeignKey(d => d.SlCardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TCGM_PKM_StockList_ibfk_1");
        });

        modelBuilder.Entity<TcgmTillBasket>(entity =>
        {
            entity.HasKey(e => e.BId).HasName("PRIMARY");

            entity.ToTable("TCGM_Till_Baskets");

            entity.Property(e => e.BId).HasColumnName("B_Id");
            entity.Property(e => e.BClosedAt)
                .HasColumnType("datetime")
                .HasColumnName("B_ClosedAt");
            entity.Property(e => e.BClosedBy).HasColumnName("B_ClosedBy");
            entity.Property(e => e.BCreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("B_CreatedAt");
            entity.Property(e => e.BCreatedBy).HasColumnName("B_CreatedBy");
            entity.Property(e => e.BNotes)
                .HasColumnType("text")
                .HasColumnName("B_Notes");
            entity.Property(e => e.BStatus)
                .HasMaxLength(100)
                .HasDefaultValueSql("'Open'")
                .HasColumnName("B_Status");
            entity.Property(e => e.BTotalAmount)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("B_TotalAmount");
            entity.Property(e => e.BUpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("B_UpdatedAt");
        });

        modelBuilder.Entity<TcgmTillBasketItem>(entity =>
        {
            entity.HasKey(e => e.BiId).HasName("PRIMARY");

            entity.ToTable("TCGM_Till_BasketItems");

            entity.HasIndex(e => e.BiBasketId, "idx_BI_Basket");

            entity.Property(e => e.BiId).HasColumnName("BI_Id");
            entity.Property(e => e.BiBasketId).HasColumnName("BI_BasketId");
            entity.Property(e => e.BiCardId).HasColumnName("BI_CardId");
            entity.Property(e => e.BiGame)
                .HasColumnType("enum('MTG','PKM')")
                .HasColumnName("BI_Game");
            entity.Property(e => e.BiPrice)
                .HasPrecision(10, 2)
                .HasColumnName("BI_Price");
            entity.Property(e => e.BiQuantity)
                .HasDefaultValueSql("'1'")
                .HasColumnName("BI_Quantity");

            entity.HasOne(d => d.BiBasket).WithMany(p => p.TcgmTillBasketItems)
                .HasForeignKey(d => d.BiBasketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TCGM_Till_BasketItems_ibfk_1");
        });

        modelBuilder.Entity<TcgmUploadSession>(entity =>
        {
            entity.HasKey(e => e.UsId).HasName("PRIMARY");

            entity.ToTable("TCGM_UploadSession");

            entity.HasIndex(e => e.UsUniqueKey, "UK_UploadSession_Key").IsUnique();

            entity.Property(e => e.UsId).HasColumnName("US_Id");
            entity.Property(e => e.UsAttemptCount)
                .HasDefaultValueSql("'0'")
                .HasColumnName("US_AttemptCount");
            entity.Property(e => e.UsCompletedAt)
                .HasColumnType("datetime")
                .HasColumnName("US_CompletedAt");
            entity.Property(e => e.UsCreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("US_CreatedAt");
            entity.Property(e => e.UsExpiresAt)
                .HasColumnType("datetime")
                .HasColumnName("US_ExpiresAt");
            entity.Property(e => e.UsIsCompleted)
                .HasDefaultValueSql("'0'")
                .HasColumnName("US_IsCompleted");
            entity.Property(e => e.UsProcessedBy).HasColumnName("US_ProcessedBy");
            entity.Property(e => e.UsTwoFacode)
                .HasMaxLength(6)
                .HasColumnName("US_TwoFACode");
            entity.Property(e => e.UsUniqueKey)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("US_UniqueKey");
        });

        modelBuilder.Entity<TcgmUploadedFile>(entity =>
        {
            entity.HasKey(e => e.UfId).HasName("PRIMARY");

            entity.ToTable("TCGM_UploadedFile");

            entity.HasIndex(e => e.UfUploadSessionId, "UF_UploadSessionId");

            entity.Property(e => e.UfId).HasColumnName("UF_Id");
            entity.Property(e => e.UfBase64Data).HasColumnName("UF_Base64Data");
            entity.Property(e => e.UfFriendlyName)
                .HasMaxLength(100)
                .HasColumnName("UF_FriendlyName");
            entity.Property(e => e.UfUploadSessionId).HasColumnName("UF_UploadSessionId");
            entity.Property(e => e.UfUploadedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("UF_UploadedAt");
            entity.Property(e => e.UfValidityExpiryDate)
                .HasColumnType("datetime")
                .HasColumnName("UF_ValidityExpiryDate");

            entity.HasOne(d => d.UfUploadSession).WithMany(p => p.TcgmUploadedFiles)
                .HasForeignKey(d => d.UfUploadSessionId)
                .HasConstraintName("TCGM_UploadedFile_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
