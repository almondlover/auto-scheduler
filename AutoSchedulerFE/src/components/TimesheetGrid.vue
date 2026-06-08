<script setup lang="ts">
import type { Group } from '@/classes/group';
import type { Timeslot, TimeslotWeekdayTimeRanges, WeekdayTimeRange } from '@/classes/timesheet';
import { dayOfTheWeek } from '@/constants/constants';
import { timeDiffInMinutes, timeRangesOverlap } from '@/utils/timediff';
import { computed, onBeforeMount, onMounted, onUpdated, ref, watch, type Ref } from 'vue';

interface SubRowsForGroup{
    headGroup:Group,
    rowCount:number,
    row:number,
    span:number
}

interface SlotGridView {
    activities: string[],
    //for slots that overlap and are left blank
    isOverriden: boolean,
    //for slots that are the intersection of slots for same activity type
    isIntersection: boolean,
    timeslot: Timeslot,
    isSelected: boolean
}

const props = defineProps<{
    timeslots:Timeslot[],
    startTime:string,
    endTime:string,
    slotDurationInMinutes:number,
    headGroup:Group,
    availableRanges:TimeslotWeekdayTimeRanges | null,
    conflictingTimeslots:Timeslot[]
}>();

const emit = defineEmits({
    selectTimeslot(payload:Timeslot){},
    selectTimerange(payload:{event:MouseEvent, timeRange:WeekdayTimeRange}){}
});

onMounted(()=>{
    setGroupRows(props.headGroup, props.timeslots);
    setGroupRowStarts();
});

watch(()=>props.timeslots, ()=>{
    setGroupRows(props.headGroup, props.timeslots);
    setGroupRowStarts();
},
{deep:true})

const headGroupSlots=computed(()=>
    props.timeslots.filter(ts=>
        groupRowCounts.value.some(grprc=>
            grprc.some((el=>
                el.headGroup.id===ts.group.id
            ))
        )
    ));
//reformat timeslots to factor in overlapping ones for same actovoty type
//i.e. clean strings to display on longer
const displaySlots = computed<SlotGridView[]>(()=>
    {
        let slots:SlotGridView[] = headGroupSlots.value.sort((a,b)=>{
                const startTimeComp = new Date(`"2000/01/01"${a.startTime}`).getTime() - new Date(`"2000/01/01"${b.startTime}`).getTime();
                return Math.floor(startTimeComp)/60000 === 0
                    ? new Date(`"2000/01/01"${a.endTime}`).getTime() - new Date(`"2000/01/01"${b.endTime}`).getTime()
                    : startTimeComp;
            }).map(ts=>{return {
                timeslot: ts,
                activities: [ts.activity.title],
                isOverriden: false,
                isIntersection: false,
                isSelected: ts===props.availableRanges?.timeslot
            }});

        const overlappingSlots:SlotGridView[] = [];

        for (let i=0; i<slots.length; i++)
        {
            const slot = slots[i];
            const ovelappingIndexes = slots.slice(i+1,)
                .map((s, idx)=>{
                    if (s.timeslot.dayOfWeek === slot.timeslot.dayOfWeek
                        && s.timeslot.activity.type?.baseType?.id === slot.timeslot.activity.type?.baseType?.id
                        && s.timeslot.group.id === slot.timeslot.group.id
                        && timeRangesOverlap(s.timeslot.startTime, s.timeslot.endTime, slot.timeslot.startTime, slot.timeslot.endTime) != null)
                    return idx + i+1;
                }).filter(idx => idx != undefined)
            
            if (ovelappingIndexes.length==0)
                continue;

            //since it overlaps with another slot it is marked to be left blank
            slots[i].isOverriden = true;

            for (const idx of ovelappingIndexes)
            {
                const newActivities = new Set<string>([...slots[i].activities, ...slots[idx].activities]);
                const overlapRange = timeRangesOverlap(slots[idx].timeslot.startTime, slots[idx].timeslot.endTime, slot.timeslot.startTime, slot.timeslot.endTime)
                //clone slot with different range
                const newTimeslot:Timeslot = {
                    id: 0,
                    timesheetId: slot.timeslot.timesheetId,
                    activity: slot.timeslot.activity,
                    hall: slot.timeslot.hall,
                    member: slot.timeslot.member,
                    group: slot.timeslot.group,
                    dayOfWeek: slot.timeslot.dayOfWeek,
                    startTime: overlapRange?.startTime ?? slot.timeslot.startTime,
                    endTime: overlapRange?.endTime ?? slot.timeslot.endTime,
                    optimizationStatus: ''
                } 
                //add new slot for intersection
                overlappingSlots.push({
                    isIntersection: true,
                    isOverriden: false,
                    timeslot: newTimeslot,
                    activities: [...newActivities],
                    isSelected: false
                });

                //since it overlaps with another slot it is marked to be left blank
                slots[idx].isOverriden = true;
            }
        }

        for (let i=0; i<overlappingSlots.length; i++)
        {
            const slot = overlappingSlots[i];
            
            //indexes of slots that overlap w/ current
            const ovelappingIndexes = overlappingSlots.slice(i+1,-1)
                .map((s, idx)=>{
                    if (s.timeslot.dayOfWeek === slot.timeslot.dayOfWeek
                        && s.timeslot.activity.type?.baseType === slot.timeslot.activity.type?.baseType
                        && s.timeslot.group === slot.timeslot.group
                        && timeRangesOverlap(s.timeslot.startTime, s.timeslot.endTime, slot.timeslot.startTime, slot.timeslot.endTime) != null)
                    return idx + i; //add 1 to index to account for removing current one
                }).filter(idx => idx != undefined)
            
            if (ovelappingIndexes.length==0)
                continue;
            console.log(overlappingSlots);

            overlappingSlots.splice(i, 1);
            i--;

            for (const idx of ovelappingIndexes)
            {
                const newActivities = new Set<string>([...slot.activities, ...slot.activities]);
                const overlapRange = timeRangesOverlap(overlappingSlots[idx].timeslot.startTime, overlappingSlots[idx].timeslot.endTime, slot.timeslot.startTime, slot.timeslot.endTime)
                //clone slot with different range
                const newTimeslot:Timeslot = {
                    id: 0,
                    timesheetId: slot.timeslot.timesheetId,
                    activity: slot.timeslot.activity,
                    hall: slot.timeslot.hall,
                    member: slot.timeslot.member,
                    group: slot.timeslot.group,
                    dayOfWeek: slot.timeslot.dayOfWeek,
                    startTime: overlapRange?.startTime ?? slot.timeslot.startTime,
                    endTime: overlapRange?.endTime ?? slot.timeslot.endTime,
                    optimizationStatus: ''
                } 

                overlappingSlots.push({
                    isIntersection: true,
                    isOverriden: false,
                    timeslot: newTimeslot,
                    activities: [...newActivities],
                    isSelected:false
                });
            }
            //remove other intersections
            for (let j = 0; j<ovelappingIndexes.length; j++)
            {
                overlappingSlots.splice(ovelappingIndexes[j]-j, 1);
                i--;
            }
        }

        slots = slots.sort((a ,b)=>timeDiffInMinutes(a.timeslot.startTime, a.timeslot.endTime) - timeDiffInMinutes(b.timeslot.startTime, b.timeslot.endTime));
        slots = slots.concat(overlappingSlots);

        return slots;
    }
);
//values for timeslot times as whole numbers representing number of slots
const timeslotStartInSlots = (startTime:string)=>Math.floor(timeDiffInMinutes(props.startTime, startTime)/props.slotDurationInMinutes);
const timeslotDurationInSlots = (startTime:string, endTime:string)=>Math.floor(timeDiffInMinutes(endTime, startTime)/props.slotDurationInMinutes);
const totalSlots = computed(()=>timeDiffInMinutes(props.startTime, props.endTime)/props.slotDurationInMinutes);

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
const weekOffset = (dayOfWeek: number) => dayOfWeek*totalRows.value;
const gridContainerClasses = computed(()=>`grid grid-cols-${totalSlots.value+1} grid-rows-${totalRows.value*5} h-300 w-9/10 m-auto`);
const gridSlotClasses = (timeslot:Timeslot)=>computed(()=>`col-start-${timeslotStartInSlots(timeslot.startTime)+2} col-span-${timeslotDurationInSlots(timeslot.startTime, timeslot.endTime)} row-start-${timeslotStartRow(timeslot).value+1} row-span-${timeslotSpan(timeslot).value}`);
const gridSlotRangeClasses = (range:WeekdayTimeRange)=>computed(()=>props.availableRanges!==null?
    `col-start-${timeslotStartInSlots(range.startTime)+2} 
    col-span-${timeslotDurationInSlots(range.startTime, range.endTime)} 
    row-start-${timeslotStartRow(props.availableRanges.timeslot).value + 1 + weekOffset(range.dayOfWeek - props.availableRanges.timeslot.dayOfWeek)} 
    row-span-${timeslotSpan(props.availableRanges.timeslot).value}`:'');
</script>

<template>
    <!-- class values prolly shouldnt be inline -->
    <h3>{{ `Timesheet for ${(headGroup.name)}` }}</h3>
    <div v-if="!Number.isNaN(totalSlots)" :class="`grid grid-cols-${totalSlots+1} h-10 w-9/10 m-auto`">
        <!-- shouldn be inline -->
        <div v-for="slot of totalSlots+1" :class="`text-right col-start-${slot} col-span-1 pl-full`" >{{ new Date(new Date("2000/01/01 " + startTime).getTime() + (slot-1) * slotDurationInMinutes * 60000).toLocaleTimeString('en-UK', { hour: '2-digit', minute: '2-digit', hour12: false })}}</div>
    </div>
    <div v-if="!Number.isNaN(totalRows)&&!Number.isNaN(totalSlots)" :class=gridContainerClasses class="border-1 border-black">
        <div v-for="row of totalRows*5" :class="`border-1 border-black text-right col-start-2 col-span-${totalSlots+1} row-start-${row} row-span-1`"></div>
        <div v-for="slot of totalSlots+1" :class="`border-1 border-black text-right col-start-${slot} col-span-1 row-start-1 row-span-${totalRows*5}`"></div>
        <div v-for="slotView in displaySlots"
            @click="$emit('selectTimeslot', slotView.timeslot)"
            :class="[gridSlotClasses(slotView.timeslot).value,
                slotView.isSelected?'z-10':'',
                conflictingTimeslots!==undefined && conflictingTimeslots.some(ts=>slotView.timeslot.activity.id==ts.activity.id&&slotView.timeslot.member?.id==ts.member?.id&&slotView.timeslot.group.id==ts.group.id&&slotView.timeslot.hall.id==ts.hall.id)?'bg-red-200':'']"
            class="border-box border-1 border-solid border-gray-500 text-center flex flex-col items-center justify-around  bg-gray-200 text-align text-xs">
            <div v-if="!slotView.isOverriden">
                <div v-if="slotView.isIntersection">{{ slotView.timeslot.activity.type?.baseType?.title }}</div>
                <div>{{ slotView.activities.join(' / ') }}</div>
                <div>{{ slotView.timeslot.member?.name }}</div>
                <div>{{ slotView.timeslot.hall.name }}</div>
                <div>{{ slotView.timeslot.group.name }}</div>
            </div>
        </div>
        <div v-for="availableRange in availableRanges?.weekdayTimeRanges" 
            @click="(e)=>$emit('selectTimerange', {event:e, timeRange:availableRange})"
            :class=gridSlotRangeClasses(availableRange).value class="border-box border-2 border-solid border-green-200 bg-green-200/25"> 
        </div>
        <div v-for="weekday in 5" :class="`vertical-text text-center row-start-${(weekday-1)*totalRows+1} row-span-${totalRows} col-start-1 col-span-1`">{{ dayOfTheWeek[weekday-1] }}</div>
    </div>
</template>