USE [HIStest]
GO

/****** Object:  Table [dbo].[OuInvoiceDtlTemp]    Script Date: 06/29/2016 12:11:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[OuInvoiceDtlTemp](
	[ID] [int] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[AmountFact] [decimal](18, 2) NULL,
	[AmountInsurance] [decimal](18, 2) NULL,
	[AmountPay] [decimal](18, 2) NULL,
	[AmountSelf] [decimal](18, 2) NULL,
	[AmountTally] [decimal](18, 2) NULL,
	[Code] [nvarchar](50) NULL,
	[DiscDiag] [decimal](18, 2) NULL,
	[DiscSelf] [decimal](18, 2) NULL,
	[DocLevId] [int] NULL,
	[DoctorId] [int] NULL,
	[DoctorName] [nvarchar](50) NULL,
	[ExecLocId] [int] NULL,
	[F1] [nvarchar](50) NULL,
	[F2] [nvarchar](50) NULL,
	[F3] [nvarchar](50) NULL,
	[F4] [nvarchar](50) NULL,
	[F5] [nvarchar](50) NULL,
	[FeeHsId] [int] NULL,
	[FeeHsMzName] [nvarchar](50) NULL,
	[FeeId] [int] NULL,
	[FeeName] [nvarchar](50) NULL,
	[InvItemId] [int] NULL,
	[InvMzItemName] [nvarchar](50) NULL,
	[InvoId] [int] NULL,
	[InvoTime] [datetime] NULL,
	[IsBack] [bit] NULL,
	[IsDoctorInput] [bit] NULL,
	[IsIssue] [bit] NULL,
	[IsModiDisc] [bit] NULL,
	[IsOtherFee] [bit] NULL,
	[IsPriority] [bit] NULL,
	[IsToBack] [bit] NULL,
	[ItemId] [int] NULL,
	[LimitFee] [decimal](18, 2) NULL,
	[LimitGroupId] [int] NULL,
	[LimitTotalMz] [decimal](18, 2) NULL,
	[ListNum] [int] NULL,
	[LocationName] [nvarchar](50) NULL,
	[LsAdviceType] [int] NULL,
	[LsGfType] [int] NULL,
	[LsGroupType] [int] NULL,
	[LsPerform] [int] NULL,
	[LsReportType] [int] NULL,
	[LsRepType] [int] NULL,
	[LsRpType] [int] NULL,
	[Memo] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NULL,
	[PrepareTime] [nvarchar](50) NULL,
	[Price] [decimal](18, 2) NULL,
	[RecipeGroupId] [int] NULL,
	[RecipeGroupIds] [nvarchar](50) NULL,
	[RecipeId] [int] NULL,
	[RecipeItemId] [int] NULL,
	[RoomId] [int] NULL,
	[Spec] [nvarchar](50) NULL,
	[Totality] [decimal](18, 2) NULL,
	[TypeGFXEId] [int] NULL,
	[UnitDiagName] [nvarchar](50) NULL,
	[UnitId] [int] NULL,
	[XDRpId] [int] NULL
) ON [PRIMARY]

GO


