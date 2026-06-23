<script setup lang="ts">
import { useTimesheetStore } from '@/stores/timesheetStore';
import { storeToRefs } from 'pinia';
import { onMounted, ref, watch, type Ref } from 'vue';
import Select from './ui/select/Select.vue';
import SelectTrigger from './ui/select/SelectTrigger.vue';
import SelectContent from './ui/select/SelectContent.vue';
import SelectValue from './ui/select/SelectValue.vue';
import SelectItem from './ui/select/SelectItem.vue';
import { useGroupStore } from '@/stores/groupStore';
import TimesheetCard from './TimesheetCard.vue';
import TimesheetDisplayForm from './TimesheetDisplayForm.vue';
import { TimesheetState, type Timesheet, type TimesheetViewRequirements } from '@/classes/timesheet';
import type { Group } from '@/classes/group';
import Tabs from './ui/tabs/Tabs.vue';
import TabsList from './ui/tabs/TabsList.vue';
import TabsTrigger from './ui/tabs/TabsTrigger.vue';
import TabsContent from './ui/tabs/TabsContent.vue';
import Card from './ui/card/Card.vue';

const timesheetStore = useTimesheetStore();
const { timesheets } = storeToRefs(timesheetStore);
const groupStore = useGroupStore();
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
    breakDurationInMinutes: 0,
    startTime: '00:00',
    endTime: '00:00'
});

const currentState:Ref<TimesheetState> = ref(TimesheetState.Active);

onMounted(()=>{
    groupStore.getRootGroupsForOrganization(currentOrganizationIdx.value);
});

watch(currentOrganizationIdx, ()=>{
    groupStore.getRootGroupsForOrganization(currentOrganizationIdx.value);
});

const showTimesheetsForGroup = (config: TimesheetViewRequirements) => {
    viewConfig.value = config;
    timesheetStore.getTimesheetsForGroup(selectedGroup.value.id, currentState.value);
};
</script>

<template>
    <h2 class="mx-5 font-bold text-lg">Timesheets for group</h2>
    <Select v-model="selectedGroup">
        <SelectTrigger class="m-5" >
            <SelectValue placeholder="Choose a Group"/>
        </SelectTrigger>
        <SelectContent>
            <SelectItem v-for="group in groups" :value="group">
                {{ group?.name }}
            </SelectItem>
        </SelectContent>
    </Select>
    <!-- <TimesheetDisplayForm @updated="showTimesheetsForGroup">Show</TimesheetDisplayForm> -->
    <Card class="m-5">
        <Tabs v-model="currentState" @update:model-value="showTimesheetsForGroup(viewConfig)" >
            <TabsList class="p-2 mx-10 bg-primary">
                <TabsTrigger class="tab-button" :value="TimesheetState.Active">
                    Active
                </TabsTrigger>
                <TabsTrigger class="tab-button" :value="TimesheetState.Draft">
                    Drafts
                </TabsTrigger>
                <TabsTrigger class="tab-button" :value="TimesheetState.Inactive">
                    History
                </TabsTrigger>
            </TabsList>
            <TabsContent :value="TimesheetState.Active">
                <h2 class="mx-5 font-bold text-xl text-center">Active timesheets </h2>
                <TimesheetCard v-for="timesheet in timesheets" 
                    :timesheet="timesheet" 
                    :generator-requirements="{ requirements:timesheet.requirements,
                                                startTime: timesheet.startTime,
                                                endTime: timesheet.endTime,
                                                slotDurationInMinutes: timesheet.baseSlotDuration,
                                                breakDurationInMinutes: timesheet.breakDuration,
                                                generalBreakStartTime: timesheet.generalBreakStart,
                                                generalBreakEndTime: timesheet.generalBreakEnd
                                            }"
                    :title="timesheet.title"/>
            </TabsContent>
            <TabsContent :value="TimesheetState.Draft">
                <h2 class="mx-5 font-bold text-xl text-center">Timesheet drafts</h2>
                <TimesheetCard v-for="timesheet in timesheets" 
                    :timesheet="timesheet" 
                    :generator-requirements="{ requirements:timesheet.requirements,
                                                startTime: timesheet.startTime,
                                                endTime: timesheet.endTime,
                                                slotDurationInMinutes: timesheet.baseSlotDuration,
                                                breakDurationInMinutes: timesheet.breakDuration,
                                                generalBreakStartTime: timesheet.generalBreakStart,
                                                generalBreakEndTime: timesheet.generalBreakEnd
                                            }"
                    :title="timesheet.title"/>
            </TabsContent>
            <TabsContent :value="TimesheetState.Inactive">
                <h2 class="mx-5 font-bold text-xl text-center">Inactive timesheets</h2>
                <TimesheetCard v-for="timesheet in timesheets" 
                    :timesheet="timesheet" 
                    :generator-requirements="{ requirements:timesheet.requirements,
                                                startTime: timesheet.startTime,
                                                endTime: timesheet.startTime,
                                                slotDurationInMinutes: timesheet.baseSlotDuration,
                                                breakDurationInMinutes: timesheet.breakDuration,
                                                generalBreakStartTime: timesheet.generalBreakStart,
                                                generalBreakEndTime: timesheet.generalBreakEnd
                                            }"
                    :title="timesheet.title"/>
            </TabsContent>
        </Tabs>
    </Card>
</template>