<script setup lang="ts">
import type { Group } from '@/classes/group';
import type { Timesheet, Timeslot } from '@/classes/timesheet';
import { dayOfTheWeek } from '@/constants/constants';
import { timeDiffInMinutes } from '@/utils/timediff';
import { computed, onBeforeMount, onMounted, onUpdated, ref, watch, type Ref } from 'vue';

const props = defineProps<{
    timesheet:Timesheet,
    startTime:string,
    endTime:string,
    slotDurationInMinutes:number,
    headGroup:Group
}>();

onMounted(()=>{
    setGroupRows(props.headGroup, props.timesheet.timeslots);
    setGroupRowStarts();
});

watch(props.timesheet, ()=>{
    setGroupRows(props.headGroup, props.timesheet.timeslots);
    console.log('here')
    setGroupRowStarts();
})

const headGroupSlots=computed(()=>
    props.timesheet.timeslots.filter(ts=>
        groupRowCounts.value.some(grprc=>
            grprc.some((el=>
                el.headGroup.id===ts.group.id
            ))
        )
    ));
//values for timeslot times as whole numbers representing number of slots
const timeslotStartInSlots = (timeslot:Timeslot)=>Math.floor(timeDiffInMinutes(props.startTime, timeslot.startTime)/props.slotDurationInMinutes);
const timeslotDurationInSlots = (timeslot:Timeslot)=>Math.floor(timeDiffInMinutes(timeslot.endTime, timeslot.startTime)/props.slotDurationInMinutes);
const totalSlots = computed(()=>timeDiffInMinutes(props.startTime, props.endTime)/props.slotDurationInMinutes);

interface SubRowsForGroup{
    headGroup:Group,
    rowCount:number,
    row:number,
    span:number
}

//type containing n/of children of parent
const groupRowCounts:Ref<SubRowsForGroup[][]> = ref([]);
const totalRows=ref(0);
//maybe this should be in be?
function setGroupRows(headGroup:Group, timeslots:Timeslot[])
{
    groupRowCounts.value=[];
    let subGroupsInSheet = timeslots.map(slot=>{if (slot.group.parentGroupId===headGroup.id) return slot.group}).filter(group=>group!==undefined).filter((grp, idx, array)=>
                                                                                                                                                            idx===array.findIndex(grp2=>grp2.id===grp.id));
    let nextCol:Group[][];
    //put the root group first iwht max length
    groupRowCounts.value.push([{headGroup:headGroup, rowCount:subGroupsInSheet.length, row:0, span:0}]);
    do
    {
        nextCol=[];
        //const subGroupCountsList=[];
        const curColGroupRowCounts=[]
        for (const group of subGroupsInSheet)
        {
            const currentSubGroups=timeslots.map(slot=>{if (slot.group.parentGroupId===group?.id) return slot.group}).filter(group=>group!==undefined).filter((grp, idx, array)=>
                                                                                                                                                            idx===array.findIndex(grp2=>grp2.id===grp.id));
            nextCol.push(currentSubGroups);

            //const subGroupsCounts=currentSubGroups.map(group=>currentSubGroups.length);
            //subGroupCountsList.push(subGroupsCounts);
            curColGroupRowCounts.push({headGroup:group, rowCount:(currentSubGroups.length===0 ? 1 : currentSubGroups.length), row:0, span:0});
        }
        //this check shouldnt be made
        if (curColGroupRowCounts.length>0)
            groupRowCounts.value.push(curColGroupRowCounts);
        //got to next row
        subGroupsInSheet=nextCol.flat();
    }
    while (nextCol.some(grps=>grps.length>0));

    //return listOfCols;
}

function setGroupRowStarts()
{
    for (let i = 0; i<groupRowCounts.value[groupRowCounts.value.length-1].length; i++)
    {
        groupRowCounts.value[groupRowCounts.value.length-1][i].row = i;
        groupRowCounts.value[groupRowCounts.value.length-1][i].span = 1;
        //prolly shouldnt be put here
        groupRowCounts.value[groupRowCounts.value.length-1][i].rowCount = 1;
    }
    //start from second to last col and end at second one
    for (let i=groupRowCounts.value.length-2; i>=0; i--)
    {
        for (let k=0; k<groupRowCounts.value[i][0].rowCount; k++)
            groupRowCounts.value[i][0].span+=groupRowCounts.value[i+1][k].span;
        
        groupRowCounts.value[i][0].row=0;
        for (let j=1; j<groupRowCounts.value[i].length; j++)
        {
            //get span of current
            let subgroupIdx=0;
            for (let k=0; k<j; k++)
                subgroupIdx+=groupRowCounts.value[i][k].rowCount-1;

            for (let k=subgroupIdx; k<subgroupIdx+groupRowCounts.value[i][j].rowCount; k++)
                groupRowCounts.value[i][j].span+=groupRowCounts.value[i+1][k].span;

            groupRowCounts.value[i][j].row=groupRowCounts.value[i][j-1].row+groupRowCounts.value[i][j-1].span;
        }
    }
    totalRows.value=groupRowCounts.value[0][0].span;
}

const timeslotStartRow = (timeslot:Timeslot)=>computed(()=>
    groupRowCounts.value.map((grpl)=>
        grpl[grpl.findIndex((grp)=>
            grp.headGroup.id===timeslot.group.id
        )]?.row
    ).filter(res=>res!==undefined)[0]+totalRows.value*timeslot.dayOfWeek
);
const timeslotSpan = (timeslot:Timeslot)=>computed(()=>
    groupRowCounts.value.map((grpl)=>
        grpl[grpl.findIndex((grp)=>
            grp.headGroup.id===timeslot.group.id
        )]?.span
    ).filter(res=>res!==undefined)[0]
);
const gridContainerClasses = computed(()=>`grid grid-cols-${totalSlots.value+1} grid-rows-${totalRows.value*5} h-300 w-9/10 m-auto`);
const gridSlotClasses = (timeslot:Timeslot)=>computed(()=>`col-start-${timeslotStartInSlots(timeslot)+2} col-span-${timeslotDurationInSlots(timeslot)+1} row-start-${timeslotStartRow(timeslot).value+1} row-span-${timeslotSpan(timeslot).value}`);
</script>

<template>
    <!-- class values prolly shouldnt be inline -->
    <h3 @click="console.log(groupRowCounts)">{{ `Timesheet for ${(headGroup.name)}` }}</h3>
    <div :class="`grid grid-cols-${totalSlots+1} h-10 w-9/10 m-auto`">
        <!-- shouldn be inline -->
        <div v-for="slot of totalSlots+1" :class="`text-right col-start-${slot} col-span-1 pl-full`" >{{ new Date(new Date("2000/01/01 " + startTime).getTime() + (slot-1) * slotDurationInMinutes * 60000).toLocaleTimeString('en-UK', { hour: '2-digit', minute: '2-digit', hour12: false })}}</div>
    </div>
    <div :class=gridContainerClasses class="border-1 border-black">
        <div v-for="timeslot in headGroupSlots" :class=gridSlotClasses(timeslot).value class="border-box border-1 border-solid border-gray-500 text-center flex flex-col items-center justify-around  bg-gray-200 text-align text-xs">
            <div>
                <div>{{ timeslot.activity.title }}</div>
                <div>{{ timeslot.member?.name }}</div>
                <div>{{ timeslot.hall.name }}</div>
            </div>
        </div>
        <div v-for="weekday in 5" :class="`vertical-text text-center row-start-${(weekday-1)*totalRows+1} row-span-${totalRows} col-start-1 col-span-1`">{{ dayOfTheWeek[weekday-1] }}</div>
    </div>
</template>