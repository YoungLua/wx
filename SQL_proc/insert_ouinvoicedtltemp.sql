
insert into OuInvoiceDtlTemp
(
[ItemId],[ListNum],[LsGroupType],[Code],[Name],[Spec],[UnitId],[Totality],[IsBack],
[IsToBack],[Amount],[Price],[LsGfType],[LimitTotalMz],[UnitDiagName],[FeeId],[FeeName],
[TypeGFXEId],[DiscDiag],[InvItemId],[InvMzItemName],[LimitGroupId],[LimitFee],[LocationName],
[FeeHsId],[FeeHsMzName],[XDRpId],[RecipeId],[DoctorId],[DoctorName],[LsRpType],[IsDoctorInput],
[RecipeItemId],[LsAdviceType],[Memo],[F1],[F2],[F3],[F4],[PrepareTime],[LsRepType],[IsOtherFee],
[LsReportType],[mzregid],[opertime]
)
select 
[ItemId],[ListNum],[LsGroupType],[Code],[Name],[Spec],[UnitId],[Totality],[IsBack],
[IsToBack],[Amount],[Price],[LsGfType],[LimitTotalMz],[UnitDiagName],[FeeId],[FeeName],
[TypeGFXEId],[DiscDiag],[InvItemId],[InvMzItemName],[LimitGroupId],[LimitFee],[LocationName],
[FeeHsId],[FeeHsMzName],[XDRpId],[RecipeId],[DoctorId],[DoctorName],[LsRpType],[IsDoctorInput],
[RecipeItemId],[LsAdviceType],[Memo],[F1],[F2],[F3],[F4],[PrepareTime],[LsRepType],[IsOtherFee],[LsReportType],0 as [mzregid],GETDATE() as opertime
from (
SELECT  OuRecipeDtl.ItemId,OuRecipeDtl.ListNum, vwBsItem.LsGroupType, vwBsItem.Code,                   
      vwBsItem.Name, vwBsItem.Spec, OuRecipeDtl.UnitDiagId AS UnitId,  
      case when (vwBsItem.LsRpType=3 and vwBsItem.MzUnitTotal>0) then                    
      CEILING(OuRecipeDtl.Totality * OuRecipe.HowMany) else   
      (OuRecipeDtl.Totality * OuRecipe.HowMany) end Totality,                   
      --OuRecipeDtl.Totality * OuRecipe.HowMany AS Totality,   
      OuRecipeDtl.IsBack, OuRecipeDtl.IsToBack,                  
     Round( OuRecipeDtl.Totality * OuRecipe.HowMany * vwBsItem.PriceDiag,2) AS Amount,                   
      vwBsItem.PriceDiag AS Price, vwBsItem.LsGfType, vwBsItem.LimitTotalMz,                   
      vwBsItem.UnitDiagName, vwBsItem.FeeMzId AS FeeId,                   
      vwBsItem.FeeMzName AS FeeName, vwBsItem.TypeGFXEId,                   
      vwBsItem.TypeGFXEName, vwBsItem.RoomInOut, vwBsItem.DiscDiag,                   
      vwBsItem.InvMzItemId AS InvItemId, vwBsItem.InvMzItemName,                   
      vwBsItem.LimitGroupId, vwBsItem.LimitFeeMz AS LimitFee,-- vwBsItem.LocationId,                   
      vwBsItem.LocationName, vwBsItem.FeeHsMzId AS FeeHsId,                   
      vwBsItem.FeeHsMzName, vwBsItem.TallyTypeId, vwBsItem.TallyGroupId,                   
      OuRecipeDtl.XDRpId, OuRecipeDtl.RecipeId, OuRecipe.DoctorId,                   
      dbo.fun_temp(BsDoctor.Name) AS DoctorName, vwBsItem.LsRpType, OuRecipeDtl.IsDoctorInput,                   
      OuRecipeDtl.ID AS RecipeItemId, vwBsItem.LsAdviceType,       
                         
      CASE WHEN LsRepType=1 AND OuRecipeDtl.IsDoctorInput=1 AND (BsUsage.IsMzDrop = 1 OR BsUsage.IsMzReject = 1 OR BsUsage.IsMzCure = 1) AND IsAttach = 1                  
         THEN case when OuRecipeDtl.ListNum>1 then '·½' + cast(OuRecipeDtl.ListNum as nvarchar(10)) + '£¬' else '' end        
          +'×é' + cast(OuRecipeDtl.GroupNum as nvarchar(10)) + '£¬' + BsFrequency.Name + '£¬' + BsUsage.Name + '£¬' + cast(OuRecipeDtl.Days as nvarchar(10)) + 'Ìì' + '£¬' + OuRecipeDtl.Memo  ELSE OuRecipeDtl.Memo  END AS Memo,                  
      OuRecipeDtl.F1, OuRecipeDtl.F2, OuRecipeDtl.F3, OuRecipeDtl.F4, Cast(OuRecipeDtl.PrepareTime as varchar(20)) as PrepareTime,                  
 OuRecipeDtl.GroupNum,OuRecipeDtl.UsageId,OuRecipeDtl.FrequencyId,OuRecipeDtl.Days,                  
 LsRepType,IsAttach,OuRecipeDtl.Memo As RecipeDtlMemo ,OuRecipeDtl.IsOtherFee,OuRecipeDtl.IsCancel ,BsDrugForm.F4 as LsReportType               
FROM vwBsItem WITH (NOLOCK) INNER JOIN                  
      OuRecipeDtl WITH (NOLOCK) ON vwBsItem.ID = OuRecipeDtl.ItemId INNER JOIN                  
      OuRecipe WITH (NOLOCK) ON                   
      OuRecipeDtl.RecipeId = OuRecipe.ID LEFT OUTER JOIN                  
      BsUsage WITH (NOLOCK) ON OuRecipeDtl.UsageId = BsUsage.ID LEFT OUTER JOIN                  
      BsFrequency WITH (NOLOCK) ON OuRecipeDtl.FrequencyId = BsFrequency.ID LEFT OUTER JOIN                  
      BsDoctor WITH (NOLOCK) ON OuRecipe.DoctorId = BsDoctor.ID    LEFT OUTER JOIN            
      BsItemDrug WITH (NOLOCK) ON BsItemDrug.ItemId=OuRecipeDtl.ItemId  LEFT OUTER JOIN            
      BsDrugForm   WITH (NOLOCK) ON BsItemDrug.FormId=BsDrugForm.ID                 
WHERE OuRecipe.MzRegId = 12637  and IsCharged=0 and (IsCancel=0 or IsIssue=1 or (IsCancel=1 and IsToBack=1)) and (IsBack=0 or IsToBack=1) 
and vwBsItem.PatTypeId =176
) a





