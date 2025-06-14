<script setup lang="ts">
import type { Group } from '@/classes/group';
import { useGroupStore } from '@/stores/groupStore';
import { storeToRefs } from 'pinia';
import { onMounted, watch } from 'vue';
import Button from './ui/button/Button.vue';

const store = useGroupStore();
const { groups, current, currentOrganizationIdx } = storeToRefs(store);

watch(currentOrganizationIdx, ()=>{
    store.getGroupsForOrganization(currentOrganizationIdx.value);
});
</script>

<template>
    <ul>
        <li v-for="group in groups">
            <h3>
                {{group.name}}
            </h3>
            <div >
                SubGroups
                <RouterLink :to="`/groups/${group.id}`" v-for="subGroup in group.subGroups">{{subGroup.name}}</RouterLink>
            </div>
            <div >
                <!-- should maybe only be seen on the individual page? put description as placeholder-->
                Activity Scheduling Requirements
                <div v-for="requirement in group.requirements">
                    {{requirement.activity?.description}}
                    <RouterLink :to="`/activities/${requirement.activity?.id}`">{{requirement.activity?.title}}</RouterLink>
                </div>
            </div>
            <div class="flex h-5 items-center justify-between">
                <RouterLink :to="`/groups/${group.id}`">Page</RouterLink>
                <RouterLink :to="`/timesheets/create`" @click="current=group.id">Create timesheet</RouterLink>
                <Button @click="store.removeGroup(group.id)">Delete</Button>
            </div>
        </li>
    </ul>
</template>