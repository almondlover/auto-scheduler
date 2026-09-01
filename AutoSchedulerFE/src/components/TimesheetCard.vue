<script setup lang="ts">
import Card from './ui/card/Card.vue';
import CardContent from './ui/card/CardContent.vue';
import TimesheetGrid from './TimesheetGrid.vue';
import { TimesheetState, type GeneratorRequirements, type Timesheet, type TimesheetViewRequirements, type Timeslot, type TimeslotPlacementChange, type TimeslotRearrangement, type WeekdayTimeRange } from '@/classes/timesheet';
import { computed, onMounted, ref, watch, type Ref } from 'vue';
import { useTimesheetStore } from '@/stores/timesheetStore.ts';
import { storeToRefs } from 'pinia';
import Button from './ui/button/Button.vue';
import Select from './ui/select/Select.vue';
import SelectTrigger from './ui/select/SelectTrigger.vue';
import SelectValue from './ui/select/SelectValue.vue';
import SelectContent from './ui/select/SelectContent.vue';
import SelectItem from './ui/select/SelectItem.vue';
import { timeDiffInMinutes } from '@/utils/timediff.ts';
import type { ActivityRequirements, Hall } from '@/classes/activity.ts';
import { createActivityRequirement } from '@/services/activityService.ts';
import Input from './ui/input/Input.vue';
import { fetchRequirementsForTimesheet } from '@/services/timesheetService.ts';

const props = defineProps<{
    timesheet:Timesheet,
    generatorRequirements:GeneratorRequirements,
    title:string
}>();

onMounted(()=>{
    if (currentGeneratorRequirements.value.requirements == null || currentGeneratorRequirements.value.requirements.length==0)
        fetchRequirementsForTimesheet(props.timesheet.id).then(req => currentGeneratorRequirements.value.requirements = req);
});

watch(props.timesheet, ()=>{
    if (currentGeneratorRequirements.value.requirements == null || currentGeneratorRequirements.value.requirements.length==0)
        fetchRequirementsForTimesheet(props.timesheet.id).then(req => currentGeneratorRequirements.value.requirements = req);
});

const currentGeneratorRequirements:Ref<GeneratorRequirements> = ref(props.generatorRequirements);

//get unique groups w/out parent in current collection
const headGroups=computed(()=>{return props.timesheet.timeslots.map(ts=>ts.group)
    .filter((grp, idx, array)=>
        idx===array.findIndex(grp2=>grp2.id===grp.id) && !array.some(grp2=>grp.parentGroupId!==undefined&&grp.parentGroupId===grp2.id)
    )});

const generalBreakDuration = computed(()=>timeDiffInMinutes(props.generatorRequirements.generalBreakStartTime??'', props.generatorRequirements.generalBreakEndTime??'') - props.generatorRequirements.breakDurationInMinutes);

const timesheetStore = useTimesheetStore();
const { timesheets, selectedTimeslot, availableRanges, timeslots, availableHalls, requirements } = storeToRefs(timesheetStore);

const newTimesheetTitle:Ref<string> = ref(props.title);

const selectedHall:Ref<Hall> = ref({
    id: 0,
    organizationId: 0,
    name: '',
    description: undefined,
    size: 0,
    availability: undefined,
    type: {
        id: 0,
        title: '',
        description: undefined
    }
});

const handleTimesheetSave = (timesheet:Timesheet) => {
    const newTimesheet = {...timesheet};
    newTimesheet.title = newTimesheetTitle.value;
    timesheetStore.resetTimesheets();
    timesheetStore.saveTimesheet(newTimesheet);
};

const handleTimesheetUpdate = (timesheet:Timesheet) => {
    timesheetStore.modifyTimesheet(timesheet);
}

const handleActiveTimesheet = (timesheet:Timesheet) => {
    timesheetStore.makeTimesheetActive(timesheet.id);
    timesheetStore.resetTimesheets();
}

const handleTimesheetRegenerate = () => {
    if (selectedTimeslot.value==null) return;

    const timeslotRearrangement:TimeslotRearrangement = {
            generatorRequirements: currentGeneratorRequirements.value,
            lockedTimeslots: [selectedTimeslot.value]
        }
    selectedTimeslot.value=null;
    timeslots.value=[];
    availableRanges.value = null;
    timesheetStore.regenerateTimesheet(timeslotRearrangement);
}

const handleTimesheetPartialRegenerate = (timesheet:Timesheet) =>{
    if (selectedTimeslot.value==null) return;
    
    //filter non-conflicting slots to keep in place
    const lockedTimeslots = timesheet.timeslots.filter(ts => !timeslots.value.some(ts1 => ts1.activity.id==ts.activity.id&&ts1.member?.id==ts.member?.id&&ts1.group.id==ts.group.id&&ts1.hall.id==ts.hall.id));//would probably need to save as draft first to compare ids
    
    const timeslotRearrangement:TimeslotRearrangement = {
            generatorRequirements: currentGeneratorRequirements.value,
            lockedTimeslots: lockedTimeslots
        }
    selectedTimeslot.value=null;
    timeslots.value=[];
    availableRanges.value = null;
    timesheetStore.regenerateTimesheet(timeslotRearrangement);
}

const handleHallChange = (timesheet:Timesheet) => {
    if (selectedTimeslot.value == null || selectedHall.value.id == 0) return;
    
    selectedTimeslot.value.hall = selectedHall.value

    const timeslotChange:TimeslotPlacementChange = {
            generatorRequirements: currentGeneratorRequirements.value,
            timeslotsForSheet: timesheet.timeslots,
            changedTimeslot: selectedTimeslot.value
        }
    
    timesheetStore.getAvailableSpaceForTimeslot(timeslotChange);
        //display conflicting slots on selecting one
    timesheetStore.getConflictingTimeslots(timeslotChange);

    timesheetStore.getAvailableHallsForTimeslot(timeslotChange);
}

const handleTimeslotSelect = (timeslot:Timeslot, timesheet:Timesheet) => {
    if (selectedTimeslot.value == timeslot)
    {
        //reset range visibility on repeated selection
        selectedTimeslot.value = null;
        availableRanges.value = null;
        timeslots.value = [];
    }
    else 
    {
        selectedTimeslot.value = timeslot;

        const timeslotChange:TimeslotPlacementChange = {
            generatorRequirements: currentGeneratorRequirements.value,
            timeslotsForSheet: timesheet.timeslots,
            changedTimeslot: timeslot
        }

        timesheetStore.getAvailableSpaceForTimeslot(timeslotChange);
        
        //display conflicting slots on selecting one
        timesheetStore.getConflictingTimeslots(timeslotChange);

        timesheetStore.getAvailableHallsForTimeslot(timeslotChange);
    }
}

const handleTimerangeSelect = (event:MouseEvent, timerange:WeekdayTimeRange, timesheet:Timesheet) => {
    if (event.target instanceof Element && selectedTimeslot.value!==null)
    {
        const rect = event.target.getBoundingClientRect();
        const relativePos = event.offsetX / rect.width;
        const fullSlotDuration = props.generatorRequirements.slotDurationInMinutes+props.generatorRequirements.breakDurationInMinutes
        const slotSpan = timeDiffInMinutes(timerange.startTime, timerange.endTime) / fullSlotDuration

        const bigBreakInSlot = selectedTimeslot.value.startTime < (props.generatorRequirements.generalBreakEndTime??selectedTimeslot.value.startTime) && selectedTimeslot.value.endTime > (props.generatorRequirements.generalBreakEndTime??selectedTimeslot.value.startTime);

        const selectedSlotSpan = (timeDiffInMinutes(selectedTimeslot.value.startTime, selectedTimeslot.value.endTime) - (bigBreakInSlot ? generalBreakDuration.value : 0)) / fullSlotDuration

        

        const generalBreakSlot = (props.generatorRequirements.generalBreakEndTime ?? timerange.startTime) < timerange.startTime
            ? -1
            : Math.floor((timeDiffInMinutes(props.generatorRequirements.generalBreakEndTime??timerange.startTime, timerange.startTime)-generalBreakDuration.value)/fullSlotDuration);
        //find start slot position from mouse poosition relative to element and num of slots in timerange
        const startSlot = Math.floor(relativePos * slotSpan);
        //make sure placing slot here will fit within range
        if (startSlot > slotSpan-selectedSlotSpan) return;
        const startTime = new Date(new Date("2000/01/01 " + timerange.startTime).getTime()
                                    + (startSlot * fullSlotDuration + ((startSlot >= generalBreakSlot && generalBreakSlot>-1) ? generalBreakDuration.value : 0)) * 60000).toLocaleTimeString('en-UK', { hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: false });
        const endTime = new Date(new Date("2000/01/01 " + startTime).getTime()
                                + (selectedSlotSpan * fullSlotDuration + ((startSlot < generalBreakSlot && startSlot+selectedSlotSpan > generalBreakSlot) ? generalBreakDuration.value : 0)) * 60000).toLocaleTimeString('en-UK', { hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: false });
        
        
        selectedTimeslot.value.startTime = startTime;
        selectedTimeslot.value.endTime = endTime;
        selectedTimeslot.value.dayOfWeek = timerange.dayOfWeek;

        const timeslotChange:TimeslotPlacementChange = {
            generatorRequirements: currentGeneratorRequirements.value,
            timeslotsForSheet: timesheet.timeslots,
            changedTimeslot: selectedTimeslot.value
        }

        timesheetStore.getConflictingTimeslots(timeslotChange);
    }
}

const handleTimesheetDelete = (timesheet:Timesheet) => {
    if (timesheet.state!==TimesheetState.Active) 
        timesheetStore.removeTimesheet(timesheet.id);
    else timesheetStore.makeTimesheetInactive(timesheet.id);
}

</script>
<template>
    <Card v-show="props.timesheet.id === 0" class="m-5">
        <CardContent class="flex flex-col items-start gap-5">
            <Input type="text" v-model="newTimesheetTitle"/>
            <Button @click="handleTimesheetSave(props.timesheet)">Save as draft</Button>
        </CardContent>
    </Card>
    <!-- potentially leave as slot and pass header content (title, buttons, et.c) -->
    <h3 class="font-semibold text-lg mx-10 my-5">{{ newTimesheetTitle }}</h3>
    <div v-show="props.timesheet.id > 0">
        <Button class="mx-10"  @click="handleActiveTimesheet(props.timesheet)">Make active</Button>
        <Button @click="handleTimesheetUpdate(props.timesheet)">Save changes</Button>
        <Button class="mx-10 bg-red-500" @click="handleTimesheetDelete(timesheet)">{{timesheet.state===TimesheetState.Active?'Deactivate':'Delete Permanently'}}</Button>
    </div>
    <Card class="m-5">
        <CardContent>
            <Button v-show="selectedTimeslot!=null && timeslots.length>0" class="m-5" @click="handleTimesheetRegenerate">Rearrange sheet</Button>
            <Button v-show="selectedTimeslot!=null && timeslots.length>0" class="m-5" @click="handleTimesheetPartialRegenerate(props.timesheet)">Rearrange conflicting</Button>
            <div v-for="headGroup of headGroups">
                <TimesheetGrid @select-timeslot="(e)=>handleTimeslotSelect(e, props.timesheet)"
                    @select-timerange="(e)=>handleTimerangeSelect(e.event, e.timeRange, props.timesheet)"
                    :timeslots="props.timesheet.timeslots" 
                    :start-time="generatorRequirements.startTime" 
                    :end-time="generatorRequirements.endTime" 
                    :slot-duration-in-minutes="generatorRequirements.slotDurationInMinutes+generatorRequirements.breakDurationInMinutes" 
                    :head-group="headGroup"
                    :available-ranges="availableRanges"
                    :conflicting-timeslots="timeslots"
                    :general-break-start="generatorRequirements.generalBreakStartTime"
                    :general-break-duration="generalBreakDuration" />
            </div>
            <Card class="fixed top-5 left-0 right-0 w-1/3 m-auto z-20" v-show="selectedTimeslot!=null">
                <CardContent>
                    <p class="m-1">
                    {{ selectedTimeslot?.activity.title }} for {{ selectedTimeslot?.group.name }} with {{ selectedTimeslot?.member?.name }} in {{ selectedTimeslot?.hall.name }} from {{ selectedTimeslot?.startTime }} to {{ selectedTimeslot?.endTime }}
                    </p>
                    <Select v-model="selectedHall">
                        <SelectTrigger class="m-1">
                            <SelectValue placeholder="Choose available hall at this time"/>
                        </SelectTrigger>
                        <SelectContent>
                            <SelectItem v-for="hall in availableHalls" :value="hall">
                                {{ hall.name }}
                            </SelectItem>
                        </SelectContent>
                    </Select>
                    <Button class="m-1" @click="handleHallChange(props.timesheet)">Change Hall</Button>
                </CardContent>
            </Card>
        </CardContent>
    </Card>
</template>