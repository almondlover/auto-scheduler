export function timeDiffInMinutes(time1:string, time2:string)
{
    const time1AsTime = new Date(`"2000/01/01"${time1}`).getTime();
    const time2AsTime = new Date(`"2000/01/01"${time2}`).getTime();

    const msecDiff = Math.floor(Math.abs(time1AsTime-time2AsTime));

    return msecDiff/60000;
};
export function timeRangesOverlap (startTime1:string, endTime1:string, startTime2:string, endTime2:string)
{
    const startTime1AsTime = new Date(`"2000/01/01"${startTime1}`).getTime();
    const endTime1AsTime = new Date(`"2000/01/01"${endTime1}`).getTime();
    const startTime2AsTime = new Date(`"2000/01/01"${startTime2}`).getTime();
    const endTime2AsTime = new Date(`"2000/01/01"${endTime2}`).getTime();

    if (!((startTime1AsTime < endTime2AsTime) && (endTime1AsTime > startTime2AsTime)))
        return null;
    
    return { startTime: startTime1AsTime > startTime2AsTime ? startTime1 : startTime2,
         endTime: endTime1AsTime < endTime2AsTime ? endTime1 : endTime2 };
}
