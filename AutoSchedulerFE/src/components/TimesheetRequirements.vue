<script setup lang="ts">
import type { ActivityRequirements } from '@/classes/activity';
import { useGroupStore } from '@/stores/groupStore';
import { useTimesheetStore } from '@/stores/timesheetStore';
import { storeToRefs } from 'pinia';
import { onMounted } from 'vue';

const groupStore = useGroupStore();
const { groups, current, currentGroup } = storeToRefs(groupStore);

onMounted(()=>{
    groupStore.getGroupsForOrganization(1);
});

const handleTimesheetGenerate = ()=>{
    if (currentGroup.value?.requirements)
        timesheetStore.generateTimesheet(currentGroup?.value.requirements)
};

const timesheetStore = useTimesheetStore();
const { timesheets, currentTimesheetIdx, currentTimesheet } = storeToRefs(timesheetStore)
</script>

<template>
    <select v-model="current">
        <option v-for="group in groups" :value="group.id">{{group.name}}</option>
    </select>
    <!-- should open a modal for the current group, general requirements on a seperate page -->
    <button>Add requirement</button>
    <button @click="handleTimesheetGenerate">Generate</button>
    <div v-show="current>0">
        <h3>Requirements</h3>
        <div v-for="requirement in currentGroup?.requirements">
            <p>
                Activity: {{requirement.activity?.title}}
            </p>
            <p>
                Duration: {{requirement.duration}}
            </p>
            <p>
                Hall Type: {{requirement.halltype}}
            </p>
            <p>
                Per week: {{requirement.timesPerWeek}}
            </p>
        </div>
    </div>
</template>