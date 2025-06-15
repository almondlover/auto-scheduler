<script setup lang="ts">
import type { ActivityRequirements } from '@/classes/activity';
import type { Group } from '@/classes/group';
import { createActivityRequirement } from '@/services/activityService';
import { useActivityStore } from '@/stores/activityStore';
import { useGroupStore } from '@/stores/groupStore';
import { storeToRefs } from 'pinia';
import { onMounted, reactive, ref, type Reactive, type Ref } from 'vue';
import Button from './ui/button/Button.vue';

//initialize pinia stores
const groupStore = useGroupStore();
const { groups, current, currentOrganizationIdx } = storeToRefs(groupStore);
const activityStore = useActivityStore();
const { activities, currentActivityIdx } = storeToRefs(activityStore);

onMounted(()=>{
    groupStore.getGroupsForOrganization(currentOrganizationIdx.value);
    activityStore.getActivitiesForOrganization(currentOrganizationIdx.value);
})

const newRequirement:Ref<ActivityRequirements> = ref({
    id: 0,
    activity: {id:0, title:"", organizationId:0, description:""},
    group: {id:0, organizationId:0, name:"", parentGroupId: 0, description:undefined, subGroups:[], requirements:[]},
    member: {id: 0, organizationId: 0, name: "", contact: "", availability:[]},
    duration: 0,
    hallsize: undefined,
    halltype: undefined,
    timesPerWeek: undefined,
}); 
</script>

<template>
    <form @submit.prevent="createActivityRequirement(newRequirement)">
        <input name="duration" type="number" v-model="newRequirement.duration" placeholder="Duration"/>
        <input name="hallSize" type="number" v-model="newRequirement.hallsize" required="false" placeholder="Hall size"/>
        <input name="hallType" type="number" v-model="newRequirement.halltype" required="false" placeholder="Hall type"/>
        <select name="group" v-model="newRequirement.group">
            <option v-for="group in groups" :value="group">{{ group.name }}</option>
        </select>
        <select name="activity" v-model="newRequirement.activity">
            <option v-for="activity in activities" :value="activity">{{ activity.title }}</option>
        </select>
        <Button type="submit">Add</Button>
    </form>
</template>