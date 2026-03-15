<script setup lang="ts">
import Card from './ui/card/Card.vue';
import CardContent from './ui/card/CardContent.vue';
import TimesheetGrid from './TimesheetGrid.vue';
import type { Timesheet, TimesheetViewRequirements } from '@/classes/timesheet';
import { computed } from 'vue';

const props = defineProps<{
    timesheet:Timesheet,
    timesheetViewConfig:TimesheetViewRequirements,
}>();

//get unique groups w/out parent in current collection
const headGroups=computed(()=>{return props.timesheet.timeslots.map(ts=>ts.group)
    .filter((grp, idx, array)=>
        idx===array.findIndex(grp2=>grp2.id===grp.id) && !array.some(grp2=>grp.parentGroupId!==undefined&&grp.parentGroupId===grp2.id)
    )});
</script>
<template>
    <Card class="m-5">
        <CardContent>
            <div v-for="headGroup of headGroups">
                <TimesheetGrid  :timeslots="timesheet.timeslots" :start-time="props.timesheetViewConfig.startTime" :end-time="props.timesheetViewConfig.endTime" :slot-duration-in-minutes="props.timesheetViewConfig.slotDurationInMinutes" :head-group="headGroup"/>
            </div>
        </CardContent>
    </Card>
</template>