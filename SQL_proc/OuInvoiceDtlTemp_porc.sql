  
  
  if exists (select * from dbo.sysobjects where id = object_id(N'dbo.OuInvoiceDtlTemp_Delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure dbo.OuInvoiceDtlTemp_Delete
  
  if exists (select * from dbo.sysobjects where id = object_id(N'dbo.OuInvoiceDtlTemp_DeleteDynamic') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure dbo.OuInvoiceDtlTemp_DeleteDynamic
  
  if exists (select * from dbo.sysobjects where id = object_id(N'dbo.OuInvoiceDtlTemp_GetTopN') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure dbo.OuInvoiceDtlTemp_GetTopN
  
  if exists (select * from dbo.sysobjects where id = object_id(N'dbo.OuInvoiceDtlTemp_Insert') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure dbo.OuInvoiceDtlTemp_Insert
  
  if exists (select * from dbo.sysobjects where id = object_id(N'dbo.OuInvoiceDtlTemp_InsertUpdate') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure dbo.OuInvoiceDtlTemp_InsertUpdate
  
  if exists (select * from dbo.sysobjects where id = object_id(N'dbo.OuInvoiceDtlTemp_Update') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure dbo.OuInvoiceDtlTemp_Update
  
  if exists (select * from dbo.sysobjects where id = object_id(N'dbo.OuInvoiceDtlTemp_Select') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure dbo.OuInvoiceDtlTemp_Select
  
  if exists (select * from dbo.sysobjects where id = object_id(N'dbo.OuInvoiceDtlTemp_SelectAll') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure dbo.OuInvoiceDtlTemp_SelectAll
  
  if exists (select * from dbo.sysobjects where id = object_id(N'dbo.OuInvoiceDtlTemp_SelectDynamic') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure dbo.OuInvoiceDtlTemp_SelectDynamic
  
  if exists (select * from dbo.sysobjects where id = object_id(N'dbo.OuInvoiceDtlTemp_DeleteByMzregid') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure dbo.OuInvoiceDtlTemp_DeleteByMzregid
  
  if exists (select * from dbo.sysobjects where id = object_id(N'dbo.OuInvoiceDtlTemp_SelectByMzregid') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure dbo.OuInvoiceDtlTemp_SelectByMzregid
  
  GO  
  --region [dbo].[OuInvoiceDtlTemp_Delete] 
   
  ------------------------------------------------------------------------------------------------------------------------ 
  -- Generated By:   using Enivision 1.2.0.0 
  -- Template:       New Enivision 1.2  
  -- Procedure Name: [dbo].[OuInvoiceDtlTemp_Delete] 
  -- Date Generated: 2016-6-29 12:59:46 
  ------------------------------------------------------------------------------------------------------------------------ 
   
  CREATE PROCEDURE [dbo].[OuInvoiceDtlTemp_Delete] 
  	@ID int 
  AS 
   
  SET NOCOUNT ON 
   
  DELETE FROM 
      [dbo].[OuInvoiceDtlTemp] 
  WHERE 
   [ID] = @ID 
   
  --endregion 
   
   
   
  GO 
   
  --region [dbo].[OuInvoiceDtlTemp_DeleteDynamic]  
    
  ------------------------------------------------------------------------------------------------------------------------  
  -- Generated By:   using Enivision 1.2.0.0   
  -- Template:       New Enivision 1.2  
  -- Procedure Name: [dbo].[OuInvoiceDtlTemp_DeleteDynamic]  
  -- Date Generated: 2016-6-29 12:59:46  
  ------------------------------------------------------------------------------------------------------------------------  
    
  CREATE PROCEDURE [dbo].[OuInvoiceDtlTemp_DeleteDynamic]  
  	@WhereCondition nvarchar(500)  
  AS  
    
  SET NOCOUNT ON  
    
  DECLARE @SQL nvarchar(3250)  
    
  SET @SQL = '  
  DELETE FROM  
  	[dbo].[OuInvoiceDtlTemp]  
  WHERE  
  	' + @WhereCondition  
    
  EXEC SP_EXECUTESQL @SQL  
    
  --endregion  
    
  GO  
    
    
  --region [dbo].[OuInvoiceDtlTemp_Insert]  
    
  ------------------------------------------------------------------------------------------------------------------------  
  -- Generated By:   using Enivision 1.2.0.0   
  -- Template:       New Enivision 1.2  
  -- Procedure Name: [dbo].[OuInvoiceDtlTemp_Insert]  
  -- Date Generated: 2016-6-29 12:59:46  
  ------------------------------------------------------------------------------------------------------------------------  
    
  CREATE PROCEDURE [dbo].[OuInvoiceDtlTemp_Insert]  
      @Amount decimal(18,2),  
      @AmountFact decimal(18,2),  
      @AmountInsurance decimal(18,2),  
      @AmountPay decimal(18,2),  
      @AmountSelf decimal(18,2),  
      @AmountTally decimal(18,2),  
      @Code nvarchar(50),  
      @DiscDiag decimal(18,2),  
      @DiscSelf decimal(18,2),  
      @DocLevId int,  
      @DoctorId int,  
      @DoctorName nvarchar(50),  
      @ExecLocId int,  
      @F1 nvarchar(50),  
      @F2 nvarchar(50),  
      @F3 nvarchar(50),  
      @F4 nvarchar(50),  
      @F5 nvarchar(50),  
      @FeeHsId int,  
      @FeeHsMzName nvarchar(50),  
      @FeeId int,  
      @FeeName nvarchar(50),  
      @InvItemId int,  
      @InvMzItemName nvarchar(50),  
      @InvoId int,  
      @InvoTime datetime,  
      @IsBack bit,  
      @IsDoctorInput bit,  
      @IsIssue bit,  
      @IsModiDisc bit,  
      @IsOtherFee bit,  
      @IsPriority bit,  
      @IsToBack bit,  
      @ItemId int,  
      @LimitFee decimal(18,2),  
      @LimitGroupId int,  
      @LimitTotalMz decimal(18,2),  
      @ListNum int,  
      @LocationName nvarchar(50),  
      @LsAdviceType int,  
      @LsGfType int,  
      @LsGroupType int,  
      @LsPerform int,  
      @LsReportType int,  
      @LsRepType int,  
      @LsRpType int,  
      @Memo nvarchar(50),  
      @Name nvarchar(50),  
      @PrepareTime nvarchar(50),  
      @Price decimal(18,2),  
      @RecipeGroupId int,  
      @RecipeGroupIds nvarchar(50),  
      @RecipeId int,  
      @RecipeItemId int,  
      @RoomId int,  
      @Spec nvarchar(50),  
      @Totality decimal(18,2),  
      @TypeGFXEId int,  
      @UnitDiagName nvarchar(50),  
      @UnitId int,  
      @XDRpId int,  
      @Mzregid int,  
      @OperTime datetime,  
  	@ID int OUTPUT  
  AS  
    
  SET NOCOUNT ON  
  IF @InvoTime <= CAST('1900-1-1' AS SMALLDATETIME)  
     SET @InvoTime = NULL  
  IF @OperTime <= CAST('1900-1-1' AS SMALLDATETIME)  
     SET @OperTime = NULL  
    
    
    
  INSERT INTO [dbo].[OuInvoiceDtlTemp] (  
      [Amount],  
      [AmountFact],  
      [AmountInsurance],  
      [AmountPay],  
      [AmountSelf],  
      [AmountTally],  
      [Code],  
      [DiscDiag],  
      [DiscSelf],  
      [DocLevId],  
      [DoctorId],  
      [DoctorName],  
      [ExecLocId],  
      [F1],  
      [F2],  
      [F3],  
      [F4],  
      [F5],  
      [FeeHsId],  
      [FeeHsMzName],  
      [FeeId],  
      [FeeName],  
      [InvItemId],  
      [InvMzItemName],  
      [InvoId],  
      [InvoTime],  
      [IsBack],  
      [IsDoctorInput],  
      [IsIssue],  
      [IsModiDisc],  
      [IsOtherFee],  
      [IsPriority],  
      [IsToBack],  
      [ItemId],  
      [LimitFee],  
      [LimitGroupId],  
      [LimitTotalMz],  
      [ListNum],  
      [LocationName],  
      [LsAdviceType],  
      [LsGfType],  
      [LsGroupType],  
      [LsPerform],  
      [LsReportType],  
      [LsRepType],  
      [LsRpType],  
      [Memo],  
      [Name],  
      [PrepareTime],  
      [Price],  
      [RecipeGroupId],  
      [RecipeGroupIds],  
      [RecipeId],  
      [RecipeItemId],  
      [RoomId],  
      [Spec],  
      [Totality],  
      [TypeGFXEId],  
      [UnitDiagName],  
      [UnitId],  
      [XDRpId],  
      [Mzregid],  
      [OperTime]  
  ) VALUES (  
      @Amount,  
      @AmountFact,  
      @AmountInsurance,  
      @AmountPay,  
      @AmountSelf,  
      @AmountTally,  
      @Code,  
      @DiscDiag,  
      @DiscSelf,  
      @DocLevId,  
      @DoctorId,  
      @DoctorName,  
      @ExecLocId,  
      @F1,  
      @F2,  
      @F3,  
      @F4,  
      @F5,  
      @FeeHsId,  
      @FeeHsMzName,  
      @FeeId,  
      @FeeName,  
      @InvItemId,  
      @InvMzItemName,  
      @InvoId,  
      @InvoTime,  
      @IsBack,  
      @IsDoctorInput,  
      @IsIssue,  
      @IsModiDisc,  
      @IsOtherFee,  
      @IsPriority,  
      @IsToBack,  
      @ItemId,  
      @LimitFee,  
      @LimitGroupId,  
      @LimitTotalMz,  
      @ListNum,  
      @LocationName,  
      @LsAdviceType,  
      @LsGfType,  
      @LsGroupType,  
      @LsPerform,  
      @LsReportType,  
      @LsRepType,  
      @LsRpType,  
      @Memo,  
      @Name,  
      @PrepareTime,  
      @Price,  
      @RecipeGroupId,  
      @RecipeGroupIds,  
      @RecipeId,  
      @RecipeItemId,  
      @RoomId,  
      @Spec,  
      @Totality,  
      @TypeGFXEId,  
      @UnitDiagName,  
      @UnitId,  
      @XDRpId,  
      @Mzregid,  
      @OperTime  
  )  
    
  SET @ID = SCOPE_IDENTITY()  
    
  --endregion  
    
  GO  
    
    
    
  --region [dbo].[OuInvoiceDtlTemp_Update]  
    
  ------------------------------------------------------------------------------------------------------------------------  
  -- Generated By:   using Enivision 1.2.0.0   
  -- Template:       New Enivision 1.2  
  -- Procedure Name: [dbo].[OuInvoiceDtlTemp_Update]  
  -- Date Generated: 2016-6-29 12:59:46  
  ------------------------------------------------------------------------------------------------------------------------  
    
  CREATE PROCEDURE [dbo].[OuInvoiceDtlTemp_Update]  
      @ID int ,  
      @Amount decimal(18,2),  
      @AmountFact decimal(18,2),  
      @AmountInsurance decimal(18,2),  
      @AmountPay decimal(18,2),  
      @AmountSelf decimal(18,2),  
      @AmountTally decimal(18,2),  
      @Code nvarchar(50),  
      @DiscDiag decimal(18,2),  
      @DiscSelf decimal(18,2),  
      @DocLevId int,  
      @DoctorId int,  
      @DoctorName nvarchar(50),  
      @ExecLocId int,  
      @F1 nvarchar(50),  
      @F2 nvarchar(50),  
      @F3 nvarchar(50),  
      @F4 nvarchar(50),  
      @F5 nvarchar(50),  
      @FeeHsId int,  
      @FeeHsMzName nvarchar(50),  
      @FeeId int,  
      @FeeName nvarchar(50),  
      @InvItemId int,  
      @InvMzItemName nvarchar(50),  
      @InvoId int,  
      @InvoTime datetime,  
      @IsBack bit,  
      @IsDoctorInput bit,  
      @IsIssue bit,  
      @IsModiDisc bit,  
      @IsOtherFee bit,  
      @IsPriority bit,  
      @IsToBack bit,  
      @ItemId int,  
      @LimitFee decimal(18,2),  
      @LimitGroupId int,  
      @LimitTotalMz decimal(18,2),  
      @ListNum int,  
      @LocationName nvarchar(50),  
      @LsAdviceType int,  
      @LsGfType int,  
      @LsGroupType int,  
      @LsPerform int,  
      @LsReportType int,  
      @LsRepType int,  
      @LsRpType int,  
      @Memo nvarchar(50),  
      @Name nvarchar(50),  
      @PrepareTime nvarchar(50),  
      @Price decimal(18,2),  
      @RecipeGroupId int,  
      @RecipeGroupIds nvarchar(50),  
      @RecipeId int,  
      @RecipeItemId int,  
      @RoomId int,  
      @Spec nvarchar(50),  
      @Totality decimal(18,2),  
      @TypeGFXEId int,  
      @UnitDiagName nvarchar(50),  
      @UnitId int,  
      @XDRpId int,  
      @Mzregid int,  
      @OperTime datetime  
  AS  
    
  SET NOCOUNT ON  
    
  IF @InvoTime <= CAST('1900-1-1' AS SMALLDATETIME)  
     SET @InvoTime = NULL  
  IF @OperTime <= CAST('1900-1-1' AS SMALLDATETIME)  
     SET @OperTime = NULL  
    
    
    
    
  UPDATE [dbo].[OuInvoiceDtlTemp] SET  
      [Amount] = @Amount,  
      [AmountFact] = @AmountFact,  
      [AmountInsurance] = @AmountInsurance,  
      [AmountPay] = @AmountPay,  
      [AmountSelf] = @AmountSelf,  
      [AmountTally] = @AmountTally,  
      [Code] = @Code,  
      [DiscDiag] = @DiscDiag,  
      [DiscSelf] = @DiscSelf,  
      [DocLevId] = @DocLevId,  
      [DoctorId] = @DoctorId,  
      [DoctorName] = @DoctorName,  
      [ExecLocId] = @ExecLocId,  
      [F1] = @F1,  
      [F2] = @F2,  
      [F3] = @F3,  
      [F4] = @F4,  
      [F5] = @F5,  
      [FeeHsId] = @FeeHsId,  
      [FeeHsMzName] = @FeeHsMzName,  
      [FeeId] = @FeeId,  
      [FeeName] = @FeeName,  
      [InvItemId] = @InvItemId,  
      [InvMzItemName] = @InvMzItemName,  
      [InvoId] = @InvoId,  
      [InvoTime] = @InvoTime,  
      [IsBack] = @IsBack,  
      [IsDoctorInput] = @IsDoctorInput,  
      [IsIssue] = @IsIssue,  
      [IsModiDisc] = @IsModiDisc,  
      [IsOtherFee] = @IsOtherFee,  
      [IsPriority] = @IsPriority,  
      [IsToBack] = @IsToBack,  
      [ItemId] = @ItemId,  
      [LimitFee] = @LimitFee,  
      [LimitGroupId] = @LimitGroupId,  
      [LimitTotalMz] = @LimitTotalMz,  
      [ListNum] = @ListNum,  
      [LocationName] = @LocationName,  
      [LsAdviceType] = @LsAdviceType,  
      [LsGfType] = @LsGfType,  
      [LsGroupType] = @LsGroupType,  
      [LsPerform] = @LsPerform,  
      [LsReportType] = @LsReportType,  
      [LsRepType] = @LsRepType,  
      [LsRpType] = @LsRpType,  
      [Memo] = @Memo,  
      [Name] = @Name,  
      [PrepareTime] = @PrepareTime,  
      [Price] = @Price,  
      [RecipeGroupId] = @RecipeGroupId,  
      [RecipeGroupIds] = @RecipeGroupIds,  
      [RecipeId] = @RecipeId,  
      [RecipeItemId] = @RecipeItemId,  
      [RoomId] = @RoomId,  
      [Spec] = @Spec,  
      [Totality] = @Totality,  
      [TypeGFXEId] = @TypeGFXEId,  
      [UnitDiagName] = @UnitDiagName,  
      [UnitId] = @UnitId,  
      [XDRpId] = @XDRpId,  
      [Mzregid] = @Mzregid,  
      [OperTime] = @OperTime  
  WHERE  
  	[ID] = @ID  
    
  --endregion  
    
    
    
  GO  
    
    
  --region [dbo].[OuInvoiceDtlTemp_Select]  
    
  ------------------------------------------------------------------------------------------------------------------------  
  -- Generated By:   using Enivision 1.2.0.0   
  -- Template:       New Enivision 1.2  
  -- Procedure Name: [dbo].[OuInvoiceDtlTemp_Select]  
  -- Date Generated: 2016-6-29 12:59:46  
  ------------------------------------------------------------------------------------------------------------------------  
    
  CREATE PROCEDURE [dbo].[OuInvoiceDtlTemp_Select]  
  	@ID int  
  AS  
    
  SET NOCOUNT ON  
  SET TRANSACTION ISOLATION LEVEL READ COMMITTED  
    
  SELECT  
       *   
  FROM  
  	[dbo].[OuInvoiceDtlTemp] WITH(NOLOCK) 
  WHERE  
  	[ID] = @ID  
    
  --endregion  
    
    
    
  GO  
    
    
  --region [dbo].[OuInvoiceDtlTemp_SelectAll]  
    
  ------------------------------------------------------------------------------------------------------------------------  
  -- Generated By:   using Enivision 1.2.0.0   
  -- Template:       New Enivision 1.2  
  -- Procedure Name: [dbo].[OuInvoiceDtlTemp_SelectAll]  
  -- Date Generated: 2016-6-29 12:59:46  
  ------------------------------------------------------------------------------------------------------------------------  
    
  CREATE PROCEDURE [dbo].[OuInvoiceDtlTemp_SelectAll]  
  AS  
    
  SET NOCOUNT ON  
  SET TRANSACTION ISOLATION LEVEL READ COMMITTED  
    
  SELECT  
       *   
  FROM  
  	[dbo].[OuInvoiceDtlTemp] WITH(NOLOCK)  
    
    
  GO  
    
  --region [dbo].[OuInvoiceDtlTemp_SelectDynamic]  
    
  ------------------------------------------------------------------------------------------------------------------------  
  -- Generated By:   using Enivision 1.2.0.0   
  -- Template:       New Enivision 1.2  
  -- Procedure Name: [dbo].[OuInvoiceDtlTemp_SelectDynamic]  
  -- Date Generated: 2016-6-29 12:59:46  
  ------------------------------------------------------------------------------------------------------------------------  
    
  CREATE PROCEDURE [dbo].[OuInvoiceDtlTemp_SelectDynamic]  
  	@WhereCondition nvarchar(500),  
  	@OrderByExpression nvarchar(250) = NULL  
  AS  
    
  SET NOCOUNT ON  
  SET TRANSACTION ISOLATION LEVEL READ COMMITTED  
    
  DECLARE @SQL nvarchar(3250)  
    
  SET @SQL = '  
  SELECT  
       *   
  FROM  
  	[dbo].[OuInvoiceDtlTemp] WITH(NOLOCK)  
  WHERE  
  	' + @WhereCondition  
    
  IF @OrderByExpression IS NOT NULL AND LEN(@OrderByExpression) > 0  
  BEGIN  
  	SET @SQL = @SQL + '  
  ORDER BY  
  	' + @OrderByExpression  
  END  
    
  EXEC SP_EXECUTESQL @SQL  
    
  --endregion  
    
  GO  
  go  
  --region [dbo].[OuInvoiceDtlTemp_DeleteByMzregid]  
    
  ------------------------------------------------------------------------------------------------------------------------  
  -- Generated By:  1.2.0.0  
  -- Template:       New SP.cst  
  -- Procedure Name: [dbo].[OuInvoiceDtlTemp_DeleteByMzregid]  
  -- Date Generated: 2016-6-29 12:59:46  
  ------------------------------------------------------------------------------------------------------------------------  
    
  CREATE PROCEDURE [dbo].[OuInvoiceDtlTemp_DeleteByMzregid]  
  	@Mzregid INT  
  AS  
    
  SET NOCOUNT ON  
    
  DELETE FROM  
  	[dbo].[OuInvoiceDtlTemp]  
  WHERE  
  	[Mzregid] = @Mzregid  
    
    
    
  GO  
    
    
  --Select By Foreign Key Procedures  
  --region [dbo].[OuInvoiceDtlTemp_SelectByMzregid]  
    
  ------------------------------------------------------------------------------------------------------------------------  
  -- Generated By:  1.2.0.0  
  -- Template:       Envision 1.2.0.0  
  -- Procedure Name: [dbo].[OuInvoiceDtlTemp_SelectByMzregid]  
  -- Date Generated: 2016-6-29 12:59:46  
  ------------------------------------------------------------------------------------------------------------------------  
    
  CREATE PROCEDURE [dbo].[OuInvoiceDtlTemp_SelectByMzregid]  
  	@Mzregid INT  
  AS  
    
  SET NOCOUNT ON  
  SET TRANSACTION ISOLATION LEVEL READ COMMITTED  
    
  SELECT  
      [ID],  
      [Amount],  
      [AmountFact],  
      [AmountInsurance],  
      [AmountPay],  
      [AmountSelf],  
      [AmountTally],  
      [Code],  
      [DiscDiag],  
      [DiscSelf],  
      [DocLevId],  
      [DoctorId],  
      [DoctorName],  
      [ExecLocId],  
      [F1],  
      [F2],  
      [F3],  
      [F4],  
      [F5],  
      [FeeHsId],  
      [FeeHsMzName],  
      [FeeId],  
      [FeeName],  
      [InvItemId],  
      [InvMzItemName],  
      [InvoId],  
      [InvoTime],  
      [IsBack],  
      [IsDoctorInput],  
      [IsIssue],  
      [IsModiDisc],  
      [IsOtherFee],  
      [IsPriority],  
      [IsToBack],  
      [ItemId],  
      [LimitFee],  
      [LimitGroupId],  
      [LimitTotalMz],  
      [ListNum],  
      [LocationName],  
      [LsAdviceType],  
      [LsGfType],  
      [LsGroupType],  
      [LsPerform],  
      [LsReportType],  
      [LsRepType],  
      [LsRpType],  
      [Memo],  
      [Name],  
      [PrepareTime],  
      [Price],  
      [RecipeGroupId],  
      [RecipeGroupIds],  
      [RecipeId],  
      [RecipeItemId],  
      [RoomId],  
      [Spec],  
      [Totality],  
      [TypeGFXEId],  
      [UnitDiagName],  
      [UnitId],  
      [XDRpId],  
      [Mzregid],  
      [OperTime]  
  FROM  
  	[dbo].[OuInvoiceDtlTemp] WITH(NOLOCK)  
  WHERE  
  	[Mzregid] = @Mzregid  
    
  --endregion  
    
    
    
  GO  
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             