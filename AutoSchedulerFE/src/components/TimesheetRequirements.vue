<script setup lang="ts">
import type { Activity, ActivityRequirements } from '@/classes/activity';
import { useGroupStore } from '@/stores/groupStore';
import { useTimesheetStore } from '@/stores/timesheetStore';
import { storeToRefs } from 'pinia';
import { onMounted, ref, watch, type Ref } from 'vue';
import ActivityRequirementForm from './ActivityRequirementForm.vue';
import Button from './ui/button/Button.vue';
import { useActivityStore } from '@/stores/activityStore';
import type { GeneratorRequirements } from '@/classes/timesheet';
import Input from './ui/input/Input.vue';
import { Form } from 'vee-validate';
import FormItem from './ui/form/FormItem.vue';
import { FormField } from './ui/form';
import FormLabel from './ui/form/FormLabel.vue';
import FormControl from './ui/form/FormControl.vue';
import { fetchActivityRequirementsForGroup } from '@/services/activityService';
import type { Group } from '@/classes/group';

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

const showRequrementsModal=ref(false);
const currentGroupRequirements:Ref<ActivityRequirements[]> = ref([]);
</script>

<template>
    <select v-model="current" @change="fetchActivityRequirementsForGroup(current).then(res=>currentGroupRequirements=res)">
        <option v-for="group in groups" :value="group.id">{{group.name}}</option>
    </select>
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
    <!-- should probably go in seperate component -->
    <div v-show="current>0">
        <h3>Requirements</h3>
        <div v-for="requirement in currentGroupRequirements">
            <p>
                Activity: {{requirement.activity?.title}}
            </p>
            <p>
                Duration: {{requirement.duration}}
            </p>
            <p>
                Hall Type: {{requirement.halltype}}
            </p>
            <Button @click="handleNewRequirement(requirement)">+</Button>
        </div>
    </div>
    <div v-show="showRequrementsModal">
        <ActivityRequirementForm/>
    </div>
</template>