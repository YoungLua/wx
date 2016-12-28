CREATE proc UspgetSchInfo    
(    
@hospitalId  varchar = '0', --医院代码    
@startDate datetime , --号源开始日期，格式：YYYY-MM-DD    
@endDate datetime ,  --号源结束日期，格式：YYYY-MM-DD    
@deptId int = 0 ,  
@doctorId int = 0   
)    
as    
    
set @startDate = convert(char(10),@startDate,120) ;    
set @endDate  = convert(char(10),dateadd(d,1,@endDate),120);    
    
  
   
 delete OugetSchInfo;  
    
 while @startDate<@endDate   
 begin  
      
 insert into OugetSchInfo  
 (doctorid,locationid, shiftid, shifttype, schdate, schregtypeid, schfee, schregfee, schtreatfee, schbegintime, schendtime,   
  schregmax, schregcount, schlimited, schsegmented,Iscancel,DiagRoom,SpecialtyRoom)   
    
      select    a.doctorid, --  医生ID  
                a.locationid,--排班科室  
                d.id as shiftId,--班别ID  
                case when d.code in ('1','5') then 1 when d.code in ('2','3')  
                     then 2 when d.code='4' then 3 end as shiftType, --班别类型 (1:上午 2：下午 3:夜班)  
                @startDate as   schDate,  -- 排班日期  
                a.regtypeid as schRegTypeId,--挂号类型ID  
                h.regfee+h.diagnofee as schFee ,--总费用  
                h.regfee as schRegfee ,--挂号费  
                h.diagnofee as schTreatfee ,--诊疗费  
                d.timebegin + ':00' as schBegintime ,-- regInfo  time  是  班别类型 (1:上午 2：下午 3:夜班)  
                d.timeend + ':00' as schEndtime ,-- regInfo  time  是  医生ID  
                a.limitnum as schRegMax,--最大挂号量  
                t.rc as  schRegCount,--      否  已挂号量  
                1 as schLimited,-- regInfo  tinyint  否  是否限号  
                1 as schSegmented,  --regInfo  tinyint  否  是否分时段（1.分时段，0不分时段）  
                0 Iscancel,--是否停诊  
                g.name as DiagRoom,--诊间  
                i.name as SpecialtyRoom--专业科室  
         from bsdocregtype a join bsdoctor b on a.doctorid=b.id  
                                left join bslocation c on a.locationid=c.id  
                                left join BSREGTIMESPAN d on d.id=a.timespanid  
                                left join bsdoclevel e on e.id=b.doclevid  
                                left join bsregtype f on b.regtypeid=f.id  
                                left join bsregpatamount h on h.regtypeid=f.id and h.pattypeid=176  
                                left join BsDiagRoom g on a.diagroomid = g.id  
                                left join BsSpecialtyRoom i on a.lastlimitnum = i.id  
                                left join ( select a.timespansubid,a.doctorid,count(1) as RC from ouhosinfo a  
                                           where  a.regtime>=(@startDate) and a.regtime<(@startDate)+1  
                                                 and a.iscancel=0 and a.isprereg=1  
                                                 group by a.timespansubid,a.doctorid  
                                          ) t  on   i.id=t.timespansubid and a.doctorid=t.doctorid  
                 where  a.weekday = datepart(dw,@startDate)-1  
                        and b.lsstatus <> 2  
                        and b.F2 = '1'  
                        --and a.isactive=1  
                        and (a.DoctorId = @doctorId or @doctorId = 0)  
                        and (a.LocationId = @deptId or @deptId = 0)  
                       and not exists (select 1 from oudocregtype where (regdate)=(@startDate) and oudocregtype.weekplanid=a.id)  
                       and (c.hospitalid=@hospitalId or @hospitalId='0')  
                       and not exists (select 1 from bscalendar   
                               where (@startDate)=bscalendar.dates and  bscalendar.isholiday=1)  
        union all  
          select  
                a.doctorid, --  医生ID  
                a.locationid,--排班科室  
                d.id as shiftId,--班别ID  
                case when d.code in ('1','5') then 1 when d.code in ('2','3')  
                     then 2 when d.code='4' then 3 end as shiftType, --班别类型 (1:上午 2：下午 3:夜班)  
                @startDate as   schDate,  -- 排班日期  
                a.regtypeid as schRegTypeId,--挂号类型ID  
              h.regfee+h.diagnofee as schFee ,--总费用  
                h.regfee as schRegfee ,--挂号费  
                h.diagnofee as schTreatfee ,--诊疗费  
                d.timebegin + ':00' as  schBegintime,--  regInfo  time  是  班别类型 (1:上午 2：下午 3:夜班)  
                d.timeend + ':00' as  schEndtime,--  regInfo  time  是  医生ID  
                a.limitnum as schRegMax,--最大挂号量  
                t.rc as  schRegCount,--      否  已挂号量  
                1 as schLimited ,-- regInfo  tinyint  否  是否限号  
                1 as schSegmented, -- regInfo  tinyint  否  是否分时段（1.分时段，0不分时段）  
                a.iscancel,--是否停诊  
                g.name as DiagRoom,--诊间  
                i.name as SpecialtyRoom--专业科室  
         from oudocregtype a join bsdoctor b on a.doctorid=b.id  
                                left join bslocation c on a.locationid=c.id  
                                left join BSREGTIMESPAN d on d.id=a.timespanid  
                                left join bsdoclevel e on e.id=b.doclevid  
                                left join bsregtype f on b.regtypeid=f.id  
                                left join bsregpatamount h on h.regtypeid=f.id and h.pattypeid=176  
                                left join BsDiagRoom g on a.diagroomid = g.id  
                                left join BsSpecialtyRoom i on a.lastlimitnum = i.id  
                                left join ( select a.timespansubid,a.doctorid,count(1) as RC from ouhosinfo a  
                                           where  a.regtime>=(@startDate) and a.regtime<(@startDate)+1  
                                                 and a.iscancel=0 and a.isprereg=1  
                                                 group by a.timespansubid,a.doctorid  
                                          ) t  on   i.id=t.timespansubid and a.doctorid=t.doctorid  
                 where     --a.iscancel=0  
                       (c.hospitalid=@hospitalId or @hospitalId='0')  
                       and b.lsstatus <> 2  
                       and b.F2 = '1'  
                       and (a.DoctorId = @doctorId or @doctorId = 0)  
                       and (a.LocationId = @deptId or @deptId = 0)  
                       and a.regdate>=@startDate and a.regdate<@endDate                         
                       and not exists (select 1 from bscalendar   
                               where (@startDate)=bscalendar.dates and  bscalendar.isholiday=1);        
       set @startDate =@startDate+1;  
 end ;    
   
 select doctorid,locationid, shiftid, shifttype, schdate, schregtypeid, schfee, schregfee, schtreatfee, schbegintime, schendtime,   
  schregmax, schregcount, schlimited, schsegmented,iscancel,DiagRoom,SpecialtyRoom from OugetSchInfo  
  