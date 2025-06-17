export function timeDiffInMinutes(time1:string, time2:string)
{
    const time1AsTime = new Date(time1).getTime();
    const time2AsTime = new Date(time2).getTime();

    const msecDiff = Math.abs(time1AsTime-time2AsTime);

    return msecDiff/60000;
};
