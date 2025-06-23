export function timeDiffInMinutes(time1:string, time2:string)
{
    const time1AsTime = new Date(`"2000/01/01"${time1}`).getTime();
    const time2AsTime = new Date(`"2000/01/01"${time2}`).getTime();

    const msecDiff = Math.floor(Math.abs(time1AsTime-time2AsTime));

    return msecDiff/60000;
};
