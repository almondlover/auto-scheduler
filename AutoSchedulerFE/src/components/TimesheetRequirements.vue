<script setup lang="ts">
import type { Activity, ActivityRequirements } from '@/classes/activity';
import { useGroupStore } from '@/stores/groupStore';
import { useTimesheetStore } from '@/stores/timesheetStore';
import { storeToRefs } from 'pinia';
import { computed, onMounted, ref, watch, type Ref } from 'vue';
import ActivityRequirementForm from './ActivityRequirementForm.vue';
import Button from './ui/button/Button.vue';
import { useActivityStore } from '@/stores/activityStore';
import type { GeneratorRequirements, Timesheet, Timeslot } from '@/classes/timesheet';
import Input from './ui/input/Input.vue';
import { Form } from 'vee-validate';
import FormItem from './ui/form/FormItem.vue';
import { FormField } from './ui/form';
import FormLabel from './ui/form/FormLabel.vue';
import FormControl from './ui/form/FormControl.vue';
import { createActivityRequirement, fetchActivityRequirementsForGroup } from '@/services/activityService';
import type { Group } from '@/classes/group';
import Accordion from './ui/accordion/Accordion.vue';
import AccordionItem from './ui/accordion/AccordionItem.vue';
import AccordionTrigger from './ui/accordion/AccordionTrigger.vue';
import AccordionContent from './ui/accordion/AccordionContent.vue';
import Card from './ui/card/Card.vue';
import CardContent from './ui/card/CardContent.vue';
import { dayOfTheWeek } from '@/constants/constants';
import { timeDiffInMinutes } from '@/utils/timediff';
import TimesheetGrid from './TimesheetGrid.vue';
import Dialog from './ui/dialog/Dialog.vue';
import DialogTrigger from './ui/dialog/DialogTrigger.vue';
import DialogContent from './ui/dialog/DialogContent.vue';
import CardHeader from './ui/card/CardHeader.vue';
import CardTitle from './ui/card/CardTitle.vue';
import Select from './ui/select/Select.vue';
import SelectTrigger from './ui/select/SelectTrigger.vue';
import SelectValue from './ui/select/SelectValue.vue';
import SelectContent from './ui/select/SelectContent.vue';
import { SelectIcon } from 'reka-ui';
import SelectItem from './ui/select/SelectItem.vue';

const groupStore = useGroupStore();
const { groups, current, currentGroup, currentOrganizationIdx } = storeToRefs(groupStore);
const activityStore = useActivityStore();
const { activityRequirements } = storeToRefs(activityStore);
const generatorRequirements:Ref<GeneratorRequirements>=ref({
    requirements: [],
    slotDurationInMinutes: 0,
    startTime: '0:00',
    endTime: 'T24:00Z'
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
    requirement.group=currentGroup.value;
    activityStore.addRequirementForGenerator(requirement);
};
//get unique groups w/out parent in current collection
const headGroups=computed(()=>{return timesheets.value.map(timesheet=>timesheet.timeslots.map(ts=>ts.group)
    .filter((grp, idx, array)=>
        idx===array.findIndex(grp2=>grp2.id===grp.id) && !array.some(grp2=>grp.parentGroupId!==undefined&&grp.parentGroupId===grp2.id)
    ))[0]});

const timesheetStore = useTimesheetStore();
const { timesheets, currentTimesheetIdx, currentTimesheet } = storeToRefs(timesheetStore);

const showRequrementsModal=ref(false);
const currentGroupRequirements:Ref<ActivityRequirements[]> = ref([]);

//maybe use computed instead not to have duplicate requirements on page?
const isAdded=(id:number)=>{
    return activityRequirements.value.findIndex(req=>req.id===id)!==-1;
};

const newTimesheet:Timesheet = {
    id: 0,
    title: '',
    active: false,
    optimized: false,
    timeslots: []
};

const handleTimesheetSave = (timeslots:Timeslot[]) => {
    newTimesheet.timeslots = timeslots;
    timesheetStore.saveTimesheet(newTimesheet);
    timesheetStore.resetTimesheets();
};

const handleCreatedRequirement = (newRequirement:ActivityRequirements)=>{
    createActivityRequirement(newRequirement); 
    currentGroupRequirements.value.push({...newRequirement});
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
                        {{ requirement.activity.title }} for {{ requirement.group?.name }}: {{ requirement.duration }} minutes
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
        <Card class="m-5" v-for="timesheet in timesheets">
            <CardContent class="flex flex-col items-start gap-5">
                <Input type="text" v-model="newTimesheet.title"/>
                <!-- <div v-for="timeslot in timesheet.timeslots">
                    {{ timeslot.activity.title }} for {{ timeslot.group.name }} with {{ timeslot.member?.name }} in {{ timeslot.hall.name }} at {{ timeslot.startTime }} - {{ timeslot.endTime }} on {{ dayOfTheWeek[(timeslot.dayOfWeek)] }}
                </div> -->
                <Button @click="handleTimesheetSave(timesheet.timeslots)">Save</Button>
            </CardContent>
        </Card>
        <Card class="m-5" v-for="timesheet in timesheets">
            <CardContent>
                <div v-for="headGroup of headGroups">
                    <TimesheetGrid  :timeslots="timesheet.timeslots" :start-time="generatorRequirements.startTime" :end-time="generatorRequirements.endTime" :slot-duration-in-minutes="generatorRequirements.slotDurationInMinutes" :head-group="headGroup"/>
                </div>
            </CardContent>
        </Card>
    </div>
</template>