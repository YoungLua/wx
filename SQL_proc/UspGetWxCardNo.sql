create  procedure UspGetWxCardNo
 as
 begin 

select 
case when ((select max(a.cardno) from bspatient a where a.cardno like '66%' and len(cardno)=9)) is null then  
 '660000001' 
 else 
  (select (select max(a.cardno) from bspatient a where a.cardno like '66%' and len(cardno)=9)+1) end ;
 end;

