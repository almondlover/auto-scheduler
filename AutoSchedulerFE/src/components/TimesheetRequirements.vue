<script setup lang="ts">
import type { Activity, ActivityRequirements } from '@/classes/activity';
import { useGroupStore } from '@/stores/groupStore';
import { useTimesheetStore } from '@/stores/timesheetStore';
import { storeToRefs } from 'pinia';
import { onMounted, ref, watch, type Ref } from 'vue';
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
import { fetchActivityRequirementsForGroup } from '@/services/activityService';
import type { Group } from '@/classes/group';
import Accordion from './ui/accordion/Accordion.vue';
import AccordionItem from './ui/accordion/AccordionItem.vue';
import AccordionTrigger from './ui/accordion/AccordionTrigger.vue';
import AccordionContent from './ui/accordion/AccordionContent.vue';
import Card from './ui/card/Card.vue';
import CardContent from './ui/card/CardContent.vue';
import { dayOfTheWeek } from '@/constants/constants';
import { timeDiffInMinutes } from '@/utils/timediff';

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
        console.log(generatorRequirements.value);
    }
};

const handleNewRequirement = (requirement:ActivityRequirements)=>{
    requirement.group=currentGroup.value;
    activityStore.addRequirementForGenerator(requirement);
};

const timesheetStore = useTimesheetStore();
const { timesheets, currentTimesheetIdx, currentTimesheet } = storeToRefs(timesheetStore);

watch(timesheets, ()=>{

});

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
    newTimesheet.timeslots = timeslots
    timesheetStore.saveTimesheet(newTimesheet);
    timesheetStore.resetTimesheets();
};
</script>

<template>
    <!-- should open a modal for the current group, general requirements on a seperate page -->
    <Button @click="showRequrementsModal=!showRequrementsModal">Add requirement</Button>
    <Form>
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
    </Form>
    <Button type="submit" @click="handleTimesheetGenerate">Generate</Button>
    <select v-model="current" @change="fetchActivityRequirementsForGroup(current).then(res=>currentGroupRequirements=res)">
        <option v-for="group in groups" :value="group.id">{{group.name}}</option>
    </select>
    <!-- should probably go in seperate component -->
    <div v-show="current>0">
        <h3>Requirements</h3>
        <div v-for="requirement in currentGroupRequirements">
            <p>
                Activity: {{requirement.activity?.title}}
            </p>
            <p>
                Presenter: {{requirement.member.name}}
            </p>
            <p>
                Duration: {{requirement.duration}} minutes
            </p>
            <p>
                Hall Type: {{requirement.halltype?.title}}
            </p>
            <Button v-show="!isAdded(requirement.id)" @click="handleNewRequirement(requirement)">+</Button>
        </div>
    </div>
    <Accordion collapsible>
        <AccordionItem value="groups">
            <AccordionTrigger>
                Activities for Timesheet
            </AccordionTrigger>
            <AccordionContent>
                <div v-for="requirement in activityRequirements" class="flex h-10 items-center justify-between">
                    <div>
                        {{ requirement.activity.title }} for {{ requirement.group?.name }}: {{ requirement.duration }} minutes
                    </div>
                    <Button @click="activityStore.removeRequirementForGenerator(requirement)" >Remove</Button>
                </div>
            </AccordionContent>
        </AccordionItem>
    </Accordion>
    <div v-show="showRequrementsModal">
        <ActivityRequirementForm/>
    </div>
    <div>
        <h3>Generated</h3>
        <Card v-for="timesheet in timesheets">
            <CardContent>
                <Input type="text" v-model="newTimesheet.title"/>
                <div v-for="timeslot in timesheet.timeslots">
                    {{ timeslot.activity.title }} for {{ timeslot.group.name }} with {{ timeslot.member?.name }} in {{ timeslot.hall.name }} at {{ timeslot.startTime }} - {{ timeslot.endTime }} on {{ dayOfTheWeek[parseInt(timeslot.dayOfWeek)] }}
                </div>
                <Button @click="handleTimesheetSave(timesheet.timeslots)">Save</Button>
            </CardContent>
        </Card>
        <Card v-for="timesheet in timesheets" @vue:mounted="">
            <CardContent>
                <!-- class values prolly shouldnt be inline
                <div :class="`grid grid-gap-1 grid-rows-${totalSlots} grid-cols-20 w-200 h-100`">
                    <div v-for="timeslot in timesheet.timeslots" :class="`row-start-${timeslotStartInSlots(timeslot)} row-span-${timeslotDurationInSlots(timeslot)} bg-black`">here</div>
                </div> -->
            </CardContent>
        </Card>
    </div>
</template>