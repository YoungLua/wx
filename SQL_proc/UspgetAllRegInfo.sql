CREATE  proc UspgetAllRegInfo    
(    
@hospitalId  varchar = '0', -- String    ҽԺ����    
@startDate  datetime,--  String  ��  ��Դ��ʼ���� ��ʽ��YYYY-MM-DD    
@endDate datetime --String �� ��Դ�������ڸ�ʽ��YYYY-MM-DD    
)    
as    
set @startDate =cast(convert(char(10),@startDate,120) as DATE);    
set @endDate  =cast(convert(char(10),dateadd(d,1,@endDate),120) as DATE);    
    
    
delete OugetAllreginfo;    
    
 while @startDate<@endDate      
  begin     
   insert into OugetAllreginfo    
   (doctorid, doctorname, doctortitle, timereginfolist,locationid, regdate, regweekday, regtotalcount, regleavecount, totalfee, regfee,     
    treatfee, istimereg, shifttype, regstatus, timereginfo, starttime, timeend, regtotalcount1, regleavecount1,iscancel,onlyId,roomname    
    )    
select     a.doctorid, --  ҽ��ID    
          b.name as doctorName,--ҽ������    
          e.name as doctorTitle ,--ҽ��ְ��    
          null TimeRegInfoList,  --�������ڼ���    
          a.locationid,--�Ű����    
          @startDate as  regDate,--�������ڣ���ʽ��YYYY-MM-DD    
          a.weekday as regweekday, --�������ڶ�Ӧ������    
          a.limitnum as regTotalCount,--���Һ���    
          a.limitnum-isnull(t.rc,0) as regleaveCount,--  ��ʱ��ʣ���Դ��    
          h.regfee+h.diagnofee as totalFee ,--�ܷ���    
          h.regfee as regFee ,--�Һŷ�    
          h.diagnofee as treatFee ,--���Ʒ�    
          0 isTimeReg ,-- �Ƿ��з�ʱ,    
          case when d.code in ('1','5') then 1 when d.code in ('2','3')    
               then 2 when d.code='4' then 3 end as shiftType, --������� (1:���� 2������ 3:ҹ��)    
          1 regStatus,--  ����״̬    
          null as timeRegInfo ,-- ���硢���硢���ϵĺ�Դ��Ϣ����    
          i.timebegin + ':00' as  startTime,--��ʱ��ʼʱ�䣬��ʽ��HH:MI    
          i.timeend + ':00' as timeend,--��ʱ����ʱ�䣬��ʽ��HH:MI    
          o.sublimitnum as regTotalCount1,-- ��ʱ�ο�ԤԼ���ܺ�Դ��    
          o.sublimitnum-isnull(t.rc,0) as regleaveCount1,--��ʱ��ʣ���Դ��    
          0 iscancel,--�Ƿ�ͣ��    
          'Z' + convert(char,o.id) onlyId,--ÿ��ʱ�εĺ�Դ��Ψһ��ʶ    
          bsdiagroom.name as roomname    
   from bsdocregtype a join bsdoctor b on a.doctorid=b.id    
                               join bslocation c on a.locationid=c.id    
                               join BSREGTIMESPAN d on d.id=a.timespanid    
                               join bsdoclevel e on e.id=b.doclevid    
                               join bsregtype f on b.regtypeid=f.id    
                               join bsregpatamount h on h.regtypeid=f.id and h.pattypeid=176    
                               join oudocspansub o on o.weekplanid = a.id    
                               join BSREGSPANSUB i on o.timespansubid = i.id    
                                 left join bsdiagroom   on a.diagroomid = bsdiagroom.id    
                         left join ( select a.timespansubid,a.doctorid,count(1) as RC from ouhosinfo a    
                                     where  a.regtime>=(@startDate) and a.regtime<(@startDate)+1    
                                           and a.iscancel=0 and a.isprereg=1    
                                           group by a.timespansubid,a.doctorid    
                                    ) t  on   i.id=t.timespansubid and a.doctorid=t.doctorid    
           where  a.weekday= datepart(dw,@startDate)-1    
                 and b.lsstatus <> 2    
                 and b.F2 = '1'    
                 --and a.isactive=1    
                 and not exists (select 1 from oudocregtype where (regdate)=(@startDate) and oudocregtype.weekplanid=a.id)    
                 and (c.hospitalid=@hospitalId or @hospitalId='0')    
   union all    
    select    
          a.doctorid, --  ҽ��ID    
          b.name as doctorName,--ҽ������    
          e.name as doctorTitle ,--ҽ��ְ��    
          null TimeRegInfoList,  --�������ڼ���    
          a.locationid,--�Ű����    
          a.opertime as  regDate,--�������ڣ���ʽ��YYYY-MM-DD    
          convert(char(10),dateadd(d,-1,a.opertime),120)  as regweekday, --�������ڶ�Ӧ������    
          a.limitnum as regTotalCount,--���Һ���    
          a.limitnum-t.rc as regleaveCount,--  ��ʱ��ʣ���Դ��    
          h.regfee+h.diagnofee as totalFee ,--�ܷ���    
          h.regfee as regFee ,--�Һŷ�    
          h.diagnofee as treatFee ,--���Ʒ�    
          0 isTimeReg ,-- �Ƿ��з�ʱ,           
          case when d.code in ('1','5') then 1 when d.code in ('2','3')    
               then 2 when d.code='4' then 3 end as shiftType, --������� (1:���� 2������ 3:ҹ��)    
          case when  a.iscancel=1 then 0 else 1 end regStatus ,--  ����״̬    
          null as timeRegInfo ,-- ���硢���硢���ϵĺ�Դ��Ϣ����    
          i.timebegin + ':00' as  startTime,--��ʱ��ʼʱ�䣬��ʽ��HH:MI    
          i.timeend + ':00' as timeend,--��ʱ����ʱ�䣬��ʽ��HH:MI    
          o.sublimitnum as regTotalCount1,-- ��ʱ�ο�ԤԼ���ܺ�Դ��    
          o.sublimitnum-isnull(t.rc,0) as regleaveCount1,--��ʱ��ʣ���Դ��    
          a.iscancel,--�Ƿ�ͣ��    
          'R' + convert(char,o.id) onlyId,--ÿ��ʱ�εĺ�Դ��Ψһ��ʶ    
          bsdiagroom.name as roomname    
   from oudocregtype a join bsdoctor b on a.doctorid=b.id    
                                join bslocation c on a.locationid=c.id    
                                join BSREGTIMESPAN d on d.id=a.timespanid    
                                join bsdoclevel e on e.id=b.doclevid    
                                join bsregtype f on b.regtypeid=f.id    
                                join bsregpatamount h on h.regtypeid=f.id and h.pattypeid=176    
                                join oudocspansub o on o.weekplanid = a.id    
                               join BSREGSPANSUB i on o.timespansubid = i.id    
                              left   join bsdiagroom   on a.diagroomid = bsdiagroom.id    
                         left join ( select a.timespansubid,a.doctorid,count(1) as RC from ouhosinfo a    
                                     where  a.regtime>=(@startDate) and a.regtime<convert(char(10),dateadd(d,1,@startDate),120)    
                                           and a.iscancel=0 and a.isprereg=1    
                                           group by a.timespansubid,a.doctorid    
                                    ) t  on   i.id=t.timespansubid and a.doctorid=t.doctorid    
           where  --a.iscancel=0    
                 (c.hospitalid=@hospitalId or @hospitalId='0')    
                 and b.lsstatus <> 2    
                 and b.F2 = '1'    
                 and a.regdate>=@startDate and a.regdate<@startDate+1;    
                    
 set @startDate = @startDate+1;    
 end ;     
     
  select doctorid, doctorname, doctortitle, timereginfolist,locationid, regdate, regweekday, regtotalcount, regleavecount, totalfee, regfee,     
treatfee, istimereg, shifttype, regstatus, timereginfo, starttime, timeend, regtotalcount1, regleavecount1,iscancel,onlyId,roomname    
  from OugetAllRegInfo 