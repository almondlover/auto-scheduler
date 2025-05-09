<script setup lang="ts">
import type { ActivityRequirements } from '@/classes/activity';
import type { Group } from '@/classes/group';
import { createActivityRequirement } from '@/services/activityService';
import { useActivityStore } from '@/stores/activityStore';
import { useGroupStore } from '@/stores/groupStore';
import { storeToRefs } from 'pinia';
import { onMounted, reactive, ref, type Reactive, type Ref } from 'vue';

//initialize pinia stores
const groupStore = useGroupStore();
const { groups, current } = storeToRefs(groupStore);
const activityStore = useActivityStore();
const { activities, currentActivityIdx } = storeToRefs(activityStore);

onMounted(()=>{
    groupStore.getGroupsForOrganization(1);
    activityStore.getActivitiesForOrganization(1);
})

const newRequirement:Ref<ActivityRequirements> = ref({
    id: 0,
    activity: {id:0, title:"", organizationId:0, description:""},
    group: {id:0, organizationId:0, name:"", description:undefined, subGroups:[], requirements:[]},
    duration: 0,
    hallsize: undefined,
    halltype: undefined,
    timesPerWeek: undefined
}); 
</script>

<template>
    <form @submit.prevent="createActivityRequirement(newRequirement)">
        <input name="duration" type="number" v-model="newRequirement.duration" placeholder="Duration"/>
        <input name="hallSize" type="number" v-model="newRequirement.hallsize" required="false" placeholder="Hall size"/>
        <input name="hallType" type="number" v-model="newRequirement.halltype" required="false" placeholder="Hall type"/>
        <input name="timesPerWeek" type="number" v-model="newRequirement.timesPerWeek" placeholder="Per week"/>
        <select name="group" v-model="newRequirement.group">
            <option v-for="group in groups" :value="group">{{ group.name }}</option>
        </select>
        <select name="activity" v-model="newRequirement.activity">
            <option v-for="activity in activities" :value="activity">{{ activity.title }}</option>
        </select>
        <button type="submit">Add</button>
    </form>
</template>