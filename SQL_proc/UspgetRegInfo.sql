CREATE proc UspgetRegInfo  
(  
@hospitalId  nvarchar = '0', -- String    医院代码  
@deptId  int = 0, --String  是  科室代码  
@doctorId  int = 0, --String  是  医生代码  
@startDate  datetime = '',--  String  是  号源开始日期 格式：YYYY-MM-DD  
@endDate  datetime = ''--String  是  号源结束日期格式：YYYY-MM-DD  
)  
as  
set @startDate =cast(convert(char(10),@startDate,120) as DATE);  
set @endDate  = convert(char(10),dateadd(d,1,@endDate),120);    
  
delete OugetRegInfo;  
  
 while @startDate<@endDate   
   
begin  
    insert into OugetRegInfo  
      ( doctorid,  doctorname,  doctortitle,  timereginfolist,locationid,  regdate,  regweekday,  regtotalcount,  regleavecount,  totalfee,  regfee,  
        treatfee,  istimereg,  shifttype,  regstatus,  timereginfo,  starttime,  timeend,  regtotalcount1,  regleavecount1,iscancel,onlyId,roomname  
      )  
        select     a.doctorid, --  医生ID  
                  b.name as doctorName,--医生名称  
                  e.name as doctorTitle ,--医生职称  
                  null TimeRegInfoList,  --出诊日期集合  
                  a.locationid,--排班科室  
                  @startDate as  regDate,--出诊日期，格式：YYYY-MM-DD  
                  a.weekday as regweekday, --出诊日期对应的星期  
                  a.limitnum as regTotalCount,-- 该时段可预约的总号源数  
                  a.limitnum-isnull(t.rc,0) as regleaveCount,--该时段剩余号源数  
                  h.regfee+h.diagnofee as totalFee ,--总费用  
                  h.regfee as regFee ,--挂号费  
                  h.diagnofee as treatFee ,--诊疗费  
                  0 isTimeReg ,-- 是否有分时,  
                  case when d.code in ('1','5') then 1 when d.code in ('2','3')  
                       then 2 when d.code='4' then 3 end as shiftType, --班别类型 (1:上午 2：下午 3:夜班)  
                  1 regStatus,--  出诊状态  
                  null as timeRegInfo ,-- 上午、下午、晚上的号源信息集合  
                  i.timebegin + ':00' as  startTime,--分时开始时间，格式：HH:MI  
                  i.timeend + ':00' as timeend,--分时结束时间，格式：HH:MI  
                  o.sublimitnum as regTotalCount1,-- 该时段可预约的总号源数  
                  o.SUBLIMITNUM-isnull(t.rc,0) as regleaveCount1,--该时段剩余号源数  
                  0 iscancel,--是否停诊  
                  'Z' + CONVERT(char,o.id) onlyId,--每个时段的号源的唯一标识  
                   bsdiagroom.name as roomname  
           from bsdocregtype a join bsdoctor b on a.doctorid=b.id  
                                       join bslocation c on a.locationid=c.id  
                                       join BSREGTIMESPAN d on d.id=a.timespanid  
                                       join bsdoclevel e on e.id=b.doclevid  
                                       join bsregtype f on b.regtypeid=f.id  
                                       join bsregpatamount h on h.regtypeid=f.id and h.pattypeid=176   
                                       join oudocspansub o on o.weekplanid = a.id  
                                       join BSREGSPANSUB i on o.timespansubid = i.id  
                                     left   join bsdiagroom   on a.diagroomid = bsdiagroom.id  
                                 left join ( select a.timespansubid,a.doctorid,count(1) as RC from ouhosinfo a  
                                             where  a.regtime>=(@startDate) and a.regtime< dateadd(d,1,@startDate)   
                                                   and a.iscancel=0 and ISPREREG=1  
                                                   group by a.timespansubid,a.doctorid  
                                            ) t  on   i.id=t.timespansubid and a.doctorid=t.doctorid  
                   where a.weekday=datepart(dw,@startDate)-1  
                         and b.lsstatus <> 2  
                         and b.F2 = '1'  
                         and not exists (select 1 from oudocregtype  
                                                where (regdate)=(@startDate) and oudocregtype.weekplanid=a.id  
                                        )  
                         and (c.hospitalid=@hospitalId or @hospitalId='0')  
                         and (@deptId =c.id or @deptId=0)  
                         and (@doctorId = b.id or @doctorId=0)  
                         and not exists (select 1 from bscalendar   
                             where (@startDate)=bscalendar.dates and  bscalendar.isholiday=1)  
  
            union all  
            select  
                  a.doctorid, --  医生ID  
                  b.name as doctorName,--医生名称  
                  e.name as doctorTitle ,--医生职称  
                  null TimeRegInfoList,  --出诊日期集合  
                  a.locationid,--排班科室  
                  @startDate as  regDate,--出诊日期，格式：YYYY-MM-DD  
                  datepart(dw,a.opertime)-1 as regweekday, --出诊日期对应的星期  
                  a.limitnum as regTotalCount,-- 该时段可预约的总号源数  
                  a.limitnum-t.rc as regleaveCount,--该时段剩余号源数  
                  h.regfee+h.diagnofee as totalFee ,--总费用  
                  h.regfee as regFee ,--挂号费  
                  h.diagnofee as treatFee ,--诊疗费  
                  0 isTimeReg ,-- 是否有分时,  
                  case when d.code in ('1','5') then 1 when d.code in ('2','3')  
                       then 2 when d.code='4' then 3 end as shiftType, --班别类型 (1:上午 2：下午 3:夜班)  
                  case when  a.iscancel=1 then 0 else 1 end regStatus,--  出诊状态  
                  null as timeRegInfo ,-- 上午、下午、晚上的号源信息集合  
                  i.timebegin + ':00' as  startTime,--分时开始时间，格式：HH:MI  
                  i.timeend + ':00' as timeend,--分时结束时间，格式：HH:MI  
                  o.sublimitnum as regTotalCount1,-- 该时段可预约的总号源数  
                  o.SUBLIMITNUM-isnull(t.rc,0) as regleaveCount1,--该时段剩余号源数  
                  a.iscancel,--是否停诊  
                  'R' + convert(char,o.id) onlyId,--每个时段的号源的唯一标识  
                  bsdiagroom.name as roomname  
           from oudocregtype a join bsdoctor b on a.doctorid=b.id  
                                        join bslocation c on a.locationid=c.id  
                                        join BSREGTIMESPAN d on d.id=a.timespanid  
                                        join bsdoclevel e on e.id=b.doclevid  
                                        join bsregtype f on b.regtypeid=f.id  
                                        join bsregpatamount h on h.regtypeid=f.id and h.pattypeid=176  
                                left join oudocspansub o on  case when o.f4 = ' ' then 0 else convert(int,o.f4) end  = a.id  
             -- join oudocspansub o on  o.f4 =a.id  
                                        join BSREGSPANSUB i on o.timespansubid = i.id  
                                        left join bsdiagroom   on a.diagroomid = bsdiagroom.id  
                                 left join ( select a.timespansubid,a.doctorid,count(1) as RC from ouhosinfo a  
                                             where  a.regtime>=(@startDate) and a.regtime<dateadd(d,1,@startDate)   
                                                   and a.iscancel=0 and ISPREREG=1  
                                                   group by a.timespansubid,a.doctorid  
                                            ) t  on   i.id=t.timespansubid and a.doctorid=t.doctorid  
                   where      --a.iscancel=0  
                         (c.hospitalid=@hospitalId or @hospitalId='0')  
                         and b.lsstatus <> 2  
                         and b.F2 = '1'  
                         and (@deptId = c.id or @deptId=0)  
                         and (@doctorId = b.id or @doctorId=0)  
                         and a.regdate>=@startDate and a.regdate<@startDate+1  
                         and not exists (select 1 from bscalendar   
                         where (@startDate)=bscalendar.dates and  bscalendar.isholiday=1);  
                           
  set @startDate=dateadd(d,1,@startDate) ;  
    
 end ;  
  
  
select * from OugetRegInfo    ;  
  