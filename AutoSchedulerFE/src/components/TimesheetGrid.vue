<script setup lang="ts">
import type { Group } from '@/classes/group';
import type { Timesheet, Timeslot } from '@/classes/timesheet';
import { timeDiffInMinutes } from '@/utils/timediff';
import { onMounted } from 'vue';

const props = defineProps<{
    timesheet:Timesheet,
    startTime:string,
    endTime:string,
    slotDurationInMinutes:number,
    headGroup:Group
}>();

const headGroups=props.timesheet.timeslots.filter((ts, idx, array)=>{
        idx===array.indexOf(ts) && !array.some(ts2=>ts.group.parentGroupId===ts2.group.id)
    }).map(ts=>ts.group);

onMounted(()=>{
    setGroupRows(props.headGroup, props.timesheet.timeslots);
    setGroupRowStarts();
});

//values for timeslot times as whole numbers representing number of slots
const timeslotStartInSlots = (timeslot:Timeslot)=>timeDiffInMinutes(props.startTime, timeslot.startTime)/props.slotDurationInMinutes;
const timeslotDurationInSlots = (timeslot:Timeslot)=>timeDiffInMinutes(timeslot.endTime, timeslot.startTime)/props.slotDurationInMinutes;
const totalSlots = ()=>timeDiffInMinutes(props.startTime, props.endTime)/props.slotDurationInMinutes;

interface SubRowsForGroup{
    headGroup:Group,
    rowCount:number,
    row:number,
    span:number
}

//type containing n/of children of parent
const groupCountsInCol:number[][]=[];
const groupRowCounts:SubRowsForGroup[][]=[];
//maybe this should be in be?
function setGroupRows(headGroup:Group, timeslots:Timeslot[])
{
    //const listOfCols=[];
    let subGroupsInSheet = timeslots.map(slot=>{if (slot.group.parentGroupId===headGroup.id) return slot.group}).filter(group=>group!==undefined);
    let nextCol:Group[][];
    //put the root group first iwht max length
    groupRowCounts.push([{headGroup:headGroup, rowCount:subGroupsInSheet.length, row:0, span:1}]);
    do
    {
        nextCol=[];
        //const subGroupCountsList=[];
        const curColGroupRowCounts=[]
        for (const group of subGroupsInSheet)
        {
            const currentSubGroups=timeslots.map(slot=>{if (slot.group.parentGroupId===group?.id) return slot.group}).filter(group=>group!==undefined);
            nextCol.push(currentSubGroups);

            //const subGroupsCounts=currentSubGroups.map(group=>currentSubGroups.length);
            //subGroupCountsList.push(subGroupsCounts);

            curColGroupRowCounts.push({headGroup:group, rowCount:currentSubGroups.length, row:0, span:0});
        }
        groupRowCounts.push(curColGroupRowCounts);
        //got to next row
        subGroupsInSheet=nextCol.flat();
    }
    while (nextCol.some(grps=>grps.length>0));

    //return listOfCols;
}

function setGroupRowStarts()
{
    for (let i = 0; i<groupRowCounts[groupRowCounts.length-1].length; i++)
        groupRowCounts[groupRowCounts.length-1][i].row = i;
    //start from second to last col and end at second one
    for (let i=groupRowCounts.length-2; i>0; i--)
    {
        groupRowCounts[i][0].row=0;
        for (let j=1; j<groupRowCounts[i].length; j++)
        {
            //get span of current
            let subgroupIdx=0;
            for (let k=0; k<j; k++)
                subgroupIdx+=groupRowCounts[i][k].rowCount;

            for (let k=subgroupIdx; k<subgroupIdx+groupRowCounts[i][j-1].rowCount; k++)
                groupRowCounts[i][k].span+=groupRowCounts[i-1][k].span;

            groupRowCounts[i][j].row=groupRowCounts[i][j-1].row+groupRowCounts[i][j-1].span;

        }
    } 
}

const timeslotStartRow = (timeslot:Timeslot)=>{ 
    groupRowCounts.map((grpl)=>{
        grpl[grpl.findIndex((grp)=>{
            grp.headGroup.id===timeslot.group.id;
        })].row;
    }) };
const timeslotSpan = (timeslot:Timeslot)=>{ 
groupRowCounts.map((grpl)=>{
    grpl[grpl.findIndex((grp)=>{
        grp.headGroup.id===timeslot.group.id;
    })].span;
}) };
</script>

<template>
    <!-- class values prolly shouldnt be inline -->
    <div :class="`grid grid-gap-1 grid-cols-${totalSlots} grid-rows-${groupRowCounts[0][0].rowCount} w-200 h-100`">
        <div v-for="timeslot in timesheet.timeslots" :class="`col-start-${timeslotStartInSlots(timeslot)} col-span-${timeslotDurationInSlots(timeslot)} row-start-${timeslotStartRow(timeslot)} row-span-${timeslotSpan(timeslot)} bg-black`">here</div>
    </div>
</template>