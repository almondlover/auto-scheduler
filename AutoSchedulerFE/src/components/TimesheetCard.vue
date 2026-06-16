<script setup lang="ts">
import Card from './ui/card/Card.vue';
import CardContent from './ui/card/CardContent.vue';
import TimesheetGrid from './TimesheetGrid.vue';
import { TimesheetState, type Timesheet, type TimesheetViewRequirements } from '@/classes/timesheet';
import { computed } from 'vue';
import { useTimesheetStore } from '@/stores/timesheetStore.ts';
import { storeToRefs } from 'pinia';
import Button from './ui/button/Button.vue';

const props = defineProps<{
    timesheet:Timesheet,
    timesheetViewConfig:TimesheetViewRequirements,
}>();

const timesheetStore = useTimesheetStore();

//get unique groups w/out parent in current collection
const headGroups=computed(()=>{return props.timesheet.timeslots.map(ts=>ts.group)
    .filter((grp, idx, array)=>
        idx===array.findIndex(grp2=>grp2.id===grp.id) && !array.some(grp2=>grp.parentGroupId!==undefined&&grp.parentGroupId===grp2.id)
    )});

const handleTimesheetDelete = (timesheet:Timesheet) => {
    if (timesheet.state!==TimesheetState.Active) 
        timesheetStore.removeTimesheet(timesheet.id);
    else timesheetStore.makeTimesheetInactive(timesheet.id);
}

</script>
<template>
    <h3 class="font-semibold text-lg mx-5">{{ timesheet.title }}</h3>
    <Button class="mx-10" v-show="timesheet.state===TimesheetState.Draft" @click="timesheetStore.makeTimesheetActive(timesheet.id)">Make active</Button>
    <Button class="mx-10" v-show="timesheet.state===TimesheetState.Draft" @click="timesheetStore.modifyTimesheet(timesheet)">Save changes</Button>
    <Button class="mx-10" @click="handleTimesheetDelete(timesheet)">{{timesheet.state===TimesheetState.Active?'Deactivate':'Delete Permanently'}}</Button>
    <Card class="m-5">
        <CardContent>
            <div v-for="headGroup of headGroups">
                <TimesheetGrid  
                    :timeslots="timesheet.timeslots" 
                    :start-time="props.timesheetViewConfig.startTime" 
                    :end-time="props.timesheetViewConfig.endTime" 
                    :slot-duration-in-minutes="props.timesheetViewConfig.slotDurationInMinutes" 
                    :head-group="headGroup" 
                    :available-ranges="null" 
                    :conflicting-timeslots="[]"/>
            </div>
        </CardContent>
    </Card>
</template>