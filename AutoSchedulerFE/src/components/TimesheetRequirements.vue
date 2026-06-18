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
import Dialog from './ui/dialog/Dialog.vue';
import DialogTrigger from './ui/dialog/DialogTrigger.vue';
import DialogContent from './ui/dialog/DialogContent.vue';
import CardHeader from './ui/card/CardHeader.vue';
import CardTitle from './ui/card/CardTitle.vue';
import TimesheetCard from './TimesheetCard.vue';

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

const timesheetStore = useTimesheetStore();
const { timesheets } = storeToRefs(timesheetStore);

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

const showRequrementsModal=ref(false);
const currentGroupRequirements:Ref<ActivityRequirements[]> = ref([]);

//maybe use computed instead not to have duplicate requirements on page?
const isAdded=(id:number)=>{
    return activityRequirements.value.findIndex(req=>req.id===id)!==-1;
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
            <TimesheetCard v-for="timesheet in timesheets" 
                :timesheet="timesheet" 
                :generator-requirements="generatorRequirements"
                :title="''">
            </TimesheetCard>
    </div>
</template>