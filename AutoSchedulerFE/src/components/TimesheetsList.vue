<script setup lang="ts">
import { useTimesheetStore } from '@/stores/timesheetStore';
import { storeToRefs } from 'pinia';
import { onMounted, ref, watch, type Ref } from 'vue';
import Button from './ui/button/Button.vue';
import Card from './ui/card/Card.vue';
import CardTitle from './ui/card/CardTitle.vue';
import CardContent from './ui/card/CardContent.vue';
import CardDescription from './ui/card/CardDescription.vue';
import CardHeader from './ui/card/CardHeader.vue';
import Select from './ui/select/Select.vue';
import SelectTrigger from './ui/select/SelectTrigger.vue';
import SelectContent from './ui/select/SelectContent.vue';
import SelectValue from './ui/select/SelectValue.vue';
import SelectItem from './ui/select/SelectItem.vue';
import { useGroupStore } from '@/stores/groupStore';
import TimesheetCard from './TimesheetCard.vue';
import TimesheetDisplayForm from './TimesheetDisplayForm.vue';
import type { TimesheetViewRequirements } from '@/classes/timesheet';
import type { Group } from '@/classes/group';

const timesheetStore = useTimesheetStore();
const groupStore = useGroupStore();
const { timesheets } = storeToRefs(timesheetStore);
const {currentOrganizationIdx, groups} = storeToRefs(groupStore);
const selectedGroup:Ref<Group> = ref({
    id: 0,
    parentGroupId: undefined,
    subGroups: [],
    requirements: [],
    name: '',
    description: '',
    organizationId: 0
});
const viewConfig:Ref<TimesheetViewRequirements> = ref({
    slotDurationInMinutes: 0,
    startTime: '0:00',
    endTime: 'T24:00Z'
});

onMounted(()=>{
    groupStore.getRootGroupsForOrganization(currentOrganizationIdx.value);
});

watch(currentOrganizationIdx, ()=>{
    groupStore.getRootGroupsForOrganization(currentOrganizationIdx.value);
});

const showTimesheetsForGroup = (config: TimesheetViewRequirements) => {
    viewConfig.value = config;
    timesheetStore.getTimesheetForGroup(selectedGroup.value.id);
};
</script>

<template>
    <h2>Timesheets for group</h2>
    <Select v-model="selectedGroup">
        <SelectTrigger>
            <SelectValue placeholder="Choose a Group"/>
        </SelectTrigger>
        <SelectContent>
            <SelectItem v-for="group in groups" :value="group">
                {{ group?.name }}
            </SelectItem>
        </SelectContent>
    </Select>
    <TimesheetDisplayForm @updated="showTimesheetsForGroup">Show</TimesheetDisplayForm>
    <TimesheetCard v-for="timesheet in timesheets" :timesheet="timesheet" :timesheet-view-config="viewConfig"/>
</template>