<script setup lang="ts">
import type { ActivityRequirements, Hall } from '@/classes/activity';
import { useGroupStore } from '@/stores/groupStore';
import { useTimesheetStore } from '@/stores/timesheetStore';
import { storeToRefs } from 'pinia';
import { computed, onMounted, ref, watch, type Ref } from 'vue';
import ActivityRequirementForm from './ActivityRequirementForm.vue';
import Button from './ui/button/Button.vue';
import { useActivityStore } from '@/stores/activityStore';
import { TimesheetState, type GeneratorRequirements, type Timesheet, type Timeslot, type TimeslotPlacementChange, type TimeslotRearrangement, type WeekdayTimeRange } from '@/classes/timesheet';
import Input from './ui/input/Input.vue';
import { Form } from 'vee-validate';
import FormItem from './ui/form/FormItem.vue';
import { FormField } from './ui/form';
import FormLabel from './ui/form/FormLabel.vue';
import FormControl from './ui/form/FormControl.vue';
import { createActivityRequirement, fetchActivityRequirementsForGroup } from '@/services/activityService';
import Accordion from './ui/accordion/Accordion.vue';
import AccordionItem from './ui/accordion/AccordionItem.vue';
import AccordionTrigger from './ui/accordion/AccordionTrigger.vue';
import AccordionContent from './ui/accordion/AccordionContent.vue';
import Card from './ui/card/Card.vue';
import CardContent from './ui/card/CardContent.vue';
import TimesheetGrid from './TimesheetGrid.vue';
import Dialog from './ui/dialog/Dialog.vue';
import DialogTrigger from './ui/dialog/DialogTrigger.vue';
import DialogContent from './ui/dialog/DialogContent.vue';
import CardHeader from './ui/card/CardHeader.vue';
import CardTitle from './ui/card/CardTitle.vue';
import { timeDiffInMinutes } from '@/utils/timediff.ts';
import Select from './ui/select/Select.vue';
import SelectTrigger from './ui/select/SelectTrigger.vue';
import SelectValue from './ui/select/SelectValue.vue';
import SelectContent from './ui/select/SelectContent.vue';
import SelectItem from './ui/select/SelectItem.vue';

const groupStore = useGroupStore();
const { groups, current, currentGroup, currentOrganizationIdx } = storeToRefs(groupStore);
const activityStore = useActivityStore();
const { activityRequirements } = storeToRefs(activityStore);
const generatorRequirements:Ref<GeneratorRequirements>=ref({
    requirements: [],
    slotDurationInMinutes: 30,
    breakDurationInMinutes: 0,
    startTime: '09:00',
    endTime: '17:00'
});

onMounted(()=>{
    groupStore.getGroupsForOrganization(currentOrganizationIdx.value);
    fetchActivityRequirementsForGroup(current.value).then(res=>currentGroupRequirements.value=res);
});

watch(currentOrganizationIdx, ()=>{
        groupStore.getGroupsForOrganization(currentOrganizationIdx.value);
    }
);

const handleTimesheetGenerate = ()=>{
    if (activityRequirements.value?.length>0)
    {
        generatorRequirements.value.requirements = activityRequirements.value;
        timesheetStore.generateTimesheet(generatorRequirements.value);
    }
};

const handleNewRequirement = (requirement:ActivityRequirements)=>{

    activityStore.addRequirementForGenerator(requirement);
};

const addAllrequirementsForGroup = () => {
    for (let requirement of currentGroupRequirements.value)
        handleNewRequirement(requirement);
}
//get unique groups w/out parent in current collection
const headGroups=computed(()=>{return timesheets.value.map(timesheet=>timesheet.timeslots.map(ts=>ts.group)
    .filter((grp, idx, array)=>
        idx===array.findIndex(grp2=>grp2.id===grp.id) && !array.some(grp2=>grp.parentGroupId!==undefined&&grp.parentGroupId===grp2.id)
    ))[0]});

const timesheetStore = useTimesheetStore();
const { timesheets, selectedTimeslot, availableRanges, timeslots, availableHalls } = storeToRefs(timesheetStore);

const showRequrementsModal=ref(false);
const currentGroupRequirements:Ref<ActivityRequirements[]> = ref([]);

//maybe use computed instead not to have duplicate requirements on page?
const isAdded=(id:number)=>{
    return activityRequirements.value.findIndex(req=>req.id===id)!==-1;
};

const newTimesheetTitle:Ref<string> = ref('');

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
    timesheet.title = newTimesheetTitle.value;
    timesheetStore.resetTimesheets();
    timesheetStore.saveTimesheet(timesheet);
};

const handleTimesheetUpdate = (timesheet:Timesheet) => {
    timesheetStore.modifyTimesheet(timesheet);
}

const handleActiveTimesheet = (timesheet:Timesheet) => {
    timesheetStore.makeTimesheetActive(timesheet.id)
    timesheetStore.resetTimesheets();
}

const handleCreatedRequirement = (newRequirement:ActivityRequirements)=>{
    createActivityRequirement(newRequirement); 
    currentGroupRequirements.value.push({...newRequirement});
}

const handleTimesheetRegenerate = () => {
    if (selectedTimeslot.value==null) return;
    
    const timeslotRearrangement:TimeslotRearrangement = {
            generatorRequirements: generatorRequirements.value,
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
            generatorRequirements: generatorRequirements.value,
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
            generatorRequirements: generatorRequirements.value,
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
        selectedTimeslot.value = timeslot

        const timeslotChange:TimeslotPlacementChange = {
            generatorRequirements: generatorRequirements.value,
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
        const fullSlotDuration = generatorRequirements.value.slotDurationInMinutes+generatorRequirements.value.breakDurationInMinutes
        const slotSpan = timeDiffInMinutes(timerange.startTime, timerange.endTime) / fullSlotDuration
        const selectedSlotSpan = timeDiffInMinutes(selectedTimeslot.value.startTime, selectedTimeslot.value.endTime) / fullSlotDuration
        //find start slot position from mouse poosition relative to element and num of slots in timerange
        const startSlot = Math.floor(relativePos * slotSpan);
        //make sure placing slot here will fit within range
        if (startSlot > slotSpan-selectedSlotSpan) return;
        const startTime = new Date(new Date("2000/01/01 " + timerange.startTime).getTime() + startSlot * fullSlotDuration * 60000).toLocaleTimeString('en-UK', { hour: '2-digit', minute: '2-digit', hour12: false });
        const endTime = new Date(new Date("2000/01/01 " + startTime).getTime() + selectedSlotSpan * fullSlotDuration * 60000).toLocaleTimeString('en-UK', { hour: '2-digit', minute: '2-digit', hour12: false });

        selectedTimeslot.value.startTime = startTime;
        selectedTimeslot.value.endTime = endTime;
        selectedTimeslot.value.dayOfWeek = timerange.dayOfWeek;

        const timeslotChange:TimeslotPlacementChange = {
            generatorRequirements: generatorRequirements.value,
            timeslotsForSheet: timesheet.timeslots,
            changedTimeslot: selectedTimeslot.value
        }

        timesheetStore.getConflictingTimeslots(timeslotChange);
    }
}
</script>

<template>
    <!-- should open a modal for the current group, general requirements on a seperate page -->
    <Dialog>
      <DialogTrigger>
        <Button class="mx-10">Add new activity requirement</Button>
      </DialogTrigger>
      <DialogContent>
        <ActivityRequirementForm @created="(e)=>handleCreatedRequirement(e)" class="flex flex-col gap-5"/>
      </DialogContent>
    </Dialog>
    <select v-model="current" @change="fetchActivityRequirementsForGroup(current).then(res=>currentGroupRequirements=res)">
        <option v-for="group in groups" :value="group.id">{{group.name}}</option>
    </select>
    <!-- should probably go in seperate component -->
    <div class="m-5" v-show="current>0">
        <h3 class="text-lg font-bold">Requirements for selected group</h3> 
        <Button class="w-1/4 mt-3" @click="addAllrequirementsForGroup">Add all</Button>
        <div class="flex flex-wrap flex-row gap-5 bg-secondary rounded-md p-5">
            <Card class="bg-light" v-for="requirement in currentGroupRequirements">
                <CardHeader>
                    <CardTitle>{{requirement.activity?.title}}</CardTitle>
                </CardHeader>
                <CardContent class="">
                    <p>
                        Presenter: {{requirement.member.name}}
                    </p>
                    <p>
                        Duration: {{requirement.duration}} minutes
                    </p>
                    <p>
                        Hall Type: {{requirement.hallType?.title}}
                    </p>
                    <p>
                        Groups: {{requirement.groups.map(g=>g.name).concat().toString()}}
                    </p>
                    <Button class="w-1/4 mt-3" v-show="!isAdded(requirement.id)" @click="handleNewRequirement(requirement)">+</Button>
                </CardContent>
            </Card>
        </div>
    </div>
    <Form class="flex flex-row items-center justify-around m-auto my-5">
        <FormField name="startTime">
            <FormItem>
                <FormLabel>Daily Start Time</FormLabel>
                <FormControl>
                    <Input type="time" v-model="generatorRequirements.startTime"/>
                </FormControl>
            </FormItem>
        </FormField>
        <FormField name="endTime">
            <FormItem>
                <FormLabel>Daily End Time</FormLabel>
                <FormControl>
                    <Input type="time" v-model="generatorRequirements.endTime"/>
                </FormControl>
            </FormItem>
        </FormField>
        <FormField name="duration">
            <FormItem>
                <FormLabel>Slot duration in minutes</FormLabel>
                <FormControl>
                    <Input type="number" v-model="generatorRequirements.slotDurationInMinutes"/>
                </FormControl>
            </FormItem>
        </FormField>
        <FormField name="breakDuration">
            <FormItem>
                <FormLabel>Break duration per slot in minutes</FormLabel>
                <FormControl>
                    <Input type="number" v-model="generatorRequirements.breakDurationInMinutes"/>
                </FormControl>
            </FormItem>
        </FormField>
        <Button type="submit" @click.prevent="handleTimesheetGenerate">Generate</Button>
    </Form>
    <Accordion class="m-5" collapsible>
        <AccordionItem value="groups">
            <AccordionTrigger class="bg-primary text-white text-md p-5">
                Activities for Timesheet
            </AccordionTrigger>
            <AccordionContent>
                <div v-for="requirement in activityRequirements" class="flex h-10 items-center justify-between">
                    <div>
                        {{ requirement.activity.title }} for {{ requirement.groups.map(g=>g.name).toString().concat() }}: {{ requirement.duration }} minutes
                    </div>
                    <Button @click.prevent="activityStore.removeRequirementForGenerator(requirement)" >Remove</Button>
                </div>
            </AccordionContent>
        </AccordionItem>
    </Accordion>
    <div v-show="showRequrementsModal">
        <ActivityRequirementForm/>
    </div>
    <div>
        <h3 class="mx-5 font-bold text-lg">Generated</h3>
        <div v-for="timesheet in timesheets">
            <Card class="m-5">
                <CardContent class="flex flex-col items-start gap-5">
                    <Input type="text" v-model="newTimesheetTitle"/>
                    <Button v-show="timesheet.id>0" @click="handleActiveTimesheet(timesheet)">Make active</Button>
                    <Button @click="timesheet.id==0 ? handleTimesheetSave(timesheet) : handleTimesheetUpdate(timesheet)">{{timesheet.id==0?'Save as draft':'Save changes'}}</Button>
                </CardContent>
            </Card>
            <Card class="m-5">
                <CardContent>
                    <Button v-show="selectedTimeslot!=null && timeslots.length>0" class="m-5" @click="handleTimesheetRegenerate">Rearrange sheet</Button>
                    <Button v-show="selectedTimeslot!=null && timeslots.length>0" class="m-5" @click="handleTimesheetPartialRegenerate(timesheet)">Rearrange conflicting</Button>
                    <div v-for="headGroup of headGroups">
                        <TimesheetGrid @select-timeslot="(e)=>handleTimeslotSelect(e, timesheet)"
                            @select-timerange="(e)=>handleTimerangeSelect(e.event, e.timeRange, timesheet)"
                            :timeslots="timesheet.timeslots" 
                            :start-time="generatorRequirements.startTime" 
                            :end-time="generatorRequirements.endTime" 
                            :slot-duration-in-minutes="generatorRequirements.slotDurationInMinutes+generatorRequirements.breakDurationInMinutes" 
                            :head-group="headGroup"
                            :available-ranges="availableRanges"
                            :conflicting-timeslots="timeslots" />
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
                            <Button class="m-1" @click="handleHallChange(timesheet)">Change Hall</Button>
                        </CardContent>
                    </Card>
                </CardContent>
            </Card>
        </div>
    </div>
</template>