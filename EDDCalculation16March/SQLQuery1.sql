DECLARE @StartDate date,@EndDate date,@CTENoOfRows INT,@counterVariable INT,@holidayDate date

SET @StartDate = DATEADD(yy, DATEDIFF(yy,0,getdate()), 0) 
SET @EndDate = DATEADD(dd,-1,DATEADD(yy, DATEDIFF(yy,0,getdate())+1, 0))


declare @TemporaryTable table
(
	weekendDates date
)

;with TemporaryTableForDates(weekendDates)
AS
(	
	select @StartDate
	UNION ALL
	Select DATEADD(d,1,weekendDates)from TemporaryTableForDates where weekendDates <= @EndDate
)
insert into @TemporaryTable select weekendDates from TemporaryTableForDates where DATENAME(dw,weekendDates) IN ('Saturday','Sunday') 

OPTION  (MAXRECURSION 500)

Select @CTENoOfRows=count(*) from @TemporaryTable

SET @counterVariable=1

-- Declare TemporaryTableCursor CURSOR FOR Select weekendDates from @TemporaryTable where DATENAME(dw,weekendDates) IN ('Saturday','Sunday') 

-- Open TemporaryTableCursor

--while @counterVariable<=@CTENoOfRows
--begin
--	set @counterVariable=@counterVariable+1
--	Fetch Next From TemporaryTableCursor INTO @holidayDate
--	insert into [C:\USERS\ABHI_AGARWAL\DOCUMENTS\VISUAL STUDIO 2013\PROJECTS\EDDCALCULATION\EDDCALCULATION\APP_DATA\HOLIDAYDATABASE.MDF].dbo.CorporateHoliday values('in',@holidayDate,DATENAME(dw,@holidayDate),1)
--end

select top 1 @holidayDate=weekendDates from @TemporaryTable
while @counterVariable<=@CTENoOfRows
begin
	set @counterVariable=@counterVariable+1
	insert into [C:\USERS\ABHI_AGARWAL\DOCUMENTS\VISUAL STUDIO 2013\PROJECTS\EDDCALCULATION\EDDCALCULATION\APP_DATA\HOLIDAYDATABASE.MDF].dbo.CorporateHoliday values('in',@holidayDate,DATENAME(dw,@holidayDate),1,'Green')
	select top 1 @holidayDate=weekendDates from @TemporaryTable where weekendDates>@holidayDate
end
